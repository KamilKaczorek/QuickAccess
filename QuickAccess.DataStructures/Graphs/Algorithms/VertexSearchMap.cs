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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Graphs.Model;

namespace QuickAccess.DataStructures.Graphs.Algorithms
{
	/// <summary>
	/// The vertex search map that is result of graph search algorithm.
	/// <seealso cref="IGraphSearch"/>
	/// </summary>
	public static class VertexSearchMap
	{
		/// <summary>
		///     Creates the new instance of the <see cref="VertexSearchMap{TKey}" /> based on specified map given as a
		///     dictionary.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="sourceIndex">Index of the source (start) vertex.</param>
		/// <param name="destToSourceMap">The source by destination map.</param>
		/// <param name="cloneDictionary">
		///     if set to <c>true</c> dictionary will be cloned (shallow copy); otherwise, <c>false</c>,
		///     it will use given reference.
		/// </param>
		/// <returns>New instance of search map.</returns>
		public static VertexSearchMap<TKey> Create<TKey>(TKey sourceIndex,
		                                                 Dictionary<TKey, TKey> destToSourceMap,
		                                                 bool cloneDictionary = false)
		{
			return new VertexSearchMap<TKey>(sourceIndex,
				cloneDictionary ? destToSourceMap.ToDictionary(p => p.Key, p => p.Value) : destToSourceMap);
		}

		/// <summary>
		///     Creates the new instance of the <see cref="VertexSearchMap{TKey}" /> based on specified map given as
		///     destination-source pairs.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="sourceIndex">Index of the source (start) vertex.</param>
		/// <param name="destToSourceMap">The destination-source pairs.</param>
		/// <returns>New instance of search map.</returns>
		public static VertexSearchMap<TKey> Create<TKey>(TKey sourceIndex, IEnumerable<KeyValuePair<TKey, TKey>> destToSourceMap)
		{
			return new VertexSearchMap<TKey>(sourceIndex, destToSourceMap.ToDictionary(p => p.Key, p => p.Value));
		}

		/// <summary>Creates the empty vertex search map.</summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <param name="srcIndex">Index of the source (start) vertex.</param>
		/// <returns>New empty instance of search map.</returns>
		public static VertexSearchMap<TKey> CreateEmpty<TKey>(TKey srcIndex)
		{
			return new VertexSearchMap<TKey>(srcIndex, null);
		}

		/// <summary>Converts existing index based map to the symbol based search map.</summary>
		/// <typeparam name="TSymbol">The type of the vertex symbol.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="symbolToIndexConverter">The symbol to index converter.</param>
		/// <returns>Symbol based vertex search map.</returns>
		public static VertexSearchMap<TSymbol> ConvertToSymbolSearchMap<TSymbol>(this VertexSearchMap<int> source,
		                                                                         ISymbolToIndexReadOnlyConverter<TSymbol>
			                                                                         symbolToIndexConverter)
		{
			return new VertexSearchMap<TSymbol>(symbolToIndexConverter.GetSymbolAt(source.SourceVertex),
				source.ToDictionary(v => symbolToIndexConverter.GetSymbolAt(v.Destination),
					v => symbolToIndexConverter.GetSymbolAt(v.Source)));
		}

		/// <summary>Converts symbol based search map to the index based search map.</summary>
		/// <typeparam name="TSymbol">The type of the vertex symbol.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="symbolToIndexConverter">The symbol index map.</param>
		/// <returns>The index based search map</returns>
		public static VertexSearchMap<int> ConvertToIndexSearchMap<TSymbol>(this VertexSearchMap<TSymbol> source,
		                                                                    ISymbolToIndexReadOnlyConverter<TSymbol>
			                                                                    symbolToIndexConverter)
		{
			return new VertexSearchMap<int>(symbolToIndexConverter.GetIndexOf(source.SourceVertex),
				source.ToDictionary(v => symbolToIndexConverter.GetIndexOf(v.Destination),
					v => symbolToIndexConverter.GetIndexOf(v.Source)));
		}

		/// <summary>
		///     Gets the regular path from the <see cref="VertexSearchMap{TKey}.SourceVertex" /> to the given
		///     <paramref name="destinationVertex" />.
		///     <remarks>
		///         Getting of reversed path is more optimal than getting of regular path.
		///         From performance perspective is recommended to use <see cref="VertexSearchMap{TKey}.GetReversedPathTo" />
		///         method instead.
		///     </remarks>
		/// </summary>
		/// <typeparam name="TKey">The type of the vertex key (index/symbol).</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="destinationVertex">The destination vertex.</param>
		/// <returns>Regular path from source to destination vertex represented as sequence of vertices.</returns>
		public static IEnumerable<TKey> GetPathTo<TKey>(this VertexSearchMap<TKey> source, TKey destinationVertex)
		{
			return source.GetReversedPathTo(destinationVertex).Reverse();
		}
	}

	/// <summary>
	///     The graph search map for specified source vertex.
	///     It exposes operations that returns path to the given destination vertex.
	///     <seealso cref="BreadthFirstSearch" />
	/// </summary>
	/// <typeparam name="TKey">The type of the vertex key (index/symbol).</typeparam>
	public struct VertexSearchMap<TKey> : IEnumerable<VerticesPair<TKey>>
	{
		private readonly Dictionary<TKey, TKey> _destToSourceMap;

		/// <summary>Gets the source vertex (starting position).</summary>
		/// <value>The source vertex.</value>
		[Pure]
		public TKey SourceVertex { get; }

		/// <summary>Initializes a new instance of the <see cref="VertexSearchMap{TKey}" /> structure.</summary>
		/// <param name="sourceVertex">The source vertex (starting position).</param>
		/// <param name="destToSourceMap">The destination to source map.</param>
		public VertexSearchMap(TKey sourceVertex, Dictionary<TKey, TKey> destToSourceMap)
		{
			SourceVertex = sourceVertex;
			_destToSourceMap = destToSourceMap;
		}

		/// <summary>
		///     Determines whether the map is empty (<see cref="SourceVertex" /> doesn't contain any adjacent vertices or is
		///     not defined.
		/// </summary>
		/// <returns><c>true</c> if this map is empty; otherwise, <c>false</c>.</returns>
		[Pure]
		public bool IsEmpty => _destToSourceMap == null || _destToSourceMap.Count <= 0;

		/// <summary>
		///     Determines whether the graph contains the path from the <see cref="SourceVertex" /> to the given
		///     <paramref name="destinationVertex" />.
		/// </summary>
		/// <param name="destinationVertex">The destination vertex.</param>
		/// <returns><c>true</c> if it has path to the specified destination; otherwise, <c>false</c>.</returns>
		[Pure]
		public bool HasPathTo(TKey destinationVertex)
		{
			return _destToSourceMap?.ContainsKey(destinationVertex) ?? false;
		}

		/// <summary>Gets the indexes of all vertices that the <see cref="SourceVertex" /> has path to.</summary>
		/// <value>The destination vertices.</value>
		[Pure]
		public IEnumerable<TKey> DestVertices => EnumerableExtensions.EnumerateNotNullOrReturnEmpty(_destToSourceMap?.Keys);

		/// <summary>Determines whether the map contains specified regular path.</summary>
		/// <param name="path">The path.</param>
		/// <returns>
		///   <c>true</c> if the map contains specified path; otherwise, <c>false</c>.</returns>
		[Pure]
		public bool ContainsPath(IEnumerable<TKey> path)
		{
			var count = 0;
			TKey expectedSourceVertex = default;
			foreach (var destVertex in path)
			{
				++count;
				if (count == 1)
				{
					expectedSourceVertex = destVertex;
					continue;
				}

				if (!_destToSourceMap.TryGetValue(destVertex, out var sourceVertex))
				{
					return false;
				}

				if (!_destToSourceMap.Comparer.Equals(expectedSourceVertex, sourceVertex))
				{
					return false;
				}

				expectedSourceVertex = destVertex;
			}

			return count != 1;
		}

		/// <summary>Determines whether the map contains specified reversed path.</summary>
		/// <param name="reversedPath">The reversed path.</param>
		/// <returns>
		///   <c>true</c> if the map contains specified reversed path; otherwise, <c>false</c>.</returns>
		[Pure]
		public bool ContainsReversedPath(IEnumerable<TKey> reversedPath)
		{
			var count = 0;
			TKey sourceVertex = default;
			foreach (var destVertex in reversedPath)
			{
				if (count < 0)
				{
					return false;
				}

				++count;

				if (count > 1 && !_destToSourceMap.Comparer.Equals(destVertex, sourceVertex))
				{
					return false;
				}

				if (!_destToSourceMap.TryGetValue(destVertex, out sourceVertex))
				{
					count = -1;
				}							
			}

			return count != 1;
		}

		/// <summary>
		///     Gets the reversed (starting from destination vertex) path from the <see cref="SourceVertex" /> to the
		///     <paramref name="destinationVertex" />.
		///     Getting of reversed path is more optimal than getting of regular path from source to destination.
		/// </summary>
		/// <param name="destinationVertex">The destination vertex.</param>
		/// <returns>Reversed path from source to destination vertex represented as sequence of vertices.</returns>
		/// <seealso cref="VertexSearchMap.GetPathTo{TKey}" />
		[Pure]
		public IEnumerable<TKey> GetReversedPathTo(TKey destinationVertex)
		{
			if (_destToSourceMap == null || !_destToSourceMap.TryGetValue(destinationVertex, out var src))
			{
				yield break;
			}

			yield return destinationVertex;
			yield return src;

			var cmp = Comparer;
			while (!cmp.Equals(src, SourceVertex))
			{
				src = _destToSourceMap[src];
				yield return src;
			}
		}

		/// <summary>Gets the vertex key comparer.</summary>
		/// <value>The comparer.</value>
		[Pure]
		public IEqualityComparer<TKey> Comparer => _destToSourceMap?.Comparer ?? EqualityComparer<TKey>.Default;

		/// <summary>Returns an enumerator that iterates through all edges of the map.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		[Pure]
		public IEnumerator<VerticesPair<TKey>> GetEnumerator()
		{
			return EnumerableExtensions.EnumerateNotNullOrReturnEmpty(_destToSourceMap)
			                           .Select(k => new VerticesPair<TKey>(k.Value, k.Key))
			                           .GetEnumerator();
		}

		/// <inheritdoc />
		[Pure]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}