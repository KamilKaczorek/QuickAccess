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

using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Algorithms
{
	/// <summary>Predicate to determine if the specified edge should be selected.</summary>
	/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <param name="edge">The edge.</param>
	/// <returns><c>true</c> if the edge is selected; otherwise, <c>false</c>.</returns>
	public delegate bool SelectAdjacentVertexPredicate<TVertexKey, TEdgeData>(Edge<TVertexKey, TEdgeData> edge);

	/// <summary>
	///     Predicate to determine if the edge specified by vertices should be selected.
	/// </summary>
	/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
	/// <param name="sourceVertex">The source vertex.</param>
	/// <param name="destinationVertex">The destination vertex.</param>
	/// <returns><c>true</c> if the edge is selected; otherwise, <c>false</c>.</returns>
	public delegate bool SelectAdjacentVertexPredicate<in TVertexKey>(TVertexKey sourceVertex, TVertexKey destinationVertex);
}