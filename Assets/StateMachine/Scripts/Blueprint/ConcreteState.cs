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
        Debug.Log("ON FIXED UPDATE");
    }
    public override void LateUpdateState()
    {
        Debug.Log("ON LATE UPDATE");
    }
    public override void ExitState()
    {   

    }
    public override void CheckSwitchStates()
    {
        Debug.Log("ON UPDATE");
        //SwitchState(StateFactory.State());
    }
    public override void InitializeSubStates()
    {
        //SetSubState(StateFactory.State());
    }
}
