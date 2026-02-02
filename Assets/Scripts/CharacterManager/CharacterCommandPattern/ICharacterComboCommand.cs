using System.Collections.Generic;

public interface ICharacterComboCommand
{
    public int ComboLength { get; }
    public bool IsFirstConditionMet(ICharacterActionCommand firstCommand);
    public bool IsMatch(IEnumerable<ICharacterActionCommand> sequence);
    public ICharacterActionCommand GetResultingComboCommand();
}
