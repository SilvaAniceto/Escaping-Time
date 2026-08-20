public interface IInteractableBehavior
{
    public EInteractionType[] InteractionType { get; }
    void Execute(CharacterContextManager context, EInteractionType interactionType);
}