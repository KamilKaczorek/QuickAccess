namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class CharFlexpression<TConstraint> : Flexpression<TConstraint> where TConstraint : IFlexpressionConstraint
    {
        public override string Name => $"'{Content}'";

        public char Content { get; }

        public CharFlexpression(char content)
        {
            Constraint.ValidateCharAllowed(content);
            Content = content; 
        }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = visitor.VisitChar(Content);
            return visitationResult;
        }
    }

    public static class CharFlexpression
    {
        public static Flexpression<TConstraint> Create<TConstraint>(char content)
            where TConstraint : IFlexpressionConstraint
        {
            return new CharFlexpression<TConstraint>(content);
        }
    }
}