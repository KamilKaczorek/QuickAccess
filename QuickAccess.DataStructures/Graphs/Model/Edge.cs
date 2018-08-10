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

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     Data structure that contains edge representation.
	///     <seealso cref="VerticesPair{TVertexKey}" />
	/// </summary>
	/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	public struct Edge<TVertexKey, TEdgeData> : IEquatable<Edge<TVertexKey, TEdgeData>>
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="Edge{TVertexKey, TEdgeData}" /> structure with vertices pair and
		///     given edge data.
		/// </summary>
		/// <param name="vertices">The vertices.</param>
		/// <param name="edgeData">The edge data.</param>
		public Edge(VerticesPair<TVertexKey> vertices, TEdgeData edgeData)
		{
			Vertices = vertices;
			Data = edgeData;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Edge{TVertexKey, TEdgeData}" /> structure with specified values.
		/// </summary>
		/// <param name="source">The source vertex key.</param>
		/// <param name="destination">The destination vertex key.</param>
		/// <param name="edgeData">The edge data.</param>
		public Edge(TVertexKey source, TVertexKey destination, TEdgeData edgeData)
			: this(new VerticesPair<TVertexKey>(source, destination), edgeData)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Edge{TVertexKey, TEdgeData}" /> structure with source vertex key and
		///     <paramref name="edgeTo" /> value.
		/// </summary>
		/// <param name="source">The source vertex key.</param>
		/// <param name="edgeTo">
		///     The key value pair where the key specifies the destination vertex key and the value defines the
		///     edge data.
		/// </param>
		public Edge(TVertexKey source, KeyValuePair<TVertexKey, TEdgeData> edgeTo)
			: this(new VerticesPair<TVertexKey>(source, edgeTo.Key), edgeTo.Value)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Edge{TVertexKey, TEdgeData}" /> structure copying value from the
		///     other edge.
		/// </summary>
		/// <param name="edge">The edge.</param>
		public Edge(Edge<TVertexKey, TEdgeData> edge)
			: this(edge.Vertices, edge.Data)
		{
		}

		/// <summary>
		///     Gets the vertices of the edge.
		/// </summary>
		/// <value>
		///     The edge vertices.
		/// </value>
		public VerticesPair<TVertexKey> Vertices { get; }

		/// <summary>
		///     Gets the edge data.
		/// </summary>
		/// <value>
		///     The edge data.
		/// </value>
		public TEdgeData Data { get; }

		/// <summary>
		///     Gets the source vertex key.
		/// </summary>
		/// <value>
		///     The source vertex key.
		/// </value>
		public TVertexKey Source => Vertices.Source;

		/// <summary>
		///     Gets the destination vertex key.
		/// </summary>
		/// <value>
		///     The destination vertex key.
		/// </value>
		public TVertexKey Destination => Vertices.Destination;

		/// <summary>
		///     Returns the edge with reversed vertices pair (with swapped <see cref="Source" /> and <see cref="Destination" />).
		/// </summary>
		/// <returns>Reversed edge.</returns>
		[Pure]
		public Edge<TVertexKey, TEdgeData> Reverse()
		{
			return new Edge<TVertexKey, TEdgeData>(Vertices.Reverse(), Data);
		}

		/// <inheritdoc />
		public bool Equals(Edge<TVertexKey, TEdgeData> other)
		{
			return Equals(other, EqualityComparer<TEdgeData>.Default);
		}

		public bool Equals(Edge<TVertexKey, TEdgeData> other, IEqualityComparer<TEdgeData> edgeDataComparer)
		{
			return Vertices.Equals(other.Vertices) && edgeDataComparer.Equals(Data, other.Data);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is Edge<TVertexKey, TEdgeData> edge && Equals(edge);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return Vertices.GetHashCode();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Vertices} : {Data}";
		}
	}

	/// <summary>
	/// The static factory and factory extensions of instances of <see cref="Edge{TVertexKey,TEdgeData}"/> type.
	/// </summary>
	/// <seealso cref="Edge{TVertexKey, TEdgeData}" />
	public static class Edge
	{
		public static Edge<TVertexKey, TEdgeData> Create<TVertexKey, TEdgeData>(TVertexKey sourceVertex,
		                                                                          TVertexKey destinationVertex,
		                                                                          TEdgeData edgeData)
		{
			return new Edge<TVertexKey, TEdgeData>(sourceVertex, destinationVertex, edgeData);
		}

		public static Edge<int, TEdgeData> Create<TEdgeData>(int sourceVertexIndex, AdjacentEdge<TEdgeData> adjacentEdge)
		{
			return adjacentEdge.ToEdge(sourceVertexIndex);
		}


		public static IEnumerable<int> GetDestinations<TEdgeData>(this IEnumerable<AdjacentEdge<TEdgeData>> edgesTo)
		{
			return edgesTo.Select(e => e.Destination);
		}

		public static IEnumerable<TKey> GetDestinations<TKey, TEdgeData>(this IEnumerable<Edge<TKey, TEdgeData>> edges)
		{
			return edges.Select(e => e.Destination);
		}

		public static IEnumerable<Edge<TSymbol, TEdgeData>> ToSymbolEdges<TSymbol, TEdgeData>(
			this IEnumerable<AdjacentEdge<TEdgeData>> source,
			int sourceIndex,
			ISymbolToIndexReadOnlyConverter<TSymbol> map)
		{
			var src = map.GetSymbolAt(sourceIndex);
			return source.Select(e => new Edge<TSymbol, TEdgeData>(src, map.GetSymbolAt(e.Destination), e.Data));
		}

		public static Edge<TSymbol, TEdgeData> ToEdge<TSymbol, TEdgeData>(this VerticesPair<TSymbol> source, TEdgeData edgeData)
		{
			return new Edge<TSymbol, TEdgeData>(source, edgeData);
		}

		public static AdjacentEdge<TEdgeData> ToAdjacentEdge<TEdgeData>(this VerticesPair<int> source, TEdgeData edgeData)
		{
			return new AdjacentEdge<TEdgeData>(source.Destination, edgeData);
		}

		public static IEnumerable<Edge<TSymbol, TEdgeData>> ToEdges<TSymbol, TEdgeData>(this IEnumerable<VerticesPair<TSymbol>> source, Func<VerticesPair<TSymbol>, TEdgeData> getEdgeDataCallback)
		{
			return source.Select(v => v.ToEdge(getEdgeDataCallback.Invoke(v)));
		}

		public static IEnumerable<AdjacentEdge<TEdgeData>> ToAdjacentEdges<TEdgeData>(this IEnumerable<VerticesPair<int>> source, Func<VerticesPair<int>, TEdgeData> getEdgeDataCallback)
		{
			return source.Select(v => v.ToAdjacentEdge(getEdgeDataCallback.Invoke(v)));
		}

		public static Edge<TSymbol, TEdgeData> ToSymbolEdge<TSymbol, TEdgeData>(this Edge<int, TEdgeData> edge, ISymbolToIndexConverter<TSymbol> symbolToIndexConverter)
		{
			return new Edge<TSymbol, TEdgeData>(symbolToIndexConverter.GetSymbolAt(edge.Source), symbolToIndexConverter.GetSymbolAt(edge.Destination), edge.Data);
		}

		public static Edge<int, TEdgeData> ToIndexEdge<TSymbol, TEdgeData>(this Edge<TSymbol, TEdgeData> edge, ISymbolToIndexConverter<TSymbol> symbolToIndexConverter)
		{
			return new Edge<int, TEdgeData>(symbolToIndexConverter.GetIndexOf(edge.Source), symbolToIndexConverter.GetIndexOf(edge.Destination), edge.Data);
		}

		public static IEnumerable<Edge<TKey, TEdgeData>> GetReversed<TKey, TEdgeData>(this IEnumerable<Edge<TKey, TEdgeData>> edges)
		{
			return edges.Select(e => e.Reverse());
		}

		public static IEnumerable<TKey> GetSources<TKey, TEdgeData>(this IEnumerable<Edge<TKey, TEdgeData>> edges)
		{
			return edges.Select(e => e.Source);
		}

		public static IEnumerable<AdjacentEdge<TEdgeData>> ToAdjacentEdges<TEdgeData>(this IEnumerable<Edge<int, TEdgeData>> edges)
		{
			return edges.Select(AdjacentEdge.Create);
		}

		public static IEnumerable<Edge<int, TEdgeData>> ToEdges<TEdgeData>(this IEnumerable<AdjacentEdge<TEdgeData>> edgesTo, int sourceIndex)
		{
			return edgesTo.Select(e => e.ToEdge(sourceIndex));
		}

	}
}