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
using QuickAccess.Infrastructure.Collections;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	/// Dictionary based symbol to index converter implementation.
	/// </summary>
	/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
	/// <seealso cref="ISymbolToIndexConverter{TSymbol}" />
	public sealed class SymbolToIndexConverter<TSymbol> : ISymbolToIndexConverter<TSymbol>
	{
		private readonly List<TSymbol> _indexToSymbolMap;
		private readonly Dictionary<TSymbol, int> _symbolToIndexMap;

		/// <summary>
		/// Initializes a new instance of the <see cref="SymbolToIndexConverter{TSymbol}"/> class.
		/// </summary>
		/// <param name="capacity">The capacity (number of expected symbols), <c>0</c> by default.</param>
		/// <param name="symbolComparer">The symbol equality comparer, when <c>null</c> (by default) default comparer will be used.</param>
		public SymbolToIndexConverter(int capacity = 0, IEqualityComparer<TSymbol> symbolComparer = null)
		{
			_symbolToIndexMap = new Dictionary<TSymbol, int>(capacity, symbolComparer);
			_indexToSymbolMap = new List<TSymbol>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SymbolToIndexConverter{TSymbol}" /> class copying data from the other map.
		/// </summary>
		/// <param name="other">The other map.</param>
		/// <param name="maxIndex">The maximal index to copy.</param>
		public SymbolToIndexConverter(ISymbolToIndexReadOnlyConverter<TSymbol> other, int maxIndex = -1)	
			: this((maxIndex > other.MaxIndex || maxIndex < 0 ? other.MaxIndex : maxIndex)+1, other.Comparer)
		{
			var count = _indexToSymbolMap.Capacity;
			for (var dstIndex = 0; dstIndex < count; dstIndex++)
			{
				AddSymbol(other.GetSymbolAt(dstIndex));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SymbolToIndexConverter{TSymbol}" /> class copying data from the other map corresponding to the items sequence before re-indexing.
		/// It maps the symbols to the new index sequence respecting the specified result of re-indexing operation.
		/// </summary>
		/// <param name="prev">The map before re-indexing.</param>
		/// <param name="reindexingResult">The re-indexing result.</param>
		public SymbolToIndexConverter(ISymbolToIndexReadOnlyConverter<TSymbol> prev, IIndexTranslator reindexingResult)
			: this(reindexingResult.ItemsCount, prev.Comparer)
		{
			var count = reindexingResult.ItemsCount;

			for (var dstIndex = 0; dstIndex < count; dstIndex++)
			{
				AddSymbol(prev.GetSymbolAt(reindexingResult.GetSrcIndex(dstIndex)));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SymbolToIndexConverter{TSymbol}"/> class.
		/// </summary>
		/// <param name="symbols">The symbols.</param>
		/// <param name="symbolComparer">The symbol equality comparer, when <c>null</c> (by default) default comparer will be used.</param>
		public SymbolToIndexConverter(
			IReadOnlyCollection<TSymbol> symbols,
			IEqualityComparer<TSymbol> symbolComparer = null)
			: this(symbols.Count, symbols, symbolComparer)
		{
		}
		

		/// <summary>
		/// Initializes a new instance of the <see cref="SymbolToIndexConverter{TSymbol}"/> class.
		/// </summary>
		/// <param name="capacity">The capacity (number of expected symbols).</param>
		/// <param name="symbols">The symbols.</param>
		/// <param name="symbolComparer">The symbol equality comparer, when <c>null</c> (by default) default comparer will be used.</param>
		public SymbolToIndexConverter(
			int capacity,
			IEnumerable<TSymbol> symbols,
			IEqualityComparer<TSymbol> symbolComparer = null)
			: this(capacity, symbolComparer)
		{	
			foreach (var symbol in symbols)
			{
				AddSymbol(symbol);
			}
		}

		/// <inheritdoc />
		public IEqualityComparer<TSymbol> Comparer => _symbolToIndexMap.Comparer;

		/// <inheritdoc />
		public int Count => _symbolToIndexMap.Count;

		/// <inheritdoc />
		public int MaxIndex => _indexToSymbolMap.Count - 1;

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
			return index >= 0 && index < _indexToSymbolMap.Count;
		}

		/// <inheritdoc />
		public bool TryGetSymbolAt(int index, out TSymbol symbol)
		{
			if (index < 0 || index >= _indexToSymbolMap.Count)
			{
				symbol = default;
				return false;
			}

			symbol = _indexToSymbolMap[index];
			return true;
		}

		
		/// <inheritdoc />
		public int AddSymbol(TSymbol symbol)
		{
			var index = _indexToSymbolMap.Count;
			_symbolToIndexMap.Add(symbol, index);
			_indexToSymbolMap.Add(symbol);

			return index;
		}


		/// <inheritdoc />
		public int GetIndexOfExistingOrNew(TSymbol symbol)
		{
			return _symbolToIndexMap.TryGetValue(symbol, out var index) ? index: AddSymbol(symbol);
		}

		/// <inheritdoc />
		public void Clear()
		{
			_symbolToIndexMap.Clear();
			_indexToSymbolMap.Clear();			
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
	}
}