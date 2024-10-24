using UnityEngine;

public class CharacterFallState : CharacterAbstractState
{
    public CharacterFallState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
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
    public override void LateUpdateState()
    {

    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        //SwitchState(StateFactory.State());
    }
    public override void InitializeSubStates()
    {
        //SetSubState(StateFactory.State());
    }
}
