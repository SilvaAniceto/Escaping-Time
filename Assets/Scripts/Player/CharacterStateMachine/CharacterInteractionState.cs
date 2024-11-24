using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractionState : CharacterAbstractState
{
    private IInteractable Interactable;

    public CharacterInteractionState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubStates();
    }
    public override void UpdateState()
    {
        if (PlayerContextManager.InteractionInput)
        {
            if (Interactable != null)
            {
                Interactable.ConfirmInteraction();
            }
        }

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
        PlayerContextManager.WaitingInteraction = false;

    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.JumpInput)
        {
            SwitchState(PlayerStateFactory.JumpState());
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
        
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.TriggerStay))
            {
                Interactable = interactable;
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.TriggerExit))
            {
                Interactable = null;
            }
        }

        SwitchState(PlayerStateFactory.GroundedState());
    }
}
