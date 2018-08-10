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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Graphs.Algorithms;
using QuickAccess.DataStructures.Graphs.Model;
using QuickAccess.DataStructures.Tests.Graphs.TestUtils;

namespace QuickAccess.DataStructures.Tests.Graphs.Algorithms
{
	/// <summary>
	///     The unit tests of the <see cref="BreadthFirstSearch" /> class.
	/// </summary>
	[TestClass]
	public sealed class BreadthFirstSearchTest
	{
		private List<VertexAdjacency<EmptyValue>> _adj;
		private VertexAdjacencyMockAssembler<EmptyValue> _adjMockAssembler;
		private Mock<GraphConnectivityDefinition<EmptyValue>> _connectivityDefinitionMock;

		private BreadthFirstSearch _uut;
		private VertexAdjacencyMockAssembler<EmptyValue> Adjacency => _adjMockAssembler.Reset();
		private GraphConnectivityDefinition<EmptyValue> Graph => _connectivityDefinitionMock.Object;

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			_uut = new BreadthFirstSearch();
			_adjMockAssembler = new VertexAdjacencyMockAssembler<EmptyValue>();
			_connectivityDefinitionMock = new Mock<GraphConnectivityDefinition<EmptyValue>>();
			_adj = new List<VertexAdjacency<EmptyValue>>(8);
			_connectivityDefinitionMock.Setup(m => m[It.Is<int>(idx => idx >= 0 && idx < _adj.Count)])
			                           .Returns((Func<int, VertexAdjacency<EmptyValue>>) (idx => _adj[idx]));
			_connectivityDefinitionMock.Setup(m => m.Count).Returns(() => _adj.Count);
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_SourceVertex_With_Adjacency_Defined_SHOULD_Return_Map_With_Given_SourceVertex()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1)); //0
			_adj.Add(Adjacency.WithAdjacentIndexes(2)); //1
			_adj.Add(Adjacency.WithAdjacentIndexes(3)); //2
			_adj.Add(Adjacency.WithAdjacentIndexes(4)); //3
			_adj.Add(Adjacency); //4

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 3);

			// Assert
			Assert.AreEqual(3, map.SourceVertex);
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_SourceVertex_Is_Not_Defined_SHOULD_Return_Map_With_Given_SourceVertex()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1));
			_adj.Add(Adjacency);

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 55);

			// Assert
			Assert.AreEqual(55, map.SourceVertex);
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_SourceVertex_Is_Not_Defined_SHOULD_Return_Empty_Map()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1));
			_adj.Add(Adjacency);

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 55);

			// Assert
			Assert.IsTrue(map.IsEmpty);
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_SourceVertex_With_Adjacency_Defined_SHOULD_Return_Map_With_All_Reachable_Vertices()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1)); //0
			_adj.Add(Adjacency.WithAdjacentIndexes(2)); //1
			_adj.Add(Adjacency.WithAdjacentIndexes(3)); //2
			_adj.Add(Adjacency.WithAdjacentIndexes(4)); //3
			_adj.Add(Adjacency); //4
			_adj.Add(Adjacency); //5

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 2);

			// Assert
			EnumerableExtensions.Enumerate(3, 4).ForEach(idx => Assert.IsTrue(map.HasPathTo(idx)));
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_SourceVertex_With_Adjacency_Defined_SHOULD_Return_Map_Without_Unreachable_Vertices()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1)); //0
			_adj.Add(Adjacency.WithAdjacentIndexes(2)); //1
			_adj.Add(Adjacency.WithAdjacentIndexes(3)); //2
			_adj.Add(Adjacency.WithAdjacentIndexes(4)); //3
			_adj.Add(Adjacency); //4
			_adj.Add(Adjacency); //5

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 2);

			// Assert
			EnumerableExtensions.Enumerate(0, 1, 2, 5).ForEach(idx => Assert.IsFalse(map.HasPathTo(idx)));
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_Single_Path_Defined_From_SourceVertex_SHOULD_Return_Map_With_Existing_Path()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1)); //0
			_adj.Add(Adjacency.WithAdjacentIndexes(2)); //1
			_adj.Add(Adjacency.WithAdjacentIndexes(3)); //2
			_adj.Add(Adjacency.WithAdjacentIndexes(4)); //3
			_adj.Add(Adjacency.WithAdjacentIndexes(5)); //4
			_adj.Add(Adjacency); //5

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 0);

			// Assert
			Assert.IsTrue(map.ContainsPath((0, 1, 2, 3, 4, 5).Enumerate()));
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_Self_Path_Defined_From_SourceVertex_SHOULD_Return_Map_With_Self_Path()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1)); //0
			_adj.Add(Adjacency.WithAdjacentIndexes(2, 1)); //1
			_adj.Add(Adjacency.WithAdjacentIndexes(0)); //2

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 1);

			// Assert
			Assert.IsTrue(map.ContainsPath((1, 1).Enumerate()));
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_Self_Path_Not_Defined_From_SourceVertex_SHOULD_Return_Map_Without_Self_Path()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1)); //0
			_adj.Add(Adjacency.WithAdjacentIndexes(2)); //1
			_adj.Add(Adjacency.WithAdjacentIndexes(0)); //2

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 1);

			// Assert
			Assert.IsFalse(map.ContainsPath((1, 1).Enumerate()));
		}

		[TestMethod]
		public void ON_GetSearchMapFrom_WHEN_Multiple_Path_Defined_From_SourceVertex_SHOULD_Return_Map_With_Shortest_Path()
		{
			// Arrange
			_adj.Add(Adjacency.WithAdjacentIndexes(1, 2, 7, 5, 8)); //0
			_adj.Add(Adjacency.WithAdjacentIndexes(2)); //1
			_adj.Add(Adjacency.WithAdjacentIndexes(3)); //2
			_adj.Add(Adjacency.WithAdjacentIndexes(4)); //3
			_adj.Add(Adjacency); //4
			_adj.Add(Adjacency.WithAdjacentIndexes(6)); //5
			_adj.Add(Adjacency.WithAdjacentIndexes(4)); //6
			_adj.Add(Adjacency.WithAdjacentIndexes(4, 6, 7)); //7
			_adj.Add(Adjacency.WithAdjacentIndexes(6)); //8

			// Act
			var map = _uut.GetSearchMapFrom(Graph, 0);

			// Assert
			Assert.IsTrue(map.ContainsPath((0, 7, 4).Enumerate()));
		}
	}
}