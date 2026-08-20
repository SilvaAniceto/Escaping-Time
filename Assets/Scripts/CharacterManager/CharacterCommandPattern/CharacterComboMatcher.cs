using System.Collections.Generic;
using System.Linq;

public class CharacterComboMatcher
{
    private List<ICharacterComboCommand> _comboRules;

    public CharacterComboMatcher(List<ICharacterComboCommand> comboRules)
    {
        _comboRules = comboRules;
    }

    public ICharacterActionCommand CheckSequenceForCombo(Queue<ICharacterActionCommand> comboSequence)
    {
        if (comboSequence.Count < 2)
        {
            return null;
        }

        for (int startIndex = 0; startIndex <= comboSequence.Count; startIndex++)
        {
            var subsequence = GetSubsequence(comboSequence, startIndex);

            foreach (ICharacterComboCommand rule in _comboRules)
            {
                if (rule.IsMatch(subsequence))
                {
                    return rule.GetResultingComboCommand();
                }
            }
        }

        return null;
    }

    private IEnumerable<ICharacterActionCommand> GetSubsequence(Queue<ICharacterActionCommand> comboSequence, int startIndex)
    {
        return comboSequence.Skip(startIndex);
    }
}