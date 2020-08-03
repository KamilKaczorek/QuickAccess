namespace QuickAccess.DataStructures.Common.CharMatching
{
    public interface IAcceptCharactersRangeDefinitionVisitor
    {
        TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor);
    }
}