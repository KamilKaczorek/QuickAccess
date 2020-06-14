using System.Collections.Generic;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
    public sealed class ProductFactory : IParsingProductFactory
    {
        public IParsingProduct CreateExpression(ExpressionTypeDescriptor expressionType, ISourceCodeFragment codeFragment, IReadOnlyCollection<IParsingProduct> subNodes)
        {
            return ExpressionNode.Create(expressionType, codeFragment, subNodes);
        }

        public IParsingProduct CreateTerm(ExpressionTypeDescriptor expressionType, ISourceCodeFragment codeFragment)
        {
            return TermNode.Create(expressionType, codeFragment);
        }
    }
}