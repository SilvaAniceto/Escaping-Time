using System.Collections.Generic;
using UnityEngine;

public enum EInteractionType
{
    Enter,
    Stay,
    Exit
}
public interface IInteractable
{
    public Animator Animator { get; set; }
    public AudioSource AudioSource { get; set; }
    public Collider2D Collider { get; set; }
    public List<EInteractionType> Interactions { get; set; }
    public bool Activated { get; set; }
    public void InteractablePauseState(bool value);
    public void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType);
    public void ConfirmInteraction();
    public Animator GetAnimator();
    public Collider2D GetCollider2D();
    public AudioSource GetAudioSource();
}