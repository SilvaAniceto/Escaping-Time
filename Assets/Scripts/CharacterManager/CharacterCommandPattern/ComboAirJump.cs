using System.Collections.Generic;
using System.Linq;

public class ComboAirJump : ICharacterComboCommand
{
    private CharacterContextManager _characterContextManager;
    public int ComboLength { get; private set; }

    public ComboAirJump(CharacterContextManager characterContextManager)
    {  
        _characterContextManager = characterContextManager;
        ComboLength = 2;
    }

    public bool IsFirstConditionMet(ICharacterActionCommand firstCommand)
    {
        return firstCommand is CharacterJumpCommand;
    }
    public bool IsMatch(IEnumerable<ICharacterActionCommand> sequence)
    {
        var sequenceArray = sequence.Take(ComboLength).ToArray();

        if (sequenceArray.Length < ComboLength)
        {
            return false;
        }

        var first  = sequenceArray[0];
        var second = sequenceArray[1];

        return first is CharacterJumpCommand && second is CharacterJumpCommand;
    }
    public ICharacterActionCommand GetResultingComboCommand()
    {
        return new CharacterAirJumpCommand(_characterContextManager);
    }
}
