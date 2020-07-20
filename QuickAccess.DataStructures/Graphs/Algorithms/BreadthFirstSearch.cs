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
using System.Diagnostics.Contracts;
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
		[Pure]
		public VertexSearchMap<int> GetSearchMapFrom<TEdgeData>(
			GraphConnectivityDefinition<TEdgeData> graph,
			int startVertexIndex,
			FilterAdjacentVerticesCallback<TEdgeData> filterAdjacentVerticesCallback = null)
		{
			Dictionary<int, int> destToSourceMap = null;

			if (graph.ContainsVertexAt(startVertexIndex))
			{
				var queue = new Queue();

				queue.Enqueue(startVertexIndex);
				destToSourceMap = new Dictionary<int, int>();

				while (!queue.IsEmpty)
				{
					var source = queue.Dequeue();
					var adj = graph[source];
                    var adjacent = filterAdjacentVerticesCallback?.Invoke(source, adj) ?? adj.AdjacentIndexes;
                        
					foreach (var destination in adjacent)
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

        private sealed class Queue
        {
            private QueueItem _head;
            private QueueItem _tail;

            public Queue()
            {
                _head = null;
                _tail = null;
            }

            public void Enqueue(int index)
            {
                _tail = new QueueItem(index, ref _head, ref _tail);
            }

            public bool IsEmpty => _head == null;

            public int Dequeue()
            {
                if(_head == null)
                {
                    throw new InvalidOperationException("Queue is empty - can't dequeue.");
                }

                var h = _head;
                _head = h.Next;

                if (_head == null)
                {
                    _tail = null;
                }

                return h.Index;
            }

            private class QueueItem
            {
                public QueueItem(int index, ref QueueItem head, ref QueueItem tail)
                {
                
                    Index = index;
                    Next = null;

                    if (head == null)
                    {
                        head = this;
                    }
                    else
                    {
                        tail.Next = this;
                    }
                }

                public int Index { get; }
                public QueueItem Next { get; private set; }
            }

        }
    }
}