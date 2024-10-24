using UnityEngine;

public class CharacterIdleState : CharacterAbstractState
{
    public CharacterIdleState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        
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
        Debug.Log("IDLE STATE");
        if (Mathf.Abs(PlayerContextManager.MoveInput) != 0f)
        {
            SwitchState(PlayerStateFactory.MoveState());
        }
    }
    public override void InitializeSubStates()
    {

    }
}
