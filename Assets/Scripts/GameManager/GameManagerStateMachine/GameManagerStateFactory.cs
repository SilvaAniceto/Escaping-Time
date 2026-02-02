using System.Collections.Generic;

public enum EGameState
{
    MainMenu,
    SaveMenu,
    Loading,
    Running,
    Paused,
    Hub,
    ScoreMenu,
}

public class GameManagerStateFactory
{
    GameContextManager _gameContextManager;
    GameUIManager _gameUIInputsManager;
    Dictionary<EGameState, GameManagerAbstractState> _states = new Dictionary<EGameState, GameManagerAbstractState>();

    public GameManagerStateFactory(GameContextManager currentContextManager, GameUIManager gameUIInputsManager)
    {
        _gameContextManager = currentContextManager;
        _gameUIInputsManager = gameUIInputsManager;
        _states[EGameState.MainMenu] = new GameManagerMainMenuState(_gameContextManager, this, _gameUIInputsManager);
        _states[EGameState.SaveMenu] = new GameManagerSaveMenuState(_gameContextManager, this, _gameUIInputsManager);
        _states[EGameState.Loading] = new GameManagerLoadingState(_gameContextManager, this, _gameUIInputsManager);
        _states[EGameState.Running] = new GameManagerRunState(_gameContextManager, this, _gameUIInputsManager);
        _states[EGameState.Paused] = new GameManagerPauseState(_gameContextManager, this, _gameUIInputsManager);
        _states[EGameState.Hub] = new GameManagerHubState(_gameContextManager, this, _gameUIInputsManager);
        _states[EGameState.ScoreMenu] = new GameManagerScoreState(_gameContextManager, this, _gameUIInputsManager);
    }

    public GameManagerMainMenuState GameMainMenuState()
    {
        return (GameManagerMainMenuState)_states[EGameState.MainMenu];
    }

    public GameManagerSaveMenuState GameSaveMenuState()
    {
        return (GameManagerSaveMenuState)_states[EGameState.SaveMenu];
    }

    public GameManagerLoadingState GameLoadingState()
    {
        return (GameManagerLoadingState)_states[EGameState.Loading];
    }

    public GameManagerRunState GameRunState()
    {
        return (GameManagerRunState)_states[EGameState.Running];
    }

    public GameManagerPauseState GamePauseState()
    {
        return (GameManagerPauseState)_states[EGameState.Paused];
    }

    public GameManagerHubState GameHubState()
    {
        return (GameManagerHubState)_states[EGameState.Hub];
    }

    public GameManagerScoreState GameScoreState()
    {
        return (GameManagerScoreState)_states[EGameState.ScoreMenu];
    }
}
