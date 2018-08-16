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
using System.Diagnostics.Contracts;
using System.Linq;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Graphs.Extensions;
using QuickAccess.DataStructures.Graphs.Factory;
using QuickAccess.DataStructures.Graphs.Factory.Internal;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     The factory of instances of <see cref="GraphConnectivityDefinition{TEdgeData}" /> type.
	/// </summary>
	public class GraphConnectivityDefinitionFactory
		: IGraphConnectivityDefinitionFactory
	{
		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TEdgeData> GetEmpty<TEdgeData>()
		{
			return EmptyGraphConnectivityDefinition<TEdgeData>.Instance;
		}

		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(List<VertexAdjacency<TEdgeData>> sourceList)
		{
			return new ListBasedGraphConnectivityDefinition<TEdgeData>(sourceList);
		}

		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(IReadOnlyList<VertexAdjacency<TEdgeData>> sourceList)
		{
			return new ReadOnlyListBasedGraphConnectivityDefinition<TEdgeData>(sourceList);
		}

		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(
			DefaultTailList<VertexAdjacency<TEdgeData>> sourceList)
		{
			return new DefaultTailListBasedGraphConnectivityDefinition<TEdgeData>(sourceList);
		}

		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(VertexAdjacency<TEdgeData>[] sourceArray)
		{
			return new ArrayBasedGraphConnectivityDefinition<TEdgeData>(sourceArray);
		}

		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TEdgeData> CreateEmptyTailWrapperOf<TEdgeData>(
			VertexAdjacency<TEdgeData>[] sourceArray,
			int totalCount)
		{
			if (totalCount < sourceArray.Length)
			{
				throw new ArgumentException("Total count is smaller than array size.", nameof(totalCount));
			}

			return totalCount == sourceArray.Length
				? (GraphConnectivityDefinition<TEdgeData>) new ArrayBasedGraphConnectivityDefinition<TEdgeData>(sourceArray)
				: new EmptyTailReadOnlyGraphConnectivityDefinition<TEdgeData>(sourceArray, totalCount);
		}

		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(
			Dictionary<int, VertexAdjacency<TEdgeData>> indexToVertexMap,
			int totalCount)
		{
			if (totalCount < indexToVertexMap.Count)
			{
				throw new ArgumentException("Total count is smaller than dictionary size.", nameof(totalCount));
			}

			return new DictionaryBasedGraphConnectivityDefinition<TEdgeData>(indexToVertexMap, totalCount);
		}

		/// <inheritdoc />
		[Pure]
		public GraphConnectivityDefinition<TTargetEdgeData> Create<TSrcEdgeData, TTargetEdgeData>(
			IReadOnlyList<VertexAdjacency<TSrcEdgeData>> sourceAdjacency,
			IVertexAdjacencyFactory<TTargetEdgeData> verticesFactory = null,
			Func<TSrcEdgeData, TTargetEdgeData> targetEdgeDataSelector = null,
			bool sharedVerticesInstances = false,
			IEqualityComparer<VertexAdjacency<TSrcEdgeData>> vertexEqualityComparer = null)
		{
			if (sourceAdjacency == null || sourceAdjacency.Count <= 0)
			{
				return GetEmpty<TTargetEdgeData>();
			}

			if (verticesFactory == null)
			{
				verticesFactory = VerticesPool.Create<TTargetEdgeData>(0);
			}

			if (targetEdgeDataSelector == null)
			{
				if (EmptyValue.IsEmptyValueType<TTargetEdgeData>())
				{
					targetEdgeDataSelector = prev => (TTargetEdgeData) (object) EmptyValue.Value;
				}
				else
				{
					if (!typeof(TTargetEdgeData).IsAssignableFrom(typeof(TSrcEdgeData)))
					{
						throw new ArgumentNullException(nameof(targetEdgeDataSelector),
							"Edge data selector can't be evaluated for specified types and must be defined.");
					}

					targetEdgeDataSelector = prev => (TTargetEdgeData) (object) prev;
				}
			}

			var oldVertices = sourceAdjacency;

			var destItemsCount = oldVertices.GetIndexOfLastNotEmptyVertex() + 1;

			var newVertices = new VertexAdjacency<TTargetEdgeData>[destItemsCount];

			var oldToNewVertexInstanceMap = sharedVerticesInstances
				? new Dictionary<VertexAdjacency<TSrcEdgeData>, VertexAdjacency<TTargetEdgeData>>(
					vertexEqualityComparer ?? VertexAdjacencyComparer<TSrcEdgeData>.Default)
				: null;

			for (var idx = 0; idx < destItemsCount; idx++)
			{
				var oldVertex = oldVertices[idx];

				VertexAdjacency<TTargetEdgeData> newVertex;

				if (oldToNewVertexInstanceMap != null)
				{
					if (!oldToNewVertexInstanceMap.TryGetValue(oldVertex, out newVertex))
					{
						newVertex = verticesFactory.GetInstance(
							oldVertex.Select(e => AdjacentEdge.Create(e.Destination, targetEdgeDataSelector.Invoke(e.Data))),
							oldVertex.Count);

						oldToNewVertexInstanceMap.Add(oldVertex, newVertex);
					}
				}
				else
				{
					newVertex = verticesFactory.GetInstance(
						oldVertex.Select(e => AdjacentEdge.Create(e.Destination, targetEdgeDataSelector.Invoke(e.Data))),
						oldVertex.Count);
				}

				newVertices[idx] = newVertex;
			}

			return CreateEmptyTailWrapperOf(newVertices, oldVertices.Count);
		}

		/// <inheritdoc />
		[Pure]
		public ReindexedDataResult<GraphConnectivityDefinition<TEdgeData>> CreateCompacted<TEdgeData>(
			GraphConnectivityDefinition<TEdgeData> source,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory,
			bool sharedVerticesInstances,
			IEqualityComparer<VertexAdjacency<TEdgeData>> vertexEqualityComparer = null)
		{
			var oldVertices = source.ToArray();

			var map = oldVertices.SortWithReindexingResult(VertexAdjacencyComparer<TEdgeData>.Default);

			var cloned = Create(oldVertices,
				new ReindexedVertexAdjacencyFactoryWrapper<TEdgeData>(map, verticesFactory ?? VerticesPool.Create<TEdgeData>(0)), d => d,
				sharedVerticesInstances, vertexEqualityComparer);

			return map.ToReindexedDataResult(cloned);
		}

		internal sealed class EmptyGraphConnectivityDefinition<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			public static EmptyGraphConnectivityDefinition<TEdgeData> Instance = new EmptyGraphConnectivityDefinition<TEdgeData>();

			/// <inheritdoc />
			public override int Count => 0;

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index] =>
				throw new IndexOutOfRangeException("Can't access elements of empty collection.");

			private EmptyGraphConnectivityDefinition()
			{
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				yield break;
			}
		}

		internal sealed class ReadOnlyListBasedGraphConnectivityDefinition<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			private readonly IReadOnlyList<VertexAdjacency<TEdgeData>> _adjacency;

			/// <inheritdoc />
			public override int Count => _adjacency.Count;

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index] => _adjacency[index];

			public ReadOnlyListBasedGraphConnectivityDefinition(IReadOnlyList<VertexAdjacency<TEdgeData>> adjacency)
			{
				_adjacency = adjacency;
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				return _adjacency.GetEnumerator();
			}
		}

		internal sealed class ListBasedGraphConnectivityDefinition<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			private readonly List<VertexAdjacency<TEdgeData>> _adjacency;

			/// <inheritdoc />
			public override int Count => _adjacency.Count;

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index] => _adjacency[index];

			public ListBasedGraphConnectivityDefinition(List<VertexAdjacency<TEdgeData>> adjacency)
			{
				_adjacency = adjacency;
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				return _adjacency.GetEnumerator();
			}
		}

		internal sealed class DefaultTailListBasedGraphConnectivityDefinition<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			private readonly DefaultTailList<VertexAdjacency<TEdgeData>> _adjacency;

			/// <inheritdoc />
			public override int Count => _adjacency.Count;

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index] => _adjacency[index];

			public DefaultTailListBasedGraphConnectivityDefinition(DefaultTailList<VertexAdjacency<TEdgeData>> adjacency)
			{
				_adjacency = adjacency;
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				return _adjacency.GetEnumerator();
			}
		}

		internal sealed class ArrayBasedGraphConnectivityDefinition<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			private readonly VertexAdjacency<TEdgeData>[] _adjacency;

			/// <inheritdoc />
			public override int Count => _adjacency.Length;

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index] => _adjacency[index];

			public ArrayBasedGraphConnectivityDefinition(VertexAdjacency<TEdgeData>[] adjacency)
			{
				_adjacency = adjacency;
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				return _adjacency.Cast<VertexAdjacency<TEdgeData>>().GetEnumerator();
			}
		}

		internal sealed class EmptyTailReadOnlyGraphConnectivityDefinition<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			private readonly VertexAdjacency<TEdgeData>[] _adjacency;

			/// <inheritdoc />
			public override int Count { get; }

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index] => index >= _adjacency.Length && index < Count
				? EmptyVertexAdjacency<TEdgeData>.Instance
				: _adjacency[index];

			public EmptyTailReadOnlyGraphConnectivityDefinition(VertexAdjacency<TEdgeData>[] adjacency, int totalCount)
			{
				_adjacency = adjacency;
				Count = totalCount;
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				return _adjacency.Concat(Enumerable.Repeat(EmptyVertexAdjacency<TEdgeData>.Instance, Count - _adjacency.Length))
				                 .GetEnumerator();
			}
		}

		internal sealed class DictionaryBasedGraphConnectivityDefinition<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			private readonly Dictionary<int, VertexAdjacency<TEdgeData>> _indexToVertexMap;

			/// <inheritdoc />
			public override int Count { get; }

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index]
			{
				get
				{
					if (index < 0 || index >= Count)
					{
						throw new IndexOutOfRangeException();
					}

					return _indexToVertexMap.TryGetValue(index, out var vertex) ? vertex : EmptyVertexAdjacency<TEdgeData>.Instance;
				}
			}

			public DictionaryBasedGraphConnectivityDefinition(Dictionary<int, VertexAdjacency<TEdgeData>> indexToVertexMap,
			                                                  int totalCount)
			{
				_indexToVertexMap = indexToVertexMap;
				Count = totalCount;
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				return GetVertexIndexes().Select(idx => this[idx]).GetEnumerator();
			}
		}

		internal sealed class GraphConnectivityDefinitionEdgeFilter<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			internal readonly GraphConnectivityDefinition<TEdgeData> Wrapped;
			internal readonly Func<Edge<int, TEdgeData>, bool> SelectEdgePredicate;
			private readonly Dictionary<int, VertexAdjacency<TEdgeData>> _adjWrappers;

			public GraphConnectivityDefinitionEdgeFilter(GraphConnectivityDefinition<TEdgeData> wrapped, Func<Edge<int, TEdgeData>, bool> selectEdgePredicate)
			{
				Wrapped = wrapped ?? throw new ArgumentNullException(nameof(wrapped));
				SelectEdgePredicate = selectEdgePredicate ?? throw new ArgumentNullException(nameof(selectEdgePredicate));
				_adjWrappers = new Dictionary<int, VertexAdjacency<TEdgeData>>(wrapped.Count);
			}

			private sealed class VertexAdjacencyWrapper : VertexAdjacency<TEdgeData>
			{
				private readonly GraphConnectivityDefinitionEdgeFilter<TEdgeData> _owner;				
				private readonly int _currentIndex;

				private VertexAdjacency<TEdgeData> Wrapped => _owner.Wrapped[_currentIndex];
				private bool IsIncluded(AdjacentEdge<TEdgeData> adj) => _owner.SelectEdgePredicate.Invoke(adj.ToEdge(_currentIndex));
				private bool IsIncluded(int dst, TEdgeData data) => _owner.SelectEdgePredicate.Invoke(Edge.Create(_currentIndex, dst, data));

				public VertexAdjacencyWrapper(GraphConnectivityDefinitionEdgeFilter<TEdgeData> owner, int currentIndex)
				{
					_owner = owner ?? throw new ArgumentNullException(nameof(owner));
					_currentIndex = currentIndex;
				}

				/// <inheritdoc />
				public override int EdgesCount =>
					Wrapped.Count(IsIncluded);

				/// <inheritdoc />
				public override IEnumerable<int> AdjacentIndexes =>
					Wrapped.Where(IsIncluded).Select(adj => adj.Destination);

				/// <inheritdoc />
				public override bool ContainsEdgeToIndex(int destVertexIndex)
				{
					return Wrapped.TryGetEdgeToIndex(destVertexIndex, out var data) &&
					       IsIncluded(destVertexIndex, data);
				}

				/// <inheritdoc />
				public override TEdgeData GetEdgeToIndex(int destVertexIndex)
				{
					var data = Wrapped.GetEdgeToIndex(destVertexIndex);

					if(!IsIncluded(destVertexIndex, data))
					{
						throw new KeyNotFoundException("Specified edge at ({_currentIndex},{destVertexIndex}) was filtered out.");
					}

					return data;
				}

				/// <inheritdoc />
				public override IEnumerator<AdjacentEdge<TEdgeData>> GetEnumerator()
				{
					return Wrapped.Where(IsIncluded).GetEnumerator();
				}

				/// <inheritdoc />
				public override bool TryGetEdgeToIndex(int destVertexIndex, out TEdgeData edgeData)
				{
					var res = Wrapped.TryGetEdgeToIndex(destVertexIndex, out edgeData) &&
					          IsIncluded(destVertexIndex, edgeData);
					if (!res)
					{
						edgeData = default;
					}

					return res;
				}
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				for (var i = 0; i < Wrapped.Count; ++i)
				{
					yield return this[i];
				}
			}

			/// <inheritdoc />
			public override int Count => Wrapped.Count;

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index]
			{
				get
				{
					if (!Wrapped.ContainsVertexAt(index))
					{
						throw new IndexOutOfRangeException();
					}

					if (!_adjWrappers.TryGetValue(index, out var wrapper))
					{
						wrapper = new VertexAdjacencyWrapper(this, index);
						_adjWrappers[index] = wrapper;
					}

					return wrapper;
				}
			}
		}
	}


}