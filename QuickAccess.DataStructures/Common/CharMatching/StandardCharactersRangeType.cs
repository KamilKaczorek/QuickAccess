namespace QuickAccess.DataStructures.Common.CharMatching
{
    public enum StandardCharactersRangeType
    {
        Empty = 0,
        Positive = 0x01,
        Negative = 0x02,
        Invalid = Positive | Negative
    }
}