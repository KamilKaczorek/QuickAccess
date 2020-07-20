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

using System.Collections.Generic;
using System.Linq;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory.Internal
{
	/// <summary>
	/// Dictionary based vertex adjacency with unlimited capacity used when number of edges > 7.
	/// <seealso cref="VertexAdjacency7Edges{TEdgeData}"/>
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="ReplaceableVertexAdjacency{TEdgeData}" />
	internal sealed class VertexAdjacencyUnlimitedEdges<TEdgeData> : ReplaceableVertexAdjacency<TEdgeData>
	{
		private Dictionary<int, TEdgeData> _adj;


		/// <inheritdoc />
		public override int EdgesCount => _adj.Count;


		/// <inheritdoc />
		public override int MaxCapacity => ReplaceableVertexAdjacency.UnlimitedCapacity;

		/// <inheritdoc />
		public override bool AddEdge(ReplaceableVertexFactoryInterface<TEdgeData> vertexFactory, int destVertexIndex, TEdgeData edgeData, out ReplaceableVertexAdjacency<TEdgeData> final)
		{
			if (_adj == null)
			{
				_adj = new Dictionary<int, TEdgeData>(1);
			}

			var count = _adj.Count;

			_adj[destVertexIndex] = edgeData;

			final = this;
			return _adj.Count > count;
		}

		/// <inheritdoc />
		public override bool RemoveEdge(ReplaceableVertexFactoryInterface<TEdgeData> vertexFactory, int destVertexIndex, out ReplaceableVertexAdjacency<TEdgeData> final)
		{
			if (_adj == null || !_adj.Remove(destVertexIndex))
			{
				final = this;
				return false;
			}

			if (!vertexFactory.ProvidesFixedCapacity(_adj.Count))
			{
				final = this;
				return true;
			}

			final = vertexFactory.GetInstance(_adj.Select(AdjacentEdge.Create), _adj.Count);
			return true;
		}

		public VertexAdjacencyUnlimitedEdges()
		{
			_adj = new Dictionary<int, TEdgeData>();
		}

		public VertexAdjacencyUnlimitedEdges(int capacity)
		{
			_adj = new Dictionary<int, TEdgeData>(capacity);
		}

		public VertexAdjacencyUnlimitedEdges(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo, int count)
			: this(count)
		{
			Initialize(edgesTo);
		}

		public override void Initialize(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo)
		{
			_adj.Clear();
			foreach (var edgeTo in edgesTo)
			{
				_adj.Add(edgeTo.Destination, edgeTo.Data);
			}
		}

		/// <inheritdoc />
		public override bool ContainsEdgeToIndex(int destVertexIndex)
		{
			return _adj.ContainsKey(destVertexIndex);
		}

		/// <inheritdoc />
		public override IEnumerable<int> AdjacentIndexes => _adj.Keys;

		/// <inheritdoc />
		public override void Reset()
		{
			_adj.Clear();
		}

		/// <inheritdoc />
		public override bool TryGetEdgeToIndex(int destVertexIndex, out TEdgeData edgeData)
		{
			return _adj.TryGetValue(destVertexIndex, out edgeData);
		}

		/// <inheritdoc />
		public override TEdgeData GetEdgeToIndex(int destVertexIndex)
		{
			return _adj[destVertexIndex];
		}

		/// <inheritdoc />
		public override IEnumerator<AdjacentEdge<TEdgeData>> GetEnumerator()
		{
			return _adj.Select(AdjacentEdge.Create).GetEnumerator();
		}		
	}

	/// <summary>
	/// Hash set based vertex adjacency with unlimited capacity used when number of edges > 7.
	/// <seealso cref="VertexAdjacency7Edges"/>
	/// </summary>
	/// <seealso cref="ReplaceableVertexAdjacency{TEdgeData}" />
	internal sealed class VertexAdjacencyUnlimitedEdges : ReplaceableVertexAdjacency
	{
		private readonly HashSet<int> _adj;

		public VertexAdjacencyUnlimitedEdges()
		{
			_adj = new HashSet<int>();
		}

		public VertexAdjacencyUnlimitedEdges(IEnumerable<int> adj)
		{
			_adj = new HashSet<int>(adj);
		}

		public override void Initialize(IEnumerable<int> destIndexes)
		{
			_adj.Clear();
			foreach (var idx in destIndexes)
			{
				_adj.Add(idx);
			}
		}

		/// <inheritdoc />
		public override int EdgesCount => _adj.Count;


		/// <inheritdoc />
		public override int MaxCapacity => UnlimitedCapacity;

		/// <inheritdoc />
		public override bool AddEdge(ReplaceableVertexFactoryInterface<EmptyValue> vertexFactory,
		                             int destVertexIndex,
		                             EmptyValue edgeData,
		                             out ReplaceableVertexAdjacency<EmptyValue> final)
		{
			final = this;
			return _adj.Add(destVertexIndex);
		}



		/// <inheritdoc />
		public override bool RemoveEdge(ReplaceableVertexFactoryInterface<EmptyValue> vertexFactory,
		                                int destVertexIndex,
		                                out ReplaceableVertexAdjacency<EmptyValue> final)
		{
			if (!_adj.Remove(destVertexIndex))
			{
				final = this;
				return false;
			}

			if (!vertexFactory.ProvidesFixedCapacity(_adj.Count))
			{
				final = this;
				return true;
			}

			final = vertexFactory.GetInstance(_adj.Select(idx => AdjacentEdge.Create(idx)), _adj.Count);
			return true;
		}

		/// <inheritdoc />
		public override bool ContainsEdgeToIndex(int destVertexIndex)
		{
			return _adj.Contains(destVertexIndex);
		}

		/// <inheritdoc />
		public override IEnumerable<int> AdjacentIndexes => _adj;

		/// <inheritdoc />
		public override void Reset()
		{
			_adj.Clear();
		}
	}
}