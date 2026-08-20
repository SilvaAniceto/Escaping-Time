public class CharacterCameraTiltDownDirectionCommand : ICharacterActionCommand
{
    private ICameraTiltController _cameraTiltController;

    public CharacterCameraTiltDownDirectionCommand(ICameraTiltController cameraTiltController)
    {
        _cameraTiltController = cameraTiltController;
    }

    public void ExecuteCommand()
    {
        _cameraTiltController.CameraTilt = (int)ECameraTiltDirection.Down;
    }
}