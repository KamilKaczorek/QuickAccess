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
// Project: QuickAccess.DataStructures.Tests
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QuickAccess.DataStructures.Graphs.Model;
using QuickAccess.DataStructures.UnitTests.Graphs.TestUtils;
using QuickAccess.Infrastructure.Collections;

namespace QuickAccess.DataStructures.UnitTests.Graphs.Model
{
	/// <summary>
	/// The unit tests of the <see cref="GraphConnectivityDefinition{TEdgeData}"/> abstract, generic class.
	/// </summary>
	[TestClass]
	public sealed class GraphConnectivityDefinitionTest
	{
		private const int VerticesCount = 10;
		private const int LastVertexIndex = VerticesCount - 1;
		private readonly int[][] _adjacency;
		private readonly int _edgesCount;
		private Mock<VertexAdjacency<int>>[] _vertexMocks;

		public GraphConnectivityDefinitionTest()
		{
			_edgesCount = 0;
			_adjacency = new int[VerticesCount][];

			// adjacency:
			// 0 => 
			// 1 => 0, 1
			// 2 => 0, 1, 2, 3
			// ...
			// (VerticesCount/2) => 0, 1, ..., (VerticesCount-1)
			// ...
			// (VerticesCount-1) => 0, 1, ..., (VerticesCount-1)

			for (var idx = 0; idx <= LastVertexIndex; idx++)
			{
				var adjEdgesCount = Math.Min(VerticesCount, idx * 2);
				_adjacency[idx] = Enumerable.Range(0, adjEdgesCount).ToArray();
				_edgesCount += adjEdgesCount;
			}
		}

		private int GetEdgeValue(int srcIdx, int destIndex)
		{
			// edge value is unique for each edge:

			// when VerticesCount = 10:

			// [1, 0] => -10
			// [1, 1] => -11
			// [2, 0] => -20
			// [2, 1] => -21
			// [2, 3] => -23
			// ...
			// [9, 9] => -99

			return -(srcIdx * VerticesCount + destIndex);
		}

		[TestInitialize]
		public void Setup()
		{
			// Arrange
			_vertexMocks = Enumerable.Range(0, VerticesCount)
									 .Select(srcIdx =>
										 GetVertexAdjacencyMock(_adjacency[srcIdx],
											 _adjacency[srcIdx].Select(dstIdx => GetEdgeValue(srcIdx, dstIdx))))
									 .ToArray();
		}

		[TestMethod]
		public void ON_GetEdgeData_SHOULD_Invoke_VertexAdjacency_GetEdgeToIndex_Of_SrcVertex_With_DestVertex_Arg()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			uut.GetEdgeData(2, 1);

			// Assert
			_vertexMocks[2].Verify(m => m.GetEdgeToIndex(1), Times.Exactly(1));

		}

		[TestMethod]
		public void ON_GetEdgeData_WHEN_Edge_Exists_SHOULD_Return_Value_Returned_By_VertexAdjacency_GetEdgeToIndex()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var data = uut.GetEdgeData(2, 1);

			// Assert
			Assert.AreEqual(GetEdgeValue(2, 1), data);
		}

		[ExpectedException(typeof(IndexOutOfRangeException))]
		[TestMethod]
		public void ON_GetEdgeData_WHEN_Src_Vertex_Index_Out_Of_Range_SHOULD_Throw_IndexOutOfRangeException()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			uut.GetEdgeData(-1, 1);

			// Assert handled by ExpectedException			
		}

		[TestMethod]
		public void ON_TryGetEdgeData_WHEN_Edge_Exists_SHOULD_Return_True_And_Proper_Edge_Data()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var res = uut.TryGetEdgeData(2, 1, out var data);

			// Assert
			Assert.IsTrue(res);
			Assert.AreEqual(GetEdgeValue(2, 1), data);
		}

		[TestMethod]
		public void ON_TryGetEdgeData_WHEN_Edge_Does_Not_Exist_SHOULD_Return_False_And_Default_Edge_Data()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var res = uut.TryGetEdgeData(VerticesCount + 1, 1, out var data);

			// Assert
			Assert.IsFalse(res);
			Assert.AreEqual(0, data);
		}

		[TestMethod]
		public void ON_ContainsEdge_WHEN_Edge_Exists_SHOULD_Return_True()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var res = uut.ContainsEdge(2, 1);

			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_ContainsEdge_WHEN_Edge_Does_Not_Exist_SHOULD_Return_False()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var res = uut.ContainsEdge(0, 10);

			// Assert
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void ON_ContainsBiDirectionalEdge_WHEN_BiDirectional_Edge_Exists_SHOULD_Return_True()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var res = uut.ContainsBiDirectionalEdge(4, 3);

			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_ContainsBiDirectionalEdge_WHEN_BiDirectional_Edge_Does_Not_Exists_SHOULD_Return_False()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var res = uut.ContainsBiDirectionalEdge(5, 2);

			// Assert
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void ON_GetEdges_WHEN_Edges_Defined_SHOULD_Return_Proper_Number_Of_Edges()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var res = uut.GetEdges().Count();

			// Assert
			Assert.AreEqual(_edgesCount, res);
		}

		[TestMethod]
		public void ON_GetEdges_WHEN_Edges_Defined_SHOULD_Return_All_Defined_Edges()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			var values = new HashSet<int>(_adjacency.SelectMany((a, srcIdx) => a.Select(dstIdx => GetEdgeValue(srcIdx, dstIdx))));

			// Act
			var res = uut.GetEdges();

			// Assert
			foreach (var edge in res)
			{
				Assert.IsTrue(values.Remove(edge.Data));
			}

			Assert.AreEqual(0, values.Count);
		}

		[TestMethod]
		public void ON_GetEdges_WHEN_Edges_Empty_SHOULD_Return_Empty_Sequence()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => throw new InvalidOperationException(), () => 0);

			// Act
			var res = uut.GetEdges();

			// Assert
			Assert.IsFalse(res.Any());
		}

		[TestMethod]
		public void ON_ContainsVertexAt_WHEN_Vertex_Exists_SHOULD_Return_True()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var all = Enumerable.Range(0, VerticesCount).All(idx => uut.ContainsVertexAt(idx));

			// Assert
			Assert.IsTrue(all);
		}

		[TestMethod]
		public void ON_ContainsVertexAt_WHEN_Vertex_Does_Not_Exist_SHOULD_Return_False()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var any = EnumerableExtensions.Enumerate(int.MinValue, -100, -1, VerticesCount, VerticesCount + 1, int.MaxValue)
										  .Any(idx => uut.ContainsVertexAt(idx));

			// Assert
			Assert.IsFalse(any);
		}

		[TestMethod]
		public void ON_TryGetVertexAt_WHEN_Vertex_Exists_SHOULD_Return_True()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var all = Enumerable.Range(0, VerticesCount).All(idx => uut.TryGetVertexAt(idx, out var _));
			// Assert
			Assert.IsTrue(all);
		}

		[TestMethod]
		public void ON_TryGetVertexAt_WHEN_Vertex_Exists_SHOULD_Return_Defined_Vertex()
		{
			// Arrange

			var expected = _vertexMocks.Select(m => m.Object).ToArray();

			var uut = new UnitUnderTest<int>(index => expected[index], () => expected.Length);

			// Act
			var vertices = Enumerable.Range(0, VerticesCount).Select(idx =>
			{
				uut.TryGetVertexAt(idx, out var v);
				return v;
			}).ToArray();

			// Assert
			CollectionAssert.AreEqual(expected, vertices);
		}

		[TestMethod]
		public void ON_GetEdgesToVertexAt_WHEN_Vertex_Exists_SHOULD_Return_Defined_Edges()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			var dest = 1;

			var expectedEdgeData =
				_adjacency.SelectMany((a, srcIdx) => a.Where(idx => idx == dest).Select(dstIdx => GetEdgeValue(srcIdx, dstIdx))).ToArray();

			// Act
			var edges = uut.GetEdgesToVertexAt(dest).ToArray();

			// Assert
			Assert.IsTrue(edges.All(e => e.Destination == dest));
			Assert.IsTrue(edges.All(e => _adjacency[e.Source].Contains(e.Destination)));
			CollectionAssert.AreEquivalent(expectedEdgeData, edges.Select(e => e.Data).ToArray());
		}

		[TestMethod]
		public void ON_GetEdgesFromVertexAt_WHEN_Vertex_Exists_SHOULD_Return_Defined_Edges()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			var src = 1;

			var expectedEdgeData =
				_adjacency[src].Select(dstIdx => GetEdgeValue(src, dstIdx)).ToArray();

			// Act
			var edges = uut.GetEdgesFromVertexAt(src).ToArray();

			// Assert
			Assert.IsTrue(edges.All(e => e.Source == src));
			Assert.IsTrue(edges.All(e => _adjacency[e.Source].Contains(e.Destination)));
			CollectionAssert.AreEquivalent(expectedEdgeData, edges.Select(e => e.Data).ToArray());
		}

		[TestMethod]
		public void ON_TryGetVertexAt_WHEN_Vertex_Does_Not_Exist_SHOULD_Return_False()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var any = EnumerableExtensions.Enumerate(int.MinValue, -100, -1, VerticesCount, VerticesCount + 1, int.MaxValue)
										  .Any(idx => uut.TryGetVertexAt(idx, out var _));

			// Assert
			Assert.IsFalse(any);
		}

		[TestMethod]
		public void ON_TryGetVertexAt_WHEN_Vertex_Does_Not_Exist_SHOULD_Return_Out_Null_Vertex()
		{
			// Arrange
			var uut = new UnitUnderTest<int>(index => _vertexMocks[index].Object, () => _vertexMocks.Length);

			// Act
			var any = EnumerableExtensions.Enumerate(int.MinValue, -100, -1, VerticesCount, VerticesCount + 1, int.MaxValue)
										  .Any(idx =>
										  {
											  uut.TryGetVertexAt(idx, out var vertex);
											  return vertex != null;
										  });

			// Assert
			Assert.IsFalse(any);
		}


		private static Mock<VertexAdjacency<TEdgeData>> GetVertexAdjacencyMock<TEdgeData>(
			IEnumerable<int> dests,
			IEnumerable<TEdgeData> data)
		{
			var assembler = new VertexAdjacencyMockAssembler<TEdgeData>();
			return assembler.WithAdjacentIndexes(dests).WithEdgesData(data).Build();			
		}

		private sealed class UnitUnderTest<TEdgeData> : GraphConnectivityDefinition<TEdgeData>
		{
			private readonly Func<int> _getCountCallback;
			private readonly Func<int, VertexAdjacency<TEdgeData>> _getVertexCallback;

			/// <inheritdoc />
			public override int Count => _getCountCallback();

			/// <inheritdoc />
			public override VertexAdjacency<TEdgeData> this[int index] => _getVertexCallback(index);

			public UnitUnderTest(Func<int, VertexAdjacency<TEdgeData>> getVertexCallback, Func<int> getCountCallback)
			{
				_getVertexCallback = getVertexCallback;
				_getCountCallback = getCountCallback;
			}

			/// <inheritdoc />
			public override IEnumerator<VertexAdjacency<TEdgeData>> GetEnumerator()
			{
				for (var idx = 0; idx < Count; idx++)
				{
					yield return _getVertexCallback(idx);
				}
			}
		}
	}
}