#region LICENSE [BSD-2-Clause]

// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2018 by Kamil Piotr Kaczorek
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, 
//     this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, 
//     this list of conditions and the following disclaimer in the documentation and/or 
//     other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF 
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// =====================================================================
// 
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickAccess.DataStructures.Graphs.Extensions;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     The interface of the symbol to index map.
	///     It provides the contract to translate the symbol into an index and vice versa.
	///     It is used as symbol/index map definition in symbol graphs.
	/// </summary>
	/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
	public interface ISymbolToIndexReadOnlyConverter<TSymbol> : IEnumerable<KeyValuePair<TSymbol, int>>
	{
		/// <summary>
		///     Gets the equality comparer to compare symbols.
		/// </summary>
		/// <value>
		///     The symbol comparer.
		/// </value>
		[Pure]
		IEqualityComparer<TSymbol> Comparer { get; }

		/// <summary>
		/// Gets the number of defined symbols.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		[Pure]
		int Count { get; }

		/// <summary>Gets the maximal index.</summary>
		/// <value>The maximal index.</value>
		[Pure]
		int MaxIndex { get; }

		/// <summary>Gets the index of the symbol specified by parameter.</summary>
		/// <seealso cref="GraphExtensions.GetIndexesOf{TEdgeData,TSymbol}(IReadOnlyGraph{TEdgeData,TSymbol},TSymbol,TSymbol,out int,out int,bool)"/>
		/// <param name="symbol">The symbol.</param>
		/// <param name="validate">
		/// if set to <c>true</c> it throws <see cref="KeyNotFoundException"/> when index is not defined for the specified symbol; otherwise, <c>false</c> (default) it will return <c>-1</c> if the index is not defined.
		/// </param>
		/// <returns>
		/// The index value or <c>-1</c> if the <paramref name="validate" /> is set to <c>false</c> and the index is not defined for the specified symbol.
		/// </returns>
		/// <exception cref="KeyNotFoundException">Thrown when the <paramref name="validate"/> is <c>true</c> and the index is not defined for the specified symbol.</exception>
		[Pure]
		int GetIndexOf(TSymbol symbol, bool validate = false);

		/// <summary>
		/// Gets the symbol at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The symbol at the specified index.</returns>
		/// <exception cref="KeyNotFoundException">Thrown when symbol is not defined for the specified index.</exception>
		[Pure]
		TSymbol GetSymbolAt(int index);

		/// <summary>Determines whether the specified map contains specified symbol.</summary>
		/// <param name="symbol">The symbol.</param>
		/// <returns><c>true</c> if the specified map contains symbol; otherwise, <c>false</c>.</returns>
		[Pure]
		bool ContainsSymbol(TSymbol symbol);

		/// <summary>
		/// Determines whether it contains the symbol definition for the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>
		///   <c>true</c> if contains the defined symbol at specified index; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		bool ContainsSymbolAt(int index);

		/// <summary>
		/// Tries to get the symbol at specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="symbol">The symbol.</param>
		/// <returns><c>true</c> if contains the defined symbol at specified index; otherwise, <c>false</c>.</returns>
		[Pure]
		bool TryGetSymbolAt(int index, out TSymbol symbol);
	}
}