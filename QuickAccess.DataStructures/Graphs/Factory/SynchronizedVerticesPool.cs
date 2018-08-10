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
using QuickAccess.DataStructures.Graphs.Factory.Internal;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory
{
	/// <summary>
	///     Wrapper that provides synchronization of vertices pool.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="IVerticesPool{TEdgeData}" />
	public sealed class SynchronizedVerticesPool<TEdgeData> : IVerticesPool<TEdgeData>
	{
		private readonly object _thisLock;
		private readonly IVerticesPool<TEdgeData> _wrapped;

		/// <summary>Initializes a new instance of the <see cref="SynchronizedVerticesPool{TEdgeData}"/> class.</summary>
		/// <param name="wrapped">The wrapped non synchronized pool.</param>
		/// <param name="synch">The synchronization object.</param>
		public SynchronizedVerticesPool(IVerticesPool<TEdgeData> wrapped, object synch = null)
		{
			_wrapped = wrapped;
			_thisLock = synch ?? new object();
		}

		/// <inheritdoc />
		public VertexAdjacency<TEdgeData> Empty => EmptyVertexAdjacency<TEdgeData>.Instance;
		
		/// <inheritdoc />
		/// <remarks>This operation is thread-safe.</remarks>
		public bool AddEdge(int destVertexIndex, TEdgeData edgeData, ref VertexAdjacency<TEdgeData> current)
		{
			lock (_thisLock)
			{
				return _wrapped.AddEdge(destVertexIndex, edgeData, ref current);
			}
		}
		/// <inheritdoc />
		/// <remarks>This operation is thread-safe.</remarks>
		public void Clear()
		{
			lock (_thisLock)
			{
				_wrapped.Clear();
			}
		}
		/// <inheritdoc />
		/// <remarks>This operation is thread-safe.</remarks>
		public VertexAdjacency<TEdgeData> GetInstance(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo, int edgesCount)
		{
			lock (_thisLock)
			{
				return _wrapped.GetInstance(edgesTo, edgesCount);
			}
		}
		/// <inheritdoc />
		/// <remarks>This operation is thread-safe.</remarks>
		public bool RemoveEdge(int destVertexIndex, ref VertexAdjacency<TEdgeData> current)
		{
			lock (_thisLock)
			{
				return _wrapped.RemoveEdge(destVertexIndex, ref current);
			}
		}

		/// <inheritdoc />
		/// <remarks>This operation is thread-safe.</remarks>
		public int RemoveAllEdges(ref VertexAdjacency<TEdgeData> current)
		{
			lock (_thisLock)
			{
				return _wrapped.RemoveAllEdges(ref current);
			}
		}
		/// <inheritdoc />
		/// <remarks>This operation is thread-safe.</remarks>
		public void ReturnRange(IEnumerable<VertexAdjacency<TEdgeData>> instances)
		{
			lock (_thisLock)
			{
				_wrapped.ReturnRange(instances);
			}
		}
	}
}