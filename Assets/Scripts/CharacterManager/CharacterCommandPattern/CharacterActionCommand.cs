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
    public void ExecuteCommand()
    {

    }
}