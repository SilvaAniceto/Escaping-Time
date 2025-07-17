public abstract class GameManagerAbstractState
{
    public GameManagerAbstractState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager)
    {
        _gameManagerContext = gameManagerContext;
        _gameManagerStateFactory = gameManagerStateFactory;
        _gameUIInputsManager = gameUIInputsManager;
    }

    private bool _isRootState = false;
    private GameManagerContext _gameManagerContext;
    private GameManagerStateFactory _gameManagerStateFactory;
    private GameUIInputsManager _gameUIInputsManager;
    private GameManagerAbstractState _currentSuperState;
    private GameManagerAbstractState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected GameManagerContext GameManagerContext { get { return _gameManagerContext; } }
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
            _gameManagerContext.CurrentState = newState;
            _gameManagerContext.CurrentState.EnterState();
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
