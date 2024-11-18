using System.Collections.Generic;
using UnityEngine;

public enum EInteractionType
{
    Enter,
    Stay,
    Exit,
    TriggerEnter,
    TriggerStay,
    TriggerExit
}
public interface IInteractable
{
    public List<EInteractionType> Interactions { get; set; }
    public bool Activated { get; set; }
    public bool MovableObject { get; set; }
    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType);
    public void ConfirmInteraction();
}
