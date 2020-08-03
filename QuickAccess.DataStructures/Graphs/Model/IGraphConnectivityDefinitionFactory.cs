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
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.DataStructures.Graphs.Factory;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     The interface of the factory of instances of <see cref="GraphConnectivityDefinition{TEdgeData}" /> type.
	/// </summary>
	public interface IGraphConnectivityDefinitionFactory
	{
		/// <summary>Gets the empty graph connectivity definition (with no vertices).</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <returns>Instance of empty graph connectivity definition.</returns>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> GetEmpty<TEdgeData>();

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source list.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceList">The source list to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(List<VertexAdjacency<TEdgeData>> sourceList);

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source list.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceList">The source list to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(IReadOnlyList<VertexAdjacency<TEdgeData>> sourceList);

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source list.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceList">The source list to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(DefaultTailList<VertexAdjacency<TEdgeData>> sourceList);

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source array.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceArray">The source array to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(VertexAdjacency<TEdgeData>[] sourceArray);

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified source
		///     array filling undefined space with empty vertices.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceArray">The source array to be wrapped.</param>
		/// <param name="totalCount">The total count of vertices.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> CreateEmptyTailWrapperOf<TEdgeData>(
			VertexAdjacency<TEdgeData>[] sourceArray,
			int totalCount);

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     dictionary filling undefined space with empty vertices.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="indexToVertexMap">The index to vertex map.</param>
		/// <param name="totalCount">The total count of vertices.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> CreateWrapperOf<TEdgeData>(
			Dictionary<int, VertexAdjacency<TEdgeData>> indexToVertexMap,
			int totalCount);

		/// <summary>
		///     Creates new instance of graph connectivity instance.
		///     It allows to convert edge data type.
		/// </summary>
		/// <typeparam name="TSrcEdgeData">The type of the source edge data.</typeparam>
		/// <typeparam name="TTargetEdgeData">The type of the edge data of the created instance.</typeparam>
		/// <param name="sourceAdjacency">The source adjacency.</param>
		/// <param name="verticesFactory">The vertices factory (if <c>null</c> default is used).</param>
		/// <param name="targetEdgeDataSelector">
		///     The target edge data selector (source instance edge data to new instance
		///     converter), if <c>null</c> it will try to evaluate default one.
		/// </param>
		/// <param name="sharedVerticesInstances">if set to <c>true</c> the new instance will reuse vertex instances if possible.</param>
		/// <param name="vertexEqualityComparer">
		///     The vertex equality comparer to select vertices for which the only one target
		///     instance will be created.
		/// </param>
		/// <returns>Graph connectivity instance.</returns>
		[Pure]
		GraphConnectivityDefinition<TTargetEdgeData> Create<TSrcEdgeData, TTargetEdgeData>(
			IReadOnlyList<VertexAdjacency<TSrcEdgeData>> sourceAdjacency,
			IVertexAdjacencyFactory<TTargetEdgeData> verticesFactory = null,
			Func<TSrcEdgeData, TTargetEdgeData> targetEdgeDataSelector = null,
			bool sharedVerticesInstances = false,
			IEqualityComparer<VertexAdjacency<TSrcEdgeData>> vertexEqualityComparer = null);

		/// <summary>
		/// Gets the compacted read-only graph connectivity with reassigned indexes.
		/// The operation will sort vertices by number of edges, putting vertices with lower number of adjacent edges at the
		/// end of the vertex list.
		/// It doesn't allocate memory for empty vertices but provides virtual access to them.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="verticesFactory">The vertices factory (if <c>null</c> default is used).</param>		
		/// <param name="sharedVerticesInstances">if set to <c>true</c> the new instance will reuse vertex instances if possible.</param>
		/// <param name="vertexEqualityComparer">The vertex equality comparer.</param>
		/// <returns>
		/// The reindexing operation result with new, optimized instance of graph connectivity and reassigned index map.
		/// </returns>
		[Pure]
		ReindexedDataResult<GraphConnectivityDefinition<TEdgeData>> CreateCompacted<TEdgeData>(
			GraphConnectivityDefinition<TEdgeData> source,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory,
			bool sharedVerticesInstances,
			IEqualityComparer<VertexAdjacency<TEdgeData>> vertexEqualityComparer = null);

		
	}
}