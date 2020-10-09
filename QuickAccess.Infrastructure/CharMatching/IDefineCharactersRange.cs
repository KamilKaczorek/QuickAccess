namespace QuickAccess.Infrastructure.CharMatching
{
    public interface IDefineCharactersRange : IDetermineCharacterMatch
    {
        string Description { get; }
    }
}