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

using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory
{
	/// <summary>
	///     The interface of a modifier of vertex adjacency.
	///     It provides the contract to add or remove edge from a vertex.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	public interface IVertexAdjacencyModifier<TEdgeData> : IVertexAdjacencyFactory<TEdgeData>
	{
		/// <summary>
		///     Adds the edge to the vertex given by the <paramref name="current" /> parameter.
		///     if it is not possible to extend the current vertex instance, it will replace the <paramref name="current" /> vertex
		///     by a new one with sufficient capacity and will copy all edges to the newly created instance.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <param name="edgeData">The edge data.</param>
		/// <param name="current">
		///     The reference to the target vertex or <c>null</c> (current vertex is empty);
		///     The reference might be replaced if it is not possible to extend the current instance.
		/// </param>
		/// <returns><c>true</c> if the new edge was added (the number of edges increased).</returns>
		bool AddEdge(int destVertexIndex, TEdgeData edgeData, ref VertexAdjacency<TEdgeData> current);

		/// <summary>
		///     Removes the edge from the vertex given by the <paramref name="current" /> parameter.
		///     If it is possible to replace the current vertex by more optimal then the <paramref name="current" /> will contain
		///     reference to the new vertex.
		/// </summary>
		/// <param name="destVertexIndex">Index of the destination vertex.</param>
		/// <param name="current">
		///     The reference to the target vertex, can be replaced in case of optimization (only if edges number
		///     changed).
		/// </param>
		/// <returns><c>true</c> if the edge was found and removed from the vertex; otherwise <c>false</c>.</returns>
		bool RemoveEdge(int destVertexIndex, ref VertexAdjacency<TEdgeData> current);

		/// <summary>
		///     Removes all edges from the vertex given by the <paramref name="current" /> parameter.
		///     If it is possible to replace the current vertex by more optimal, then the <paramref name="current" /> will contain
		///     reference to the new empty vertex.
		/// </summary>
		/// <param name="current">
		///     The reference to the target vertex, can be replaced in case of optimization (only if edges number
		///     changed).
		/// </param>
		/// <returns>Number of removed edges; if <c>0</c> - vertex was empty.</returns>
		int RemoveAllEdges(ref VertexAdjacency<TEdgeData> current);
	}
}