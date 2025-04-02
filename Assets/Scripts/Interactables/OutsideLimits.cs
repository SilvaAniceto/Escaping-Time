using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OutsideLimits : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _targetPosition;

    private Collider2D _collider;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; } = false;
    public bool MovableObject { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Stay);

        _collider = GetComponent<Collider2D>();
    }

    public void SetInteraction(CharacterContextManager characterContextManager)
    {
        characterContextManager.SpawningPosition = _targetPosition.position;
        characterContextManager.SpawningCharacter = true;
    }
    public void ConfirmInteraction()
    {

    }
}
