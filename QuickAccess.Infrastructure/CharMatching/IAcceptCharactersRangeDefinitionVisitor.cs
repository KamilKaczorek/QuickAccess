namespace QuickAccess.Infrastructure.CharMatching
{
    public interface IAcceptCharactersRangeDefinitionVisitor
    {
        TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor);
    }
}