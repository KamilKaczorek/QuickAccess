using System.Collections.Generic;
using System.Linq;
using QuickAccess.Infrastructure.Algebra;
using QuickAccess.Infrastructure.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class MultiArgBinaryOperatorFlexpression : Flexpression 
    {
        /// <inheritdoc />
        public override string Name => string.Join(BinaryOperator.GetSymbol(), (IEnumerable<string>) Arguments.Select(p => $"({p.Name})"));

        public OverloadableCodeBinarySymmetricOperator BinaryOperator { get; }
        public IReadOnlyList<IFlexpression> Arguments { get; }

        public MultiArgBinaryOperatorFlexpression(OverloadableCodeBinarySymmetricOperator binaryOperator, IReadOnlyList<IFlexpression> args)
        {
            Guard.ArgCountAtLeast(args, nameof(args), 3);

            BinaryOperator = binaryOperator;
            Arguments = args;
        }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = visitor.VisitBinaryOperator(BinaryOperator, EnumerateArgsVisitationResults(visitor), Arguments.Count);
            return visitationResult;
        }

        private IEnumerable<TVisitationResult> EnumerateArgsVisitationResults<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            return Arguments.Select(pFlexpression => pFlexpression.AcceptVisitor(visitor));
        }
    }
}