namespace QuickAccess.Infrastructure.CharMatching
{
    public interface IDefineSpecialCharacters
    {
        /// <summary>Determines whether the specified character is a special regex character (symbol).</summary>
        /// <param name="character">The character.</param>
        /// <returns><c>true</c> if is a special regex character; otherwise, <c>false</c>.</returns>
        bool IsSpecialCharacter(char character);

        /// <summary>Determines whether the specified character is a tab (<c>'\t'</c>) character.</summary>
        /// <param name="character">The character.</param>
        /// <returns><c>true</c> if is a tab; otherwise, <c>false</c>.</returns>
        bool IsTab(char character);

        /// <summary>Determines whether the specified character is a white space character.</summary>
        /// <param name="character">The character.</param>
        /// <returns><c>true</c> if is a white space; otherwise, <c>false</c>.</returns>
        bool IsWhiteSpaceCharacter(char character);
    }
}