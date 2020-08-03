using System.Collections.Generic;

namespace QuickAccess.DataStructures.Common.CharMatching
{
    public interface IDefineCharactersRange
    {
        string Description { get; }
        bool IsMatch(char character);
        bool IsMatchAny(IEnumerable<char> characters);
        IEnumerable<char> Matches(IEnumerable<char> characters);
    }
}