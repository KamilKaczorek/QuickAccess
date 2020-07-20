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

using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Common.Freezable;
using QuickAccess.DataStructures.Common.ValueContract;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	/// Defines the graph modification operations.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
	public interface IGraphSource<in TEdgeData, in TVertexKey> : IFreezable
	{
		/// <summary>
		/// Removes the directed edge between the vertices specified by the given indexes.
		/// </summary>
		/// <param name="srcVertexKey">The source vertex key.</param>
		/// <param name="destVertexKey">The destination vertex key.</param>
		/// <returns><c>true</c> if the edge was found and removed; otherwise <c>false</c>.</returns>
		bool RemoveEdge(TVertexKey srcVertexKey, TVertexKey destVertexKey);

		/// <summary>
		/// Adds the edge coming to the graph.
		/// </summary>
		/// <param name="srcVertexKey">The source vertex key.</param>
		/// <param name="destVertexKey">The destination vertex key.</param>
		/// <param name="edgeData">The edge data.</param>
		/// <returns>
		/// <c>true</c> if the new edge was added (number of edges increased); 
		/// otherwise <c>false</c> (edge between the specified indexes existed, only edge data was updated).
		/// </returns>
		bool AddEdge(TVertexKey srcVertexKey, TVertexKey destVertexKey, TEdgeData edgeData);

		/// <summary>
		/// Removes all edges coming out of the source vertex specified by the key.
		/// </summary>
		/// <param name="srcVertexKey">The source vertex key.</param>
		/// <returns>The number of removed edges.</returns>
		int RemoveAllEdgesFrom(TVertexKey srcVertexKey);

		/// <summary>Adds the vertex represented by the specified key.</summary>
		/// <param name="vertexKey">The vertex key.</param>
		/// <returns><c>true</c> if the new vertex was added; otherwise <c>false</c> - vertex with specified key already exists.</returns>
		bool AddVertex(TVertexKey vertexKey);

		/// <summary>
		/// Removes all edges incoming to the destination vertex specified by the key.
		/// </summary>
		/// <param name="destVertexKey">The destination vertex key.</param>
		/// <returns>The number of removed edges.</returns>
		int RemoveAllEdgesTo(TVertexKey destVertexKey);

		/// <summary>Clears the graph - removes all edges.</summary>
		/// <param name="removeAdjacencyOnly">if set to <c>true</c> removes only adjacency data of the graph (all edges), it leaves empty vertices and symbol-index map.</param>
		void Clear(bool removeAdjacencyOnly = false);
	}
}