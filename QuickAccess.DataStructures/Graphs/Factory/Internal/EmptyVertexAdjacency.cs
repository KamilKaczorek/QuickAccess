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
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Factory.Internal
{
	/// <summary>
	/// Empty vertex adjacency.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="ReplaceableVertexAdjacency{TEdgeData}" />
	internal sealed class EmptyVertexAdjacency<TEdgeData> : ReplaceableVertexAdjacency<TEdgeData>
	{
		public static readonly EmptyVertexAdjacency<TEdgeData> Instance = new EmptyVertexAdjacency<TEdgeData>();

		private EmptyVertexAdjacency()
		{			
		}

		/// <inheritdoc />
		public override int EdgesCount => 0;


		/// <inheritdoc />
		public override int MaxCapacity => 0;

		/// <inheritdoc />
		public override bool AddEdge(ReplaceableVertexFactoryInterface<TEdgeData> vertexFactory,
		                             int destVertexIndex,
		                             TEdgeData edgeData,
		                             out ReplaceableVertexAdjacency<TEdgeData> final)
		{
			final = vertexFactory.GetInstance(EnumerableExtensions.Enumerate(AdjacentEdge.Create(destVertexIndex, edgeData)), 1);
			return true;
		}

		/// <inheritdoc />
		public override bool RemoveEdge(ReplaceableVertexFactoryInterface<TEdgeData> vertexFactory, int destVertexIndex, out ReplaceableVertexAdjacency<TEdgeData> final)
		{
			final = this;
			return false;
		}

		/// <inheritdoc />
		public override bool ContainsEdgeToIndex(int destVertexIndex)
		{
			return false;
		}

		/// <inheritdoc />
		public override IEnumerable<int> AdjacentIndexes
		{
			get
			{
				yield break;
			}
		}

		/// <inheritdoc />
		public override bool TryGetEdgeToIndex(int destVertexIndex, out TEdgeData edgeData)
		{
			edgeData = default;
			return false;
		}

		/// <inheritdoc />
		public override TEdgeData GetEdgeToIndex(int destVertexIndex)
		{
			throw new IndexOutOfRangeException();
		}

		

		/// <inheritdoc />
		public override void Initialize(IEnumerable<AdjacentEdge<TEdgeData>> edgesTo)
		{
			if (edgesTo?.Any() ?? false)
			{
				throw new InvalidOperationException("Can't add edges to empty vertex.");
			}
		}

		/// <inheritdoc />
		public override void Reset()
		{
		}

		/// <inheritdoc />
		public override IEnumerator<AdjacentEdge<TEdgeData>> GetEnumerator()
		{
			yield break;
		}		
	}
}