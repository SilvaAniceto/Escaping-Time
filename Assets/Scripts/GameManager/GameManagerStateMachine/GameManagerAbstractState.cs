public abstract class GameManagerAbstractState
{
    public GameManagerAbstractState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager)
    {
        _gameContextManager = gameContextManager;
        _gameManagerStateFactory = gameManagerStateFactory;
        _gameUIInputsManager = gameUIInputsManager;
    }

    private bool _isRootState = false;
    private GameContextManager _gameContextManager;
    private GameManagerStateFactory _gameManagerStateFactory;
    private GameUIInputsManager _gameUIInputsManager;
    private GameManagerAbstractState _currentSuperState;
    private GameManagerAbstractState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected GameContextManager GameContextManager { get { return _gameContextManager; } }
    public GameManagerStateFactory GameManagerStateFactory { get { return _gameManagerStateFactory; } }
    public GameUIInputsManager GameUIInputsManager { get { return _gameUIInputsManager; } }
    protected GameManagerAbstractState CurrentSuperState { get { return _currentSuperState; } }
    protected GameManagerAbstractState CurrentSubState { get { return _currentSubState; } }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void CheckSwitchSubStates();

    public void UpdateStates()
    {
        UpdateState();

        CheckSwitchStates();
        CheckSwitchSubStates();

        if (GameUIInputsManager.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject != null)
            {
                GameAudioManager.Instance.PlaySFX("Menu_Select");

                GameContextManager.WaitSeconds(null, GameAudioManager.Instance.AudioClipLength("Menu_Select"));
            }
        }

        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(GameManagerAbstractState newState)
    {
        ExitState();

        if (_isRootState)
        {
            _gameContextManager.CurrentState = newState;
            _gameContextManager.CurrentState.EnterState();
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(GameManagerAbstractState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(GameManagerAbstractState newSubState)
    {
        if (_currentSubState != null)
        {
            _currentSubState.ExitState();
        }

        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        _currentSubState.EnterState();
    }
}
