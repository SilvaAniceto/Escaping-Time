public class CharacterAirJumpCommand : ICharacterActionCommand
{
    private IStateController _stateController;
    private IJumpCapability _jumpCapability;

    public CharacterAirJumpCommand(IStateController stateController, IJumpCapability jumpCapability)
    {
        _stateController = stateController;
        _jumpCapability = jumpCapability;
    }

    public void ExecuteCommand()
    {
        if (_jumpCapability.HasAirJump && _jumpCapability.AirJumpIsAllowed)
        {
            _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.AirJumpState());
        }
    }
}