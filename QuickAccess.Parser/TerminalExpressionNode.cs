using System.Collections.Generic;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
    public abstract class TerminalExpressionNode : IParsingProduct
    {
        public ISourceCodeFragment Fragment { get; }
        public IReadOnlyList<IParsingProduct> SubNodes => EmptySubNodes.Instance;

        protected TerminalExpressionNode(
            ExpressionTypeDescriptor expressionType,
            ISourceCodeFragment fragment)
        {
            Fragment = fragment;
            ExpressionType = expressionType;
        }

        public override string ToString()
        {
            return Fragment.ToString();
        }

        public ParsingProductType ProductType => ParsingProductType.Term;
        public ExpressionTypeDescriptor ExpressionType { get; }
    }
}