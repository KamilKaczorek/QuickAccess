using System.Collections.Generic;

namespace QuickAccess.Parser.Product
{
    public interface IComposite<out T> where T : IComposite<T>
    {
        /// <summary>
        /// Gets the sub nodes (parameters) - only available when the product represents expression.
        /// </summary>
        /// <value>
        /// The sub nodes.
        /// </value>
        IReadOnlyList<T> SubNodes { get; }
    }
}