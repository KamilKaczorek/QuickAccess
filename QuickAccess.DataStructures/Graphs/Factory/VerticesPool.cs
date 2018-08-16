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
using QuickAccess.DataStructures.Graphs.Factory.Internal;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory
{
	/// <summary>
	/// The static factory of instances of <see cref="VerticesPool{TEdgeData}"/> and <see cref="SynchronizedVerticesPool{TEdgeData}"/> types.
	/// </summary>
	public static class VerticesPool
	{
			
		public static IVerticesPool<TEdgeData> Create<TEdgeData>(int totalCapacity)
		{
			return new VerticesPool<TEdgeData>(totalCapacity, VerticesPoolCapacityType.TotalPoolCapacity);
		}

		public static IVerticesPool<TEdgeData> CreateWithCapacityPerVertexType<TEdgeData>(int capacityPerVertexType)
		{
			return new VerticesPool<TEdgeData>(capacityPerVertexType, VerticesPoolCapacityType.CapacityPerVertexType);
		}

		public static IVerticesPool<TEdgeData> CreateSynchronized<TEdgeData>(int totalCapacity, object synchRoot = null)
		{
			return new SynchronizedVerticesPool<TEdgeData>(new VerticesPool<TEdgeData>(totalCapacity, VerticesPoolCapacityType.TotalPoolCapacity), synchRoot);
		}

		public static IVerticesPool<TEdgeData> CreateSynchronizedWithCapacityPerVertexType<TEdgeData>(int capacityPerVertexType, object synchRoot = null)
		{
			return new SynchronizedVerticesPool<TEdgeData>(new VerticesPool<TEdgeData>(capacityPerVertexType, VerticesPoolCapacityType.CapacityPerVertexType), synchRoot);
		}

		public static IVerticesPool<TEdgeData> CreateWithNoPool<TEdgeData>()
		{
			return new VerticesPool<TEdgeData>(0, VerticesPoolCapacityType.TotalPoolCapacity);
		}
	}

	/// <summary>
	///     The implementation of vertices pool.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="IVerticesPool{TEdgeData}" />
	public sealed class VerticesPool<TEdgeData> : IVerticesPool<TEdgeData>
	{
		private readonly InternalPoolBase<TEdgeData> _pool;

		/// <summary>Initializes a new instance of the <see cref="VerticesPool{TEdgeData}" /> class.</summary>
		/// <param name="capacity">The total capacity.</param>
		/// <param name="capacityType">Type of the capacity.</param>
		public VerticesPool(int capacity, VerticesPoolCapacityType capacityType)
		{
			_pool = EmptyValue.IsEmptyValueType<TEdgeData>() ? 
				(InternalPoolBase<TEdgeData>)(object)new InternalEmptyValueEdgePool(capacity, capacityType) :				
				new InternalPool(capacity, capacityType);
		}

		/// <inheritdoc />
		public VertexAdjacency<TEdgeData> Empty => EmptyVertexAdjacency<TEdgeData>.Instance;

		/// <inheritdoc />
		public VertexAdjacency<TEdgeData> GetInstance(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo, int edgesCount)
		{
			if (edgesCount <= 0)
			{
				return Empty;
			}

			return _pool.GetInstance(edgesTo, edgesCount);
		}

		private PoolableVertexAdjacency<TEdgeData> GetPoolable(VertexAdjacency<TEdgeData> current)
		{
			if (current is PoolableVertexAdjacency<TEdgeData> poolable)
			{
				return poolable;
			}

			return current == null || current.EdgesCount <= 0
				? EmptyVertexAdjacency<TEdgeData>.Instance
				: _pool.GetInstance(current, current.EdgesCount);
		}

		/// <inheritdoc />
		public bool AddEdge(int destVertexIndex, TEdgeData edgeData, ref VertexAdjacency<TEdgeData> current)
		{
			var res = GetPoolable(current);

			if (!res.AddEdge(_pool, destVertexIndex, edgeData, out res))
			{
				return false;
			}

			current = res;
			return true;
		}

		/// <inheritdoc />
		public bool RemoveEdge(int destVertexIndex, ref VertexAdjacency<TEdgeData> current)
		{
			if (current == null || current.EdgesCount <= 0)
			{
				return false;
			}

			var res = GetPoolable(current);

			if (!res.RemoveEdge(_pool, destVertexIndex, out res))
			{
				return false;
			}

			current = res;
			return true;
		}

		/// <inheritdoc />
		public int RemoveAllEdges(ref VertexAdjacency<TEdgeData> current)
		{
			var count = current?.EdgesCount ?? 0;

			if (count <= 0)
			{
				return 0;
			}

			if (current is PoolableVertexAdjacency<TEdgeData> poolable)
			{
				_pool.ReturnInstance(poolable);
			}

			current = Empty;
			return count;
		}

		
		/// <inheritdoc />
		public void ReturnRange(IEnumerable<VertexAdjacency<TEdgeData>> instances)
		{
			foreach (var instance in instances.OfType<PoolableVertexAdjacency<TEdgeData>>())
			{
				if (!_pool.ReturnInstance(instance))
				{
					return;
				}
			}
		}

		/// <inheritdoc />
		public void Clear()
		{
			_pool.Clear();
		}

		private abstract class InternalPoolBase<T> : PoolableVertexFactoryInterface<T>
		{
			public abstract void Clear();


			protected int GetCapacityPerVertexType(int capacity, VerticesPoolCapacityType capacityType, int numberOfPools)
			{
				if (capacity <= 0)
				{
					return 0;
				}

				if (capacityType == VerticesPoolCapacityType.CapacityPerVertexType)
				{
					return capacity;
				}

				return Math.Max(1, capacity / numberOfPools);
			}
		}

		private sealed class InternalEmptyValueEdgePool : InternalPoolBase<EmptyValue>
		{
			private readonly List<PoolableVertexAdjacency>[] _poolPerType;

			private bool _hasCapacity;

			private const int FixedCapacityEdgeCount = 7;

			public InternalEmptyValueEdgePool(int capacity, VerticesPoolCapacityType poolCapacityType)
			{
				var singleCapacity = GetCapacityPerVertexType(capacity, poolCapacityType, FixedCapacityEdgeCount);

				if (singleCapacity == 0)
				{
					_hasCapacity = false;
					return;
				}

				_hasCapacity = true;

			

				_poolPerType = new List<PoolableVertexAdjacency>[FixedCapacityEdgeCount];

				for (var idx = 0; idx < _poolPerType.Length; idx++)
				{
					_poolPerType[idx] = new List<PoolableVertexAdjacency>(singleCapacity);
				}				
			}

			/// <inheritdoc />
			public override PoolableVertexAdjacency<EmptyValue> Empty => EmptyVertexAdjacency<EmptyValue>.Instance;

			private PoolableVertexAdjacency Create(int count)
			{
				switch (count)
				{				
					case 1 : return new VertexAdjacency1Edge();
					case 2 : return new VertexAdjacency2Edges();
					case 3 : return new VertexAdjacency3Edges();
					case 4 : return new VertexAdjacency4Edges();
					case 5 : return new VertexAdjacency5Edges();
					case 6 : return new VertexAdjacency6Edges();
					case 7 : return new VertexAdjacency7Edges();
					default: return new VertexAdjacencyUnlimitedEdges();
				}
			}

			private PoolableVertexAdjacency GetFromPool(int count)
			{
				if (_poolPerType == null || count > FixedCapacityEdgeCount)
				{
					return null;
				}

				var res = _poolPerType[count - 1].ExtractLastOrDefault();

				if (res != null && !_hasCapacity)
				{
					_hasCapacity = true;
				}

				return res;
			}
			

			/// <inheritdoc />
			public override PoolableVertexAdjacency<EmptyValue> GetInstance(IEnumerable<AdjacentEdge<EmptyValue>> adjacentEdges, int count)
			{
				if (count == 0)
				{
					return Empty;
				}

				var res = GetFromPool(count) ?? Create(count);

				res.Initialize(adjacentEdges.Select(e => e.Destination));

				if (res.EdgesCount != count)
				{
					throw new ArgumentException($"The specified number of edges ({count}) differs from the number of items in a sequence ({res.EdgesCount}).", nameof(count));
				}

				return res;
			}

			/// <inheritdoc />
			public override bool ProvidesFixedCapacity(int edgesCount)
			{
				return edgesCount <= FixedCapacityEdgeCount;
			}

			/// <inheritdoc />
			public override bool ReturnInstance(PoolableVertexAdjacency<EmptyValue> instance)
			{
				if (!_hasCapacity || !(instance is PoolableVertexAdjacency adj))
				{
					return _hasCapacity;
				}

			
				if (!ProvidesFixedCapacity(adj.MaxCapacity))
				{
					return _hasCapacity;
				}

				var poolIndex = adj.MaxCapacity-1;
				var pool = _poolPerType[poolIndex];

				if (pool.Count < pool.Capacity)
				{
					adj.Reset();
					pool.Add(adj);
				}

				_hasCapacity = _poolPerType.Any(p => p.Count < p.Capacity);

				return _hasCapacity;
			}

			public override void Clear()
			{
				foreach (var pool in _poolPerType)
				{
					pool.Clear();
				}
			}
		}

		
		private sealed class InternalPool : InternalPoolBase<TEdgeData>
		{
			private readonly List<PoolableVertexAdjacency<TEdgeData>>[] _poolPerType;

			private bool _hasCapacity;
			private const int FixedCapacityEdgeCount = 7;
			


			public InternalPool(int capacity, VerticesPoolCapacityType capacityType)
			{
				var singleCapacity = GetCapacityPerVertexType(capacity, capacityType, FixedCapacityEdgeCount);

				if (singleCapacity == 0)
				{
					_hasCapacity = false;
					return;
				}

				_hasCapacity = true;

			

				_poolPerType = new List<PoolableVertexAdjacency<TEdgeData>>[FixedCapacityEdgeCount];

				for (var idx = 0; idx < _poolPerType.Length; idx++)
				{
					_poolPerType[idx] = new List<PoolableVertexAdjacency<TEdgeData>>(singleCapacity);
				}				
			}

			/// <inheritdoc />
			public override PoolableVertexAdjacency<TEdgeData> Empty => EmptyVertexAdjacency<TEdgeData>.Instance;


			private PoolableVertexAdjacency<TEdgeData> Create(int count)
			{
				switch (count)
				{				
					case 1 : return new VertexAdjacency1Edge<TEdgeData>();
					case 2 : return new VertexAdjacency2Edges<TEdgeData>();
					case 3 : return new VertexAdjacency3Edges<TEdgeData>();
					case 4 : return new VertexAdjacency4Edges<TEdgeData>();
					case 5 : return new VertexAdjacency5Edges<TEdgeData>();
					case 6 : return new VertexAdjacency6Edges<TEdgeData>();
					case 7 : return new VertexAdjacency7Edges<TEdgeData>();
					default: return new VertexAdjacencyUnlimitedEdges<TEdgeData>();
				}
			}


			private PoolableVertexAdjacency<TEdgeData> GetFromPool(int count)
			{
				if (_poolPerType == null || count > FixedCapacityEdgeCount)
				{
					return null;
				}

				var res = _poolPerType[count - 1].ExtractLastOrDefault();

				if (res != null && !_hasCapacity)
				{
					_hasCapacity = true;
				}

				return res;
			}
			

			/// <inheritdoc />
			public override PoolableVertexAdjacency<TEdgeData> GetInstance(IEnumerable<AdjacentEdge<TEdgeData>> adjacentEdges, int count)
			{
				if (count == 0)
				{
					return Empty;
				}

				var res = GetFromPool(count) ?? Create(count);

				res.Initialize(adjacentEdges);

				if (res.EdgesCount != count)
				{
					throw new ArgumentException($"The specified number of edges ({count}) differs from the number of items in a sequence ({res.EdgesCount}).", nameof(count));
				}

				return res;
			}

			/// <inheritdoc />
			public override bool ProvidesFixedCapacity(int edgesCount)
			{
				return edgesCount <= FixedCapacityEdgeCount;
			}

			/// <inheritdoc />
			public override bool ReturnInstance(PoolableVertexAdjacency<TEdgeData> instance)
			{
				if (!_hasCapacity || instance == null)
				{
					return _hasCapacity;
				}

			
				if (!ProvidesFixedCapacity(instance.MaxCapacity))
				{
					return _hasCapacity;
				}

				var poolIndex = instance.MaxCapacity-1;
				var pool = _poolPerType[poolIndex];

				if (pool.Count < pool.Capacity)
				{
					instance.Reset();
					pool.Add(instance);
				}

				_hasCapacity = _poolPerType.Any(p => p.Count < p.Capacity);

				return _hasCapacity;
			}

			public override void Clear()
			{
				foreach (var pool in _poolPerType)
				{
					pool.Clear();
				}
			}
		}
	}
}