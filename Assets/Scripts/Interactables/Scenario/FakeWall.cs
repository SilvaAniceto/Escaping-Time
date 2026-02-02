using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FakeWall : InteractableItem
{
    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);

        Collider.isTrigger = true;

        gameObject.SetActive(true);
    }

    public override void ConfirmInteraction()
    {
        
    }

    public override void InteractablePauseState(bool value)
    {
        base.Awake();
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        gameObject.SetActive(false);
    }
}
