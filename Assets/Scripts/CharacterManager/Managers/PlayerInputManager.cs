using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    private CharacterContextManager _characterContextManager;

    public static PlayerInputManager PlayerInputInstance;

    private const float _maxTimeForClearBuffer = 0.05f;
    private const int _maxBufferLength = 5;

    private float _jumpCommandBufferTimer;
    private float _dashCommandBufferTimer;
    private float _interactCommandBufferTimer;

    private CharacterActionCommandInvoker _characterActionCommandInvoker;

    private Queue<ICharacterActionCommand> _jumpCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _airJumpCommandCombo = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _dashCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _interactCommandBuffer = new Queue<ICharacterActionCommand>();

    private List<ICharacterComboCommand> _characterComboRules;

    public PlayerInputActions PlayerInputActions { get; private set; }

    public bool Cancel { get => PlayerInputActions.PlayerActionMap.Cancel.triggered; }
    public float MoveInput
    {
        get
        {
            Vector2 move = new Vector2(PlayerInputActions.PlayerActionMap.Move.ReadValue<float>(), 0.00f);
            move = move.normalized;
            return move.x;
        }
    }
    public bool InteractionInput { get => PlayerInputActions.PlayerActionMap.Interact.WasPressedThisFrame(); }
    public float CameraTiltInput { get => PlayerInputActions.PlayerActionMap.CameraTilt.ReadValue<float>(); }
    public bool WallMoveInput { get => PlayerInputActions.PlayerActionMap.WallMove.IsPressed(); }

    private void Update()
    {
        ProcessCommandBuffer(Time.deltaTime);
    }
    private void OnEnable()
    {
        if (PlayerInputActions == null)
        {
            return;
        }

        PlayerInputActions.Enable();
    }
    private void OnDisable()
    {
        if (PlayerInputActions == null)
        {
            return;
        }

        PlayerInputActions.Disable();
    }
    private void OnDestroy()
    {
        if (PlayerInputActions == null)
        {
            return;
        }

        PlayerInputActions.Disable();
    }

    public void Initialize(CharacterContextManager characterContextManager)
    {
        if (PlayerInputInstance == null)
        {
            PlayerInputInstance = this;
        }
        else
        {
            Destroy(PlayerInputInstance);
        }

        if (PlayerInputActions == null)
        {
            PlayerInputActions = new PlayerInputActions();
        }

        _characterActionCommandInvoker = new CharacterActionCommandInvoker();
        _characterContextManager = characterContextManager;

        _characterComboRules = new List<ICharacterComboCommand>
        {
            new ComboAirJump(_characterContextManager)
        };

        PlayerInputActions.PlayerActionMap.Jump.started += ctx => HandleJumpCommand();
        PlayerInputActions.PlayerActionMap.Jump.canceled += ctx => HandleCancelJumpCommand();
        PlayerInputActions.PlayerActionMap.Dash.started += ctx => HandleDashCommand();
        PlayerInputActions.PlayerActionMap.Interact.performed += ctx => HandleInteractCommand();
    }
    private void ProcessCommandBuffer(float deltaTime)
    {
        ProcessJumpCommandBuffer(deltaTime);
        ProcessDashCommandBuffer(deltaTime);
        ProcessInteractCommandBuffer(deltaTime);
    }
    private void ClearCommandBuffer(Queue<ICharacterActionCommand> commandBuffer)
    {
        commandBuffer.Clear();
    }

    #region JUMP COMMAND
    private void HandleJumpCommand()
    {
        var jumpCommand = new CharacterJumpCommand(_characterContextManager);

        if (_jumpCommandBuffer.Count > _maxBufferLength)
        {
            ClearCommandBuffer(_jumpCommandBuffer);
            ClearCommandBuffer(_airJumpCommandCombo);
        }

        _jumpCommandBuffer.Enqueue(jumpCommand);
        _airJumpCommandCombo.Enqueue(jumpCommand);

        _characterActionCommandInvoker.ExecuteActionCommand(jumpCommand);
    }
    private void HandleCancelJumpCommand()
    {
        var cancelJumpCommand = new CharacterCancelJumpCommand(_characterContextManager);

        _characterActionCommandInvoker.ExecuteActionCommand(cancelJumpCommand);            
    }
    private void ProcessJumpCommandBuffer(float deltaTime)
    {
        if (_jumpCommandBuffer.Count == 0)
        {
            return;
        }

        _jumpCommandBufferTimer += deltaTime;

        if (_jumpCommandBufferTimer > _maxTimeForClearBuffer)
        {
            _jumpCommandBufferTimer = 0;

            _characterActionCommandInvoker.ExecuteActionCommand(_jumpCommandBuffer.Peek());
            ClearCommandBuffer(_jumpCommandBuffer);

            return;
        }

        CheckAndExecuteCharacterCombo(_airJumpCommandCombo);
    }
    public void ClearAirJumpCommandCombo()
    {
        _airJumpCommandCombo.Clear();
    }
    #endregion

    #region DASH COMMAND
    private void HandleDashCommand()
    {
        var dashCommand = new CharacterDashCommand(_characterContextManager);

        if (_dashCommandBuffer.Count >= _maxBufferLength)
        {
            _dashCommandBuffer.Dequeue();
        }

        _dashCommandBuffer.Enqueue(dashCommand);
        _characterActionCommandInvoker.ExecuteActionCommand(dashCommand);
    }
    private void ProcessDashCommandBuffer(float deltaTime)
    {
        if (_dashCommandBuffer.Count == 0)
        {
            return;
        }

        _dashCommandBufferTimer += deltaTime;

        if (_dashCommandBufferTimer > _maxTimeForClearBuffer)
        {
            _dashCommandBufferTimer = 0;

            _characterActionCommandInvoker.ExecuteActionCommand(_dashCommandBuffer.Peek());
            ClearCommandBuffer(_dashCommandBuffer);
        }
    }
    #endregion

    #region INTERACT COMMAND
    private void HandleInteractCommand()
    {
        var interactCommand = new CharacterInteractCommand();

        if (_interactCommandBuffer.Count > _maxBufferLength)
        {
            ClearCommandBuffer(_interactCommandBuffer);
        }

        _jumpCommandBuffer.Enqueue(interactCommand);
    }
    private void ProcessInteractCommandBuffer(float deltaTime)
    {
        if (_interactCommandBuffer.Count == 0)
        {
            return;
        }

        _interactCommandBufferTimer += deltaTime;

        if (_interactCommandBufferTimer > _maxTimeForClearBuffer)
        {
            ClearCommandBuffer(_interactCommandBuffer);
            _interactCommandBufferTimer = 0;
        }
    }
    #endregion

    #region MATCH COMBO COMMANDS
    private void CheckAndExecuteCharacterCombo(Queue<ICharacterActionCommand> comboSequence)
    {
        if (comboSequence.Count < 2) 
        {
            return;
        }

        ICharacterActionCommand comboCommand = CheckSequenceForCombo(comboSequence);

        if (comboCommand != null)
        {
            _characterActionCommandInvoker.ExecuteActionCommand(comboCommand);
            ClearCommandBuffer(comboSequence);
        }
    }
    private ICharacterActionCommand CheckSequenceForCombo(Queue<ICharacterActionCommand> comboSequence)
    {
        for (int startIndex = 0; startIndex <= comboSequence.Count; startIndex++)
        {
            var subsequence = GetSubsequence(comboSequence, startIndex);

            foreach (ICharacterComboCommand rule in _characterComboRules)
            {
                if (rule.IsMatch(subsequence))
                {
                    return rule.GetResultingComboCommand();
                }
            }
        }
        return null;
    }
    private IEnumerable<ICharacterActionCommand> GetSubsequence(Queue<ICharacterActionCommand> comboSequence, int startIndex)
    {
        return comboSequence.Skip(startIndex);
    }
    #endregion
}