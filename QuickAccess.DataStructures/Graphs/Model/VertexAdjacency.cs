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

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     The interface of the vertex of the graph in a form of abstract class, 
	///     where the vertices are represented as integer indexes
	///     and the edges contain data.
	///     The index of the first vertex is equal to zero, indexes of subsequent vertices are successive natural numbers.
	///     The interface is implemented as an abstract class to increase performance.
	/// <remarks>
	/// Please note that the VertexAdjacency instance can be replaced during the graph modification.
	/// </remarks>
	/// </summary>
	public abstract class VertexAdjacency<TEdgeData> : IReadOnlyCollection<AdjacentEdge<TEdgeData>>
	{
		/// <summary>Gets the number of outgoing edges from the current vertex.</summary>
		/// <value>The number of outgoing edges.</value>
		public abstract int EdgesCount { get; }

		/// <summary>Gets the indexes of the adjacent vertices.</summary>
		/// <value>The indexes of adjacent vertices.</value>
		public abstract IEnumerable<int> AdjacentIndexes { get; }

		/// <summary>Gets the number of edges, is equivalent to the <see cref="EdgesCount"/> property.</summary>
		public int Count => EdgesCount;

		/// <summary>
		/// Determines whether the current vertex contains outgoing edge to the vertex specified by the given index.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <returns>
		/// <c>true</c> if it contains outgoing edge to the vertex specified by the index; otherwise, <c>false</c>.
		/// </returns>
		public abstract bool ContainsEdgeToIndex(int destVertexIndex);

		/// <summary>
		/// Gets the data of the edge outgoing from the current vertex to the vertex specified by the given index.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <returns>The value</returns>
		public abstract TEdgeData GetEdgeToIndex(int destVertexIndex);

		/// <summary>Returns an enumerator that iterates through the adjacent edges of the current vertex.</summary>
		/// <returns>An enumerator that can be used to iterate through the adjacent edges.</returns>
		public abstract IEnumerator<AdjacentEdge<TEdgeData>> GetEnumerator();

		/// <summary>
		/// Tries to get the outgoing edge from the current vertex to the vertex specified by the given index.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <param name="edgeData">The edge data.</param>
		/// <returns><c>true</c> if the edge between current and destination vertex exists; otherwise, <c>false</c>.</returns>
		public abstract bool TryGetEdgeToIndex(int destVertexIndex, out TEdgeData edgeData);
	
		
		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}	
}