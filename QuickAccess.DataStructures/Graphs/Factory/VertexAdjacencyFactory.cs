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
using System.Linq;
using QuickAccess.DataStructures.Graphs.Factory.Internal;
using QuickAccess.DataStructures.Graphs.Model;
using QuickAccess.Infrastructure;

namespace QuickAccess.DataStructures.Graphs.Factory
{
	/// <summary>
	///     The implementation of the factory of <see cref="VertexAdjacency{TEdgeData}"/> instances.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="IVertexAdjacencyModifier{TEdgeData}" />
	public sealed class VertexAdjacencyFactory<TEdgeData> : IVertexAdjacencyModifier<TEdgeData>
	{
		private readonly ReplaceableVertexFactoryInterface<TEdgeData> _factory;

		private readonly bool _canReplaceVertexInstanceOnModification;

		/// <summary>Initializes a new instance of the <see cref="VertexAdjacencyFactory{TEdgeData}" /> class.</summary>
		/// <param name="canReplaceVertexInstanceOnModification">
		/// if set to <c>true</c> the vertex instance can be replaced after modification of the vertex adjacency; 
		/// otherwise, <c>false</c> - it will always produce vertex with unlimited capacity.
		/// </param>
		public VertexAdjacencyFactory(bool canReplaceVertexInstanceOnModification = true)
		{

			var replace = _canReplaceVertexInstanceOnModification = canReplaceVertexInstanceOnModification;

			_factory = EmptyValue.IsEmptyValueType<TEdgeData>() ? 
				(ReplaceableVertexFactoryInterface<TEdgeData>)(object)new InternalEmptyValueEdgeFactory(replace) :				
				new InternalFactory(replace);
		}

		/// <inheritdoc />
		public VertexAdjacency<TEdgeData> Empty => _factory.Empty;

		/// <inheritdoc />
		public VertexAdjacency<TEdgeData> GetInstance(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo, int edgesCount)
		{
			return _factory.GetInstance(edgesTo, edgesCount);
		}

		private ReplaceableVertexAdjacency<TEdgeData> GetReplaceable(VertexAdjacency<TEdgeData> current)
		{
			if (!_canReplaceVertexInstanceOnModification)
			{
				return (ReplaceableVertexAdjacency<TEdgeData>) current;
			}

			if (current is ReplaceableVertexAdjacency<TEdgeData> replaceable)
			{
				return replaceable;
			}

			return current == null || current.EdgesCount <= 0
				? EmptyVertexAdjacency<TEdgeData>.Instance
				: _factory.GetInstance(current, current.EdgesCount);
		}

		/// <inheritdoc />
		/// <remarks>
		/// The <paramref name="current"/> instance will be replaced only if the factory was initialized with 
		/// <c>canReplaceVertexInstanceOnModification</c> parameter set to <c>true</c>.
		/// </remarks>
		public bool AddEdge(int destVertexIndex, TEdgeData edgeData, ref VertexAdjacency<TEdgeData> current)
		{
			var res = GetReplaceable(current);

			if (!res.AddEdge(_factory, destVertexIndex, edgeData, out res))
			{
				return false;
			}

			current = res;
			return true;
		}

		/// <inheritdoc />
		/// <remarks>
		/// The <paramref name="current"/> instance will be replaced only if the factory was initialized with 
		/// <c>canReplaceVertexInstanceOnModification</c> parameter set to <c>true</c>.
		/// </remarks>
		public bool RemoveEdge(int destVertexIndex, ref VertexAdjacency<TEdgeData> current)
		{
			if (current == null || current.EdgesCount <= 0)
			{
				return false;
			}

			var res = GetReplaceable(current);

			if (!res.RemoveEdge(_factory, destVertexIndex, out res))
			{
				return false;
			}

			current = res;
			return true;
		}

		/// <inheritdoc />
		/// <remarks>
		/// The <paramref name="current"/> instance will be replaced only if the factory was initialized with 
		/// <c>canReplaceVertexInstanceOnModification</c> parameter set to <c>true</c>.
		/// </remarks>
		public int RemoveAllEdges(ref VertexAdjacency<TEdgeData> current)
		{
			var count = current?.EdgesCount ?? 0;

			if (count <= 0)
			{
				return 0;
			}

			if (!_canReplaceVertexInstanceOnModification)
			{
				var replaceable = GetReplaceable(current);
			    replaceable.Reset();
				return count;
			}

			current = Empty;
			return count;
		}

		private sealed class InternalEmptyValueEdgeFactory : ReplaceableVertexFactoryInterface<EmptyValue>
		{
			private const int FixedCapacityEdgeCount = 7;

			private readonly bool _replacementEnabled;

			public InternalEmptyValueEdgeFactory(bool replacementEnabled)
			{
				_replacementEnabled = replacementEnabled;
			}

			/// <inheritdoc />
			public override ReplaceableVertexAdjacency<EmptyValue> Empty => _replacementEnabled
				? (ReplaceableVertexAdjacency<EmptyValue>)EmptyVertexAdjacency<EmptyValue>.Instance
				: new VertexAdjacencyUnlimitedEdges();

			private ReplaceableVertexAdjacency Create(int count)
			{
				if (!_replacementEnabled)
				{
					return new VertexAdjacencyUnlimitedEdges();
				}

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

			/// <inheritdoc />
			public override ReplaceableVertexAdjacency<EmptyValue> GetInstance(IEnumerable<AdjacentEdge<EmptyValue>> adjacentEdges, int count)
			{
				if (count == 0)
				{
					return Empty;
				}

				var res = Create(count);

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
				return _replacementEnabled && edgesCount <= FixedCapacityEdgeCount;
			}
		}

		private sealed class InternalFactory : ReplaceableVertexFactoryInterface<TEdgeData>
		{
			private const int FixedCapacityEdgeCount = 7;

			private readonly bool _replacementEnabled;

			public InternalFactory(bool replacementEnabled)
			{
				_replacementEnabled = replacementEnabled;
			}

			/// <inheritdoc />
			public override ReplaceableVertexAdjacency<TEdgeData> Empty => _replacementEnabled
				? (ReplaceableVertexAdjacency<TEdgeData>)EmptyVertexAdjacency<TEdgeData>.Instance
				: new VertexAdjacencyUnlimitedEdges<TEdgeData>();


			private ReplaceableVertexAdjacency<TEdgeData> Create(int count)
			{
				if (!_replacementEnabled)
				{
					return new VertexAdjacencyUnlimitedEdges<TEdgeData>(count);
				}

				switch (count)
				{				
					case 1 : return new VertexAdjacency1Edge<TEdgeData>();
					case 2 : return new VertexAdjacency2Edges<TEdgeData>();
					case 3 : return new VertexAdjacency3Edges<TEdgeData>();
					case 4 : return new VertexAdjacency4Edges<TEdgeData>();
					case 5 : return new VertexAdjacency5Edges<TEdgeData>();
					case 6 : return new VertexAdjacency6Edges<TEdgeData>();
					case 7 : return new VertexAdjacency7Edges<TEdgeData>();
					default: return new VertexAdjacencyUnlimitedEdges<TEdgeData>(count);
				}
			}

			/// <inheritdoc />
			public override ReplaceableVertexAdjacency<TEdgeData> GetInstance(IEnumerable<AdjacentEdge<TEdgeData>> adjacentEdges, int count)
			{
				if (count == 0)
				{
					return Empty;
				}

				var res = Create(count);

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
				return _replacementEnabled && edgesCount <= FixedCapacityEdgeCount;
			}
		}
	}
}