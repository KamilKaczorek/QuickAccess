namespace QuickAccess.Parser.Product
{
    public interface IDescribeParsingProduct
    {
        /// <summary>
        /// Gets the type of the product.
        /// </summary>
        ParsingProductType ProductType { get; }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        ExpressionTypeDescriptor ExpressionType { get; }
    }
}