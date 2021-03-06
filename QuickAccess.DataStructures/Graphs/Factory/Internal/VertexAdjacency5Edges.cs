﻿#region LICENSE [BSD-2-Clause]

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

using System.Collections.Generic;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory.Internal
{
	/// <summary>
	/// Vertex adjacency with fixed capacity provided for performance optimization.
	/// Performance tests executed on .net 4.7 have shown that fixed capacity structure up to 7 edges is superior to unlimited 
	/// <see cref="HashSet{T}"/>/<see cref="Dictionary{TKey,TValue}"/> based structure, when it comes to adjacency 
	/// enumeration and contains operations execution time.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="ReplaceableVertexAdjacency{TEdgeData}" />
	internal sealed class VertexAdjacency5Edges<TEdgeData> : ReplaceableVertexAdjacencyWithFixedCapacity<TEdgeData>
	{
		private int _destVertexIndex1;
		private int _destVertexIndex2;
		private int _destVertexIndex3;
		private int _destVertexIndex4;
		private int _destVertexIndex5;
		private TEdgeData _edgeData1;
		private TEdgeData _edgeData2;
		private TEdgeData _edgeData3;
		private TEdgeData _edgeData4;
		private TEdgeData _edgeData5;

		/// <inheritdoc />
		public override int EdgesCount => 5;

		/// <inheritdoc />
		public override IEnumerable<int> AdjacentIndexes
		{
			get
			{
				yield return _destVertexIndex1;
				yield return _destVertexIndex2;
				yield return _destVertexIndex3;
				yield return _destVertexIndex4;
				yield return _destVertexIndex5;
			}
		}

		/// <inheritdoc />
		public override bool ContainsEdgeToIndex(int destVertexIndex)
		{
			return destVertexIndex == _destVertexIndex1 || destVertexIndex == _destVertexIndex2 ||
			       destVertexIndex == _destVertexIndex3 || destVertexIndex == _destVertexIndex4 ||
			       destVertexIndex == _destVertexIndex5;
		}

		/// <inheritdoc />
		public override TEdgeData GetEdgeToIndex(int destVertexIndex)
		{
			if (_destVertexIndex1 == destVertexIndex)
			{
				return _edgeData1;
			}

			if (_destVertexIndex2 == destVertexIndex)
			{
				return _edgeData2;
			}

			if (_destVertexIndex3 == destVertexIndex)
			{
				return _edgeData3;
			}

			if (_destVertexIndex4 == destVertexIndex)
			{
				return _edgeData4;
			}

			if (_destVertexIndex5 == destVertexIndex)
			{
				return _edgeData5;
			}

			throw new KeyNotFoundException();
		}

		/// <inheritdoc />
		public override IEnumerator<AdjacentEdge<TEdgeData>> GetEnumerator()
		{
			yield return AdjacentEdge.Create(_destVertexIndex1, _edgeData1);
			yield return AdjacentEdge.Create(_destVertexIndex2, _edgeData2);
			yield return AdjacentEdge.Create(_destVertexIndex3, _edgeData3);
			yield return AdjacentEdge.Create(_destVertexIndex4, _edgeData4);
			yield return AdjacentEdge.Create(_destVertexIndex5, _edgeData5);
		}

		/// <inheritdoc />
		protected override void Set(AdjacencyEnumerator data)
		{
			_destVertexIndex1 = data.GetNext(out _edgeData1);
			_destVertexIndex2 = data.GetNext(out _edgeData2);
			_destVertexIndex3 = data.GetNext(out _edgeData3);
			_destVertexIndex4 = data.GetNext(out _edgeData4);
			_destVertexIndex5 = data.GetNext(out _edgeData5);
		}
	}

	/// <summary>
	/// Vertex adjacency with fixed capacity provided for performance optimization.
	/// Performance tests executed on .net 4.7 have shown that fixed capacity structure up to 7 edges is superior to unlimited 
	/// <see cref="HashSet{T}"/>/<see cref="Dictionary{TKey,TValue}"/> based structure, when it comes to adjacency 
	/// enumeration and contains operations execution time.
	/// </summary>
	/// <seealso cref="ReplaceableVertexAdjacency{TEdgeData}" />
	internal sealed class VertexAdjacency5Edges : ReplaceableVertexAdjacencyWithFixedCapacity
	{
		private int _destVertexIndex1;
		private int _destVertexIndex2;
		private int _destVertexIndex3;
		private int _destVertexIndex4;
		private int _destVertexIndex5;

		/// <inheritdoc />
		public override int EdgesCount => 5;

		/// <inheritdoc />
		public override IEnumerable<int> AdjacentIndexes
		{
			get
			{
				yield return _destVertexIndex1;
				yield return _destVertexIndex2;
				yield return _destVertexIndex3;
				yield return _destVertexIndex4;
				yield return _destVertexIndex5;
			}
		}

		/// <inheritdoc />
		public override bool ContainsEdgeToIndex(int destVertexIndex)
		{
			return destVertexIndex == _destVertexIndex1
			       || destVertexIndex == _destVertexIndex2
			       || destVertexIndex == _destVertexIndex3
			       || destVertexIndex == _destVertexIndex4
			       || destVertexIndex == _destVertexIndex5;
		}

		/// <inheritdoc />
		protected override void Set(AdjacencyEnumerator adj)
		{
			_destVertexIndex1 = adj.GetNext();
			_destVertexIndex2 = adj.GetNext();
			_destVertexIndex3 = adj.GetNext();
			_destVertexIndex4 = adj.GetNext();
			_destVertexIndex5 = adj.GetNext();
		}
	}
}