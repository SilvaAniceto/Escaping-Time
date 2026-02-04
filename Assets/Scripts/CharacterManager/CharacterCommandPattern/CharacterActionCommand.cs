public class CharacterLeftDirectionCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterLeftDirectionCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        _characterContextManager.MoveDirection = (int)ECharacterDirection.Left;

        _characterContextManager.CurrentState.SetSubState(_characterContextManager.CurrentState.CharacterStateFactory.MoveState());
    }
}

public class CharacterRightDirectionCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterRightDirectionCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        _characterContextManager.MoveDirection = (int)ECharacterDirection.Right;

        _characterContextManager.CurrentState.SetSubState(_characterContextManager.CurrentState.CharacterStateFactory.MoveState());
    }
}

public class CharacterNoneDirectionCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterNoneDirectionCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        _characterContextManager.MoveDirection = (int)ECharacterDirection.None;

        _characterContextManager.CurrentState.SetSubState(_characterContextManager.CurrentState.CharacterStateFactory.IdleState());
    }
}

public class CharacterJumpCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterJumpCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }

    public void ExecuteCommand()
    {
        if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.GroundedState())
        {
            _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.JumpState());
        }
        else if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.FallState())
        {
            if (_characterContextManager.CoyoteTime)
            {
                _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.JumpState());
            }
        }
        else if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.OnWallState())
        {
            _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.WallJumpState());
        }
        else if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.InteractionState())
        {
            _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.JumpState());
        }
    }
}

public class CharacterAirJumpCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterAirJumpCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        if (_characterContextManager.HasAirJump && _characterContextManager.AirJumpIsAllowed)
        {
            _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.AirJumpState());
        }
    }
}

public class CharacterCancelJumpCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterCancelJumpCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.JumpState() ||
            _characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.AirJumpState() ||
            _characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.WallJumpState())
        {
            _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.FallState());
        }
    }
}

public class CharacterDashCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterDashCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }

    public void ExecuteCommand()
    {
        if (_characterContextManager.DashIsAllowed)
        {
            _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.DashState());
        }
    }
}

public class CharacterInteractCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterInteractCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.InteractionState())
        {
            _characterContextManager.Interactable.ConfirmInteraction();
        }
    }
}

public class CharacterWallMoveCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterWallMoveCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        if (_characterContextManager.DamageOnCoolDown) return;

        if (_characterContextManager.HasWallMove && _characterContextManager.CurrentState.IsWallColliding)
        {
            if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.FallState())
            {
                _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.OnWallState());
            }
        }
    }
}

public class CharacterCancelWallMoveCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterCancelWallMoveCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        if (_characterContextManager.CurrentState == _characterContextManager.CurrentState.CharacterStateFactory.OnWallState())
        {
            _characterContextManager.CurrentState.SwitchState(_characterContextManager.CurrentState.CharacterStateFactory.FallState());
        }
    }
}

public class CharacterCameraTiltUpDirectionCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterCameraTiltUpDirectionCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        _characterContextManager.CameraTilt = (int)ECameraTiltDirection.Up;
    }
}

public class CharacterCameraTiltDownDirectionCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterCameraTiltDownDirectionCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        _characterContextManager.CameraTilt = (int)ECameraTiltDirection.Down;
    }
}

public class CharacterCameraTiltNoneDirectionCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterCameraTiltNoneDirectionCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }
    public void ExecuteCommand()
    {
        _characterContextManager.CameraTilt = (int)ECameraTiltDirection.None;
    }
}