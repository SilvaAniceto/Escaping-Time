public class CharacterWallMoveCommand : ICharacterActionCommand
{
    private IStateController _stateController;
    private IWallMoveCapability _wallMoveCapability;
    private IDamageCapability _damageCapability;

    public CharacterWallMoveCommand(IStateController stateController, IWallMoveCapability wallMoveCapability, IDamageCapability damageCapability)
    {
        _stateController = stateController;
        _wallMoveCapability = wallMoveCapability;
        _damageCapability = damageCapability;
    }

    public void ExecuteCommand()
    {
        if (_damageCapability.IsInvincible)
        {
            return;
        }

        if (_wallMoveCapability.HasWallMove && _stateController.CurrentState.IsWallColliding)
        {
            if (_stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.FallState() ||
            _stateController.CurrentState == _stateController.CurrentState.CharacterStateFactory.WallJumpState())
            {
                _stateController.CurrentState.SwitchState(_stateController.CurrentState.CharacterStateFactory.OnWallState());
            }
        }
    }
}