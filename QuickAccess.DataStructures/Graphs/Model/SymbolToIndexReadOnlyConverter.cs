#region LICENSE [BSD-2-Clause]

// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2020 by Kamil Piotr Kaczorek
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

using System.Collections;
using System.Collections.Generic;
using QuickAccess.DataStructures.Common.Collections;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	/// The read-only map of symbols to indexes and vice versa.
	/// </summary>
	/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
	/// <seealso cref="ISymbolToIndexReadOnlyConverter{TSymbol}" />
	public sealed class SymbolToIndexReadOnlyConverter<TSymbol> : ISymbolToIndexReadOnlyConverter<TSymbol>
	{
		private readonly TSymbol[] _indexToSymbolMap;
		private readonly Dictionary<TSymbol, int> _symbolToIndexMap;

		/// <summary>Initializes a new instance of the <see cref="SymbolToIndexReadOnlyConverter{TSymbol}"/> class.</summary>
		/// <param name="indexToSymbolMap">The index to symbol map.</param>
		/// <param name="symbolEqualityComparer">The symbol equality comparer.</param>
		public SymbolToIndexReadOnlyConverter(TSymbol[] indexToSymbolMap, IEqualityComparer<TSymbol> symbolEqualityComparer)
		{
			_indexToSymbolMap = indexToSymbolMap;
			var count = _indexToSymbolMap.Length;
			_symbolToIndexMap = new Dictionary<TSymbol, int>(count, symbolEqualityComparer);
			for (var idx = 0; idx < count; idx++)
			{
				_symbolToIndexMap.Add(_indexToSymbolMap[idx], idx);
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="SymbolToIndexConverter{TSymbol}" /> class copying data from the other map.
		/// </summary>
		/// <param name="other">The other map.</param>
		/// <param name="maxIndex">The maximal index to copy.</param>
		public SymbolToIndexReadOnlyConverter(ISymbolToIndexReadOnlyConverter<TSymbol> other, int maxIndex = -1)
		{
			if (maxIndex > other.MaxIndex || maxIndex < 0)
			{
				maxIndex = other.MaxIndex;
			}

			_symbolToIndexMap = new Dictionary<TSymbol, int>(maxIndex + 1, other.Comparer);
			_indexToSymbolMap = new TSymbol[maxIndex + 1];

			for (var idx = 0; idx <= maxIndex; ++idx)
			{
				var symbol = other.GetSymbolAt(idx);
				_indexToSymbolMap[idx] = symbol;
				_symbolToIndexMap.Add(symbol, idx);
			}
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="SymbolToIndexConverter{TSymbol}" /> class copying data from the other map
		///     corresponding to the items sequence before re-indexing.
		///     It maps the symbols to the new index sequence respecting the specified result of re-indexing operation.
		/// </summary>
		/// <param name="prev">The map before re-indexing.</param>
		/// <param name="reindexingResult">The re-indexing result.</param>
		public SymbolToIndexReadOnlyConverter(ISymbolToIndexReadOnlyConverter<TSymbol> prev, IIndexTranslator reindexingResult)
		{
			var count = reindexingResult.ItemsCount;

			_symbolToIndexMap = new Dictionary<TSymbol, int>(count, prev.Comparer);
			_indexToSymbolMap = new TSymbol[count];

			for (var dstIndex = 0; dstIndex < count; dstIndex++)
			{
				var srcIndex = reindexingResult.GetSrcIndex(dstIndex);
				var symbol = prev.GetSymbolAt(srcIndex);

				_symbolToIndexMap.Add(symbol, dstIndex);
				_indexToSymbolMap[dstIndex] = symbol;
			}
		}

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<TSymbol, int>> GetEnumerator()
		{
			return _symbolToIndexMap.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public IEqualityComparer<TSymbol> Comparer => _symbolToIndexMap.Comparer;

		/// <inheritdoc />
		public int Count => _indexToSymbolMap.Length;

		/// <inheritdoc />
		public int MaxIndex => _indexToSymbolMap.Length - 1;

		/// <inheritdoc />
		public int GetIndexOf(TSymbol symbol, bool validate = false)
		{
			if (_symbolToIndexMap.TryGetValue(symbol, out var idx))
			{
				return idx;
			}

			if (validate)
			{
				throw new KeyNotFoundException($"The index is not defined for the specified symbol '{symbol}'.");
			}

			return -1;
		}

		/// <inheritdoc />
		public TSymbol GetSymbolAt(int index)
		{
			return _indexToSymbolMap[index];
		}

		/// <inheritdoc />
		public bool ContainsSymbol(TSymbol symbol)
		{
			return _symbolToIndexMap.ContainsKey(symbol);
		}

		/// <inheritdoc />
		public bool ContainsSymbolAt(int index)
		{
			return index >= 0 && index < _indexToSymbolMap.Length;
		}

		/// <inheritdoc />
		public bool TryGetSymbolAt(int index, out TSymbol symbol)
		{
			if (index < 0 || index >= _indexToSymbolMap.Length)
			{
				symbol = default;
				return false;
			}

			symbol = _indexToSymbolMap[index];
			return true;
		}
	}
}