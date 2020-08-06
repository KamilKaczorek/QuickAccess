using QuickAccess.DataStructures.Common.CharMatching.Categories;

namespace QuickAccess.DataStructures.Common.CharMatching
{
    public interface IDefineCharactersCategory<out TCategory> : IDetermineCharacterMatch
    {
        CharacterCategoryType CategoryType { get; }
        TCategory Category { get; }
    }
}