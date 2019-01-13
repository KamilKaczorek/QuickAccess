
using System.Diagnostics.Contracts;

namespace QuickAccess.DataStructures.Common.Collections
{
	/// <summary>
	/// The interface of the index translator that translate source index into destination index and vice versa.
	/// Useful in sorting operation when the index information of an item is relevant.
	/// <seealso cref="EnumerableExtensions.SortWithReindexingResult{T}(T[],System.Collections.Generic.IComparer{T})"/>
	/// </summary>
	public interface IIndexTranslator
	{
		/// <summary>
		/// Gets the number of items.
		/// Last index, of which translation is available, is equal to the <see cref="ItemsCount"/> - 1. 
		/// </summary>
		/// <value>The items count.</value>
		[Pure]
		int ItemsCount { get; }

		/// <summary>Gets the new (destination) index corresponding to the specified old (source) index.</summary>
		/// <param name="sourceIndex">The old (source) index.</param>
		/// <returns>The new (destination) index or <c>-1</c> if there is no corresponding destination index to specified source index.</returns>
		/// <exception cref="System.IndexOutOfRangeException">thrown when index is smaller than zero or greater or equal to <see cref="ItemsCount"/>.</exception>
		[Pure]
		int GetDestIndex(int sourceIndex);

		/// <summary>Gets the old (source) index corresponding to the specified new (destination) index.</summary>
		/// <param name="destIndex">The new (destination) index.</param>
		/// <returns>The old (source) index.</returns>
		/// <exception cref="System.IndexOutOfRangeException">thrown when index is smaller than zero or greater or equal to <see cref="ItemsCount"/>.</exception>
		[Pure]
	    int GetSrcIndex(int destIndex);
	}
}