using UnityEngine;

public class CharacterGroundedState : CharacterAbstractState
{
    public CharacterGroundedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubStates();
    }
    public override void UpdateState()
    {
        Debug.Log("GROUNDED STATE");
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
        Debug.Log("GROUNDED INITIALIZED");
        SetSubState(PlayerStateFactory.IdleState());
        SetSubState(PlayerStateFactory.MoveState());
    }
}
