using System.Collections.Generic;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
    public interface IParsingProductFactory
    {
        /// <summary>
        /// Creates the expression for the accepted fragment.
        /// </summary>
        /// <param name="codeFragment">The accepted code fragment that is represented by the created expression.</param>
        /// <param name="subNodes">The expression sub nodes.</param>
        /// <param name="expressionType">The descriptor of the type of expression.</param>
        /// <returns>Expression</returns>
        IParsingProduct CreateExpression(ExpressionTypeDescriptor expressionType, ISourceCodeFragment codeFragment, IReadOnlyCollection<IParsingProduct> subNodes);

        /// <summary>
        /// Creates the term for the accepted fragment.
        /// </summary>
        /// <param name="codeFragment">The accepted code fragment that is represented by the created expression.</param>
        /// <param name="expressionType">The descriptor of the type of expression.</param>
        /// <returns>Expression</returns>
        IParsingProduct CreateTerm(ExpressionTypeDescriptor expressionType, ISourceCodeFragment codeFragment);
    }
}