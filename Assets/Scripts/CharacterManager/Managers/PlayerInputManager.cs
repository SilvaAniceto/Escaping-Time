using System.Collections.Generic;
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
    public PlayerInputManager(GameContextManager gameContextManager, CharacterContextManager characterContextManager, CameraBehaviourController cameraBehaviourController, PlayerInputActions playerInputActions)
    {
        _gameContextManager = gameContextManager;
        _characterContextManager = characterContextManager;
        _cameraBehaviourController = cameraBehaviourController;
        _playerInputActions = playerInputActions;

        _commandBufferManager = new CharacterCommandBufferManager();

        var comboRules = new List<ICharacterComboCommand>
        {
            new ComboAirJump(_characterContextManager)
        };
        _comboMatcher = new CharacterComboMatcher(comboRules);
    }

    private bool _initialized = false;

    private GameContextManager _gameContextManager;
    private CharacterContextManager _characterContextManager;
    private CameraBehaviourController _cameraBehaviourController;
    private PlayerInputActions _playerInputActions;

    private ECharacterDirection _characterDirection = ECharacterDirection.None;
    private ECameraTiltDirection _cameraTiltDirection = ECameraTiltDirection.None;

    private CharacterCommandBufferManager _commandBufferManager;
    private CharacterComboMatcher _comboMatcher;

    private CharacterActionCommandInvoker _characterActionCommandInvoker;

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

    #region INPUT CALLBACK METHODS
    private void OnJumpStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => HandleJumpCommand();
    private void OnJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => HandleCancelJumpCommand();
    private void OnWallMoveStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => HandleWallMoveCommand();
    private void OnWallMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => HandleCancelWallMoveCommand();
    private void OnDashStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => HandleDashCommand();
    private void OnInteractStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => HandleInteractCommand();
    private void OnCancelStarted(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => HandlePauseCommand();
    #endregion

    #region CLASS METHODS
    public void Initialize()
    {
        _initialized = false;

        _characterActionCommandInvoker = new CharacterActionCommandInvoker();

        _playerInputActions.PlayerActionMap.Jump.started += OnJumpStarted;
        _playerInputActions.PlayerActionMap.Jump.canceled += OnJumpCanceled;

        _playerInputActions.PlayerActionMap.WallMove.started += OnWallMoveStarted;
        _playerInputActions.PlayerActionMap.WallMove.canceled += OnWallMoveCanceled;

        _playerInputActions.PlayerActionMap.Dash.started += OnDashStarted;

        _playerInputActions.PlayerActionMap.Interact.started += OnInteractStarted;

        _playerInputActions.PlayerActionMap.Cancel.started += OnCancelStarted;

        _initialized = true;
    }
    public void ClearPlayerActionsCallback()
    {
        _playerInputActions.PlayerActionMap.Jump.started -= OnJumpStarted;
        _playerInputActions.PlayerActionMap.Jump.canceled -= OnJumpCanceled;

        _playerInputActions.PlayerActionMap.WallMove.started -= OnWallMoveStarted;
        _playerInputActions.PlayerActionMap.WallMove.canceled -= OnWallMoveCanceled;

        _playerInputActions.PlayerActionMap.Dash.started -= OnDashStarted;

        _playerInputActions.PlayerActionMap.Interact.started -= OnInteractStarted;

        _playerInputActions.PlayerActionMap.Cancel.started -= OnCancelStarted;

        _playerInputActions.PlayerActionMap.Disable();
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
        _commandBufferManager.ClearCommandBuffer(commandBuffer);
    }
    #endregion

    #region PAUSE COMMAND
    private void HandlePauseCommand()
    {
        var pauseCommand = new GamePauseCommand(ServiceLocator.GameFlowManager as IGameStateManager);

        _characterActionCommandInvoker.ExecuteActionCommand(pauseCommand);
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
                var leftCommand = new CharacterLeftDirectionCommand(_characterContextManager, _characterContextManager);
                _characterActionCommandInvoker.ExecuteActionCommand(leftCommand);
                break;
            case ECharacterDirection.None:
                var noneCommand = new CharacterNoneDirectionCommand(_characterContextManager, _characterContextManager);
                _characterActionCommandInvoker.ExecuteActionCommand(noneCommand);
                break;
            case ECharacterDirection.Right:
                var rightCommand = new CharacterRightDirectionCommand(_characterContextManager, _characterContextManager);
                _characterActionCommandInvoker.ExecuteActionCommand(rightCommand);
                break;
        }
    }
    #endregion

    #region JUMP COMMAND
    private void HandleJumpCommand()
    {
        var jumpCommand = new CharacterJumpCommand(_characterContextManager, _characterContextManager);

        _commandBufferManager.EnqueueJumpCommand(jumpCommand);

        _characterActionCommandInvoker.ExecuteActionCommand(jumpCommand);
    }
    private void HandleCancelJumpCommand()
    {
        var cancelJumpCommand = new CharacterCancelJumpCommand(_characterContextManager);

        _characterActionCommandInvoker.ExecuteActionCommand(cancelJumpCommand);
    }
    private void ProcessJumpCommandBuffer(float deltaTime)
    {
        if (_commandBufferManager.JumpCommandBuffer.Count == 0)
        {
            return;
        }

        _commandBufferManager.JumpCommandBufferTimer += deltaTime;

        if (_commandBufferManager.JumpCommandBufferTimer > 0.1f)
        {
            _commandBufferManager.JumpCommandBufferTimer = 0;

            _commandBufferManager.ClearCommandBuffer(_commandBufferManager.JumpCommandBuffer);

            return;
        }

        CheckAndExecuteCharacterCombo(_commandBufferManager.AirJumpCommandCombo);
    }
    public void ClearAirJumpCommandCombo()
    {
        _commandBufferManager.ClearAirJumpCommandCombo();
    }
    #endregion

    #region DASH COMMAND
    private void HandleDashCommand()
    {
        var dashCommand = new CharacterDashCommand(_characterContextManager, _characterContextManager);

        _commandBufferManager.EnqueueDashCommand(dashCommand);

        _characterActionCommandInvoker.ExecuteActionCommand(dashCommand);
    }
    private void ProcessDashCommandBuffer(float deltaTime)
    {
        if (_commandBufferManager.DashCommandBuffer.Count == 0)
        {
            return;
        }

        _commandBufferManager.DashCommandBufferTimer += deltaTime;

        if (_commandBufferManager.DashCommandBufferTimer > 0.1f)
        {
            _commandBufferManager.DashCommandBufferTimer = 0;

            _characterActionCommandInvoker.ExecuteActionCommand(_commandBufferManager.DashCommandBuffer.Peek());
            _commandBufferManager.ClearCommandBuffer(_commandBufferManager.DashCommandBuffer);
        }
    }
    #endregion

    #region WALL MOVE COMMAND
    private void HandleWallMoveCommand()
    {
        var wallMoveCommand = new CharacterWallMoveCommand(_characterContextManager, _characterContextManager, _characterContextManager);

        _commandBufferManager.EnqueueWallMoveCommand(wallMoveCommand);

        _characterActionCommandInvoker.ExecuteActionCommand(wallMoveCommand);
    }
    private void HandleCancelWallMoveCommand()
    {
        var cancelWallMoveCommand = new CharacterCancelWallMoveCommand(_characterContextManager);

        _characterActionCommandInvoker.ExecuteActionCommand(cancelWallMoveCommand);
        _commandBufferManager.ClearCommandBuffer(_commandBufferManager.WallMoveCommandBuffer);
    }
    private void ProcessWallMoveCommandBuffer(float deltaTime)
    {
        if (_commandBufferManager.WallMoveCommandBuffer.Count == 0)
        {
            return;
        }

        _commandBufferManager.WallMoveCommandBufferTimer += deltaTime;

        if (_commandBufferManager.WallMoveCommandBufferTimer > 0.1f)
        {
            _commandBufferManager.WallMoveCommandBufferTimer = 0;

            _characterActionCommandInvoker.ExecuteActionCommand(_commandBufferManager.WallMoveCommandBuffer.Peek());
        }
    }
    #endregion

    #region INTERACT COMMAND
    private void HandleInteractCommand()
    {
        var interactCommand = new CharacterInteractCommand(_characterContextManager);

        _commandBufferManager.EnqueueInteractCommand(interactCommand);

        _characterActionCommandInvoker.ExecuteActionCommand(interactCommand);
    }
    private void ProcessInteractCommandBuffer(float deltaTime)
    {
        if (_commandBufferManager.InteractCommandBuffer.Count == 0)
        {
            return;
        }

        _commandBufferManager.InteractCommandBufferTimer += deltaTime;

        if (_commandBufferManager.InteractCommandBufferTimer > 0.1f)
        {
            _commandBufferManager.InteractCommandBufferTimer = 0;

            _commandBufferManager.ClearCommandBuffer(_commandBufferManager.InteractCommandBuffer);
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
        ICharacterActionCommand comboCommand = _comboMatcher.CheckSequenceForCombo(comboSequence);

        if (comboCommand != null)
        {
            _characterActionCommandInvoker.ExecuteActionCommand(comboCommand);
            _commandBufferManager.ClearCommandBuffer(comboSequence);
        }
    }
    #endregion
}