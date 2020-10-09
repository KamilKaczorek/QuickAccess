using QuickAccess.Infrastructure.CharMatching.Categories;

namespace QuickAccess.Infrastructure.CharMatching
{
    public interface IDefineCharactersCategory<out TCategory> : IDetermineCharacterMatch
    {
        CharacterCategoryType CategoryType { get; }
        TCategory Category { get; }
    }
}