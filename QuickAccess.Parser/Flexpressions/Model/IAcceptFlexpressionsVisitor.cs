namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IAcceptFlexpressionsVisitor
    {
        TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor);
    }
}