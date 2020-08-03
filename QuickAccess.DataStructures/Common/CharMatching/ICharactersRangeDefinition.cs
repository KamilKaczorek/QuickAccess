namespace QuickAccess.DataStructures.Common.CharMatching
{
    public interface ICharactersRangeDefinition : IDefineCharactersRange, IAcceptCharactersRangeDefinitionVisitor
    {
        CharactersRangeDefinitionType DefinitionType { get; }
    }
}