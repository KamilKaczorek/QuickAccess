using System.Collections.Generic;
using QuickAccess.Infrastructure.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{


    public sealed class BinaryOperatorFlexpression : Flexpression 
    {
        public static Flexpression Create(
            OverloadableCodeBinarySymmetricOperator binaryOperator, IFlexpression leftArgument, IFlexpression rightArgument)
        {
            if (leftArgument is BinaryOperatorFlexpression binOperator && binOperator.BinaryOperator == binaryOperator)
            {
                return new MultiArgBinaryOperatorFlexpression(binaryOperator, new[] { binOperator.LeftArgument, binOperator.RightArgument, rightArgument });
            }

            if (leftArgument is MultiArgBinaryOperatorFlexpression multiBinOperator && multiBinOperator.BinaryOperator == binaryOperator)
            {
                var leftArgs = multiBinOperator.Arguments;
                var args = new IFlexpression[leftArgs.Count + 1];

                for (var idx = leftArgs.Count - 1; idx >= 0; --idx)
                {
                    args[idx] = leftArgs[idx];
                }

                args[^1] = rightArgument;

                return new MultiArgBinaryOperatorFlexpression(binaryOperator, args);
            }

            return new BinaryOperatorFlexpression(binaryOperator, leftArgument, rightArgument);
        }

        /// <inheritdoc />
        public override string Name => BinaryOperator.ToCodeRepresentation(LeftArgument.Name, RightArgument.Name);

        public OverloadableCodeBinarySymmetricOperator BinaryOperator { get; }
        public IFlexpression LeftArgument { get; }
        public IFlexpression RightArgument { get; }

        public BinaryOperatorFlexpression(OverloadableCodeBinarySymmetricOperator binaryOperator, IFlexpression leftArgument, IFlexpression rightArgument)
        {
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