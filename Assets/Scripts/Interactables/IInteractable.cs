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
    public List<EInteractionType> Interactions { get; set; }
    public bool Activated { get; set; }
    public void InteractablePauseState(bool value);
    public void SetInteraction(CharacterContextManager characterContextManager);
    public void ConfirmInteraction();
}
