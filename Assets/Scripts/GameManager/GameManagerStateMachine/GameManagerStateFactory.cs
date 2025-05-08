using System.Collections.Generic;

public enum EGameState
{
    MainMenu,
    SaveMenu,
    Loading,
    Running,
    Paused
}

public class GameManagerStateFactory
{
    GameManagerContext _gameManagerContext;
    Dictionary<EGameState, GameManagerAbstractState> _states = new Dictionary<EGameState, GameManagerAbstractState>();

    public GameManagerStateFactory(GameManagerContext currentContextManager)
    {
        _gameManagerContext = currentContextManager;
        _states[EGameState.MainMenu] = new GameManagerMainMenuState(_gameManagerContext, this);
        _states[EGameState.SaveMenu] = new GameManagerSaveMenuState(_gameManagerContext, this);
        _states[EGameState.Loading] = new GameManagerLoadingState(_gameManagerContext, this);
        _states[EGameState.Running] = new GameManagerRunState(_gameManagerContext, this);
        _states[EGameState.Paused] = new GameManagerPauseState(_gameManagerContext, this);
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
}
