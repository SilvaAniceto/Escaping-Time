public class CharacterCancelJumpCommand : ICharacterActionCommand
{
    private IStateController _stateController;

    public CharacterCancelJumpCommand(IStateController stateController)
    {
        _stateController = stateController;
    }

    public void ExecuteCommand()
    {
        if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.JumpState() ||
            _stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.AirJumpState() ||
            _stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.WallJumpState())
        {
            _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.FallState());
        }
    }
}