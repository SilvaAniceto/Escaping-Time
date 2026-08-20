public class CharacterCancelWallMoveCommand : ICharacterActionCommand
{
    private IStateController _stateController;

    public CharacterCancelWallMoveCommand(IStateController stateController)
    {
        _stateController = stateController;
    }

    public void ExecuteCommand()
    {
        if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.OnWallState())
        {
            _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.FallState());
        }
    }
}