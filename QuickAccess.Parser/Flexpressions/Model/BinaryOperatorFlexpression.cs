using System.Collections.Generic;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public static class BinaryOperatorFlexpression
    {


        public static Flexpression<TConstraint> Create<TConstraint>(
            OverloadableCodeBinarySymmetricOperator binaryOperator, IFlexpression<TConstraint> leftArgument, IFlexpression<TConstraint> rightArgument) where TConstraint : IFlexpressionConstraint
        {
            if (leftArgument is BinaryOperatorFlexpression<TConstraint> binOperator && binOperator.BinaryOperator == binaryOperator)
            {
                return new MultiArgBinaryOperatorFlexpression<TConstraint>(binaryOperator, new []{binOperator.LeftArgument, binOperator.RightArgument, rightArgument});
            }

            if (leftArgument is MultiArgBinaryOperatorFlexpression<TConstraint> multiBinOperator && multiBinOperator.BinaryOperator == binaryOperator)
            {
                var leftArgs = multiBinOperator.Arguments;
                var args = new IFlexpression<TConstraint>[leftArgs.Count + 1];

                for (var idx = leftArgs.Count - 1; idx >= 0; --idx)
                {
                    args[idx] = leftArgs[idx];
                }

                args[^1] = rightArgument;

                return new MultiArgBinaryOperatorFlexpression<TConstraint>(binaryOperator, args);
            }

            return new BinaryOperatorFlexpression<TConstraint>(binaryOperator, leftArgument, rightArgument);
        }
    }

    public sealed class BinaryOperatorFlexpression<TConstraint> : Flexpression<TConstraint> where TConstraint : IFlexpressionConstraint
    {
        /// <inheritdoc />
        public override string Name => BinaryOperator.ToCodeRepresentation(LeftArgument.Name, RightArgument.Name);

        public OverloadableCodeBinarySymmetricOperator BinaryOperator { get; }
        public IFlexpression<TConstraint> LeftArgument { get; }
        public IFlexpression<TConstraint> RightArgument { get; }

        public BinaryOperatorFlexpression(OverloadableCodeBinarySymmetricOperator binaryOperator, IFlexpression<TConstraint> leftArgument, IFlexpression<TConstraint> rightArgument)
        {
            Constraint.ValidateBinaryOperatorAllowed(binaryOperator, 2);
            BinaryOperator = binaryOperator;
            LeftArgument = leftArgument;
            RightArgument = rightArgument;
        }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = visitor.VisitBinaryOperator(BinaryOperator, EnumerateArgumentsVisitationResults(visitor), 2);

            return visitationResult;
        }

        private IEnumerable<TVisitationResult> EnumerateArgumentsVisitationResults<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            yield return LeftArgument.AcceptVisitor(visitor);
            yield return RightArgument.AcceptVisitor(visitor);
        }

    }
}