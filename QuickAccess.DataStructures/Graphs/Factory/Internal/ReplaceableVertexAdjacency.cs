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
using QuickAccess.DataStructures.Graphs.Model;
using QuickAccess.Infrastructure;

namespace QuickAccess.DataStructures.Graphs.Factory.Internal
{
	/// <summary>
	///     The base abstract class of replaceable vertex.
	///     <remarks>
	///         The replaceable vertex instance is capable to store specified number of edges.
	///         It is possible that adding or removing an edge will result in creating of new vertex instance.
	///         E.g. <see cref="VertexAdjacency1Edge{TEdgeData}" /> can store exactly one edge, adding another edge to this
	///         vertex will
	///         result in creating <see cref="VertexAdjacencyUnlimitedEdges{TEdgeData}" /> and returning single vertex instance
	///         to the vertexFactory.
	///         This behavior allows to provide optimal vertex data structure for reading access.
	///     </remarks>
	///     <seealso cref="ReplaceableVertexFactoryInterface{TEdgeData}" />
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	internal abstract class ReplaceableVertexAdjacency<TEdgeData> : VertexAdjacency<TEdgeData>
	{
		/// <summary>The unlimited capacity.</summary>
		public const int UnlimitedCapacity = int.MaxValue;

		/// <summary>
		///     Gets the maximum capacity of current instance.
		///     Determines how many edges can be defined in current vertex instance.
		/// </summary>
		/// <value>
		///     The maximum capacity.
		/// </value>
		public abstract int MaxCapacity { get; }

		/// <summary>
		///     Adds the edge to the vertex given by the <paramref name="final" /> parameter.
		///     If the maximal capacity of the current vertex is not enough, it will replace the <paramref name="final" /> vertex
		///     by a new one with sufficient capacity and will copy all edges to the new vertex instance, returning the current
		///     instance
		///     to the specified <paramref name="vertexFactory" />.
		/// </summary>
		/// <param name="vertexFactory"></param>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <param name="edgeData">The edge data.</param>
		/// <param name="final">The reference to the target vertex, can be replaced in case of to small capacity.</param>
		/// <returns>
		///     <c>true</c> if new edge was added (the number of edges increased).
		/// </returns>
		public abstract bool AddEdge(ReplaceableVertexFactoryInterface<TEdgeData> vertexFactory,
		                             int destVertexIndex,
		                             TEdgeData edgeData,
		                             out ReplaceableVertexAdjacency<TEdgeData> final);

		/// <summary>
		///     Initializes the vertex with specified adjacent edges.
		/// </summary>
		/// <param name="edgesTo">The adjacent edges.</param>
		public abstract void Initialize(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo);

		/// <summary>
		///     Removes the edge from the vertex given by the <paramref name="final" /> parameter.
		///     If it is possible to replace the current vertex by more optimal then the <paramref name="final" /> will contain
		///     reference to the new vertex.
		/// </summary>
		/// <param name="vertexFactory"></param>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <param name="final">The reference to the target vertex, can be replaced in case of optimization.</param>
		/// <returns>
		///     <c>true</c> if the edge was found and removed from the vertex; otherwise <c>false</c>.
		/// </returns>
		public abstract bool RemoveEdge(ReplaceableVertexFactoryInterface<TEdgeData> vertexFactory,
		                                int destVertexIndex,
		                                out ReplaceableVertexAdjacency<TEdgeData> final);

		/// <summary>
		///     Resets this instance.
		///     It is called by the vertices vertexFactory when the instance is returned to the vertexFactory.
		/// </summary>
		public abstract void Reset();
	}

	/// <summary>
	///     The base abstract class of replaceable vertex with empty edges.
	///     <remarks>
	///         The replaceable vertex instance is capable to store specified number of edges.
	///         It is possible that adding or removing an edge will result in creating of new vertex instance.
	///         E.g. <see cref="VertexAdjacency1Edge{TEdgeData}" /> can store exactly one edge, adding another edge to this
	///         vertex will
	///         result in creating <see cref="VertexAdjacencyUnlimitedEdges{TEdgeData}" /> and returning single vertex instance
	///         to the vertexFactory.
	///         This behavior allows to provide optimal vertex data structure for reading access.
	///     </remarks>
	///     <seealso cref="ReplaceableVertexFactoryInterface{TEdgeData}" />
	///		<seealso cref="EmptyValue"/>
	/// </summary>
	internal abstract class ReplaceableVertexAdjacency : ReplaceableVertexAdjacency<EmptyValue>
	{
		/// <inheritdoc />
		public override IEnumerator<AdjacentEdge<EmptyValue>> GetEnumerator()
		{
			return AdjacentIndexes.Select(i => AdjacentEdge.Create(i)).GetEnumerator();
		}

		/// <inheritdoc />
		public override bool TryGetEdgeToIndex(int destVertexIndex, out EmptyValue edgeData)
		{
			edgeData = default;
			return ContainsEdgeToIndex(destVertexIndex);
		}

		/// <inheritdoc />
		public override EmptyValue GetEdgeToIndex(int destVertexIndex)
		{
			if (!ContainsEdgeToIndex(destVertexIndex))
			{
				throw new IndexOutOfRangeException();
			}

			return default;
		}

		/// <inheritdoc />
		public override void Initialize(IEnumerable<AdjacentEdge<EmptyValue>> edgesTo)
		{
			Initialize(edgesTo.GetDestinations());
		}

		/// <summary>
		///     Initializes the vertex with specified adjacent vertex indexes.
		/// </summary>
		/// <param name="destIndexes">The adjacent vertex indexes.</param>
		public abstract void Initialize(IEnumerable<int> destIndexes);
	}
}