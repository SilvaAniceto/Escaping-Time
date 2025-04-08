using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable
{
    private Animator _animator;

    public List<EInteractionType> Interactions { get ; set ; } = new List<EInteractionType>();
    public bool Activated { get; set; }
    public bool MovableObject { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Stay);

        _animator = GetComponent<Animator>();

        GameManagerContext.OnRunOrPauseStateChanged.AddListener(InteractablePauseState);
    }

    private void OnDestroy()
    {
        
    }

    public void SetInteraction(CharacterContextManager characterContextManager)
    {
        Destroy(gameObject);
    }
    public void ConfirmInteraction()
    {

    }

    public void InteractablePauseState(bool value)
    {
        if (_animator != null)
        {
            _animator.enabled = value;
        }

        this.enabled = value;
    }
}
