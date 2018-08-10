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
using System.Linq;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	/// The static factory of <seealso cref="GraphEdgeSequenceAssembler{TVertexKey}"/>.
	/// </summary>
	public static class GraphEdgesSequenceAssembler
	{
		/// <summary>Creates the new instance of the <see cref="GraphEdgeSequenceAssembler{TVertexKey}"/>.</summary>
		/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
		/// <param name="capacity">The edges capacity.</param>
		/// <returns>The graph edge sequence assembler instance.</returns>
		public static GraphEdgeSequenceAssembler<TVertexKey> Create<TVertexKey>(int capacity = 0)
		{
			return new GraphEdgeSequenceAssembler<TVertexKey>(capacity);
		}

		/// <summary>Creates the new instance of the <see cref="GraphEdgeSequenceAssembler{TVertexKey}" />.</summary>
		/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
		/// <param name="edges">The edges.</param>
		/// <returns>The graph edge sequence assembler instance.</returns>
		public static GraphEdgeSequenceAssembler<TVertexKey> Create<TVertexKey>(IEnumerable<VerticesPair<TVertexKey>> edges)
		{
			return Wrap(edges.ToList());
		}

		/// <summary>Creates the new instance of the <see cref="GraphEdgeSequenceAssembler{TVertexKey}"/> wrapping the specified list.</summary>
		/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
		/// <param name="wrappedList">The wrapped list.</param>
		/// <returns>The graph edge sequence assembler instance.</returns>
		public static GraphEdgeSequenceAssembler<TVertexKey> Wrap<TVertexKey>(List<VerticesPair<TVertexKey>> wrappedList)
		{
			return new GraphEdgeSequenceAssembler<TVertexKey>(wrappedList);
		}
	}


	/// <summary>
	/// Allows to configure connectivity aspect of graph edges (without edge data).
	/// <seealso cref="GraphEdgesSequenceAssembler"/>
	/// </summary>
	/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
	public sealed class GraphEdgeSequenceAssembler<TVertexKey>
	{
		private readonly List<VerticesPair<TVertexKey>> _pairs;

		/// <summary>Gets the configured adjacent vertices pairs.</summary>
		/// <value>The adjacent vertices pairs.</value>
		public IReadOnlyList<VerticesPair<TVertexKey>> AdjacentVerticesPairs => _pairs;

		/// <summary>Gets the <see cref="VertexConfig"/> with at the specified vertex key.</summary>
		/// <value>The <see cref="VertexConfig"/>.</value>
		/// <param name="vertexKey">The key.</param>
		/// <returns></returns>
		public VertexConfig this[TVertexKey vertexKey] => ConfigureVertex(vertexKey);

		/// <summary>Initializes a new instance of the <see cref="GraphEdgeSequenceAssembler{TVertexKey}"/> class.</summary>
		/// <param name="capacity">The capacity.</param>
		public GraphEdgeSequenceAssembler(int capacity = 0)
		{
			_pairs = new List<VerticesPair<TVertexKey>>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GraphEdgeSequenceAssembler{TVertexKey}" /> class that wraps the given list.
		/// </summary>
		/// <param name="wrappedList">The wrapped list.</param>
		public GraphEdgeSequenceAssembler(List<VerticesPair<TVertexKey>> wrappedList)
		{
			_pairs = wrappedList;
		}

		/// <summary>Configures the vertex specified by the given vertex key.</summary>
		/// <param name="vertex">The vertex key.</param>
		/// <returns>The vertex configurator.</returns>
		public VertexConfig ConfigureVertex(TVertexKey vertex)
		{
			return new VertexConfig(_pairs, vertex);
		}

		/// <summary>Defines the edge between specified vertices.</summary>
		/// <param name="src">The source vertex.</param>
		/// <param name="dst">The destination vertex.</param>
		/// <returns><c>this</c> for the fluent API.</returns>
		public GraphEdgeSequenceAssembler<TVertexKey> Define(TVertexKey src, TVertexKey dst)
		{
			_pairs.Add(new VerticesPair<TVertexKey>(src, dst));
			return this;
		}

		/// <summary>Defines the self edges for the specified vertices.</summary>
		/// <param name="vertices">The vertices.</param>
		/// <returns><c>this</c> for the fluent API.</returns>
		public GraphEdgeSequenceAssembler<TVertexKey> DefineSelfEdges(IEnumerable<TVertexKey> vertices)
		{
			foreach (var vertex in vertices)
			{
				_pairs.Add(new VerticesPair<TVertexKey>(vertex, vertex));
			}

			return this;
		}

		/// <summary>Defines the self edges for the specified vertices.</summary>
		/// <param name="vertices">The vertices.</param>
		/// <returns><c>this</c> for the fluent API.</returns>
		public GraphEdgeSequenceAssembler<TVertexKey> DefineSelfEdges(params TVertexKey[] vertices)
		{
			foreach (var vertex in vertices)
			{
				_pairs.Add(new VerticesPair<TVertexKey>(vertex, vertex));
			}

			return this;
		}

		/// <summary>
		/// The configurator of vertex edges.
		/// </summary>
		public struct VertexConfig
		{
			private readonly List<VerticesPair<TVertexKey>> _pairs;
			private readonly TVertexKey _vertex;

			public VertexConfig(List<VerticesPair<TVertexKey>> pairs, TVertexKey vertex)
			{
				_pairs = pairs;
				_vertex = vertex;
			}

			public VertexConfig DefineTo(TVertexKey dst)
			{
				_pairs.Add(new VerticesPair<TVertexKey>(_vertex, dst));
				return this;
			}

			public VertexConfig DefineTo(params TVertexKey[] dsts)
			{
				foreach (var dst in dsts)
				{
					_pairs.Add(new VerticesPair<TVertexKey>(_vertex, dst));
				}

				return this;
			}

			public VertexConfig DefineTo(IEnumerable<TVertexKey> dsts)
			{
				foreach (var dst in dsts)
				{
					_pairs.Add(new VerticesPair<TVertexKey>(_vertex, dst));
				}

				return this;
			}

			public VertexConfig DefineFrom(TVertexKey src)
			{
				_pairs.Add(new VerticesPair<TVertexKey>(src, _vertex));
				return this;
			}

			public VertexConfig DefineFrom(params TVertexKey[] srcs)
			{
				foreach (var src in srcs)
				{
					_pairs.Add(new VerticesPair<TVertexKey>(src, _vertex));
				}

				return this;
			}

			public VertexConfig DefineFrom(IEnumerable<TVertexKey> srcs)
			{
				foreach (var src in srcs)
				{
					_pairs.Add(new VerticesPair<TVertexKey>(src, _vertex));
				}

				return this;
			}

			public VertexConfig DefineToSelf()
			{
				_pairs.Add(new VerticesPair<TVertexKey>(_vertex, _vertex));
				return this;
			}
		}
	}
}