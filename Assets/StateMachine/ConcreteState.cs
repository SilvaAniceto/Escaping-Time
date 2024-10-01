using UnityEngine;

public class ConcreteState : AbstractState
{
    public ConcreteState(ContextManager currentContextManager, StateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubStates();
    }
    public override void UpdateState() 
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {

    }
    public override void ExitState()
    {   

    }
    public override void CheckSwitchStates()
    {
        SwitchState(StateFactory.State());
    }
    public override void InitializeSubStates()
    {
        SetSubState(StateFactory.State());
    }
}
