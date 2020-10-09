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
using QuickAccess.DataStructures.Graphs.Algorithms;
using QuickAccess.DataStructures.Graphs.Extensions;
using QuickAccess.DataStructures.Graphs.Model;
using QuickAccess.DataStructures.UnitTests.Graphs.Model;
using QuickAccess.Infrastructure.Collections;

namespace QuickAccess.DataStructures.UnitTests.Graphs.Integration
{
	/// <summary>
	///     The integration tests of <see cref="SymbolGraph{TEdgeData,TSymbol}" /> integrated with
	///     <see cref="GraphExtensions" /> (see also<see cref="GraphExtensions.BFS" />),
	///     <see cref="IndexGraph{TEdgeData}" /> (see also <see cref="GraphConnectivityDefinitionFactory" />),
	///     <see cref="VertexSearchMap" />.
	/// </summary>
	/// <seealso cref="TestEdgesFactory" />
	[TestClass]
	public sealed class SymbolGraphIntergrationTest
	{
		[TestMethod]
		public void ON_GetPathTo_WHEN_Single_Path_Exist_SHOULD_Return_Proper_Path()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination, TestEdgesFactory.DefinitionSet.PathA2J);

			uut.Freeze();

			var map = uut.GetSearchMapFrom("A");

			// Act
			var path = map.GetPathTo("J").ToArray();

			// Assert
			CollectionAssert.AreEqual(TestEdgesFactory.VertexSymbolsA2J, path);
		}

		[TestMethod]
		public void ON_ContainsVertex_WHEN_SymbolComparer_Set_To_InvariantCultureIgnoreCase_AND_Vertex_Lower_Case_SHOULD_Return_True()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination, TestEdgesFactory.DefinitionSet.PathA2J);
			// Act
			var res = uut.ContainsVertex("a");
			// Assert
			Assert.IsTrue(res);
		}

		[TestMethod]
		public void ON_ContainsVertex_WHEN_SymbolComparer_Set_To_InvariantCulture_AND_Vertex_Lower_Case_SHOULD_Return_False()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCulture);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination, TestEdgesFactory.DefinitionSet.PathA2J);
			// Act
			var res = uut.ContainsVertex("a");
			// Assert
			Assert.IsFalse(res);
		}

		[TestMethod]
		public void ON_GetPathTo_WHEN_Multiple_Paths_SHOULD_Return_Shortest_Path()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J | TestEdgesFactory.DefinitionSet.EMFNBIGD);

			uut.Freeze();

			var map = uut.GetSearchMapFrom("A");
			// Act
			var path = map.GetPathTo("J").ToArray();
			// Assert
			CollectionAssert.AreEqual("ABIJ".AsStrings().ToArray(), path);
		}

		[TestMethod]
		public void ON_ConvertPathToEdgeData_WHEN_Path_Exists_SHOULD_Return_Path_Edge_Data()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J | TestEdgesFactory.DefinitionSet.JA);

			uut.Freeze();

			// Act
			var edges = uut.ConvertPathToEdgeData("ABCD".AsStrings()).ToArray();
			// Assert
			CollectionAssert.AreEqual(EnumerableExtensions.Enumerate("AB", "BC", "CD").ToArray(), edges.ToArray());
		}

		[ExpectedException(typeof(KeyNotFoundException))]
		[TestMethod]
		public void ON_ConvertPathToEdgeData_WHEN_Path_Does_Not_Exist_SHOULD_Throw_Exception()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J | TestEdgesFactory.DefinitionSet.JA);

			uut.Freeze();

			// Act
			var _ = uut.ConvertPathToEdgeData("ADJF".AsStrings()).ToArray();
			// Assert handled by expected exception.
		}

		[TestMethod]
		public void ON_GetPathTo_WHEN_Vertex_Self_Looped_And_Alternative_Path_Exist_SHOULD_Return_Buckle_Path()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J | TestEdgesFactory.DefinitionSet.JA);

			uut.AddEdge("B", "B", "SelfB");

			uut.Freeze();

			var map = uut.GetSearchMapFrom("B");
			// Act
			var path = map.GetPathTo("B").ToArray();
			// Assert
			CollectionAssert.AreEqual("BB".AsStrings().ToArray(), path);
		}

		[TestMethod]
		public void ON_GetPathTo_WHEN_Vertex_Self_Looped_SHOULD_Return_Buckle_Path()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J | TestEdgesFactory.DefinitionSet.JA | TestEdgesFactory.DefinitionSet.SelfA2J);

			uut.Freeze();

			var map = uut.GetSearchMapFrom("B");
			// Act
			var path = map.GetPathTo("B").ToArray();
			// Assert
			CollectionAssert.AreEqual("BB".AsStrings().ToArray(), path);
		}

		[TestMethod]
		public void ON_GetPathTo_WHEN_Path_Does_Not_Exist_SHOULD_Return_Empty()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J);

			uut.Freeze();

			var map = uut.GetSearchMapFrom("B");
			// Act
			var any = map.GetPathTo("B").Any();
			// Assert
			Assert.IsFalse(any);
		}

		[TestMethod]
		public void ON_GetPathTo_WHEN_Vertex_Does_Not_Exist_SHOULD_Return_Empty()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J);

			uut.Freeze();

			var map = uut.GetSearchMapFrom("@");
			// Act
			var any = map.GetPathTo("!").Any();

			// Assert
			Assert.IsFalse(any);
		}

		[ExpectedException(typeof(KeyNotFoundException))]
		[TestMethod]
		public void ON_GetEdgeData_WHEN_Src_Vertex_Does_Not_Exist_SHOULD_Throw_KeyNotFoundException()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J);
			// Act
			var _ = uut.GetEdgeData("@", "B");
			// Assert handled by expected exception
		}

		[ExpectedException(typeof(KeyNotFoundException))]
		[TestMethod]
		public void ON_GetEdgeData_WHEN_Dst_Vertex_Does_Not_Exist_SHOULD_Throw_KeyNotFoundException()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J);
			// Act
			var _ = uut.GetEdgeData("B", "@");
			// Assert handled by expected exception
		}

		[ExpectedException(typeof(KeyNotFoundException))]
		[TestMethod]
		public void ON_GetEdgeData_WHEN_Vertices_Exist_But_Edge_Does_Not_Exist_SHOULD_Throw_KeyNotFoundException()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J);
			// Act
			var _ = uut.GetEdgeData("B", "A");
			// Assert handled by expected exception
		}

		[TestMethod]
		public void ON_GetEdgeData_WHEN_Edge_Exists_SHOULD_Return_Edge_Data()
		{
			// Arrange
			var uut = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(uut, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J);
			// Act
			var data = uut.GetEdgeData("C", "D");
			// Assert
			Assert.AreEqual("CD", data);
		}

		[TestMethod]
		public void ON_GetCompacted_SHOULD_Return_Graph_With_All_Source_Graph_Edges()
		{
			// Arrange
			var source = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(source, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J | TestEdgesFactory.DefinitionSet.PathsFromQRS |
				TestEdgesFactory.DefinitionSet.SelfT);
			// Act
			var uut = source.GetCompacted().Data;
			// Assert
			CollectionAssert.AreEquivalent(source.GetEdges().ToArray(), uut.GetEdges().ToArray());
		}

		[TestMethod]
		public void ON_GetCompacted_WHEN_Edges_Filtered_Out_SHOULD_Return_Graph_With_All_Not_Filtered_Edges_Source_Graph_Edges()
		{
			// Arrange
			var source = new SymbolGraph<string, string>(40, StringComparer.InvariantCultureIgnoreCase);

			TestEdgesFactory.Define(source, v => v.Source + v.Destination,
				TestEdgesFactory.DefinitionSet.PathA2J | TestEdgesFactory.DefinitionSet.PathsFromQRS |
				TestEdgesFactory.DefinitionSet.SelfT);
			// Act
			var uut = source.GetCompacted().Data;
			// Assert
			CollectionAssert.AreEquivalent(source.GetEdges().ToArray(), uut.GetEdges().ToArray());
		}
	}
}