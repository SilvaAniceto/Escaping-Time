public class GamePauseCommand : ICharacterActionCommand
{
    private IGameStateManager _gameStateManager;
    public GamePauseCommand(IGameStateManager gameStateManager)
    {
        _gameStateManager = gameStateManager;
    }

    public void ExecuteCommand()
    {
        if (_gameStateManager.CurrentState == null) 
        {
            return;
        }

        if (_gameStateManager.CurrentState == _gameStateManager.CurrentState.GameManagerStateFactory.GameHubState())
        {
            _gameStateManager.PauseGameOnHubState();
            return;
        }
        if (_gameStateManager.CurrentState == _gameStateManager.CurrentState.GameManagerStateFactory.GamePauseState())
        {
            _gameStateManager.CurrentState.SwitchState(_gameStateManager.ExitState);
            return;
        }
        if (_gameStateManager.CurrentState == _gameStateManager.CurrentState.GameManagerStateFactory.GameRunState())
        {
            _gameStateManager.PauseOnRunState();
            return;
        }
    }
}