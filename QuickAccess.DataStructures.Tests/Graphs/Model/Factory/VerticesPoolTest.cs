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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Graphs.Factory;
#if DEBUG
using QuickAccess.DataStructures.Graphs.Factory.Internal;
#endif
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Tests.Graphs.Model.Factory
{

	public class VerticesPoolTest<TEdgeData>
	{

		private IEnumerable<AdjacentEdge<TEdgeData>> TakeAdjacentEdges(int count)
		{
			return Enumerable.Range(0, count).Select(idx => new AdjacentEdge<TEdgeData>(idx, default));
		}

		private VerticesPool<TEdgeData> CreatePool(int totalCapacity)
		{
			return new VerticesPool<TEdgeData>(totalCapacity, VerticesPoolCapacityType.TotalPoolCapacity);
		}

		[TestMethod]
		public void ON_get_Empty_SHOULD_Return_Always_Same_Singleton_Instance_From_Each_Pool_Instance()
		{
			// Arrange:
			var uut1 = CreatePool(10);
			var uut2 = CreatePool(0);

			// Act:
			var e1 = uut1.Empty;
			var e2 = uut1.Empty;
			var e3 = uut2.Empty;

			// Assert:
			Assert.AreSame(e1, e2);
			Assert.AreSame(e1, e3);
		}

		[TestMethod]
		public void ON_get_Empty_SHOULD_Return_Empty_Not_Null_Instance()
		{
			// Arrange:
			var uut = CreatePool(10);

			// Act:
			var res = uut.Empty;
			
			// Assert:
			Assert.IsNotNull(res);
			Assert.AreEqual(0, res.Count);
		}

		[TestMethod]
		public void ON_GetInstance_WHEN_Count_Zero_SHOULD_Return_Always_Same_Singleton_Empty_Instance_From_Each_Pool_Instance()
		{
			// Arrange:
			var uut1 = CreatePool(10);
			var uut2 = CreatePool(0);

			// Act:
			var e1 = uut1.GetInstance(null, 0);
			var e2 = uut1.GetInstance(Array.Empty<AdjacentEdge<TEdgeData>>(), 0);
			var e3 = uut2.GetInstance(null, 0);

			// Assert:
			Assert.AreSame(e1, e2);
			Assert.AreSame(e1, e3);
			Assert.AreSame(e1, uut1.Empty);
		}

		[TestMethod]
		public void ON_AddEdge_WHEN_Vertex_Empty_SHOULD_Return_True()
		{
			// Arrange:
			var uut = CreatePool(10);
			var v = uut.Empty;
			// Act:
			var res = uut.AddEdge(20, default, ref v);
		
			// Assert:
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_AddEdge_WHEN_Vertex_Empty_SHOULD_Replace_Vertex_Instance()
		{
			// Arrange:
			var uut = CreatePool(10);
			var v = uut.Empty;
			var prev = v;
			// Act:
			uut.AddEdge(20, default, ref v);
		
			// Assert:
			Assert.AreNotSame(prev, v);
		}

		[TestMethod]
		public void ON_AddEdge_WHEN_Vertex_Count_From_1_To_7_SHOULD_Replace_Vertex_Instance()
		{
			// Arrange:
			var uut = CreatePool(10);
			for (var count = 1; count <= 7; count++)
			{
				var v = uut.GetInstance(TakeAdjacentEdges(count), count);
				var prev = v;
				// Act:
				uut.AddEdge(1000, default, ref v);

				// Assert:
				Assert.AreNotSame(prev, v);
			}
		}

		[TestMethod]
		public void ON_RemoveEdge_WHEN_Vertex_Count_From_1_To_8_SHOULD_Replace_Vertex_Instance()
		{
			// Arrange:
			var uut = CreatePool(10);
			for (var count = 1; count <= 8; count++)
			{
				var v = uut.GetInstance(TakeAdjacentEdges(count), count);
				var prev = v;
				// Act:
				uut.RemoveEdge(0, ref v);

				// Assert:
				Assert.AreNotSame(prev, v);
			}
		}

		[TestMethod]
		public void ON_RemoveEdge_WHEN_Contains_Edge_SHOULD_Return_True_And_Vertex_With_Decreased_Count()
		{
			// Arrange:
			var uut = CreatePool(7);
			for (var expectedCount = 0; expectedCount <= 10; expectedCount++)
			{
				var v = uut.GetInstance(TakeAdjacentEdges(expectedCount+1), expectedCount+1);
				// Act:
				var res = uut.RemoveEdge(0, ref v);

				// Assert:
				Assert.IsTrue(res);
				Assert.AreEqual(expectedCount, v.Count);
			}
		}

		[TestMethod]
		public void ON_RemoveEdge_WHEN_Does_Not_Contain_Edge_SHOULD_Return_False_And_Vertex_With_Same_Count()
		{
			// Arrange:
			var uut = CreatePool(7);
			for (var expectedCount = 0; expectedCount <= 10; expectedCount++)
			{
				var v = uut.GetInstance(TakeAdjacentEdges(expectedCount), expectedCount);
				// Act:
				var res = uut.RemoveEdge(1000, ref v);

				// Assert:
				Assert.IsFalse(res);
				Assert.AreEqual(expectedCount, v.Count);
			}
		}

		[TestMethod]
		public void ON_GetInstance_WHEN_Vertex_Count_From_1_To_7_AND_Vertices_Returned_SHOULD_Return_Instance_From_The_Pool()
		{
			// Arrange:
			var uut = CreatePool(7);
			var returned = new VertexAdjacency<TEdgeData>[7];
			for (var count = 1; count <= 7; count++)
			{
				returned[count - 1] = uut.GetInstance(TakeAdjacentEdges(count), count);				
			}

			uut.ReturnRange(returned);

			for (var count = 1; count <= 7; count++)
			{
				
				// Act:
				var v = uut.GetInstance(TakeAdjacentEdges(count), count);

				// Assert:
				Assert.AreSame(returned[count-1], v);
			}
		}

		[TestMethod]
		public void ON_GetInstance_WHEN_GetInstance_Already_Invoked_Once_SHOULD_NOT_Return_Same_Instance_Twice()
		{
			// Arrange:
			var uut = CreatePool(7);
			var returned = new VertexAdjacency<TEdgeData>[7];
			for (var count = 1; count <= 7; count++)
			{
				returned[count - 1] = uut.GetInstance(TakeAdjacentEdges(count), count);				
			}

			uut.ReturnRange(returned);

			for (var count = 1; count <= 7; count++)
			{
				// Act:
				var v = uut.GetInstance(TakeAdjacentEdges(count), count);
				var other = uut.GetInstance(TakeAdjacentEdges(count), count);

				// Assert:
				Assert.AreNotSame(v, other);
				Assert.AreNotSame(returned[count-1], other);
			}
		}

		[TestMethod]
		public void ON_GetInstance_WHEN_Pool_Capacity_0_SHOULD_Not_Return_Instance_From_The_Pool()
		{
			// Arrange:
			var uut = CreatePool(0);
			var returned = new VertexAdjacency<TEdgeData>[7];
			for (var count = 1; count <= 7; count++)
			{
				returned[count - 1] = uut.GetInstance(TakeAdjacentEdges(count), count);				
			}

			uut.ReturnRange(returned);

			for (var count = 1; count <= 7; count++)
			{
				
				// Act:
				var v = uut.GetInstance(TakeAdjacentEdges(count), count);

				// Assert:
				Assert.AreNotSame(returned[count-1], v);
			}
		}

		[TestMethod]
		public void ON_GetInstance_WHEN_Pool_Clear_Invoked_SHOULD_Not_Return_Instance_From_The_Pool()
		{
			// Arrange:
			var uut = CreatePool(10);
			var returned = new VertexAdjacency<TEdgeData>[7];
			for (var count = 1; count <= 7; count++)
			{
				returned[count - 1] = uut.GetInstance(TakeAdjacentEdges(count), count);				
			}

			uut.ReturnRange(returned);


			for (var count = 1; count <= 7; count++)
			{
				
				// Act:
				uut.Clear();
				var v = uut.GetInstance(TakeAdjacentEdges(count), count);

				// Assert:
				Assert.AreNotSame(returned[count-1], v);
			}
		}

		[TestMethod]
		public void ON_AddEdge_WHEN_Vertex_Count_From_Higher_Than_7_SHOULD_NOT_Replace_Vertex_Instance()
		{
			// Arrange:
			var uut = CreatePool(10);
			
			var v = uut.GetInstance(TakeAdjacentEdges(8), 8);
			var prev = v;

			// Act:
			uut.AddEdge(1000, default, ref v);

			// Assert:
			Assert.AreSame(prev, v);

			// Act:
			uut.AddEdge(1001, default, ref v);

			// Assert:
			Assert.AreSame(prev, v);			
		}

		[TestMethod]
		public void ON_AddEdge_WHEN_Adj_Vertex_Not_Yet_Added_SHOULD_Return_True_And_Vertex_With_Increased_Count_By_One()
		{
			// Arrange:
			var uut = CreatePool(10);
			var v = uut.Empty;

			for (int expectedCount = 1, adj = 0; expectedCount <= 10; ++expectedCount, ++adj)
			{
				// Act:
				var res = uut.AddEdge(adj, default, ref v);

				// Assert:
				Assert.IsTrue(res);
				Assert.AreEqual(expectedCount, v.Count);
			}				
		}

		[TestMethod]
		public void ON_AddEdge_WHEN_Adj_Vertex_Already_Added_SHOULD_Return_False_And_Same_Vertex_Instance_With_Not_Changed_Count()
		{
			// Arrange:
			var uut = CreatePool(10);

			for (var expectedCount = 1; expectedCount <= 10; ++expectedCount)
			{
				// Arrange:
				var adj = TakeAdjacentEdges(expectedCount).ToArray();
				var v = uut.GetInstance(adj, expectedCount);
				var prev = v;
				// Act:
				var res = uut.AddEdge(adj.First().Destination, default, ref v);

				// Assert:
				Assert.IsFalse(res);
				Assert.AreSame(prev, v);
				Assert.AreEqual(expectedCount, v.Count);
			}				
		}

		#if DEBUG
		[TestMethod]
		public void ON_GetInstance_WHEN_Count_From_1_To_7_SHOULD_Return_Instance_With_Fixed_Capacity()
		{
			// Arrange:
			var uut1 = CreatePool(10);

			for (var count = 1; count <= 7; count++)
			{
				// Act:
				var v = (PoolableVertexAdjacency<TEdgeData>)uut1.GetInstance(TakeAdjacentEdges(count), count);

				// Assert:
				Assert.AreEqual(count, v.MaxCapacity);
			}
		}

		[TestMethod]
		public void ON_AddEdge_WHEN_Vertex_Count_From_0_To_6_SHOULD_Return_Instance_With_Fixed_Capacity()
		{
			// Arrange:
			var uut = CreatePool(10);
			for (var count = 0; count <= 6; count++)
			{
				var v = uut.GetInstance(TakeAdjacentEdges(count), count);			
				// Act:
				var res = uut.AddEdge(1000, default, ref v);

				// Assert:
				Assert.IsTrue(res);
				Assert.AreEqual(count+1, ((PoolableVertexAdjacency<TEdgeData>)v).MaxCapacity);
			}
		}

		[TestMethod]
		public void ON_GetInstance_WHEN_Count_Higher_Than_7_SHOULD_Return_Instance_With_Unlimited_Capacity()
		{
			// Arrange:
			var uut1 = CreatePool(10);

			var unlimited = PoolableVertexAdjacency<TEdgeData>.UnlimitedCapacity;
			
			// Act:
			var v8 = (PoolableVertexAdjacency<TEdgeData>)uut1.GetInstance(TakeAdjacentEdges(8), 8);
			var v9 = (PoolableVertexAdjacency<TEdgeData>)uut1.GetInstance(TakeAdjacentEdges(9), 9);
			var v100 = (PoolableVertexAdjacency<TEdgeData>)uut1.GetInstance(TakeAdjacentEdges(100), 100);
			

			// Assert:
			Assert.AreEqual(unlimited, v8.MaxCapacity);
			Assert.AreEqual(unlimited, v9.MaxCapacity);
			Assert.AreEqual(unlimited, v100.MaxCapacity);			
		}
		#endif
	}

	[TestClass]
	// ReSharper disable once InconsistentNaming
	public class VerticesPoolTest_WHEN_TEdgeDataIsEmptyValue : VerticesPoolTest<EmptyValue>
	{
		
	}

	[TestClass]
	// ReSharper disable once InconsistentNaming
	public class VerticesPoolTest_WHEN_TEdgeDataIsString : VerticesPoolTest<string>
	{
		
	}
}