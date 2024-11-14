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
        if (PlayerContextManager.Damaged)
        {
            SwitchState(PlayerStateFactory.DamagedState());
        }

        ProccessMoveInput(PlayerContextManager.MoveInput);

        ProccessJumpInput(PlayerContextManager.JumpInput);
    }

    public override void InitializeSubStates()
    {

    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Enter))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Enter);
            }
        }
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
            if (interactable.Interactions.Contains(EInteractionType.Enter))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Enter);
            }
        }
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Stay))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Stay);
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Exit))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Exit);
            }
        }

        if (!PlayerContextManager.PerformingJump)
        {
            SwitchState(PlayerStateFactory.UngroundedState());
        }
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
