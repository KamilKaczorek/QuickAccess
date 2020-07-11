using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public static class UnaryOperatorFlexpression
    {
        public static UnaryOperatorFlexpression<TConstraint> Create<TConstraint>(OverloadableCodeUnarySymmetricOperator unaryOperator,
                                                                                     IFlexpression<TConstraint> argument) where TConstraint : IFlexpressionConstraint
        {
            return new UnaryOperatorFlexpression<TConstraint>(unaryOperator, argument);
        }
    }

    public sealed class UnaryOperatorFlexpression<TConstraint> : Flexpression<TConstraint> where TConstraint : IFlexpressionConstraint
    {
        /// <inheritdoc />
        public override string Name => UnaryOperator.ToCodeRepresentation(Argument.Name);

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var argVisitationResult = Argument.AcceptVisitor(visitor);
            var result = visitor.VisitUnaryOperator(UnaryOperator, argVisitationResult);
            return result;
        }

        public OverloadableCodeUnarySymmetricOperator UnaryOperator { get; }
        public IFlexpression<TConstraint> Argument { get; }

        public UnaryOperatorFlexpression(OverloadableCodeUnarySymmetricOperator unaryOperator, IFlexpression<TConstraint> argument)
        {
            Constraint.ValidateUnaryOperatorAllowed(unaryOperator);
            UnaryOperator = unaryOperator;
            Argument = argument;
        }
    }
}