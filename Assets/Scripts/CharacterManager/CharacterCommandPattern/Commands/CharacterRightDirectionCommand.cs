public class CharacterRightDirectionCommand : ICharacterActionCommand
{
    private IStateController _stateController;
    private IMovementDirection _movementDirection;

    public CharacterRightDirectionCommand(IStateController stateController, IMovementDirection movementDirection)
    {
        _stateController = stateController;
        _movementDirection = movementDirection;
    }

    public void ExecuteCommand()
    {
        _movementDirection.MoveDirection = (int)ECharacterDirection.Right;

        if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.OnWallState() ||
             _stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.WallJumpState() ||
             _stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.DashState())
        {
            return;
        }

        _stateController.CurrentState.SetSubState(_stateController.CurrentState.CharacterStateFactory.MoveState());
    }
}