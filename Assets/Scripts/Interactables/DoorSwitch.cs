using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour, IInteractable
{
    private Collider2D _collider;
    private Animator _animator;

    public List<EInteractionType> Interactions { get ; set ; } = new List<EInteractionType>();
    public bool Activated { get ; set ; } = false;
    public bool MovableObject { get ; set ; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Stay);

        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    public void SetInteraction(CharacterContextManager characterContextManager)
    {
       
    }
    public void ConfirmInteraction()
    {
        Activated = true;
        _collider.enabled = false;
        _animator.Play("Activated_Switch");
    }
}