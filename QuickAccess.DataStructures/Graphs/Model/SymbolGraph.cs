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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     The editable symbol graph.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
	/// <seealso cref="IReadOnlyGraph{TEdgeData, TSymbol}" />
	/// <seealso cref="IGraphSource{TEdgeData, TSymbol}" />
	public sealed class SymbolGraph<TEdgeData, TSymbol>
		: IReadOnlyGraph<TEdgeData, TSymbol>,
		  IGraphSource<TEdgeData, TSymbol>
	{
		private readonly IndexGraph<TEdgeData> _indexGraphSource;
		private readonly ISymbolToIndexConverter<TSymbol> _symbolToIndexConverter;

		/// <summary>
		///     Initializes a new instance of the <see cref="SymbolGraph{TEdgeData, TSymbol}" /> class.
		/// </summary>
		/// <param name="indexGraphSource">The index graph source.</param>
		/// <param name="symbolToIndexConverter">The symbol source.</param>
		public SymbolGraph(
			IndexGraph<TEdgeData> indexGraphSource,
			ISymbolToIndexConverter<TSymbol> symbolToIndexConverter)
		{
			_indexGraphSource = indexGraphSource;
			_symbolToIndexConverter = symbolToIndexConverter;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="SymbolGraph{TEdgeData, TSymbol}" /> class.
		/// </summary>
		/// <param name="verticesCapacity">The vertices capacity.</param>
		/// <param name="symbolComparer">The symbol comparer.</param>
		public SymbolGraph(int verticesCapacity = 0, IEqualityComparer<TSymbol> symbolComparer = null)
			: this(new IndexGraph<TEdgeData>(0, verticesCapacity),
				new SymbolToIndexConverter<TSymbol>(verticesCapacity, symbolComparer))
		{
		}

		/// <inheritdoc />
		public void Clear(bool removeAdjacencyOnly = false)
		{
			_indexGraphSource.Clear(removeAdjacencyOnly);

			if (!removeAdjacencyOnly)
			{
				_symbolToIndexConverter.Clear();
			}
		}

		/// <inheritdoc />
		public bool AddEdge(
			TSymbol srcVertexKey,
			TSymbol destVertexKey,
			TEdgeData edgeData)
		{
			var srcIdx = GetIndexOfExistingOrNewIfNotFrozen(srcVertexKey);
			var dstIdx = GetIndexOfExistingOrNewIfNotFrozen(destVertexKey);

			return _indexGraphSource.AddEdge(srcIdx, dstIdx, edgeData);
		}

		/// <inheritdoc />
		public int RemoveAllEdgesFrom(TSymbol srcVertexKey)
		{
			var srcIdx = GetIndexIfNotFrozen(srcVertexKey);
			return _indexGraphSource.RemoveAllEdgesFrom(srcIdx);
		}

		/// <inheritdoc />
		public bool AddVertex(TSymbol vertexKey)
		{
			var idx = GetIndexOfExistingOrNewIfNotFrozen(vertexKey);
			return _indexGraphSource.AddVertex(idx);
		}

		/// <inheritdoc />
		public int RemoveAllEdgesTo(TSymbol destVertexKey)
		{
			var destIndex = GetIndexIfNotFrozen(destVertexKey);
			return _indexGraphSource.RemoveAllEdgesTo(destIndex);
		}

		/// <inheritdoc />
		public void Freeze()
		{
			_indexGraphSource.Freeze();
		}

		/// <inheritdoc />
		[Pure]
		public bool IsReadOnly => _indexGraphSource.IsReadOnly;

		/// <inheritdoc />
		public bool RemoveEdge(TSymbol srcVertexKey, TSymbol destVertexKey)
		{
			var srcIdx = GetIndexIfNotFrozen(srcVertexKey);
			var dstIdx = GetIndexIfSrcValidAndGraphNotFrozen(srcIdx, destVertexKey);

			return _indexGraphSource.RemoveEdge(srcIdx, dstIdx);
		}

		/// <inheritdoc />
		public event EventHandler<GraphConnectivityChangedEventArgs> ConnectivityChanged
		{
			add => _indexGraphSource.ConnectivityChanged += value;
			remove => _indexGraphSource.ConnectivityChanged -= value;
		}


		/// <inheritdoc />
		public GraphConnectivityDefinition<TEdgeData> Connectivity =>
			_indexGraphSource.Connectivity;

		/// <inheritdoc />
		[Pure]
		public ISymbolToIndexReadOnlyConverter<TSymbol> SymbolToIndexConverter => _symbolToIndexConverter;

		private int GetIndexIfNotFrozen(TSymbol symbol)
		{
			return IsReadOnly ? -1 : _symbolToIndexConverter.GetIndexOf(symbol);
		}

		private int GetIndexIfSrcValidAndGraphNotFrozen(int srcIndex, TSymbol symbol)
		{
			return srcIndex < 0 || IsReadOnly ? -1 : _symbolToIndexConverter.GetIndexOf(symbol);
		}

		private int GetIndexOfExistingOrNewIfNotFrozen(TSymbol symbol)
		{
			return IsReadOnly ? -1 : _symbolToIndexConverter.GetIndexOfExistingOrNew(symbol);
		}
	}
}