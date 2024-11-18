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

    }
    public override void CheckSwitchStates()
    {

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
        
    }
}
