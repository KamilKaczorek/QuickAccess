namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class StringFlexpression<TConstraint> : Flexpression<TConstraint> where TConstraint : IFlexpressionConstraint
    {
        public override string Name => $"\"{Content}\"";
        public StringFlexpression(string content) { Content = content; }
        public string Content { get; }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            return visitor.VisitString(Content);
        }

        public override int GetHashCode() { return string.IsNullOrEmpty(Content) ? 0 : Content.GetHashCode(); }
    }

    public static class StringFlexpression
    {
        public static Flexpression<TConstraint> Create<TConstraint>(string content)
            where TConstraint : IFlexpressionConstraint
        {
            return new StringFlexpression<TConstraint>(content);
        }
    }
}