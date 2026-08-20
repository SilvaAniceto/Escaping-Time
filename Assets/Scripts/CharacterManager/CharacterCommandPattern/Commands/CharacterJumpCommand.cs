public class CharacterJumpCommand : ICharacterActionCommand
{
    private IStateController _stateController;
    private IJumpCapability _jumpCapability;

    public CharacterJumpCommand(IStateController stateController, IJumpCapability jumpCapability)
    {
        _stateController = stateController;
        _jumpCapability = jumpCapability;
    }

    public void ExecuteCommand()
    {
        if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.GroundedState())
        {
            _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.JumpState());
        }

        if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.FallState())
        {
            if (_jumpCapability.CoyoteTime)
            {
                _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.JumpState());
                return;
            }

            if (_jumpCapability.HasAirJump && _jumpCapability.AirJumpIsAllowed)
            {
                _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.AirJumpState());
            }
        }

        if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.OnWallState())
        {
            _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.WallJumpState());
        }

        if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.InteractionState())
        {
            _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.JumpState());
        }
    }
}