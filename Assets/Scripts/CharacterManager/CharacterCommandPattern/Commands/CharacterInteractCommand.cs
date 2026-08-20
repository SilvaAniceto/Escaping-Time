public class CharacterInteractCommand : ICharacterActionCommand
{
    private CharacterContextManager _characterContextManager;

    public CharacterInteractCommand(CharacterContextManager characterContextManager)
    {
        _characterContextManager = characterContextManager;
    }

    public void ExecuteCommand()
    {
        if (_characterContextManager.InteractableGameObject == null)
        {
            return;
        }

        IConfirmable confirmable = _characterContextManager.InteractableGameObject.GetComponent<IConfirmable>();
        if (confirmable != null)
        {
            confirmable.ConfirmInteraction(_characterContextManager);
        }
    }
}