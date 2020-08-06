namespace QuickAccess.DataStructures.Common.CharMatching.Categories
{
    public enum StandardCharacterCategory
    {
        None = 0,

        UpperLetter = StandardCharacterCategories.UpperLetter,
        LowerLetter = StandardCharacterCategories.LowerLetter,
        Digit = StandardCharacterCategories.Digit,
        Underscore = StandardCharacterCategories.Underscore,

        Space = StandardCharacterCategories.Space,
        Tab = StandardCharacterCategories.Tab,
        NewLine = StandardCharacterCategories.NewLine,
        Return = StandardCharacterCategories.Return,

        NonWordCharacterOrWhiteSpace = StandardCharacterCategories.NonWordCharacterOrWhiteSpace,
    }
}