public class CharacterCameraTiltUpDirectionCommand : ICharacterActionCommand
{
    private ICameraTiltController _cameraTiltController;

    public CharacterCameraTiltUpDirectionCommand(ICameraTiltController cameraTiltController)
    {
        _cameraTiltController = cameraTiltController;
    }

    public void ExecuteCommand()
    {
        _cameraTiltController.CameraTilt = (int)ECameraTiltDirection.Up;
    }
}