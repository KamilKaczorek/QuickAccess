using System.Collections.Generic;

namespace QuickAccess.DataStructures.Common.CharMatching
{
    public interface IDetermineCharacterMatch
    {
        bool IsMatch(char character);
        bool IsMatchAny(IEnumerable<char> characters);
        IEnumerable<char> Matches(IEnumerable<char> characters);
    }
}