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
using System.Linq;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory
{
	/// <summary>
	///     Wrapper of the <see cref="IVertexAdjacencyFactory{TEdgeData}" /> that allows to create new instance of edge data of cloned vertex.
	/// It is useful in conjunction with graph clone and compact operations.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="Factory.IVertexAdjacencyFactory{TEdgeData}" />
	public sealed class EdgeDataCloningVertexAdjacencyFactoryWrapper<TEdgeData> : IVertexAdjacencyFactory<TEdgeData>
	{
		private readonly Func<AdjacentEdge<TEdgeData>, TEdgeData> _copyEdgeDataCallback;
		private readonly IVertexAdjacencyFactory<TEdgeData> _wrapped;

		/// <summary>
		///     Initializes a new instance of the <see cref="EdgeDataCloningVertexAdjacencyFactoryWrapper{TEdgeData}" /> class.
		/// </summary>
		/// <param name="wrapped">The wrapped.</param>
		/// <param name="copyEdgeDataCallback">The copy edge data callback.</param>
		public EdgeDataCloningVertexAdjacencyFactoryWrapper(IVertexAdjacencyFactory<TEdgeData> wrapped,
		                                                 Func<TEdgeData, TEdgeData> copyEdgeDataCallback)
		{
			_wrapped = wrapped;
			_copyEdgeDataCallback = adj => copyEdgeDataCallback(adj.Data);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="EdgeDataCloningVertexAdjacencyFactoryWrapper{TEdgeData}" /> class.
		/// </summary>
		/// <param name="wrapped">The wrapped.</param>
		/// <param name="copyEdgeDataCallback">The copy edge data callback.</param>
		public EdgeDataCloningVertexAdjacencyFactoryWrapper(IVertexAdjacencyFactory<TEdgeData> wrapped,
		                                                 Func<AdjacentEdge<TEdgeData>, TEdgeData> copyEdgeDataCallback)
		{
			_wrapped = wrapped;
			_copyEdgeDataCallback = copyEdgeDataCallback;
		}

		/// <inheritdoc />
		public VertexAdjacency<TEdgeData> Empty => _wrapped.Empty;

		/// <inheritdoc />
		public VertexAdjacency<TEdgeData> GetInstance(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo, int edgesCount)
		{
			return _wrapped.GetInstance(edgesTo.Select(ReplaceData), edgesCount);
		}

		private AdjacentEdge<TEdgeData> ReplaceData(AdjacentEdge<TEdgeData> source)
		{
			return AdjacentEdge.Create(source.Destination, _copyEdgeDataCallback(source));
		}
	}
}