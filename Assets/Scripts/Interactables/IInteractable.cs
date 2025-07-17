using System.Collections.Generic;

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
    public void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType);
    public void ConfirmInteraction();
}
