using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
   
    public sealed class UnaryOperatorFlexpression : Flexpression 
    {
        public static UnaryOperatorFlexpression Create(OverloadableCodeUnarySymmetricOperator unaryOperator,
                                                       IFlexpression argument)
        {
            return new UnaryOperatorFlexpression(unaryOperator, argument);
        }

        /// <inheritdoc />
        public override string Name => UnaryOperator.ToCodeRepresentation(Argument.Name);

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var argVisitationResult = Argument.AcceptVisitor(visitor);
            var result = visitor.VisitUnaryOperator(UnaryOperator, argVisitationResult);
            return result;
        }

        public OverloadableCodeUnarySymmetricOperator UnaryOperator { get; }
        public IFlexpression Argument { get; }

        public UnaryOperatorFlexpression(OverloadableCodeUnarySymmetricOperator unaryOperator, IFlexpression argument)
        {
            UnaryOperator = unaryOperator;
            Argument = argument;
        }
    }
}