using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OutsideLimits : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _targetPosition;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; } = false;

    private void Awake()
    {
        Interactions.Add(EInteractionType.Stay);
    }

    public void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        characterContextManager.SpawningPosition = _targetPosition.position;
        characterContextManager.SpawningCharacter = true;
    }
    public void ConfirmInteraction()
    {

    }

    public void InteractablePauseState(bool value)
    {

    }
}
