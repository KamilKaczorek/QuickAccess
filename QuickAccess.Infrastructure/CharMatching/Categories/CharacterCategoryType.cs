namespace QuickAccess.Infrastructure.CharMatching.Categories
{
    public enum CharacterCategoryType
    {
        None = 0,

        /// <summary>
        /// Letters, Digits, Underscore, Space, Tab, New-line characters
        /// </summary>
        Standard = 0x01,

        /// <summary>
        /// All controls beside new line characters, tab 
        /// </summary>
        Control = 0x02,

        /// <summary>
        /// &lt;, &gt;, {, }, [, ], (, ),
        /// </summary>
        Bracket = 0x04,

        /// <summary>
        /// +-*/%=^|~\&amp;
        /// </summary>
        Operator = 0x10,

        /// <summary>
        /// .!?`'",;…—
        /// </summary>
        PunctuationMark = 0x20,

        /// <summary>
        /// @#$£™℠®©℗
        /// </summary>
        SpecialSign = 0x40,

        /// <summary>
        /// 
        /// </summary>
        Other = 0x40_00,
    }
}