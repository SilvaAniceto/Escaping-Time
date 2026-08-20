public static class ServiceLocator
{
    private static IAudioManager _audioManager;
    private static IScoreManager _scoreManager;
    private static IUIManager _uiManager;
    private static IGameFlowManager _gameFlowManager;
    private static ISaveSystem _saveSystem;

    public static IAudioManager AudioManager
    {
        get { return _audioManager; }
        set { _audioManager = value; }
    }

    public static IScoreManager ScoreManager
    {
        get { return _scoreManager; }
        set { _scoreManager = value; }
    }

    public static IUIManager UIManager
    {
        get { return _uiManager; }
        set { _uiManager = value; }
    }

    public static IGameFlowManager GameFlowManager
    {
        get { return _gameFlowManager; }
        set { _gameFlowManager = value; }
    }

    public static ISaveSystem SaveSystem
    {
        get { return _saveSystem; }
        set { _saveSystem = value; }
    }
}