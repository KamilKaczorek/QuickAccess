namespace QuickAccess.Infrastructure.CharMatching
{
    public interface ICharactersRangeDefinition : IDefineCharactersRange, IAcceptCharactersRangeDefinitionVisitor
    {
        CharactersRangeDefinitionType DefinitionType { get; }
    }
}