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

using System;
using System.Collections.Generic;
using System.Linq;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory.Internal
{
	/// <summary>
	/// The base abstract class of vertex adjacency with fixed capacity provided for performance optimization.
	/// Performance tests executed on .net 4.7 have shown that fixed capacity structure up to 7 edges is superior to unlimited 
	/// <see cref="HashSet{T}"/>/<see cref="Dictionary{TKey,TValue}"/> based structure, when it comes to adjacency 
	/// enumeration and contains operations execution time.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="ReplaceableVertexAdjacency{TEdgeData}" />
	internal abstract class ReplaceableVertexAdjacencyWithFixedCapacity<TEdgeData> : ReplaceableVertexAdjacency<TEdgeData>
	{
		/// <inheritdoc />
		public override int MaxCapacity => EdgesCount;

		/// <inheritdoc />
		public override void Initialize(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo)
		{
			using (var enumerator = edgesTo.GetEnumerator())
			{
				var data = new AdjacencyEnumerator(enumerator);
				Set(data);

				var leftItems = 0;
				while (enumerator.MoveNext())
				{
					leftItems++;
				}

				ValidateInitializedNumberOfEdges(EdgesCount + leftItems);
			}
		}

		protected abstract void Set(AdjacencyEnumerator data);

		protected void ValidateInitializedNumberOfEdges(int count)
		{
			if (count == EdgesCount)
			{
				return;
			}

			throw new InvalidOperationException(
				$"Not able to initialize vertex with capacity fixed to {EdgesCount} with {count} edges.");
		}

		/// <inheritdoc />
		public override bool TryGetEdgeToIndex(int destVertexIndex, out TEdgeData edgeData)
		{
			if (!ContainsEdgeToIndex(destVertexIndex))
			{
				edgeData = default;
				return false;
			}

			edgeData = GetEdgeToIndex(destVertexIndex);
			return true;
		}

		/// <inheritdoc />
		public override bool AddEdge(ReplaceableVertexFactoryInterface<TEdgeData> pool,
		                             int destVertexIndex,
		                             TEdgeData edgeData,
		                             out ReplaceableVertexAdjacency<TEdgeData> final)
		{
			if (ContainsEdgeToIndex(destVertexIndex))
			{
				final = this;
				return false;
			}

			final = pool.GetInstance(
				this.Append(AdjacentEdge.Create(destVertexIndex, edgeData)), EdgesCount + 1);
			return true;
		}

		/// <inheritdoc />
		public override bool RemoveEdge(ReplaceableVertexFactoryInterface<TEdgeData> pool,
		                                int destVertexIndex,
		                                out ReplaceableVertexAdjacency<TEdgeData> final)
		{
			if (!ContainsEdgeToIndex(destVertexIndex))
			{
				final = default;
				return false;
			}

			final = pool.GetInstance(
				this.Where(a => a.Destination != destVertexIndex), EdgesCount - 1);
			return true;
		}

		/// <inheritdoc />
		public override void Reset()
		{
			Set(new AdjacencyEnumerator(null));
		}

		protected struct AdjacencyEnumerator
		{
			private readonly IEnumerator<AdjacentEdge<TEdgeData>> _adjacency;

			public AdjacencyEnumerator(IEnumerator<AdjacentEdge<TEdgeData>> adjacency)
			{
				_adjacency = adjacency;
			}

			public int GetNext(out TEdgeData edgeData)
			{
				if (_adjacency == null)
				{
					edgeData = default;
					return -1;
				}

				if (!_adjacency.MoveNext())
				{
					throw new InvalidOperationException($"Not enough vertices to initialize vertex with fixed capacity.");
				}

				var idx = _adjacency.Current.Destination;

				ValidateIndex(idx);

				edgeData = _adjacency.Current.Data;
				return idx;
			}

			private static void ValidateIndex(int index)
			{
				if (index < 0)
				{
					throw new IndexOutOfRangeException(
						$"Not valid vertex index value ({index}). Vertex index must be equal or greater than zero.");
				}
			}
		}
	}

	/// <summary>
	/// The base abstract class of vertex adjacency with fixed capacity provided for performance optimization.
	/// Performance tests executed on .net 4.7 have shown that fixed capacity structure up to 7 edges is superior to unlimited 
	/// <see cref="HashSet{T}"/>/<see cref="Dictionary{TKey,TValue}"/> based structure, when it comes to adjacency 
	/// enumeration and contains operations execution time.
	/// </summary>
	/// <seealso cref="ReplaceableVertexAdjacency" />
	internal abstract class ReplaceableVertexAdjacencyWithFixedCapacity : ReplaceableVertexAdjacency
	{
		/// <inheritdoc />
		public override int MaxCapacity => EdgesCount;

		/// <inheritdoc />
		public override void Initialize(IEnumerable<int> destIndexes)
		{
			using (var enumerator = destIndexes.GetEnumerator())
			{
				var data = new AdjacencyEnumerator(enumerator);
				Set(data);

				var leftItems = 0;
				while (enumerator.MoveNext())
				{
					leftItems++;
				}

				ValidateInitializedNumberOfEdges(EdgesCount + leftItems);
			}
		}

		protected abstract void Set(AdjacencyEnumerator adj);

		/// <inheritdoc />
		public override void Reset()
		{
			Set(new AdjacencyEnumerator(null));
		}

		protected void ValidateIndex(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException(
					$"Not valid vertex index value ({index}). Vertex index must be equal or greater than zero.");
			}
		}

		protected void ValidateIndexesNotEqual(int x, int y)
		{
			if (x == y)
			{
				throw new InvalidOperationException(
					$"Two adjacent vertices have indexes with same values ({x}). Edges must have unique vertices.");
			}
		}

		protected void ValidateInitializedNumberOfEdges(int count)
		{
			if (count == EdgesCount)
			{
				return;
			}

			throw new InvalidOperationException(
				$"Not able to initialize vertex with capacity fixed to {EdgesCount} with {count} edges.");
		}

		/// <inheritdoc />
		public override bool AddEdge(ReplaceableVertexFactoryInterface<EmptyValue> pool,
		                             int destVertexIndex,
		                             EmptyValue edgeData,
		                             out ReplaceableVertexAdjacency<EmptyValue> final)
		{
			if (destVertexIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(destVertexIndex));
			}

			if (ContainsEdgeToIndex(destVertexIndex))
			{
				final = this;
				return false;
			}

			final = pool.GetInstance(
				GetValidatedAdjacentIndexes().Append(destVertexIndex).Select(idx => AdjacentEdge.Create(idx)), EdgesCount + 1);
			return true;
		}

		private IEnumerable<int> GetValidatedAdjacentIndexes()
		{
			foreach (var adjacentIndex in AdjacentIndexes)
			{
				if (adjacentIndex < 0)
				{
					throw new InvalidOperationException("Vertex instance is not initialized.");
				}

				yield return adjacentIndex;
			}
		}

		/// <inheritdoc />
		public override bool RemoveEdge(ReplaceableVertexFactoryInterface<EmptyValue> pool,
		                                int destVertexIndex,
		                                out ReplaceableVertexAdjacency<EmptyValue> final)
		{
			if (destVertexIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(destVertexIndex));
			}

			if (!ContainsEdgeToIndex(destVertexIndex))
			{
				final = default;
				return false;
			}

			final = pool.GetInstance(
				GetValidatedAdjacentIndexes().Where(idx => idx != destVertexIndex).Select(idx => AdjacentEdge.Create(idx)), EdgesCount - 1);
			return true;
		}

		protected struct AdjacencyEnumerator
		{
			private readonly IEnumerator<int> _adjacency;

			public AdjacencyEnumerator(IEnumerator<int> adjacency)
			{
				_adjacency = adjacency;
			}

			public int GetNext()
			{
				if (_adjacency == null)
				{
					return -1;
				}

				if (!_adjacency.MoveNext())
				{
					throw new InvalidOperationException($"Not enough vertices to initialize vertex with fixed capacity.");
				}

				var adj = _adjacency.Current;

				ValidateIndex(adj);

				return adj;
			}

			private static void ValidateIndex(int index)
			{
				if (index < 0)
				{
					throw new IndexOutOfRangeException(
						$"Not valid vertex index value ({index}). Vertex index must be equal or greater than zero.");
				}
			}
		}
	}
}