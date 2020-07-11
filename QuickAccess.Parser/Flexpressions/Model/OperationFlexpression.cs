using System.Collections.Generic;
using System.Linq;
using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class OperationFlexpression<TConstraint> : Flexpression<TConstraint> where TConstraint : IFlexpressionConstraint
    {
        /// <inheritdoc />
        public override string Name => OperationName;

        public string OperationName { get; }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var argsProducts = Arguments.Select(p => p.AcceptVisitor(visitor));
            var product = visitor.VisitOperation(OperationName, argsProducts, Arguments.Count);
            return product;
        }

        public IReadOnlyList<IFlexpression<TConstraint>> Arguments { get; }

        public OperationFlexpression(string name, IReadOnlyList<IFlexpression<TConstraint>> args)
        {
            Guard.ArgCountAtLeast(args, nameof(args), 3);

            Arguments = args;
            OperationName = name;
        }
    }
}