using UnityEngine;

public class OutsideLimits : InteractableItem
{
    [SerializeField] protected Transform _targetPosition;

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Stay);
    }

    public override void SetCharacterReset(CharacterContextManager characterContextManager)
    {
        characterContextManager.SpawningPosition = _targetPosition.position;
        characterContextManager.ResetCharacter();
    }
    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        SetCharacterReset(characterContextManager);
    }
    public override void ConfirmInteraction()
    {

    }

    public override void InteractablePauseState(bool value)
    {

    }
}
