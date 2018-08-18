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
using QuickAccess.DataStructures.Graphs.Algorithms;
using QuickAccess.DataStructures.Graphs.Factory;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Extensions
{
	/// <summary>
	///     The extension methods of <see cref="IReadOnlyGraph{TEdgeData}" /> and
	///     <see cref="IReadOnlyGraph{TEdgeData, TSymbol}" />
	/// </summary>
	public static class GraphExtensions
	{
		private static readonly BreadthFirstSearch BFS = new BreadthFirstSearch();

		internal static IGraphConnectivityDefinitionFactory ConnectivityDefinitionFactory = new GraphConnectivityDefinitionFactory();

		/// <summary>Gets the search map from the specified start vertex.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The graph connectivity.</param>
		/// <param name="startVertexIndex">The index of the start vertex.</param>
		/// <param name="filterAdjacentVerticesCallback">The filter adjacent vertices callback.</param>
		/// <returns>The graph search map from the specified start vertex.</returns>
		[Pure]
		public static VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> graph,
			int startVertexIndex,
			FilterAdjacentVerticesCallback<TEdgeData> filterAdjacentVerticesCallback)
		{
			return BFS.GetSearchMapFrom(graph, startVertexIndex, filterAdjacentVerticesCallback);
		}

		/// <summary>Converts the index path to edge data.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The source graph.</param>
		/// <param name="indexPath">The index path (sequence of vertices represented by indexes).</param>
		/// <returns>The sequence of data items of edges.</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when vertex of specified index doesn't exist.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when edge doesn't exist.</exception>
		[Pure]
		public static IEnumerable<TEdgeData> ConvertPathToEdgeData<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> graph,
			IEnumerable<int> indexPath)

		{
			var prev = -1;
			var count = 0;
			foreach (var current in indexPath)
			{
				if (count > 0)
				{
					yield return graph[prev].GetEdgeToIndex(current);
				}

				count++;
				prev = current;
			}

			if (count == 1)
			{
				yield return graph[prev].GetEdgeToIndex(prev);
			}
		}

		/// <summary>Converts the reversed index path to edge data (in reversed order).</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The source graph.</param>
		/// <param name="reversedIndexPath">The reversed index path (reversed sequence of vertices represented by indexes).</param>
		/// <returns>The sequence of data items of edges (the sequence is corresponding to reversed path, so it's also reversed).</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when vertex of specified index doesn't exist.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when edge doesn't exist.</exception>
		[Pure]
		public static IEnumerable<TEdgeData> ConvertReversedPathToEdgeData<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> graph,
			IEnumerable<int> reversedIndexPath)

		{
			var prev = -1;
			var count = 0;
			foreach (var current in reversedIndexPath)
			{
				if (count > 0)
				{
					yield return graph[current].GetEdgeToIndex(prev);
				}

				count++;
				prev = current;
			}

			if (count == 1)
			{
				yield return graph[prev].GetEdgeToIndex(prev);
			}
		}

		/// <summary>Converts the symbol path to edge data.</summary>
		/// <typeparam name="TSymbol">The type of the vertex symbol.</typeparam>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The source graph.</param>
		/// <param name="symbolPath">The symbol path (sequence of vertices represented by symbols).</param>
		/// <returns>The sequence of data items of edges.</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when vertex of specified index doesn't exist.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when edge doesn't exist.</exception>
		[Pure]
		public static IEnumerable<TEdgeData> ConvertPathToEdgeData<TSymbol, TEdgeData>(
			this IReadOnlyGraph<TEdgeData, TSymbol> graph,
			IEnumerable<TSymbol> symbolPath)
		{
			return ConvertPathToEdgeData(graph.Connectivity,
				symbolPath.Select(v => graph.SymbolToIndexConverter.GetIndexOf(v, true)));
		}

		/// <summary>Converts the reversed symbol path to edge data (in reversed order).</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the vertex symbol.</typeparam>
		/// <param name="graph">The source graph.</param>
		/// <param name="reversedSymbolPath">The reversed symbol path (reversed sequence of vertices represented by symbols).</param>
		/// <returns>The sequence of data items of edges (the sequence is corresponding to reversed path, so it's also reversed).</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when vertex of specified index doesn't exist.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when edge doesn't exist.</exception>
		[Pure]
		public static IEnumerable<TEdgeData> ConvertReversedPathToEdgeData<TSymbol, TEdgeData>(
			this IReadOnlyGraph<TEdgeData, TSymbol> graph,
			IEnumerable<TSymbol> reversedSymbolPath)

		{
			return ConvertReversedPathToEdgeData(graph.Connectivity,
				reversedSymbolPath.Select(v => graph.SymbolToIndexConverter.GetIndexOf(v, true)));
		}

		/// <summary>Converts the index path to edge data.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The source graph.</param>
		/// <param name="indexPath">The index path (sequence of vertices represented by indexes).</param>
		/// <returns>The sequence of data items of edges.</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when vertex of specified index doesn't exist.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when edge doesn't exist.</exception>
		[Pure]
		public static IEnumerable<TEdgeData> ConvertIndexPathToEdgeData<TEdgeData>(
			this IReadOnlyGraph<TEdgeData> graph,
			IEnumerable<int> indexPath)
		{
			return ConvertPathToEdgeData(graph.Connectivity, indexPath);
		}

		/// <summary>Converts the reversed index path to edge data (in reversed order).</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The source graph.</param>
		/// <param name="reversedIndexPath">The reversed index path (reversed sequence of vertices represented by indexes).</param>
		/// <returns>The sequence of data items of edges (the sequence is corresponding to reversed path, so it's also reversed).</returns>
		/// <exception cref="IndexOutOfRangeException">Thrown when vertex of specified index doesn't exist.</exception>
		/// <exception cref="KeyNotFoundException">Thrown when edge doesn't exist.</exception>
		[Pure]
		public static IEnumerable<TEdgeData> ConvertReversedPathToEdgeData<TEdgeData>(
			this IReadOnlyGraph<TEdgeData> graph,
			IEnumerable<int> reversedIndexPath)

		{
			return graph.Connectivity.ConvertReversedPathToEdgeData(reversedIndexPath);
		}

		/// <summary>Adds the vertex self loop to the graph source.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="vertex">The vertex to be self looped.</param>
		/// <param name="edgeData">The data of a buckle edge.</param>
		/// <returns>
		///   <c>true</c> if new edge was added (number of edges was increased); otherwise <c>false</c>, the data of existing edge was replaced.
		/// </returns>
		public static bool AddSelfLoop<TEdgeData, TSymbol>(
			this IGraphSource<TEdgeData, TSymbol> source,
			TSymbol vertex,
			TEdgeData edgeData)
		{
			return source.AddEdge(vertex, vertex, edgeData);
		}

		/// <summary>Adds the vertex self loop to the graph source.</summary>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="vertex">The vertex to be self looped.</param>
		/// <returns>
		///   <c>true</c> if new edge was added (number of edges was increased); otherwise <c>false</c>, the data of existing edge was replaced.
		/// </returns>
		public static bool AddSelfLoop<TSymbol>(
			this IGraphSource<EmptyValue, TSymbol> source,
			TSymbol vertex)
		{
			return source.AddEdge(vertex, vertex, default);
		}

		/// <summary>Adds the symbol edge to the graph source.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="srcDestVertices">The pair of vertices (source and destination vertex).</param>
		/// <param name="edgeData">The edge data.</param>
		/// <returns><c>true</c> if new edge was added (number of edges was increased); otherwise <c>false</c>, the data of existing edge was replaced.</returns>
		public static bool AddEdge<TEdgeData, TSymbol>(
			this IGraphSource<TEdgeData, TSymbol> source,
			VerticesPair<TSymbol> srcDestVertices,
			TEdgeData edgeData)
		{
			return source.AddEdge(srcDestVertices.Source, srcDestVertices.Destination, edgeData);
		}

		/// <summary>Adds the symbol edge to the graph source.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="edge">The edge.</param>
		/// <returns><c>true</c> if new edge was added (number of edges was increased); otherwise <c>false</c>, the data of existing edge was replaced.</returns>
		public static bool AddEdge<TEdgeData, TSymbol>(
			this IGraphSource<TEdgeData, TSymbol> source,
			Edge<TSymbol, TEdgeData> edge)
		{
			return source.AddEdge(edge.Source, edge.Destination, edge.Data);
		}

		/// <summary>Adds the range of edges to the graph source.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="edgeSequence">The edge sequence.</param>
		/// <param name="addEdgePredicate">The add edge predicate.</param>
		/// <returns>Number of added new edges (updated edges are not counted).</returns>
		public static int AddEdgeRange<TEdgeData, TSymbol>(
			this IGraphSource<TEdgeData, TSymbol> source,
			IEnumerable<Edge<TSymbol, TEdgeData>> edgeSequence,
			Func<Edge<TSymbol, TEdgeData>, bool> addEdgePredicate = null)
		{
			return addEdgePredicate == null
				? edgeSequence.Count(source.AddEdge)
				: edgeSequence.Count(e => addEdgePredicate.Invoke(e) && source.AddEdge(e));
		}

		/// <summary>Adds the range of edges to the graph source.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="verticesPairsSequence">The vertices pairs sequence.</param>
		/// <param name="getEdgeDataCallback">The get edge data callback.</param>
		/// <param name="addEdgePredicate">The add edge predicate.</param>
		/// <returns>Number of added new edges (updated edges are not counted).</returns>
		public static int AddEdgeRange<TEdgeData, TSymbol>(
			this IGraphSource<TEdgeData, TSymbol> source,
			IEnumerable<VerticesPair<TSymbol>> verticesPairsSequence,
			Func<VerticesPair<TSymbol>, TEdgeData> getEdgeDataCallback,
			Func<Edge<TSymbol, TEdgeData>, bool> addEdgePredicate = null)
		{
			return source.AddEdgeRange(verticesPairsSequence.ToEdges(getEdgeDataCallback), addEdgePredicate);
		}

		/// <summary>Adds the range of edges to the graph source.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="srcIndex">Index of the source vertex.</param>
		/// <param name="adjacentEdges">The adjacent edges (destination vertices with data)</param>
		/// <param name="addEdgePredicate">The add edge predicate.</param>
		/// <returns>Number of added new edges (updated edges are not counted).</returns>
		public static int AddEdgeRange<TEdgeData>(
			this IGraphSource<TEdgeData, int> source,
			int srcIndex,
			IEnumerable<AdjacentEdge<TEdgeData>> adjacentEdges,
			Func<Edge<int, TEdgeData>, bool> addEdgePredicate = null)
		{
			return source.AddEdgeRange(adjacentEdges.ToEdges(srcIndex), addEdgePredicate);
		}

		/// <summary>Adds the outgoing edge to the graph.</summary>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="edge">The edge.</param>
		/// <returns>
		/// <c>true</c> if the new edge was added (number of edges increased);
		/// otherwise <c>false</c> (edge between the specified indexes existed, only edge data was updated).
		/// </returns>
		public static bool AddEdge<TSymbol>(
			this IGraphSource<EmptyValue, TSymbol> source,
			VerticesPair<TSymbol> edge)
		{
			return source.AddEdge(edge.Source, edge.Destination, default);
		}

		/// <summary>Adds the outgoing edge to the symbol graph with empty edge value.</summary>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The graph editing source.</param>
		/// <param name="srcVertex">The source vertex.</param>
		/// <param name="destVertex">The destination vertex.</param>
		/// <returns>
		/// <c>true</c> if the new edge was added (number of edges increased);
		/// otherwise <c>false</c> (edge between the specified indexes existed, only edge data was updated).
		/// </returns>
		public static bool AddEdge<TSymbol>(
			this IGraphSource<EmptyValue, TSymbol> source,
			TSymbol srcVertex,
			TSymbol destVertex)
		{
			return source.AddEdge(srcVertex, destVertex, default);
		}

		[Pure]
		public static VertexSearchMap<T> GetSearchMapFrom<TEdgeData, T>(
			this IReadOnlyGraph<TEdgeData, T> graph,
			T fromVertex)
		{
			var start = graph.SymbolToIndexConverter.GetIndexOf(fromVertex);

			if (start < 0)
			{
				return VertexSearchMap.CreateEmpty(fromVertex);
			}

			var res = GetSearchMapFrom(graph.Connectivity, start);

			return res.ConvertToSymbolSearchMap(graph.SymbolToIndexConverter);
		}

		[Pure]
		public static VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			this IReadOnlyGraph<TEdgeData> graph,
			int start)
		{
			if (start < 0)
			{
				return VertexSearchMap.CreateEmpty(start);
			}

			return GetSearchMapFrom(graph.Connectivity, start);
		}

		/// <summary>Gets the search map from the specified start vertex.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="graph">The graph connectivity.</param>
		/// <param name="startVertexIndex">The index of the start vertex.</param>
		/// <returns>The graph search map from the specified start vertex.</returns>
		[Pure]
		public static VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> graph,
			int startVertexIndex)
		{
			return BFS.GetSearchMapFrom(graph, startVertexIndex);
		}

		[Pure]
		public static VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> graph,
			int start,
			SelectAdjacentVertexPredicate<int, TEdgeData> selectAdjPredicate)
		{
			return BFS.GetSearchMapFrom(graph, start,
				(src, v) => v.Where(e => selectAdjPredicate.Invoke(e.ToEdge(src))).Select(e => e.Destination));
		}

		[Pure]
		public static VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> graph,
			int start,
			SelectAdjacentVertexPredicate<int> selectAdjPredicate)
		{
			return BFS.GetSearchMapFrom(graph, start,
				(src, v) => v.AdjacentIndexes.Where(dst => selectAdjPredicate.Invoke(src, dst)));
		}

		[Pure]
		public static VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			this IReadOnlyGraph<TEdgeData> graph,
			int start,
			SelectAdjacentVertexPredicate<int, TEdgeData> selectAdjPredicate)
		{
			return graph.Connectivity.GetSearchMapFrom(start, selectAdjPredicate);
		}

		[Pure]
		public static VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			this IReadOnlyGraph<TEdgeData> graph,
			int start,
			SelectAdjacentVertexPredicate<int> selectAdjPredicate)
		{
			return graph.Connectivity.GetSearchMapFrom(start, selectAdjPredicate);
		}

		[Pure]
		public static VertexSearchMap<TSymbol> GetSearchMapFrom<TEdgeData, TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> graph,
			TSymbol start,
			SelectAdjacentVertexPredicate<TSymbol, TEdgeData> selectAdjPredicate)
		{
			var map = graph.SymbolToIndexConverter;

			return BFS.GetSearchMapFrom(graph.Connectivity, map.GetIndexOf(start),
				          (srcIdx, v) =>
				          {
					          var src = map.GetSymbolAt(srcIdx);
					          return v.Where(adj => selectAdjPredicate(Edge.Create(src, map.GetSymbolAt(adj.Destination), adj.Data)))
					                  .Select(e => e.Destination);
				          })
			          .ConvertToSymbolSearchMap(map);
		}

		[Pure]
		public static VertexSearchMap<TSymbol> GetSearchMapFrom<TEdgeData, TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> graph,
			TSymbol start,
			SelectAdjacentVertexPredicate<TSymbol> selectAdjPredicate)
		{
			var map = graph.SymbolToIndexConverter;
			return BFS.GetSearchMapFrom(graph.Connectivity, map.GetIndexOf(start),
				          (srcIdx, v) =>
				          {
					          var src = map.GetSymbolAt(srcIdx);
					          return v.AdjacentIndexes.Where(dstIndex => selectAdjPredicate(src, map.GetSymbolAt(dstIndex)));
				          })
			          .ConvertToSymbolSearchMap(map);
		}

		/// <summary>
		///     Gets the sequence of indexes of all vertices contained by the current graph.
		/// </summary>
		/// <param name="source">The extension source (<see cref="GraphConnectivityDefinition{TEdgeData}" />).</param>
		/// <returns>
		///     The vertex indexes.
		/// </returns>
		[Pure]
		public static IEnumerable<int> GetVerticesIndexes<TEdgeData>(this IReadOnlyGraph<TEdgeData> source)
		{
			return source.Connectivity.GetVertexIndexes();
		}

		/// <summary>
		///     Gets the edges from the vertex at the specified position.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="srcVertex">The source vertex.</param>
		/// <returns>Edges from the specified vertex as enumerable sequence.</returns>
		[Pure]
		public static IEnumerable<Edge<int, TEdgeData>> GetEdgesFromVertexAt<TEdgeData>(
			this IReadOnlyGraph<TEdgeData> source,
			int srcVertex)
		{
			return source.Connectivity.GetEdgesFromVertexAt(srcVertex);
		}

		/// <summary>
		///     Gets the edges to the vertex at the specified position.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="dstVertex">The destination vertex.</param>
		/// <returns>Edges to the specified vertex as enumerable sequence.</returns>
		[Pure]
		public static IEnumerable<Edge<int, TEdgeData>> GetEdgesToVertexAt<TEdgeData>(
			this IReadOnlyGraph<TEdgeData> source,
			int dstVertex)
		{
			return source.Connectivity.GetEdgesToVertexAt(dstVertex);
		}

		/// <summary>
		///     Determines whether the source graph contains vertex specified by the given index.
		/// </summary>
		/// <param name="source">The extension source (<see cref="GraphConnectivityDefinition{TEdgeData}" />).</param>
		/// <param name="vertexIndex">Index of the vertex.</param>
		/// <returns>
		///     <c>true</c> if the graph contains specified vertex; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public static bool ContainsVertexAt<TEdgeData>(this IReadOnlyGraph<TEdgeData> source, int vertexIndex)
		{
			return source.Connectivity.ContainsVertexAt(vertexIndex);
		}

		/// <summary>
		///     Gets the edges from the source vertex specified by the given symbol.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="sourceVertex">The source vertex.</param>
		/// <returns>The edges as a enumerable sequence.</returns>
		[Pure]
		public static IEnumerable<Edge<TSymbol, TEdgeData>> GetEdgesFromVertex<TEdgeData, TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> source,
			TSymbol sourceVertex)
		{
			var map = source.SymbolToIndexConverter;
			return source.Connectivity[map.GetIndexOf(sourceVertex)]
			             .Select(adj => new Edge<TSymbol, TEdgeData>(sourceVertex, map.GetSymbolAt(adj.Destination), adj.Data));
		}

		/// <summary>
		///     Gets the edges to the destination vertex specified by the given symbol.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="destVertex">The destination vertex.</param>
		/// <returns>The edges as a enumerable sequence.</returns>
		[Pure]
		public static IEnumerable<Edge<TSymbol, TEdgeData>> GetEdgesToVertex<TEdgeData, TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> source,
			TSymbol destVertex)
		{
			var map = source.SymbolToIndexConverter;
			return source.Connectivity.GetEdgesToVertexAt(map.GetIndexOf(destVertex))
			             .Select(adj => new Edge<TSymbol, TEdgeData>(map.GetSymbolAt(adj.Source), destVertex, adj.Data));
		}

		/// <summary>
		///     Gets all symbol edges from the specified symbol graph.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <returns>The edges as a enumerable sequence.</returns>
		[Pure]
		public static IEnumerable<Edge<TSymbol, TEdgeData>> GetEdges<TEdgeData, TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> source)
		{
			var map = source.SymbolToIndexConverter;
			var con = source.Connectivity;

			return con.GetVertexIndexes()
			          .SelectMany(srcIdx =>
			          {
				          var src = map.GetSymbolAt(srcIdx);
				          return con.GetEdgesFromVertexAt(srcIdx)
				                    .Select(e => new Edge<TSymbol, TEdgeData>(src, map.GetSymbolAt(e.Destination), e.Data));
			          });
		}

		/// <summary>
		///     Determines whether the vertex is defined for the specified key.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The extension source.</param>
		/// <param name="vertexKey">The vertex key.</param>
		/// <returns>
		///     <c>true</c> if the vertex represented by the specified key is defined; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public static bool ContainsVertex<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source, TSymbol vertexKey)
		{
			var idx = source.SymbolToIndexConverter.GetIndexOf(vertexKey);
			return idx >= 0 && source.Connectivity.ContainsVertexAt(idx);
		}

		/// <summary>
		///     Tries to get the value of the outgoing edge between the vertices represented by the specified symbols.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The extension source (<see cref="IReadOnlyGraph{TEdgeData, TSymbol}" />).</param>
		/// <param name="srcVertex">The symbol of the source vertex.</param>
		/// <param name="dstVertex">The symbol of the destination vertex.</param>
		/// <param name="edgeData">The edge data.</param>
		/// <returns>
		///     <c>true</c> if vertices of the given symbols are defined and there is outgoing edge between them; otherwise,
		///     <c>false</c>.
		/// </returns>
		[Pure]
		public static bool TryGetEdgeData<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source,
		                                                      TSymbol srcVertex,
		                                                      TSymbol dstVertex,
		                                                      out TEdgeData edgeData)
		{
			if (!source.GetIndexesOf(srcVertex, dstVertex, out var src, out var dst))
			{
				edgeData = default;
				return false;
			}

			return source.Connectivity.TryGetEdgeData(src, dst, out edgeData);
		}

		/// <summary>Gets the indexes of vertices specified by the given symbol.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The extension source.</param>
		/// <param name="vertex1">The vertex1.</param>
		/// <param name="vertex2">The vertex2.</param>
		/// <param name="vertex1Index">Index of the vertex1.</param>
		/// <param name="vertex2Index">Index of the vertex2.</param>
		/// <param name="validate">if set to <c>true</c> it will throw <see cref="KeyNotFoundException" /> if one of specified vertex is not defined.</param>
		/// <returns>
		/// <c>true</c> if vertex indexes are defined for the specified vertex symbols; otherwise <c>false</c> (or throws exception if <paramref name="validate"/> is <c>true</c>).
		/// </returns>
		[Pure]
		public static bool GetIndexesOf<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source,
																   TSymbol vertex1,
																   TSymbol vertex2,
																   out int vertex1Index,
																   out int vertex2Index,
																   bool validate = false)
		{
			if ((vertex1Index = source.SymbolToIndexConverter.GetIndexOf(vertex1, validate)) < 0)
			{
				vertex2Index = -1;
				return false;
			}

			if ((vertex2Index = source.SymbolToIndexConverter.GetIndexOf(vertex2, validate)) < 0)
			{
				return false;
			}

			return true;
		}

		/// <summary>Gets the indexes of vertices specified by the given symbol.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The extension source.</param>
		/// <param name="vertex1">The vertex1.</param>
		/// <param name="vertex2">The vertex2.</param>
		/// <param name="vertex3">The vertex3.</param>
		/// <param name="vertex1Index">Index of the vertex1.</param>
		/// <param name="vertex2Index">Index of the vertex2.</param>
		/// <param name="vertex3Index">Index of the vertex3.</param>
		/// <param name="validate">if set to <c>true</c> it will throw <see cref="KeyNotFoundException" /> if one of specified vertex is not defined.</param>
		/// <returns>
		///   <c>true</c> if vertex indexes are defined for the specified vertex symbols; otherwise <c>false</c> (or throws exception if <paramref name="validate" /> is <c>true</c>).
		/// </returns>
		[Pure]
		public static bool GetIndexesOf<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source,
														   TSymbol vertex1,
														   TSymbol vertex2,
														   TSymbol vertex3,
														   out int vertex1Index,
														   out int vertex2Index,
														   out int vertex3Index,
														   bool validate = false)
		{
			if ((vertex1Index = source.SymbolToIndexConverter.GetIndexOf(vertex1, validate)) < 0)
			{
				vertex2Index = -1;
				vertex3Index = -1;
				return false;
			}

			if ((vertex2Index = source.SymbolToIndexConverter.GetIndexOf(vertex2, validate)) < 0)
			{
				vertex3Index = -1;
				return false;
			}

			if ((vertex3Index = source.SymbolToIndexConverter.GetIndexOf(vertex3, validate)) < 0)
			{
				return false;
			}

			return true;
		}

		/// <summary>Gets the indexes of vertices specified by the given symbol.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The extension source.</param>
		/// <param name="vertex1">The vertex1.</param>
		/// <param name="vertex2">The vertex2.</param>
		/// <param name="vertex3">The vertex3.</param>
		/// <param name="vertex4">The vertex4.</param>
		/// <param name="vertex1Index">Index of the vertex1.</param>
		/// <param name="vertex2Index">Index of the vertex2.</param>
		/// <param name="vertex3Index">Index of the vertex3.</param>
		/// <param name="vertex4Index">Index of the vertex4.</param>
		/// <param name="validate">if set to <c>true</c> it will throw <see cref="KeyNotFoundException" /> if one of specified vertex is not defined.</param>
		/// <returns>
		///   <c>true</c> if vertex indexes are defined for the specified vertex symbols; otherwise <c>false</c> (or throws exception if <paramref name="validate" /> is <c>true</c>).
		/// </returns>
		[Pure]
		public static bool GetIndexesOf<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source,
														   TSymbol vertex1,
														   TSymbol vertex2,
														   TSymbol vertex3,
														   TSymbol vertex4,
														   out int vertex1Index,
														   out int vertex2Index,
														   out int vertex3Index,
														   out int vertex4Index,
														   bool validate)
		{
			if ((vertex1Index = source.SymbolToIndexConverter.GetIndexOf(vertex1, validate)) < 0)
			{
				vertex2Index = -1;
				vertex3Index = -1;
				vertex4Index = -1;
				return false;
			}

			if ((vertex2Index = source.SymbolToIndexConverter.GetIndexOf(vertex2, validate)) < 0)
			{
				vertex3Index = -1;
				vertex4Index = -1;
				return false;
			}

			if ((vertex3Index = source.SymbolToIndexConverter.GetIndexOf(vertex3, validate)) < 0)
			{
				vertex4Index = -1;
				return false;
			}

			if ((vertex4Index = source.SymbolToIndexConverter.GetIndexOf(vertex4, validate)) < 0)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		///     Gets the value of the outgoing edge between the vertices represented by the specified symbols.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The extension source (<see cref="IReadOnlyGraph{TEdgeData, TSymbol}" />).</param>
		/// <param name="srcVertex">The symbol of the source vertex.</param>
		/// <param name="dstVertex">The symbol of the destination vertex.</param>
		/// <returns>
		///     Edge data.
		/// </returns>
		[Pure]
		public static TEdgeData GetEdgeData<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source,
		                                                        TSymbol srcVertex,
		                                                        TSymbol dstVertex
		)
		{
			source.GetIndexesOf(srcVertex, dstVertex, out var src, out var dst, true);
			return source.Connectivity.GetEdgeData(src, dst);
		}

		/// <summary>Gets the index of the last not empty vertex (vertex with edges).</summary>
		/// <seealso cref="GetIndexOfLastNotEmptyVertex{TEdgeData}(IReadOnlyList{VertexAdjacency{TEdgeData}})"/>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="vertices">The vertices.</param>
		/// <returns>
		///     The index of the last not empty vertex if exists;
		///     otherwise <c>-1</c>, specified sequence doesn't contain vertex with edges.
		/// </returns>
		[Pure]
	    public static int GetIndexOfLastNotEmptyVertex<TEdgeData>(this VertexAdjacency<TEdgeData>[] vertices)
		{
			for (var idx = vertices.Length - 1; idx >= 0; --idx)
			{
				if (vertices[idx].Count > 0)
				{
					return idx;
				}
			}

			return -1;
		}


		/// <summary>Gets the index of the last not empty vertex.</summary>
		/// <remarks>
		/// This overload is provided for performance reason.
		/// <seealso cref="GetIndexOfLastNotEmptyVertex{TEdgeData}(IReadOnlyList{VertexAdjacency{TEdgeData}})"/>
		/// </remarks>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="vertices">The vertices.</param>
		/// <returns>The index of the last not empty vertex or <c>-1</c> if there are no vertices with edges in specified sequence.</returns>
		[Pure]
		public static int GetIndexOfLastNotEmptyVertex<TEdgeData>(this GraphConnectivityDefinition<TEdgeData> vertices)
		{
			for (var idx = vertices.Count - 1; idx >= 0; --idx)
			{
				if (vertices[idx].Count > 0)
				{
					return idx;
				}
			}

			return -1;
		}

		/// <summary>Gets the index of the last not empty vertex.</summary>
		/// <seealso cref="GetIndexOfLastNotEmptyVertex{TEdgeData}(VertexAdjacency{TEdgeData}[])"/>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="vertices">The vertices.</param>
		/// <returns>The index of the last not empty vertex or <c>-1</c> if there are no vertices with edges in specified sequence.</returns>
		[Pure]
		public static int GetIndexOfLastNotEmptyVertex<TEdgeData>(this IReadOnlyList<VertexAdjacency<TEdgeData>> vertices)
		{
			for (var idx = vertices.Count - 1; idx >= 0; --idx)
			{
				if (vertices[idx].Count > 0)
				{
					return idx;
				}
			}

			return -1;
		}

		/// <summary>
		///		Gets the compacted read-only graph with reassigned indexes.
		///     The operation will sort vertices by number of edges, 
		///     putting vertices with lower number of adjacent edges at the end.
		///     It doesn't allocate memory for empty vertices but provides virtual (dummy) access to them.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="verticesFactory">The vertices factory.</param>
		/// <returns>The reindexing result with compacted graph.</returns>
		[Pure]
		public static ReindexedDataResult<ReadOnlySymbolGraph<TEdgeData, TSymbol>> GetCompacted<TEdgeData, TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> source,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory = null)
		{
			var res = source.Connectivity.ToCompacted(verticesFactory);
			return res.IndexTranslator.ToReindexedDataResult(new ReadOnlySymbolGraph<TEdgeData, TSymbol>(res.Data,
				new SymbolToIndexReadOnlyConverter<TSymbol>(source.SymbolToIndexConverter, res.IndexTranslator)));
		}

		[Pure]
		public static ReindexedDataResult<ReadOnlySymbolGraph<TEdgeData, TSymbol>> GetCompactedWithSharedVertexInstances<TEdgeData,
			TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> source,
			IEqualityComparer<TEdgeData> edgeDataEqualityComparer,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory = null)
		{
			var res = source.Connectivity.ToCompactedWithSharedVertexInstances(edgeDataEqualityComparer, verticesFactory);
			return res.IndexTranslator.ToReindexedDataResult(new ReadOnlySymbolGraph<TEdgeData, TSymbol>(res.Data,
				new SymbolToIndexReadOnlyConverter<TSymbol>(source.SymbolToIndexConverter, res.IndexTranslator)));
		}

		[Pure]
		public static ReindexedDataResult<ReadOnlySymbolGraph<TEdgeData, TSymbol>> GetCompactedWithSharedVertexInstances<TEdgeData,
			TSymbol>(
			this IReadOnlyGraph<TEdgeData, TSymbol> source,
			IEqualityComparer<VertexAdjacency<TEdgeData>> vertexEqualityComparer = null,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory = null)
		{
			var res = source.Connectivity.ToCompactedWithSharedVertexInstances(vertexEqualityComparer, verticesFactory);
			return res.IndexTranslator.ToReindexedDataResult(new ReadOnlySymbolGraph<TEdgeData, TSymbol>(res.Data,
				new SymbolToIndexReadOnlyConverter<TSymbol>(source.SymbolToIndexConverter, res.IndexTranslator)));
		}

		/// <summary>
		///     Determines whether there is outgoing edge from the source vertex specified by the symbol to the destination vertex
		///     specified by the symbol.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
		/// <param name="source">The extension source (<see cref="IReadOnlyGraph{TEdgeData, TSymbol}" />).</param>
		/// <param name="srcVertex">The symbol of the source vertex.</param>
		/// <param name="dstVertex">The symbol of the destination vertex.</param>
		/// <returns>
		///     <c>true</c> if vertices of the given symbols are defined and there is outgoing edge between them; otherwise,
		///     <c>false</c>.
		/// </returns>
		[Pure]
		public static bool ContainsEdge<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source,
		                                                    TSymbol srcVertex,
		                                                    TSymbol dstVertex)
		{
			var map = source.SymbolToIndexConverter;
			var src = map.GetIndexOf(srcVertex);
			if (src < 0)
			{
				return false;
			}

			var dst = map.GetIndexOf(dstVertex);
			if (dst < 0)
			{
				return false;
			}

			return source.Connectivity.ContainsEdge(src, dst);
		}

		/// <summary>
		///     Gets the sequence of symbols of all vertices contained by the current graph.
		/// </summary>
		/// <param name="source">The extension source (<see cref="GraphConnectivityDefinition{TEdgeData}" />).</param>
		/// <returns>
		///     The vertex indexes.
		/// </returns>
		[Pure]
		public static IEnumerable<TSymbol> GetVerticesSymbols<TEdgeData, TSymbol>(this IReadOnlyGraph<TEdgeData, TSymbol> source)
		{
			var map = source.SymbolToIndexConverter;
			var conn = source.Connectivity;

			return conn.GetVertexIndexes().Select(idx => map.GetSymbolAt(idx));
		}

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source list.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceList">The source list to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TEdgeData> WrapAsGraphConnectivity<TEdgeData>(
			this List<VertexAdjacency<TEdgeData>> sourceList)
		{
			return ConnectivityDefinitionFactory.CreateWrapperOf(sourceList);
		}

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source list.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceList">The source list to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TEdgeData> WrapAsGraphConnectivity<TEdgeData>(
			this IReadOnlyList<VertexAdjacency<TEdgeData>> sourceList)
		{
			return ConnectivityDefinitionFactory.CreateWrapperOf(sourceList);
		}

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source list.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceList">The source list to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TEdgeData> WrapAsGraphConnectivity<TEdgeData>(
			this DefaultTailList<VertexAdjacency<TEdgeData>> sourceList)
		{
			return ConnectivityDefinitionFactory.CreateWrapperOf(sourceList);
		}

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     source array.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceArray">The source array to be wrapped.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TEdgeData> WrapAsGraphConnectivity<TEdgeData>(
			this VertexAdjacency<TEdgeData>[] sourceArray)
		{
			return ConnectivityDefinitionFactory.CreateWrapperOf(sourceArray);
		}

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified source
		///     array filling undefined space with empty vertices.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceArray">The source array to be wrapped.</param>
		/// <param name="totalCount">The total count of vertices.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TEdgeData> WrapAsEmptyTailGraphConnectivity<TEdgeData>(
			this VertexAdjacency<TEdgeData>[] sourceArray,
			int totalCount)
		{
			return ConnectivityDefinitionFactory.CreateEmptyTailWrapperOf(sourceArray, totalCount);
		}

		/// <summary>
		///     Creates a new instance of the <see cref="GraphConnectivityDefinition{TEdgeData}" /> that wraps the specified
		///     dictionary filling undefined space with empty vertices.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="indexToVertexMap">The index to vertex map.</param>
		/// <param name="totalCount">The total count of vertices.</param>
		/// <returns>New instance of <see cref="GraphConnectivityDefinition{TEdgeData}" />.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TEdgeData> WrapAsGraphConnectivity<TEdgeData>(
			this Dictionary<int, VertexAdjacency<TEdgeData>> indexToVertexMap,
			int totalCount)
		{
			return ConnectivityDefinitionFactory.CreateWrapperOf(indexToVertexMap, totalCount);
		}

		/// <summary>
		///     Gets the compacted read-only graph connectivity with reassigned indexes.
		///     The operation will sort vertices by number of edges, putting vertices with lower number of adjacent edges at the
		///     end.
		///     It doesn't allocate memory for empty vertices but provides virtual (dummy) access to them.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="verticesFactory">The vertices factory (if <c>null</c> default is used).</param>
		/// <returns>The reindexing operation result with new, optimized instance of graph connectivity and reassigned index map.</returns>
		[Pure]
		public static ReindexedDataResult<GraphConnectivityDefinition<TEdgeData>> ToCompacted<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> source,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory = null)
		{
			return ConnectivityDefinitionFactory.CreateCompacted(source, verticesFactory, false);
		}

		/// <summary>
		///     Gets the compacted read-only graph connectivity with reassigned indexes.
		///     The operation will sort vertices by number of edges, 
		///     putting vertices with lower number of adjacent edges at the end.
		///     It doesn't allocate memory for empty vertices but provides virtual (dummy) access to them.
		///		<remarks>
		///		It reuses vertex instance with same adjacency. 
		///     It uses given <see cref="edgeDataEqualityComparer"/> to determine if the vertices adjacency is equal.
		///     </remarks>
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="edgeDataEqualityComparer">The edge data equality comparer.</param>
		/// <param name="verticesFactory">The vertices factory (if <c>null</c> default is used).</param>
		/// <returns>
		///     The reindexing operation result with new, optimized instance of graph connectivity and reassigned index map.
		/// </returns>
		[Pure]
		public static ReindexedDataResult<GraphConnectivityDefinition<TEdgeData>> ToCompactedWithSharedVertexInstances<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> source,
			IEqualityComparer<TEdgeData> edgeDataEqualityComparer,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory = null)
		{
			return ConnectivityDefinitionFactory.CreateCompacted(source, verticesFactory, true,
				new VertexAdjacencyComparer<TEdgeData>(edgeDataEqualityComparer, VertexComparisonType.HighestNumberOfEdgesFirst));
		}

		/// <summary>
		///     Gets the compacted read-only graph connectivity with reassigned indexes.
		///     The operation will sort vertices by number of edges, 
		///     putting vertices with lower number of adjacent edges at the end.
		///     It doesn't allocate memory for empty vertices but provides virtual (dummy) access to them.
		///		<remarks>
		///		It reuses vertex instance with same adjacency. 
		///     It uses given <see cref="vertexEqualityComparer"/> to determine if the vertices adjacency is equal.
		///     </remarks>
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="vertexEqualityComparer">The vertex adjacency equality comparer.</param>
		/// <param name="verticesFactory">The vertices factory (if <c>null</c> default is used).</param>
		/// <returns>
		///     The reindexing operation result with new, optimized instance of graph connectivity and reassigned index map.
		/// </returns>
		[Pure]
		public static ReindexedDataResult<GraphConnectivityDefinition<TEdgeData>> ToCompactedWithSharedVertexInstances<TEdgeData>(
			this GraphConnectivityDefinition<TEdgeData> source,
			IEqualityComparer<VertexAdjacency<TEdgeData>> vertexEqualityComparer = null,
			IVertexAdjacencyFactory<TEdgeData> verticesFactory = null)
		{
			return ConnectivityDefinitionFactory.CreateCompacted(source, verticesFactory, true, vertexEqualityComparer);
		}

		/// <summary>Converts the source list of vertex adjacency items to the read-only graph connectivity instance.</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="targetEdgeDataSelector">The target edge data selector.</param>
		/// <param name="verticesFactory">The vertices factory.</param>
		/// <returns>Read-only graph connectivity.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TEdgeData> ToGraphConnectivity<TEdgeData>(
					this IReadOnlyList<VertexAdjacency<TEdgeData>> source,
					Func<TEdgeData, TEdgeData> targetEdgeDataSelector =
						null,
					IVertexAdjacencyFactory<TEdgeData> verticesFactory =
						null)
		{
			return ConnectivityDefinitionFactory.Create(source, verticesFactory, targetEdgeDataSelector, EmptyValue.IsEmptyValueType<TEdgeData>());
		}

		/// <summary>Converts the source graph connectivity to the new graph connectivity with different type of the edge data.</summary>
		/// <typeparam name="TSrcEdgeData">The type of the source edge data.</typeparam>
		/// <typeparam name="TTargetEdgeData">The type of the target edge data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="targetEdgeDataSelector">The target edge data selector based on source edge data.</param>
		/// <param name="verticesFactory">The vertex factory.</param>
		/// <returns>New instance of read-only graph connectivity.</returns>
		[Pure]
		public static GraphConnectivityDefinition<TTargetEdgeData> ConvertEdgeDataType<TSrcEdgeData, TTargetEdgeData>(
					this GraphConnectivityDefinition<TSrcEdgeData> source,
					Func<TSrcEdgeData, TTargetEdgeData> targetEdgeDataSelector = null,
					IVertexAdjacencyFactory<TTargetEdgeData> verticesFactory =
						null)
		{
			return ConnectivityDefinitionFactory.Create(source, verticesFactory, targetEdgeDataSelector, EmptyValue.IsEmptyValueType<TTargetEdgeData>());
		}
	}
}