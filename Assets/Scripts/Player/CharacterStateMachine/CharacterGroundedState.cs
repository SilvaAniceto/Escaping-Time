using UnityEngine;

public class CharacterGroundedState : CharacterAbstractState
{
    public CharacterGroundedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        PlayerContextManager.CoyoteTime = 0.15f;

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
        if (PlayerContextManager.MoveInput != 0)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
        else
        {
            SetSubState(PlayerStateFactory.IdleState());
        }

        if (PlayerContextManager.JumpInput)
        {
            PlayerContextManager.PerformingJump = true;

            SwitchState(PlayerStateFactory.UngroundedState());
        }
    }
    public override void InitializeSubStates()
    {

    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public override void OnCollisionStay(Collision collision)
    {
       
    }

    public override void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
       
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        SwitchState(PlayerStateFactory.UngroundedState());
    }
}
