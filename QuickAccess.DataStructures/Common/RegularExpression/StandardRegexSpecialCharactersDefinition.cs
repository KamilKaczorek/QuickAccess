using System.Collections.Generic;
using QuickAccess.DataStructures.Common.CharMatching;

namespace QuickAccess.DataStructures.Common.RegularExpression
{
    public class StandardRegexSpecialCharactersDefinition : IDefineSpecialCharacters
    {
        private static readonly HashSet<char> SpecialRegexCharacters = new HashSet<char> { '\\', '^', '$', '.', '|', '?', '*', '+', '(', ')', '{', '}' };

        public bool IsSpecialCharacter(char ch)
        {
            return SpecialRegexCharacters.Contains(ch);
        }

        public bool IsTab(char ch)
        {
            return ch == '\t';
        }

        public bool IsWhiteSpaceCharacter(char ch)
        {
            return char.IsWhiteSpace(ch);
        }
    }
}