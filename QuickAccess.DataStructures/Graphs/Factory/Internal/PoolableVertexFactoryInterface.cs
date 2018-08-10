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

using System.Collections.Generic;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory.Internal
{
	/// <summary>
	/// The interface in form of an abstract class (performance) of a factory of poolable vertex instances.
	/// The interface is defined to pass the vertex factory to <see cref="PoolableVertexAdjacency{TEdgeData}.AddEdge"/>/
	/// <see cref="PoolableVertexAdjacency{TEdgeData}.RemoveEdge"/> operation
	/// For more details refer to <see cref="PoolableVertexAdjacency{TEdgeData}"/>.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	internal abstract class PoolableVertexFactoryInterface<TEdgeData>
	{
		/// <summary>
		/// Gets the empty vertex instance.
		/// </summary>
		/// <value>
		/// The empty vertex.
		/// </value>
		public abstract PoolableVertexAdjacency<TEdgeData> Empty { get; }

		/// <summary>
		/// Gets the vertex instance with specified edges.
		/// </summary>
		/// <param name="adjacentEdges">The adjacent edges.</param>
		/// <param name="count">The number of edges.</param>
		/// <returns>The vertex instance with edges specified by parameters.</returns>
		public abstract PoolableVertexAdjacency<TEdgeData> GetInstance(IEnumerable<AdjacentEdge<TEdgeData>> adjacentEdges, int count);

		/// <summary>
		/// Determines whether it provides specific vertex adjacency type with fixed edges capacity for the specified count of edges. 
		/// </summary>
		/// <param name="edgesCount">The edges count.</param>
		/// <returns><c>true</c> if provides specific fixed capacity type; otherwise, <c>false</c>.</returns>
		public abstract bool ProvidesFixedCapacity(int edgesCount);

		/// <summary>
		/// Returns the specified instance to the pool.
		/// </summary>
		/// <param name="instance">The instance to return.</param>
		/// <returns><c>true</c> if the pool has space to take more instances; otherwise, <c>false</c> - the pool is full.</returns>
		public abstract bool ReturnInstance(PoolableVertexAdjacency<TEdgeData> instance);
	}
}