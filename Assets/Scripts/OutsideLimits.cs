using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OutsideLimits : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _targetPosition;

    private PlayerContextManager _playerContextManager;
    private Collider2D _collider;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; } = false;
    public bool MovableObject { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.TriggerEnter);

        _collider = GetComponent<Collider2D>();
    }

    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType)
    {
        p_gameObject.TryGetComponent(out _playerContextManager);

        _playerContextManager.SpawningPosition = _targetPosition.position;
        _playerContextManager.SpawningCharacter = true;
    }
    public void ConfirmInteraction()
    {

    }
}
