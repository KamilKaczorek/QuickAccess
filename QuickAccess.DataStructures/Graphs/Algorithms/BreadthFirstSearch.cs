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

namespace QuickAccess.DataStructures.Graphs.Algorithms
{
	/// <summary>
	/// The breadth first search implementation.
	/// </summary>
	/// <seealso cref="IGraphSearch" />
	public sealed class BreadthFirstSearch
		: IGraphSearch
	{
		/// <inheritdoc />
		public VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			GraphConnectivityDefinition<TEdgeData> graph,
			int startVertexIndex,
			FilterAdjacentVerticesCallback<TEdgeData> filterAdjacentVerticesCallback = null)
		{
			Dictionary<int, int> destToSourceMap = null;

			if (graph.ContainsVertexAt(startVertexIndex))
			{
				var queue = new Queue<int>();

				queue.Enqueue(startVertexIndex);
				destToSourceMap = new Dictionary<int, int>();

				while (queue.Count > 0)
				{
					var source = queue.Dequeue();

					var adj = graph[source];
					foreach (var destination in filterAdjacentVerticesCallback == null
						? adj.AdjacentIndexes
						: filterAdjacentVerticesCallback.Invoke(source, adj))
					{
						if (destination == source && startVertexIndex != source)
						{
							continue;
						}

						if (destToSourceMap.ContainsKey(destination))
						{
							continue;
						}

						destToSourceMap[destination] = source;

						if (graph[destination].EdgesCount > 0)
						{
							queue.Enqueue(destination);
						}
					}
				}

				if (destToSourceMap.Count <= 0)
				{
					destToSourceMap = null;
				}
			}

			return new VertexSearchMap<int>(startVertexIndex, destToSourceMap);
		}
	}
}