using System;
using System.Diagnostics.Contracts;

namespace QuickAccess.DataStructures.Common.CharMatching.Categories
{
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class CharCategoryAttribute : Attribute
    {

    }


    public enum CharacterCategory
    {
        None,
        Bracket,
        StandardRange
    }

    [CharCategory]
    public enum Bracket
    {
        None = 0,

        OpeningRoundBracket = Brackets.OpeningRoundBracket,
        OpeningSquareBracket = Brackets.OpeningSquareBracket,
        OpeningCurlyBracket = Brackets.OpeningCurlyBracket,
        OpeningAngleBracket = Brackets.OpeningAngleBracket,

        ClosingRoundBracket = Brackets.ClosingRoundBracket,
        ClosingSquareBracket = Brackets.ClosingSquareBracket,
        ClosingCurlyBracket = Brackets.ClosingCurlyBracket,
        ClosingAngleBracket = Brackets.ClosingAngleBracket,
    }

    [Flags]
    [CharCategory]
    public enum Brackets
    {
        None = 0,

        OpeningRoundBracket = 0x01,
        ClosingRoundBracket = 0x02,

        OpeningAngleBracket = 0x04,
        ClosingAngleBracket = 0x08,

        OpeningSquareBracket = 0x10,
        ClosingSquareBracket = 0x20,

        OpeningCurlyBracket = 0x40,
        ClosingCurlyBracket = 0x80,
        

        RoundBracket = OpeningRoundBracket | ClosingRoundBracket,
        SquareBracket = OpeningSquareBracket | ClosingSquareBracket,
        CurlyBracket = OpeningCurlyBracket | ClosingCurlyBracket,
        AngleBracket = OpeningAngleBracket | ClosingAngleBracket,

        OpeningBracket = OpeningSquareBracket | OpeningAngleBracket | OpeningCurlyBracket | OpeningRoundBracket,
        ClosingBracket = ClosingSquareBracket | ClosingAngleBracket | ClosingCurlyBracket | ClosingRoundBracket,

        Bracket = OpeningBracket | ClosingBracket,
    }

    public static class BracketsExtensions
    {
        public const char Undefined = '\0';

        [Pure]
        public static Bracket CategorizeByBracketType(char character)
        {
            return character switch
            {
                '(' => Bracket.OpeningRoundBracket,
                ')' => Bracket.ClosingRoundBracket,
                '<' => Bracket.OpeningAngleBracket,
                '>' => Bracket.ClosingAngleBracket,
                '[' => Bracket.OpeningSquareBracket,
                ']' => Bracket.ClosingSquareBracket,
                '{' => Bracket.OpeningCurlyBracket,
                '}' => Bracket.ClosingCurlyBracket,
                _ => Bracket.None,
            };
        }

        [Pure]
        public static bool IsAny(this char character, Brackets brackets)
        {
            var bracket = CategorizeByBracketType(character);
            return ((int) brackets & (int) bracket) != 0;
        }

        [Pure]
        public static bool IsOpening(this Bracket bracket)
        {
            return ((int) Brackets.OpeningBracket & (int) bracket) != 0;
        }

        [Pure]
        public static bool IsClosing(this Bracket bracket)
        {
            return ((int) Brackets.ClosingBracket & (int) bracket) != 0;
        }

        [Pure]
        public static bool IsAny(this Bracket bracket, Brackets brackets = Brackets.Bracket)
        {
            return ((int) brackets & (int) bracket) != 0;
        }

        [Pure]
        public static Bracket Opposite(this Bracket bracket)
        {
            return bracket switch
            {
                Bracket.OpeningRoundBracket => Bracket.ClosingRoundBracket,
                Bracket.ClosingRoundBracket => Bracket.OpeningRoundBracket,
                Bracket.OpeningAngleBracket => Bracket.ClosingAngleBracket,
                Bracket.ClosingAngleBracket => Bracket.OpeningAngleBracket,
                Bracket.OpeningSquareBracket => Bracket.ClosingSquareBracket,
                Bracket.ClosingSquareBracket => Bracket.OpeningSquareBracket,
                Bracket.OpeningCurlyBracket => Bracket.ClosingCurlyBracket,
                Bracket.ClosingCurlyBracket => Bracket.OpeningCurlyBracket,
                _ => Bracket.None,
            };
        }


        [Pure]
        public static char ToCharacter(this Bracket bracket)
        {
            return bracket switch
            {
                Bracket.OpeningRoundBracket => '(',
                Bracket.ClosingRoundBracket => ')',
                Bracket.OpeningAngleBracket => '<',
                Bracket.ClosingAngleBracket => '>',
                Bracket.OpeningSquareBracket => '[',
                Bracket.ClosingSquareBracket => ']',
                Bracket.OpeningCurlyBracket => '{',
                Bracket.ClosingCurlyBracket => '}',
                _ => Undefined,
            };
        }
    }
}