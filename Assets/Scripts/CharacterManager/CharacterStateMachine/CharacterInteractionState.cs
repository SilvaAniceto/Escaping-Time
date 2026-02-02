using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractionState : CharacterAbstractState
{
    private IInteractable Interactable;

    public CharacterInteractionState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        
    }
    public override void UpdateState()
    {
        if (PlayerInputManager.InteractionInput)
        {
            if (Interactable != null)
            {
                Interactable.ConfirmInteraction();
            }
        }
    }
    public override void FixedUpdateState()
    {

    }

    public override void LateUpdateState()
    {

    }
    public override void ExitState()
    {
        Interactable = null;

    }
    public override void CheckSwitchStates()
    {
        //if (PlayerInputManager.StartJumpInput)
        //{
        //    SwitchState(CharacterStateFactory.JumpState());
        //}
    }
    public override void CheckSwitchSubStates()
    {
        if (CurrentSubState == null)
        {
            if (PlayerInputManager.MoveInput != 0)
            {
                SetSubState(CharacterStateFactory.MoveState());
            }
            else if (PlayerInputManager.MoveInput == 0)
            {
                SetSubState(CharacterStateFactory.IdleState());
            }
        }
        else
        {
            if (PlayerInputManager.MoveInput != 0 && CurrentSubState == CharacterStateFactory.IdleState())
            {
                SetSubState(CharacterStateFactory.MoveState());
            }
            else if (PlayerInputManager.MoveInput == 0 && CurrentSubState == CharacterStateFactory.MoveState())
            {
                SetSubState(CharacterStateFactory.IdleState());
            }
        }
    }

    public override Quaternion CurrentLookRotation()
    {
        return new Quaternion();
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
            if (interactable.Interactions.Contains(EInteractionType.Stay) && Interactable == null)
            {
                Interactable = interactable;
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        SwitchState(CharacterStateFactory.GroundedState());
    }
}
