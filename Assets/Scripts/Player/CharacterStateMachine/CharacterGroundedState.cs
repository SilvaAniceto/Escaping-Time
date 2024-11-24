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
        if (PlayerContextManager.JumpInput)
        {
            SwitchState(PlayerStateFactory.JumpState());
        }

        if (PlayerContextManager.WaitingInteraction)
        {
            SwitchState(PlayerStateFactory.InteractionState());
        }
    }

    public override void InitializeSubStates()
    {
        if (PlayerContextManager.MoveInput != 0)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
        else if (PlayerContextManager.MoveInput == 0)
        {
            SetSubState(PlayerStateFactory.IdleState());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.CompareTag("Ground"))
        {
            SwitchState(PlayerStateFactory.UngroundedState());
        }
    }
}
