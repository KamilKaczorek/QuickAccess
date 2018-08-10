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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     Interface of a graph connectivity definition defined as abstract class for better performance.
	///     It is provided by the <see cref="IReadOnlyGraph{TEdgeData}" />.
	///     It defines the contract to access vertices adjacency definition for graph search algorithms.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	public abstract class GraphConnectivityDefinition<TEdgeData> : IReadOnlyList<VertexAdjacency<TEdgeData>>
	{
		/// <inheritdoc />
		public abstract IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator();

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public abstract int Count { get; }

		/// <inheritdoc />
		public abstract VertexAdjacency<TEdgeData> this[int index] { get; }

		/// <summary>
		///     Gets the sequence of indexes of all vertices contained by the current graph.
		/// </summary>
		/// <returns>
		///     The vertex indexes.
		/// </returns>
		[Pure]
		public IEnumerable<int> GetVerticesIndexes()
		{
			return Enumerable.Range(0, Count);
		}

		/// <summary>
		///     Gets all edges of the graph.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <returns>Edges as enumerable sequence.</returns>
		[Pure]
		public IEnumerable<Edge<int, TEdgeData>> GetEdges()
		{
			return this.SelectMany((vertex, srcIndex) => vertex.Select(edgeTo => edgeTo.ToEdge(srcIndex)));
		}

		/// <summary>
		///     Gets the edges from the vertex at the specified position.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="srcVertex">The source vertex.</param>
		/// <returns>Edges from the specified vertex as enumerable sequence.</returns>
		[Pure]
		public IEnumerable<Edge<int, TEdgeData>> GetEdgesFromVertexAt(int srcVertex)
		{
			return this[srcVertex].Select(edgeTo => edgeTo.ToEdge(srcVertex));
		}

		/// <summary>
		///     Gets the edges to the vertex at the specified position.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="dstVertex">The destination vertex.</param>
		/// <returns>Edges to the specified vertex as enumerable sequence.</returns>
		[Pure]
		public IEnumerable<Edge<int, TEdgeData>> GetEdgesToVertexAt(int dstVertex)
		{
			for (var srcVertex = 0; srcVertex < Count; srcVertex++)
			{
				var adj = this[srcVertex];

				if (adj.TryGetEdgeToIndex(dstVertex, out var value))
				{
					yield return new Edge<int, TEdgeData>(srcVertex, dstVertex, value);
				}
			}
		}

		/// <summary>
		///     Determines whether the source graph contains vertex specified by the given index.
		/// </summary>
		/// <param name="vertexIndex">Index of the vertex.</param>
		/// <returns>
		///     <c>true</c> if the graph contains specified vertex; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool ContainsVertexAt(int vertexIndex)
		{
			return vertexIndex >= 0 && vertexIndex < Count;
		}

		/// <summary>
		///     Determines whether there is outgoing edge from the source vertex specified by the index to the destination vertex
		///     specified by the index.
		/// </summary>
		/// <param name="sourceVertexIndex">Index of the source vertex.</param>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <returns>
		///     <c>true</c> if there is edge from source to destination vertex; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool ContainsEdge(
			int sourceVertexIndex,
			int destVertexIndex)
		{
			if (!TryGetVertexAt(sourceVertexIndex, out var vertex))
			{
				return false;
			}

			return vertex.ContainsEdgeToIndex(destVertexIndex);
		}

		/// <summary>
		///     Determines whether the source graph contains specified edge.
		/// </summary>
		/// <param name="edge">The edge.</param>
		/// <returns>
		///     <c>true</c> if the graph contains specified edge; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool ContainsEdge(
			VerticesPair<int> edge)
		{
			return ContainsEdge(edge.Source, edge.Destination);
		}

		/// <summary>
		///     Determines whether the source graph contains specified edge.
		/// </summary>
		/// <param name="edge">The edge.</param>
		/// <returns>
		///     <c>true</c> if the graph contains specified edge; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool ContainsBiDirectionalEdge(
			VerticesPair<int> edge)
		{
			return ContainsBiDirectionalEdge(edge.Source, edge.Destination);
		}

		/// <summary>
		///     Determines whether the source graph contains both directional direct edge between specified vertices.
		/// </summary>
		/// <param name="vertex1Index">Index of first vertex.</param>
		/// <param name="vertex2Index">Index of second vertex.</param>
		/// <returns>
		///     <c>true</c> if there is both directional direct edge between specified vertices; otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool ContainsBiDirectionalEdge(
			int vertex1Index,
			int vertex2Index)
		{
			return ContainsEdge(vertex1Index, vertex2Index) &&
			       (vertex1Index == vertex2Index || ContainsEdge(vertex2Index, vertex1Index));
		}

		/// <summary>
		///     Tries to get the vertex at the specified index.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="index">The vertex index.</param>
		/// <param name="vertex">The vertex at specified index.</param>
		/// <returns><c>true</c> if there is vertex defined for the specified index; otherwise <c>false</c>.</returns>
		[Pure]
		public bool TryGetVertexAt(
			int index,
			out VertexAdjacency<TEdgeData> vertex)
		{
			if (ContainsVertexAt(index))
			{
				vertex = this[index];
				return true;
			}

			vertex = default;
			return false;
		}

		/// <summary>
		///     Tries to get the value of the outgoing edge between the vertices represented by the specified indexes.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="sourceIndex">The index of the source vertex.</param>
		/// <param name="destIndex">The index of the destination vertex.</param>
		/// <param name="edgeData">The edge data.</param>
		/// <returns>
		///     <c>true</c> if there are vertices defined for the specified indexes and there is outgoing edge between them;
		///     otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool TryGetEdgeData(
			int sourceIndex,
			int destIndex,
			out TEdgeData edgeData)
		{
			if (TryGetVertexAt(sourceIndex, out var vertex) &&
			    vertex.TryGetEdgeToIndex(destIndex, out edgeData))
			{
				return true;
			}

			edgeData = default;
			return false;
		}

		/// <summary>
		///     Tries to get the value of the outgoing edge between the vertices represented by the specified indexes.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="verticesPair">The vertices pair.</param>
		/// <param name="edgeData">The edge data.</param>
		/// <returns>
		///     <c>true</c> if there are vertices defined for the specified indexes and there is outgoing edge between them;
		///     otherwise, <c>false</c>.
		/// </returns>
		[Pure]
		public bool TryGetEdgeData(
			VerticesPair<int> verticesPair,
			out TEdgeData edgeData)
		{
			return TryGetEdgeData(verticesPair.Source, verticesPair.Destination, out edgeData);
		}

		/// <summary>
		///     Gets the data of the edge defined by the specified vertices pair.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="verticesPair">The vertices pair.</param>
		/// <returns>The edge data.</returns>
		/// <exception cref="KeyNotFoundException">Thrown when specified edge doesn't exist.</exception>
		public TEdgeData GetEdgeData(
			VerticesPair<int> verticesPair)
		{
			return this[verticesPair.Source].GetEdgeToIndex(verticesPair.Destination);
		}

		/// <summary>
		///     Gets the data of the edge defined by the specified vertices pair.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="srcIndex"></param>
		/// <param name="dstIndex"></param>
		/// <returns>The edge data.</returns>
		/// <exception cref="KeyNotFoundException">Thrown when specified edge doesn't exist.</exception>
		public TEdgeData GetEdgeData(
			int srcIndex,
			int dstIndex)
		{
			return this[srcIndex].GetEdgeToIndex(dstIndex);
		}
	}
}