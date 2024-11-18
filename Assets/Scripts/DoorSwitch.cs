using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour, IInteractable
{
    private PlayerContextManager _playerContextManager;
    private Collider2D _collider;
    private Animator _animator;

    public List<EInteractionType> Interactions { get ; set ; } = new List<EInteractionType>();
    public bool Activated { get ; set ; } = false;
    public bool MovableObject { get ; set ; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.TriggerStay);

        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType)
    {
        p_gameObject.TryGetComponent(out _playerContextManager);

        switch (p_interactionType)
        {
            case EInteractionType.TriggerStay:
                if (_playerContextManager.KeyItem != 0 && !Activated)
                {
                    _playerContextManager.WaitingInteraction = true;
                }
                break;
        }
    }
    public void ConfirmInteraction()
    {
        _playerContextManager.KeyItem--;
        Activated = true;
        _collider.enabled = false;
        _animator.Play("Activated_Switch");
    }
}