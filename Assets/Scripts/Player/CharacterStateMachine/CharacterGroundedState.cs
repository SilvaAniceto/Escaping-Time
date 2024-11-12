using System.Collections;
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
        ProccessMoveInput(PlayerContextManager.MoveInput);

        ProccessJumpInput(PlayerContextManager.JumpInput);
    }

    public override void InitializeSubStates()
    {

    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public override void OnCollisionStay(Collision2D collision)
    {
       
    }

    public override void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            interactable.SetInteraction();
        }
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        SwitchState(PlayerStateFactory.UngroundedState());
    }

    protected override void ProccessJumpInput(bool actionInput)
    {
        if (actionInput)
        {
            PlayerContextManager.PerformingJump = true;

            SwitchState(PlayerStateFactory.UngroundedState());
        }
    }

    protected override void ProccessMoveInput(float moveInput)
    {
        base.ProccessMoveInput(moveInput);

        if (moveInput != 0 && !PlayerContextManager.IsWallColliding)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
        else
        {
            SetSubState(PlayerStateFactory.IdleState());
        }
    }
}
