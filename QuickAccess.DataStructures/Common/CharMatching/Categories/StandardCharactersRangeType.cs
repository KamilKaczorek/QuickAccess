namespace QuickAccess.DataStructures.Common.CharMatching.Categories
{
    public enum StandardCharactersPatternType
    {
        Empty = 0,
        Positive = 0x01,
        Negative = 0x02,
        Invalid = Positive | Negative
    }
}