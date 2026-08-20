public class CharacterDashCommand : ICharacterActionCommand
{
    private IStateController _stateController;
    private IDashCapability _dashCapability;

    public CharacterDashCommand(IStateController stateController, IDashCapability dashCapability)
    {
        _stateController = stateController;
        _dashCapability = dashCapability;
    }

    public void ExecuteCommand()
    {
        if (_dashCapability.DashIsAllowed)
        {
            _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.DashState());
        }
    }
}