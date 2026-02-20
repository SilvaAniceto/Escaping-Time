using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ECharacterDirection
{
    Left = -1,
    None = 0,
    Right = 1
}

public enum ECameraTiltDirection
{
    Up = 1,
    None = 0,
    Down = -1
}

public class PlayerInputManager
{
    public PlayerInputManager(CharacterContextManager characterContextManager, CameraBehaviourController cameraBehaviourController, PlayerInputActions playerInputActions)
    {
        _characterContextManager = characterContextManager;
        _cameraBehaviourController = cameraBehaviourController;
        _playerInputActions = playerInputActions;
    }

    private bool _initialized = false;

    private CharacterContextManager _characterContextManager;
    private CameraBehaviourController _cameraBehaviourController;
    private PlayerInputActions _playerInputActions;

    private const float _maxTimeForClearBuffer = 0.1f;
    private const int _maxBufferLength = 5;

    private ECharacterDirection _characterDirection = ECharacterDirection.None;
    private ECameraTiltDirection _cameraTiltDirection = ECameraTiltDirection.None;

    private float _jumpCommandBufferTimer;
    private float _dashCommandBufferTimer;
    private float _wallMoveCommandBufferTimer;
    private float _interactCommandBufferTimer;

    private Queue<ICharacterActionCommand> _jumpCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _airJumpCommandCombo = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _dashCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _wallMoveCommandBuffer = new Queue<ICharacterActionCommand>();
    private Queue<ICharacterActionCommand> _interactCommandBuffer = new Queue<ICharacterActionCommand>();

    private List<ICharacterComboCommand> _characterComboRules;

    private CharacterActionCommandInvoker _characterActionCommandInvoker;

    public bool Cancel { get => _playerInputActions.PlayerActionMap.Cancel.triggered; }

    public void UpdatePlayerInputManager()
    {
        if (!_initialized) return;

        ProcessMoveInput();
        ProcessCameraTiltInput();
        ProcessCommandBuffer(Time.deltaTime);
    }
    public void EnableInputAction()
    {
        if (_playerInputActions == null)
        {
            return;
        }

        _playerInputActions.Enable();
    }
    public void DisableInputAction()
    {
        if (_playerInputActions == null)
        {
            return;
        }

        _playerInputActions.Disable();
    }

    #region CLASS METHODS
    public void Initialize()
    {
        _initialized = false;

        _characterActionCommandInvoker = new CharacterActionCommandInvoker();

        _characterComboRules = new List<ICharacterComboCommand>
        {
            new ComboAirJump(_characterContextManager)
        };

        _playerInputActions.PlayerActionMap.Jump.started += ctx => HandleJumpCommand();
        _playerInputActions.PlayerActionMap.Jump.canceled += ctx => HandleCancelJumpCommand();

        _playerInputActions.PlayerActionMap.WallMove.started += ctx => HandleWallMoveCommand();
        _playerInputActions.PlayerActionMap.WallMove.canceled += ctx => HandleCancelWallMoveCommand();

        _playerInputActions.PlayerActionMap.Dash.started += ctx => HandleDashCommand();

        _playerInputActions.PlayerActionMap.Interact.started += ctx => HandleInteractCommand();

        _initialized = true;
    }
    private void ProcessCommandBuffer(float deltaTime)
    {
        ProcessJumpCommandBuffer(deltaTime);
        ProcessDashCommandBuffer(deltaTime);
        ProcessWallMoveCommandBuffer(deltaTime);
        ProcessInteractCommandBuffer(deltaTime);
    }
    private void ClearCommandBuffer(Queue<ICharacterActionCommand> commandBuffer)
    {
        commandBuffer.Clear();
    }
    #endregion

    #region MOVE COMMAND
    private void ProcessMoveInput()
    {
        Vector2 move = new Vector2(_playerInputActions.PlayerActionMap.Move.ReadValue<float>(), 0.00f);
        move = move.normalized;

        ECharacterDirection direction =  ProcessCharacterDirection(move.x);

        ProcessDirectionChange(direction);
    }
    private ECharacterDirection ProcessCharacterDirection(float inputValue)
    {
        if (inputValue < -0.25f) return ECharacterDirection.Left;
        if (inputValue > 0.25f) return ECharacterDirection.Right;

        return ECharacterDirection.None;
    }
    private void ProcessDirectionChange(ECharacterDirection direction)
    {
        if (_characterDirection != direction)
        {
            _characterDirection = direction;
        }

        HandleDirectionCommand();
    }
    private void HandleDirectionCommand()
    {
        switch (_characterDirection)
        {
            case ECharacterDirection.Left:
                var leftCommand = new CharacterLeftDirectionCommand(_characterContextManager);

                _characterActionCommandInvoker.ExecuteActionCommand(leftCommand);
                break;
            case ECharacterDirection.None:
                var noneCommand = new CharacterNoneDirectionCommand(_characterContextManager);

                _characterActionCommandInvoker.ExecuteActionCommand(noneCommand);
                break;
            case ECharacterDirection.Right:
                var rightCommand = new CharacterRightDirectionCommand(_characterContextManager);

                _characterActionCommandInvoker.ExecuteActionCommand(rightCommand);
                break;
        }
    }
    #endregion

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

    #region WALL MOVE COMMAND
    private void HandleWallMoveCommand()
    {
        var wallMoveCommand = new CharacterWallMoveCommand(_characterContextManager);

        if (_wallMoveCommandBuffer.Count > _maxBufferLength)
        {
            ClearCommandBuffer(_wallMoveCommandBuffer);
        }

        _wallMoveCommandBuffer.Enqueue(wallMoveCommand);

        _characterActionCommandInvoker.ExecuteActionCommand(wallMoveCommand);
    }
    private void HandleCancelWallMoveCommand()
    {
        var cancelWallMoveCommand = new CharacterCancelWallMoveCommand(_characterContextManager);

        _characterActionCommandInvoker.ExecuteActionCommand(cancelWallMoveCommand);
        ClearCommandBuffer(_wallMoveCommandBuffer);
    }
    private void ProcessWallMoveCommandBuffer(float deltaTime)
    {
        if (_wallMoveCommandBuffer.Count == 0)
        {
            return;
        }

        _wallMoveCommandBufferTimer += deltaTime;

        if (_wallMoveCommandBufferTimer > _maxTimeForClearBuffer)
        {
            _wallMoveCommandBufferTimer = 0;

            _characterActionCommandInvoker.ExecuteActionCommand(_wallMoveCommandBuffer.Peek());
        }
    }
    #endregion

    #region INTERACT COMMAND
    private void HandleInteractCommand()
    {
        var interactCommand = new CharacterInteractCommand(_characterContextManager);

        if (_interactCommandBuffer.Count > _maxBufferLength)
        {
            ClearCommandBuffer(_interactCommandBuffer);
        }

        _interactCommandBuffer.Enqueue(interactCommand);

        _characterActionCommandInvoker.ExecuteActionCommand(interactCommand);
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
            _interactCommandBufferTimer = 0;

            _characterActionCommandInvoker.ExecuteActionCommand(_interactCommandBuffer.Peek());
            ClearCommandBuffer(_interactCommandBuffer);
        }
    }
    #endregion

    #region CAMERA TILT COMMAND
    private void ProcessCameraTiltInput()
    {
        float cameraTilt = _playerInputActions.PlayerActionMap.CameraTilt.ReadValue<float>();

        ECameraTiltDirection direction = ProcessCameraTiltDirection(cameraTilt);

        ProcessCameraTiltDirectionChange(direction);
    }
    private ECameraTiltDirection ProcessCameraTiltDirection(float inputValue)
    {
        if (inputValue < -0.25f) return ECameraTiltDirection.Down;
        if (inputValue > 0.25f) return ECameraTiltDirection.Up;

        return ECameraTiltDirection.None;
    }
    private void ProcessCameraTiltDirectionChange(ECameraTiltDirection direction)
    {
        if (_cameraTiltDirection != direction)
        {
            _cameraTiltDirection = direction;
            HandleCameraTiltDirectionCommand();
        }
    }
    private void HandleCameraTiltDirectionCommand()
    {
        switch (_cameraTiltDirection)
        {
            case ECameraTiltDirection.Up:
                var UpCommand = new CharacterCameraTiltUpDirectionCommand(_cameraBehaviourController);

                _characterActionCommandInvoker.ExecuteActionCommand(UpCommand);
                break;
            case ECameraTiltDirection.None:
                var noneCommand = new CharacterCameraTiltNoneDirectionCommand(_cameraBehaviourController);

                _characterActionCommandInvoker.ExecuteActionCommand(noneCommand);
                break;
            case ECameraTiltDirection.Down:
                var downCommand = new CharacterCameraTiltDownDirectionCommand(_cameraBehaviourController);

                _characterActionCommandInvoker.ExecuteActionCommand(downCommand);
                break;
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