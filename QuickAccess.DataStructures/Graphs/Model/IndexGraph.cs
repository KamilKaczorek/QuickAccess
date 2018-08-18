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
using System.Data;
using System.Linq;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Graphs.Extensions;
using QuickAccess.DataStructures.Graphs.Factory;
using QuickAccess.DataStructures.Graphs.Factory.Internal;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	/// The implementation of the editable index graph.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="IReadOnlyGraph{TEdgeData}" />
	/// <seealso cref="IGraphSource{TEdgeData, TVertexKey}" />
	public sealed class IndexGraph<TEdgeData>
		: IReadOnlyGraph<TEdgeData>,
			IGraphSource<TEdgeData, int>
	{
		private readonly GraphEdgesCounter _edgesCounter;
		private readonly DefaultTailList<VertexAdjacency<TEdgeData>> _vertices;
		private IVertexAdjacencyModifier<TEdgeData> _vertexModifier;


		/// <summary>Initializes a new instance of the <see cref="IndexGraph{TEdgeData}"/> class.</summary>
		/// <param name="adjacency">The adjacency.</param>
		/// <param name="capacity">The capacity.</param>
		/// <param name="vertexModifier">The vertex modifier.</param>
		public IndexGraph(IReadOnlyList<VertexAdjacency<TEdgeData>> adjacency, int capacity = 0, IVertexAdjacencyModifier<TEdgeData> vertexModifier = null)
		{
			var specifiedVerticesCount = adjacency.GetIndexOfLastNotEmptyVertex() + 1;
			capacity = Math.Max(capacity, specifiedVerticesCount);

			_vertexModifier = vertexModifier ?? new VertexAdjacencyFactory<TEdgeData>();
			_edgesCounter = new GraphEdgesCounter();

			var source = new List<VertexAdjacency<TEdgeData>>(capacity);

			source.AddRange(adjacency.Take(specifiedVerticesCount));

			_vertices = new DefaultTailList<VertexAdjacency<TEdgeData>>(source, _vertexModifier.Empty, adjacency.Count);

			Connectivity = _vertices.WrapAsGraphConnectivity();
		}

		/// <summary>Initializes a new instance of the <see cref="IndexGraph{TEdgeData}" /> class.</summary>
		/// <param name="vertexModifier">The vertex adjacency modifier.</param>
		/// <param name="initialVerticesCount">The initial vertices count.</param>
		/// <param name="capacity">The vertices capacity.</param>
		public IndexGraph(int initialVerticesCount = 0, int capacity = 0, IVertexAdjacencyModifier<TEdgeData> vertexModifier = null)
		{
			_vertexModifier = vertexModifier ?? new VertexAdjacencyFactory<TEdgeData>();
			_edgesCounter = new GraphEdgesCounter();
			_vertices = new DefaultTailList<VertexAdjacency<TEdgeData>>(
				_vertexModifier.Empty, initialVerticesCount, capacity);
			Connectivity = _vertices.WrapAsGraphConnectivity();
		}

		/// <inheritdoc />
		public event EventHandler<GraphConnectivityChangedEventArgs> ConnectivityChanged
		{
			add
			{
				if (IsFrozen)
				{
					return;
				}

				InternalChanged += value;
			}
			remove => InternalChanged -= value;
		}

		private event EventHandler<GraphConnectivityChangedEventArgs> InternalChanged;

		/// <inheritdoc />
		public bool IsReadOnly => IsFrozen;

		/// <inheritdoc />
		public GraphConnectivityDefinition<TEdgeData> Connectivity { get; }

		/// <inheritdoc />
		public bool AddEdge(int srcVertexKey, int destVertexKey, TEdgeData edgeData)
		{
			if (IsFrozen)
			{
				throw new ReadOnlyException("Graph source is frozen, can't add an edge to a frozen graph.");
			}

			AddVertex(Math.Max(srcVertexKey, destVertexKey));

			var vertex = _vertices[srcVertexKey];

			var res = _edgesCounter.IncreseWhen(_vertexModifier.AddEdge(destVertexKey, edgeData, ref vertex));

			_vertices[srcVertexKey] = vertex;

			if (res)
			{
				InternalChanged?.Invoke(this, GraphConnectivityChangedEventArgs.CreateEdgeAdded(srcVertexKey, destVertexKey));
			}
			else
			{
				if (!EmptyValue.IsEmptyValueType<TEdgeData>())
				{
					InternalChanged?.Invoke(this,
						GraphConnectivityChangedEventArgs.CreateEdgeDataChanged(srcVertexKey, destVertexKey));
				}
			}

			return res;
		}

		/// <inheritdoc />
		public int RemoveAllEdgesFrom(int srcVertexKey)
		{
			if (IsFrozen)
			{
				throw new ReadOnlyException("Graph source is frozen, can't remove edge from a frozen graph.");
			}

			if (!_vertices.ContainsDefinedInstanceAt(srcVertexKey))
			{
				return 0;
			}

			var vertex = _vertices[srcVertexKey];
			var count = _vertexModifier.RemoveAllEdges(ref vertex);

			if (count <= 0)
			{
				return 0;
			}

			_vertices[srcVertexKey] = vertex;

	        InternalChanged?.Invoke(this, GraphConnectivityChangedEventArgs.CreateAllEdgesFromRemoved(srcVertexKey));
			
			return count;			
		}

		/// <inheritdoc />
		public bool AddVertex(int vertexKey)
		{
			if (IsFrozen)
			{
				throw new ReadOnlyException("Graph source is frozen, can't add a vertex to a frozen graph.");
			}

			if (vertexKey < 0)
			{
				throw new IndexOutOfRangeException();
			}

			var count = _vertices.ExtendCountTo(vertexKey + 1);

			if (count <= 0)
			{
				return false;
			}
			
			InternalChanged?.Invoke(this,
				GraphConnectivityChangedEventArgs.CreateVertexAdded(vertexKey+1-count, vertexKey));

			return true;
		}

		/// <inheritdoc />
		public int RemoveAllEdgesTo(int destVertexKey)
		{
			if (IsFrozen)
			{
				throw new ReadOnlyException("Graph source is frozen, can't remove edge from a frozen graph.");
			}

			if (destVertexKey < 0 || destVertexKey >= _vertices.Count)			   
			{
				return 0;
			}

			var count = 0;
			for (var srcIndex = 0; srcIndex < _vertices.DefinedInstancesCount; srcIndex++)
			{
				if (RemoveEdge(srcIndex, destVertexKey))
				{
					++count;						
				}
			}

			if (count > 0)
			{
				InternalChanged?.Invoke(this, GraphConnectivityChangedEventArgs.CreateAllEdgesToRemoved(destVertexKey));					
			}

			return count;
		}

		/// <inheritdoc />
		public void Freeze()
		{
			_vertexModifier = null;
			InternalChanged?.Invoke(this, GraphConnectivityChangedEventArgs.CreateFrozen());
		}

		/// <inheritdoc />
		public bool IsFrozen => _vertexModifier == null;

		/// <inheritdoc />
		public void Clear(bool removeAdjacencyOnly = false)
		{
			if (IsFrozen)
			{
				throw new ReadOnlyException("Graph source is frozen, can't clear a frozen graph.");
			}

			var count = _vertices.Count;

			_edgesCounter.Reset();

			if (removeAdjacencyOnly)
			{
				for (int i = 0; i < _vertices.Count; i++)
				{
					_vertices[i] = _vertexModifier.Empty;
				}
			}
			else
			{
				_vertices.Clear();
			}

			if (count > 0)
			{
				InternalChanged?.Invoke(this, GraphConnectivityChangedEventArgs.CreateReset());					
			}
		}

		/// <inheritdoc />
		public bool RemoveEdge(int srcVertexKey, int destVertexKey)
		{
			if (IsFrozen)
			{
				throw new ReadOnlyException("Graph source is frozen, can't remove edge from a frozen graph.");
			}

			if (Math.Min(srcVertexKey, destVertexKey) < 0 || Math.Max(srcVertexKey, destVertexKey) >= _vertices.Count ||
			    !_vertices.ContainsDefinedInstanceAt(srcVertexKey))
			{
				return false;
			}

			var vertex = _vertices[srcVertexKey];

			var res = _edgesCounter.DecreaseWhen(_vertexModifier.RemoveEdge(destVertexKey, ref vertex));

			if (res)
			{
				_vertices[srcVertexKey] = vertex;
				InternalChanged?.Invoke(this, GraphConnectivityChangedEventArgs.CreateEdgeRemoved(srcVertexKey, destVertexKey));					
			}

			return res;
		}
	}
}