using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour, IInteractable
{
    public Animator Animator { get; set; }
    public AudioSource AudioSource { get; set; }
    public Collider2D Collider { get; set; }
    public List<EInteractionType> Interactions { get ; set ; } = new List<EInteractionType>();
    public bool Activated { get; set; }

    public virtual void Awake()
    {
        Animator = GetAnimator();
        AudioSource = GetAudioSource();
        Collider = GetCollider2D();

        GameContextManager.OnRunOrPauseStateChanged.AddListener(InteractablePauseState);
    }
    public virtual void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {

    }
    public virtual void SetCharacterReset(CharacterContextManager characterContextManager)
    {

    }
    public virtual void ConfirmInteraction()
    {

    }
    public virtual void InteractablePauseState(bool value)
    {
        if (Animator != null)
        {
            Animator.enabled = value;
        }

        if (AudioSource != null)
        {
            AudioSource.enabled = value;
        }

        this.enabled = value;
    }

    public Animator GetAnimator()
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null) return animator;

        animator = GetComponentInChildren<Animator>();

        if (animator != null) return animator; else return null;
    }
    public Collider2D GetCollider2D()
    {
        Collider2D collider = GetComponent<Collider2D>();

        if (collider != null) return collider;

        collider = GetComponentInChildren<Collider2D>();

        if (collider != null) return collider; else return null;
    }
    public AudioSource GetAudioSource()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null) return audioSource;

        audioSource = GetComponentInChildren<AudioSource>();

        if (audioSource != null) return audioSource; else return null;
    }
}
