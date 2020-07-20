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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.DataStructures.Graphs.Algorithms;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.UnitTests.Graphs.Algorithms
{
	/// <summary>
	///     The unit tests of the <see cref="VertexSearchMap{TKey}" /> structure and <see cref="VertexSearchMap" />.
	/// </summary>
	[TestClass]
	public sealed class VertexSearchMapTest
	{
		[TestMethod]
		public void ON_ContainsPath_WITH_Initial_Path_WHEN_Map_Created_From_Path_SHOULD_Return_True()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());
			// Act
			var res = uut.ContainsPath((1, 2, 3, 4).Enumerate());
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_GetPathTo_WHEN_Map_Created_From_Path_AND_Path_To_Given_Vertex_Exists_SHOULD_Return_Proper_Path()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());
			// Act
			var to4 = uut.GetPathTo(4).ToArray();
			var to3 = uut.GetPathTo(3).ToArray();
			var to2 = uut.GetPathTo(2).ToArray();
			// Assert
			CollectionAssert.AreEqual((1, 2, 3, 4).Enumerate().ToArray(), to4);
			CollectionAssert.AreEqual((1, 2, 3).Enumerate().ToArray(), to3);
			CollectionAssert.AreEqual((1, 2).Enumerate().ToArray(), to2);
		}

		[TestMethod]
		public void
			ON_GetReversedPathTo_WHEN_Map_Created_From_Path_AND_Path_To_Given_Vertex_Exists_SHOULD_Return_Proper_Reversed_Path()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());
			// Act
			var to4 = uut.GetReversedPathTo(4).ToArray();
			var to3 = uut.GetReversedPathTo(3).ToArray();
			var to2 = uut.GetReversedPathTo(2).ToArray();
			// Assert
			CollectionAssert.AreEqual((4, 3, 2, 1).Enumerate().ToArray(), to4);
			CollectionAssert.AreEqual((3, 2, 1).Enumerate().ToArray(), to3);
			CollectionAssert.AreEqual((2, 1).Enumerate().ToArray(), to2);
		}

		[TestMethod]
		public void ON_GetPathTo_WITH_SourceVertex_WHEN_SourceVertex_Has_Self_Loop_SHOULD_Return_Buckle_Path()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 1, 2, 3).Enumerate());
			// Act
			var to1 = uut.GetPathTo(1).ToArray();
			// Assert
			CollectionAssert.AreEqual((1, 1).Enumerate().ToArray(), to1);
		}

		[TestMethod]
		public void ON_GetReversedPathTo_WITH_SourceVertex_WHEN_SourceVertex_Has_Self_Loop_SHOULD_Return_Buckle_Path()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 1, 2, 3).Enumerate());
			// Act
			var to1 = uut.GetReversedPathTo(1).ToArray();
			// Assert
			CollectionAssert.AreEqual((1, 1).Enumerate().ToArray(), to1);
		}

		[TestMethod]
		public void ON_get_HasSelfLoop_WHEN_SourceVertex_Has_Self_Loop_SHOULD_Return_True()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 1, 2, 3).Enumerate());
			// Act
			var res = uut.HasSelfLoop;
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_get_HasSelfLoop_WHEN_SourceVertex_Does_Not_Have_Self_Loop_SHOULD_Return_False()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 1).Enumerate());
			// Act
			var res = uut.HasSelfLoop;
			// Assert
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void ON_get_HasPathToSelf_WHEN_SourceVertex_Has_Self_Loop_SHOULD_Return_True()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 1, 2, 3).Enumerate());
			// Act
			var res = uut.HasPathToSelf;
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_get_HasPathToSelf_WHEN_SourceVertex_Has_Path_To_Self_AND_Does_Not_Have_Self_Loop_SHOULD_Return_True()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 1).Enumerate());
			// Act
			var res = uut.HasPathToSelf;
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_get_HasPathToSelf_WHEN_SourceVertex_Does_Not_Have_Self_Path_SHOULD_Return_False()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3).Enumerate());
			// Act
			var res = uut.HasPathToSelf;
			// Assert
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void ON_GetPathTo_WHEN_Map_Created_From_Path_AND_Path_To_Given_Vertex_Does_Not_Exist_SHOULD_Return_Empty()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());
			// Act
			var to1 = uut.GetPathTo(1).ToArray();
			var to0 = uut.GetPathTo(0).ToArray();
			// Assert
			Assert.AreEqual(0, to1.Length);
			Assert.AreEqual(0, to0.Length);
		}

		[TestMethod]
		public void ON_ContainsReversedPath_WITH_Reversed_Initial_Path_WHEN_Map_Created_From_Path_SHOULD_Return_True()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());
			// Act
			var res = uut.ContainsReversedPath((4, 3, 2, 1).Enumerate());
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_ContainsPath_WITH_Initial_Path_WHEN_Map_Created_From_Reversed_Path_SHOULD_Return_True()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromReversedPath((1, 2, 3, 4).Enumerate());
			// Act
			var res = uut.ContainsPath((4, 3, 2, 1).Enumerate());
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_ContainsPath_WITH_Sub_Path_Of_Initial_Path_WHEN_Map_Created_From_Path_SHOULD_Return_True()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());
			// Act
			var res =
				(uut.ContainsPath((2, 3).Enumerate()), uut.ContainsPath((1, 2, 3).Enumerate()), uut.ContainsPath((2, 3, 4).Enumerate()))
				.Enumerate()
				.ToArray();

			// Assert
			Assert.IsTrue(res.All(b => b));
		}

		[TestMethod]
		public void ON_ContainsPath_WITH_Reversed_Path_WHEN_Map_Created_From_Path_SHOULD_Return_False()
		{
			// Arrange
			var uut = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());
			// Act
			var res = uut.ContainsPath((4, 3, 2, 1).Enumerate());
			// Assert
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void ON_CreateFromPath_WHEN_Path_Not_Empty_SHOULD_Return_Map_With_Proper_SourceVertex()
		{
			// Arrange Act
			var map = VertexSearchMap.CreateFromPath((1, 2, 3, 4).Enumerate());

			// Assert
			Assert.AreEqual(1, map.SourceVertex);
		}

		
		[TestMethod]
		public void ON_get_IsEmpty_WHEN_Wrapped_Dictionary_Count_Is_0_SHOULD_Return_True()
		{
			// Arrange
			var map = new VertexSearchMap<int>(0, new Dictionary<int, int>());
			// Act
			var res = map.IsEmpty;

			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_get_IsEmpty_WHEN_Wrapped_Dictionary_Is_Null_SHOULD_Return_True()
		{
			// Arrange
			var map = new VertexSearchMap<int>(0, null);
			// Act
			var res = map.IsEmpty;

			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_get_IsEmpty_WHEN_Wrapped_Dictionary_Is_Not_Empty_SHOULD_Return_False()
		{
			// Arrange
			var map = new VertexSearchMap<int>(0, new Dictionary<int, int> {{0, 0}});
			// Act
			var res = map.IsEmpty;
			// Assert
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void ON_GetEnumerator_SHOULD_Return_Enumerator_With_All_Specified_Adjacencies()
		{
			// Arrange
			var asm = GraphEdgesSequenceAssembler.Create<int>();
			asm.Define(1, 1);
			asm.Define(1, 2);
			asm.Define(2, 3);
			asm.Define(3, 4);
			asm.Define(1, 5);
			asm.Define(5, 6);
			asm.Define(1, 7);

			var adj = asm.AdjacentVerticesPairs.ToArray();

			var map = new VertexSearchMap<int>(1, adj.ToDictionary(a => a.Destination, a => a.Source));
			// Act
			var res = map.ToArray();
			// Assert
			CollectionAssert.AreEquivalent(adj, res);
		}

		[TestMethod]
		public void ON_GetEnumerator_WHEN_Initialized_With_Null_Dictionary_SHOULD_Return_Empty_Sequence_Enumerator()
		{
			// Arrange		
			var map = new VertexSearchMap<int>(0, null);
			// Act
			var res = map.ToArray();
			// Assert
			Assert.AreEqual(0, res.Length);
		}

		[TestMethod]
		public void ON_GetEnumerator_WHEN_Initialized_With_Empty_Dictionary_SHOULD_Return_Empty_Sequence_Enumerator()
		{
			// Arrange		
			var map = new VertexSearchMap<int>(0, new Dictionary<int, int>());
			// Act
			var res = map.ToArray();
			// Assert
			Assert.AreEqual(0, res.Length);
		}

		[TestMethod]
		public void ON_get_Comparer_SHOULD_Return_Dictionary_Comparer()
		{
			// Arrange		
			var comparer = new Mock<IEqualityComparer<int>>().Object;
			var map = new VertexSearchMap<int>(0, new Dictionary<int, int>(comparer));
			// Act
			var res = map.Comparer;
			// Assert
			Assert.AreEqual(comparer, res);
		}
	}
}