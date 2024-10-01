using UnityEngine;

public abstract class AbstractState
{
    private bool _isRootState = false;
    private ContextManager _contextManager;
    private StateFactory _stateFactory;
    private AbstractState _currentSuperState;
    private AbstractState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected ContextManager ContextManager { get { return _contextManager; } }
    protected StateFactory StateFactory { get { return _stateFactory; } }

    public AbstractState(ContextManager currentContextManager, StateFactory stateFactory)
    {
        _contextManager = currentContextManager;
        _stateFactory = stateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubStates();
    public void UpdateStates()
    {
        UpdateState();

        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }
    protected void SwitchState(AbstractState newState)
    {
        ExitState();

        newState.EnterState();

        if ( _isRootState )
        {
            _contextManager.CurrentState = newState;
        }
        else if ( _currentSuperState != null ) 
        {
            _currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(AbstractState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(AbstractState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
