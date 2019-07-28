using System.Collections.Generic;

namespace QuickAccess.Parser
{
    public abstract class TerminalExpressionNode : IParsedExpressionNode
    {
        public string ExpressionTypeId { get; }
        public string ValueTypeId { get; }
        public ISourceCodeFragment Fragment { get; }
        public IReadOnlyList<IParsedExpressionNode> SubNodes => EmptySubNodes.Instance;

        protected TerminalExpressionNode(
            string expressionTypeId,
            ISourceCodeFragment fragment,
            string valueTypeId = null)
        {
            ExpressionTypeId = expressionTypeId;
            Fragment = fragment;
            ValueTypeId = valueTypeId;
        }

        public override string ToString()
        {
            return Fragment.ToString();
        }
    }
}