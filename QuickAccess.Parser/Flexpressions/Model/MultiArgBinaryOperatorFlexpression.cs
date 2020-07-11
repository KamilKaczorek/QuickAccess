using System.Collections.Generic;
using System.Linq;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class MultiArgBinaryOperatorFlexpression<TConstraint> : Flexpression<TConstraint> where TConstraint : IFlexpressionConstraint
    {
        /// <inheritdoc />
        public override string Name => string.Join(BinaryOperator.GetSymbol(), (IEnumerable<string>) Arguments.Select(p => $"({p.Name})"));

        public OverloadableCodeBinarySymmetricOperator BinaryOperator { get; }
        public IReadOnlyList<IFlexpression<TConstraint>> Arguments { get; }

        public MultiArgBinaryOperatorFlexpression(OverloadableCodeBinarySymmetricOperator binaryOperator, IReadOnlyList<IFlexpression<TConstraint>> args)
        {
            Guard.ArgCountAtLeast(args, nameof(args), 3);
            Constraint.ValidateBinaryOperatorAllowed(binaryOperator, args.Count);

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