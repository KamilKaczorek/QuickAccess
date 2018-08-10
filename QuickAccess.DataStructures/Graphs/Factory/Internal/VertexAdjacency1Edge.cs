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
using System.Linq;
using QuickAccess.DataStructures.Common;
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
	/// <seealso cref="PoolableVertexAdjacency{TEdgeData}" />
	internal sealed class VertexAdjacency1Edge<TEdgeData> : PoolableVertexAdjacency<TEdgeData>
	{
		private int _destVertexIndex;
		private TEdgeData _edgeData;

		/// <inheritdoc />
		public override int EdgesCount => 1;


		/// <inheritdoc />
		public override int MaxCapacity => 1;

		/// <inheritdoc />
		public override IEnumerable<int> AdjacentIndexes
		{
			get { yield return _destVertexIndex; }
		}

		

		/// <inheritdoc />
		public override bool AddEdge(PoolableVertexFactoryInterface<TEdgeData> pool,
		                             int destVertexIndex,
		                             TEdgeData edgeData,
		                             out PoolableVertexAdjacency<TEdgeData> final)
		{
			if (destVertexIndex == _destVertexIndex)
			{
				_edgeData = edgeData;
				final = this;
				return false;
			}

			final = pool.GetInstance(
				EnumerableExtensions.Enumerate(new AdjacentEdge<TEdgeData>(_destVertexIndex, _edgeData),
					new AdjacentEdge<TEdgeData>(destVertexIndex, edgeData)), 2);

			pool.ReturnInstance(this);
			return true;
		}

		/// <inheritdoc />
		public override bool RemoveEdge(PoolableVertexFactoryInterface<TEdgeData> pool,
		                                int destVertexIndex,
		                                out PoolableVertexAdjacency<TEdgeData> final)
		{
			if (destVertexIndex != _destVertexIndex)
			{
				final = this;
				return false;
			}

			final = pool.Empty;

			pool.ReturnInstance(this);
			return true;
		}

		/// <inheritdoc />
		public override bool ContainsEdgeToIndex(int destVertexIndex)
		{
			return destVertexIndex == _destVertexIndex;
		}

		/// <inheritdoc />
		public override void Initialize(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo)
		{
			var first = edgesTo.Single();
			_edgeData = first.Data;
			_destVertexIndex = first.Destination;
		}

		/// <inheritdoc />
		public override void Reset()
		{
			_edgeData = default;
			_destVertexIndex = -1;
		}

		/// <inheritdoc />
		public override bool TryGetEdgeToIndex(int destVertexIndex, out TEdgeData edgeData)
		{
			edgeData = _edgeData;
			return ContainsEdgeToIndex(destVertexIndex);
		}

		/// <inheritdoc />
		public override TEdgeData GetEdgeToIndex(int destVertexIndex)
		{
			if (!ContainsEdgeToIndex(destVertexIndex))
			{
				throw new KeyNotFoundException();
			}

			return _edgeData;
		}

		/// <inheritdoc />
		public override IEnumerator<AdjacentEdge<TEdgeData>> GetEnumerator()
		{
			yield return AdjacentEdge.Create(_destVertexIndex, _edgeData);
		}
	}

	/// <summary>
	/// Vertex adjacency with fixed capacity provided for performance optimization.
	/// Performance tests executed on .net 4.7 have shown that fixed capacity structure up to 7 edges is superior to unlimited 
	/// <see cref="HashSet{T}"/>/<see cref="Dictionary{TKey,TValue}"/> based structure, when it comes to adjacency 
	/// enumeration and contains operations execution time.
	/// </summary>
	/// <seealso cref="PoolableVertexAdjacency{TEdgeData}" />
	internal sealed class VertexAdjacency1Edge : PoolableVertexAdjacency
	{
		private int _destVertexIndex;

		/// <inheritdoc />
		public override int EdgesCount => 1;


		/// <inheritdoc />
		public override int MaxCapacity => 1;

		/// <inheritdoc />
		public override IEnumerable<int> AdjacentIndexes
		{
			get { yield return _destVertexIndex; }
		}

		
		/// <inheritdoc />
		public override bool AddEdge(PoolableVertexFactoryInterface<EmptyValue> pool,
		                             int destVertexIndex,
		                             EmptyValue edgeData,
		                             out PoolableVertexAdjacency<EmptyValue> final)
		{
			if (destVertexIndex == _destVertexIndex)
			{
				final = this;
				return false;
			}

			final = pool.GetInstance(
				EnumerableExtensions.Enumerate(new AdjacentEdge<EmptyValue>(_destVertexIndex, default),
					new AdjacentEdge<EmptyValue>(destVertexIndex, edgeData)), 2);
			pool.ReturnInstance(this);
			return true;
		}

		

		/// <inheritdoc />
		public override bool RemoveEdge(PoolableVertexFactoryInterface<EmptyValue> pool,
		                                int destVertexIndex,
		                                out PoolableVertexAdjacency<EmptyValue> final)
		{
			if (destVertexIndex != _destVertexIndex)
			{
				final = this;
				return false;
			}

			final = pool.Empty;
			pool.ReturnInstance(this);
			return true;
		}

		/// <inheritdoc />
		public override bool ContainsEdgeToIndex(int destVertexIndex)
		{
			return destVertexIndex == _destVertexIndex;
		}

		/// <inheritdoc />
		public override void Initialize(IEnumerable<int> destIndexes)
		{
			_destVertexIndex = destIndexes.Single();
		}

		/// <inheritdoc />
		public override void Reset()
		{
			_destVertexIndex = -1;
		}
	}
}