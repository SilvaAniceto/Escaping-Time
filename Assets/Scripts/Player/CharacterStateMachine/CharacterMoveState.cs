using UnityEngine;

public class CharacterMoveState : CharacterAbstractState
{
    public CharacterMoveState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        
    }
    public override void UpdateState()
    {
        Debug.Log("MOVE STATE");
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
        if (Mathf.Abs(PlayerContextManager.MoveInput) == 0f)
        {
            SwitchState(PlayerStateFactory.IdleState());
        }
    }
    public override void InitializeSubStates()
    {
        
    }
}
