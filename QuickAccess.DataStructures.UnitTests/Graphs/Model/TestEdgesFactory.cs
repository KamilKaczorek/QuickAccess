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
using System.Linq;
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.DataStructures.Graphs.Extensions;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.UnitTests.Graphs.Model
{
	internal static class TestEdgesFactory
	{
		public static void Define<T>(IGraphSource<T, string> graph,
		                             Func<VerticesPair<string>, T> getEdgeDataCallback,
		                             DefinitionSet set,
		                             Func<Edge<string, T>, bool> addEdgePredicate = null)
		{
			var e = new GraphEdgeSequenceAssembler<string>();

			if ((set & DefinitionSet.Set0) != 0)
			{
				for (var idx = 0; idx < VertexSymbolsA2J.Length - 1; ++idx)
				{
					e.Define(VertexSymbolsA2J[idx], VertexSymbolsA2J[idx + 1]);
				}
			}

			if ((set & DefinitionSet.Set1) != 0)
			{
				e.Define("E", "M");
				e.Define("F", "N");
				e.Define("G", "D");
				e.Define("B", "I");
			}

			if ((set & DefinitionSet.Set2) != 0)
			{
				e.DefineSelfLoops(VertexSymbolsA2J);
			}

			if ((set & DefinitionSet.Set3) != 0)
			{
				e["Q"].DefineTo(VertexSymbolsA2J.Take(7)); //< A-G
				e["R"].DefineTo(VertexSymbolsA2J.Take(8)); //< A-H
				e["S"].DefineTo(VertexSymbolsA2J.Take(9)); //< A-I
			}

			if ((set & DefinitionSet.Set4) != 0)
			{
				e["U"].DefineFrom(VertexSymbolsA2J);
			}

			if ((set & DefinitionSet.Set5) != 0)
			{
				e.Define("X", "Y");
				e.Define("Y", "Z");
				e.Define("Z", "X");
			}

			if ((set & DefinitionSet.Set6) != 0)
			{
				e.Define("Y", "X");
				e.Define("Z", "Y");
				e.Define("X", "Z");
			}

			if ((set & DefinitionSet.Set7) != 0)
			{
				e["T"].DefineSelfLoop();
			}

			if ((set & DefinitionSet.Set8) != 0)
			{
				e.Define("J", "A");
			}

			graph.AddEdgeRange(e.AdjacentVerticesPairs, getEdgeDataCallback, addEdgePredicate);
		}

		// ReSharper disable InconsistentNaming
		/// <summary>
		///     <see cref="Set0" />
		///     J ← I ← H ← G ← F
		///     _               ↑
		///     A → B → C → D → E
		///     <see cref="Set1" />
		///     I       G   F → N
		///     ↑       ↓   ↑
		///     B       D   E → M
		///     <see cref="Set1" /> + <see cref="Set0" />
		///     J ← I ← H ← G ← F → N
		///     _   ↑       ↓   ↑
		///     A → B → C → D → E → M
		///     <see cref="Set2" />
		///     A ↔ A
		///     B ↔ B
		///     ...
		///     J ↔ J
		///     <see cref="Set3" />
		///     Q → : A, B, C, D, E, F, G
		///     R → : A, B, C, D, E, F, G, H
		///     S → : A, B, C, D, E, F, G, H, I
		///     <see cref="Set4" />
		///     A, B, C, D, E, F, G, H, I, J: → U
		///     <see cref="Set5" />
		///     X → Y
		///     Y → Z
		///     Z → X
		///     <see cref="Set6" />
		///     X ← Y
		///     Y ← Z
		///     Z ← X
		///     <see cref="Set7" />
		///     T ↔ T
		///     <see cref="Set8" />
		///     J → A
		/// </summary>
		[Flags]
		public enum DefinitionSet
		{
			None = 0,

			/// <summary>
			///     J ← I ← H ← G ← F
			///     _               ↑
			///     A → B → C → D → E
			/// </summary>
			Set0 = 0x01,

			/// <summary>
			///     I       G   F → N
			///     ↑       ↓   ↑
			///     B       D   E → M
			/// </summary>
			Set1 = 0x02,

			/// <summary>
			///		A ↔ A
			///     B ↔ B
			///     ...
			///     J ↔ J
			/// </summary>
			Set2 = 0x04,

			/// <summary>
			///		Q → : A, B, C, D, E, F, G
			///     R → : A, B, C, D, E, F, G, H
			///     S → : A, B, C, D, E, F, G, H, I
			/// </summary>
			Set3 = 0x08,

			/// <summary>A, B, C, D, E, F, G, H, I, J: → U</summary>
			Set4 = 0x10,

			/// <summary>
			///		X → Y
			///     Y → Z
			///     Z → X
			/// </summary>
			Set5 = 0x20,

			/// <summary>
			///		X ← Y
			///     Y ← Z
			///     Z ← X
			/// </summary>
			Set6 = 0x40,

			/// <summary>T ↔ T</summary>
			Set7 = 0x80,

			/// <summary>J → A</summary>
			Set8 = 0x100,

			All = Set0 | Set1 | Set2 | Set3 | Set4 | Set5 | Set6 | Set7 | Set8,

			PathA2J = Set0,
			EMFNBIGD = Set1,
			SelfA2J = Set2,
			PathsFromQRS = Set3,
			AJ2U = Set4,
			PathXYZ = Set5,
			PathZYX = Set6,
			SelfT = Set7,
			JA = Set8
		}

		public static readonly string[] VertexSymbolsA2J = "ABCDEFGHIJ".AsStrings().ToArray();
		public static readonly string[] VertexSymbolsXYZ = "XYZ".AsStrings().ToArray();
		public static readonly string[] VertexSymbolsQRS = "QRS".AsStrings().ToArray();

		// ReSharper restore InconsistentNaming
	}
}