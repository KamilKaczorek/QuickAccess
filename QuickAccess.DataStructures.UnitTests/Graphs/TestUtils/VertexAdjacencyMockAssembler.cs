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
// Project: QuickAccess.DataStructures.Tests
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using QuickAccess.DataStructures.Graphs.Model;
using QuickAccess.DataStructures.UnitTests.TestUtils;
using Range = Moq.Range;

namespace QuickAccess.DataStructures.UnitTests.Graphs.TestUtils
{
	/// <summary>
	/// The assembler of mock objects (<see cref="Moq.Mock"/>) of <see cref="VertexAdjacency{TEdgeData}"/> type.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	public sealed class VertexAdjacencyMockAssembler<TEdgeData>
	{
		private readonly List<TEdgeData> _data = new List<TEdgeData>();
		private readonly List<int> _destinations = new List<int>();

		public VertexAdjacencyMockAssembler<TEdgeData> WithAdjacentEdges(IEnumerable<AdjacentEdge<TEdgeData>> edges)
		{
			foreach (var adjacentEdge in edges)
			{
				_data.Add(adjacentEdge.Data);
				_destinations.Add(adjacentEdge.Destination);
			}

			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> WithAdjacentEdges(IEnumerable<KeyValuePair<int, TEdgeData>> edges)
		{
			foreach (var adjacentEdge in edges)
			{
				_data.Add(adjacentEdge.Value);
				_destinations.Add(adjacentEdge.Key);
			}

			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> WithAdjacentIndexes(IEnumerable<int> indexes)
		{
			_destinations.AddRange(indexes);
			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> WithEdgesData(IEnumerable<TEdgeData> data)
		{
			_data.AddRange(data);
			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> WithAdjacentIndexes(params int[] indexes)
		{
			_destinations.AddRange(indexes);
			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> WithEdgesData(params TEdgeData[] data)
		{
			_data.AddRange(data);
			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> Reset()
		{
			_data.Clear();
			_destinations.Clear();
			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> Build(out Mock<VertexAdjacency<TEdgeData>> mock)
		{
			mock = Build();
			return this;
		}

		public VertexAdjacencyMockAssembler<TEdgeData> Build(out VertexAdjacency<TEdgeData> mockedObject)
		{
			mockedObject = Build().Object;
			return this;
		}

		public Mock<VertexAdjacency<TEdgeData>> Build()
		{
			if (_data.Count < _destinations.Count)
			{
				_data.AddRange(Enumerable.Repeat(default(TEdgeData), _destinations.Count - _data.Count));
			}

			return GetVertexAdjacencyMock(_destinations, _data);
		}

		public static implicit operator VertexAdjacency<TEdgeData>(VertexAdjacencyMockAssembler<TEdgeData> source)
		{
			return source.Build().Object;
		}

		private static Mock<VertexAdjacency<TEdgeData>> GetVertexAdjacencyMock(
			IEnumerable<int> dests,
			IEnumerable<TEdgeData> data)
		{
			var mock = new Mock<VertexAdjacency<TEdgeData>>();
			var adj = dests.Zip(data, AdjacentEdge.Create).ToArray();

			mock.Setup(m => m.EdgesCount).Returns(adj.Length);
			mock.Setup(m => m.AdjacentIndexes).Returns(adj.Select(a => a.Destination));
			mock.Setup(m => m.GetEdgeToIndex(It.IsInRange(0, adj.Length, Range.Inclusive)))
			    .Returns((Func<int, TEdgeData>) (i => adj[i].Data));
			mock.Setup(m => m.ContainsEdgeToIndex(It.IsAny<int>())).Returns((Func<int, bool>) (i => i >= 0 && i < adj.Length));
			mock.Setup(m => m.GetEnumerator()).Returns(adj.Cast<AdjacentEdge<TEdgeData>>().GetEnumerator());

			// ReSharper disable once NotAccessedVariable
			TEdgeData d = default;
			mock.Setup(m => m.TryGetEdgeToIndex(It.IsAny<int>(), out d))
			    .TryGetCallback<VertexAdjacency<TEdgeData>, int, TEdgeData>(i => i >= 0 && i < adj.Length, i => adj[i].Data);

			return mock;
		}
	}
}