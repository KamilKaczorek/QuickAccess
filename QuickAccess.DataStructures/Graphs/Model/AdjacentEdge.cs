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
using QuickAccess.DataStructures.Common;

namespace QuickAccess.DataStructures.Graphs.Model
{
	public static class AdjacentEdge
	{
		/// <summary>
		/// Creates a new instance of the <see cref="AdjacentEdge{TEdgeData}" /> structure.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <param name="edgeData">The edge data.</param>
		public static AdjacentEdge<TEdgeData> Create<TEdgeData>(int destVertexIndex, TEdgeData edgeData)
		{
			return new AdjacentEdge<TEdgeData>(destVertexIndex, edgeData);
		}

		/// <summary>
		/// Creates a new instance of the <see cref="AdjacentEdge{TEdgeData}" /> structure 
		/// with specified destination vertex index and <see cref="EmptyValue"/> edge data.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		public static AdjacentEdge<EmptyValue> Create(int destVertexIndex)
		{
			return new AdjacentEdge<EmptyValue>(destVertexIndex, default);
		}

		/// <summary>
		/// Creates a new instance of the <see cref="AdjacentEdge{TEdgeData}" /> structure copying data from specified edge.
		/// </summary>
		/// <param name="edge">The edge.</param>
		public static AdjacentEdge<TEdgeData> Create<TEdgeData>(Edge<int, TEdgeData> edge)
		{
			return new AdjacentEdge<TEdgeData>(edge.Destination, edge.Data);
		}

		/// <summary>
		/// Creates a new instance of the <see cref="AdjacentEdge{TEdgeData}" /> structure.
		/// </summary>
		/// <param name="destinationIndexEdgeDataPair">The destination index edge data pair.</param>
		public static AdjacentEdge<TEdgeData> Create<TEdgeData>(KeyValuePair<int, TEdgeData> destinationIndexEdgeDataPair)
		{
			return new AdjacentEdge<TEdgeData>(destinationIndexEdgeDataPair.Key, destinationIndexEdgeDataPair.Value);
		}
	}

	/// <summary>
	/// Defines the adjacent (index based) edge of the source vertex containing index of the destination vertex and the edge data.
	/// <remarks>
	/// It contains only partial edge information, Use <see cref="Edge{TVertexKey,TEdgeData}"/> to have full edge definition 
	/// containing source vertex index. 
	/// </remarks>
	/// <seealso cref="Edge"/>
	/// <seealso cref="VertexAdjacency{TEdgeData}"/>
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="IEquatable{T}" />
	public readonly struct AdjacentEdge<TEdgeData> : IEquatable<AdjacentEdge<TEdgeData>>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AdjacentEdge{TEdgeData}" /> structure.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <param name="edgeData">The edge data.</param>
		public AdjacentEdge(int destVertexIndex, TEdgeData edgeData)
		{
			Destination = destVertexIndex;
			Data = edgeData;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AdjacentEdge{TEdgeData}" /> structure copying data from the other adjacent edge.
		/// </summary>
		/// <param name="other">The other adjacent edge.</param>
		public AdjacentEdge(AdjacentEdge<TEdgeData> other)
		{
			Destination = other.Destination;
			Data = other.Data;
		}
		/// <summary>
		/// Converts the current adjacent edge to full edge definition (with source vertex index).
		/// </summary>
		/// <param name="sourceVertexIndex">Index of the source vertex.</param>
		/// <returns>Full edge definition.</returns>
		public Edge<int, TEdgeData> ToEdge(int sourceVertexIndex)
		{
			return new Edge<int, TEdgeData>(sourceVertexIndex, Destination, Data);
		}

		/// <summary>
		/// Gets the edge data.
		/// </summary>
		/// <value>
		/// The edge data.
		/// </value>
		public TEdgeData Data { get; }

		/// <summary>
		/// Gets the index of the destination vertex.
		/// </summary>
		/// <value>
		/// The destination vertex index.
		/// </value>
		public int Destination { get; }

        public void Deconstruct(out int destination, out TEdgeData edgeData)
        {
            destination = Destination;
            edgeData = Data;
        }

		/// <summary>
		/// Evaluates if the other adjacent edge is equal to the current one, using default comparer for edge data comparison.
		/// To specify edge data comparer use <see cref="Equals(AdjacentEdge{TEdgeData}, IEqualityComparer{TEdgeData})"/> instead.
		/// </summary>
		/// <param name="other">The other adjacent edge.</param>
		/// <returns><c>true</c> if other edge is equal to the current one; otherwise, <c>false</c>.</returns>
		public bool Equals(AdjacentEdge<TEdgeData> other)
		{
			return Equals(other, EqualityComparer<TEdgeData>.Default);
		}

		/// <summary>
		/// Evaluates if the other adjacent edge is equal to the current one, using specified <paramref name="comparer"/> for
		/// edge data comparison.
		/// <seealso cref="Equals(AdjacentEdge{TEdgeData})"/>
		/// </summary>
		/// <param name="other">The other adjacent edge.</param>
		/// <param name="comparer">The edge data comparer.</param>
		/// <returns><c>true</c> if other edge is equal to the current one; otherwise, <c>false</c>.</returns>
		public bool Equals(AdjacentEdge<TEdgeData> other, IEqualityComparer<TEdgeData> comparer)
		{
			return Destination == other.Destination && comparer.Equals(Data, other.Data);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is AdjacentEdge<TEdgeData> edge && Equals(edge);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{			
			return Destination.GetHashCode();			
		}
	}
}