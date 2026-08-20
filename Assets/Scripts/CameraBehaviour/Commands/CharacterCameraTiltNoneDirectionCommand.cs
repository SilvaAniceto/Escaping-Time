public class CharacterCameraTiltNoneDirectionCommand : ICharacterActionCommand
{
    private ICameraTiltController _cameraTiltController;

    public CharacterCameraTiltNoneDirectionCommand(ICameraTiltController cameraTiltController)
    {
        _cameraTiltController = cameraTiltController;
    }

    public void ExecuteCommand()
    {
        _cameraTiltController.CameraTilt = (int)ECameraTiltDirection.None;
    }
}