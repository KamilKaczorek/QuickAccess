namespace QuickAccess.Parser.Product
{
    public interface IRepresentParsedCode
    {
        /// <summary>
        /// Gets the fragment of the source code that defines this expression.
        /// </summary>
        /// <value>
        /// The source code fragment.
        /// </value>
        ISourceCodeFragment Fragment { get; }
    }
}