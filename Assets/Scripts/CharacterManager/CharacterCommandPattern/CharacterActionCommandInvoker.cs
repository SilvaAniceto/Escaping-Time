public class CharacterActionCommandInvoker
{
    public void ExecuteActionCommand(ICharacterActionCommand actionCommand)
    {
        actionCommand.ExecuteCommand();
    }
}
