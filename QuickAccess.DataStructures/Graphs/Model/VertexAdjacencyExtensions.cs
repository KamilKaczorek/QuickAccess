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

using System;
using System.Collections.Generic;
using System.Linq;
using QuickAccess.DataStructures.Common;

namespace QuickAccess.DataStructures.Graphs.Model
{

	/// <summary>
	/// Extension methods of <see cref="VertexAdjacency{TEdgeData}"/>.
	/// <seealso cref="VertexAdjacency{TEdgeData}"/>
	/// </summary>
	public static class VertexAdjacencyExtensions
	{
		/// <summary>Determines whether the source vertex has empty edge data (of <see cref="EmptyValue"/> type).</summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The source.</param>
		/// <returns><c>true</c> if the edge data is of <see cref="EmptyValue"/>; otherwise, <c>false</c>.</returns>
		public static bool HasEmptyEdgeData<TEdgeData>(this VertexAdjacency<TEdgeData> source)
		{
			return EmptyValue.IsEmptyValueType<TEdgeData>();
		}

		/// <summary>
		/// Determines whether the specified vertices are equal.
		/// The vertices are equal if references are equal or if both vertices have the same number of adjacent edges and the second vertex
		/// contains all edges of the first one.
		/// Two edges are equal if destination vertices are equal and execution of <see cref=" IEqualityComparer{T}.Equals(T,T)" />
		/// method for edges data of <paramref name="edgeDataComparer" /> returns <c>true</c>.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="source">The first vertex to compare.</param>
		/// <param name="other">The second vertex to compare.</param>
		/// <param name="edgeDataComparer">The edge data comparer.</param>
		/// <returns><c>true</c> if the specified vertices are equal; otherwise, <c>false</c>.</returns>
		public static bool IsEquivalent<TEdgeData>(this VertexAdjacency<TEdgeData> source,
		                                           VertexAdjacency<TEdgeData> other,
		                                           IEqualityComparer<TEdgeData> edgeDataComparer = null)
		{
			if (ReferenceEquals(source, other))
			{
				return true;
			}

			if (source == null || other == null)
			{
				return false;
			}

			if (source.EdgesCount != other.EdgesCount)
			{
				return false;
			}

			if (source.HasEmptyEdgeData())
			{
				return source.AdjacentIndexes.All(other.ContainsEdgeToIndex);
			}

			edgeDataComparer = edgeDataComparer ?? EqualityComparer<TEdgeData>.Default;
			foreach (var edgeTo in source)
			{
				if (!other.TryGetEdgeToIndex(edgeTo.Destination, out var edgeData) || !edgeDataComparer.Equals(edgeData, edgeTo.Data))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		///     Enumerates vertices to the last not empty vertex (trims the last empty vertices).
		///     Replaces all empty (in-between) vertices by the <paramref name="empty" /> instance.
		/// </summary>
		/// <typeparam name="T">The type of the vertex.</typeparam>
		/// <param name="source">The source of an extension of <see cref="IEnumerable{IVertex}" /> interface.</param>
		/// <param name="empty">The empty vertex.</param>
		/// <returns>
		///     Sequence of vertices without last empty ones.
		/// </returns>
		public static IEnumerable<VertexAdjacency<T>> TrimEmptyEnd<T>(this IEnumerable<VertexAdjacency<T>> source,
		                                                              VertexAdjacency<T> empty)
		{
			var emptyCount = 0;
			foreach (var item in source)
			{
				if (item.Count <= 0)
				{
					emptyCount++;
				}
				else
				{
					for (var idx = 0; idx < emptyCount; idx++)
					{
						yield return empty;
					}

					emptyCount = 0;

					yield return item;
				}
			}
		}

		/// <summary>
		///     Enumerates vertices to the last not empty vertex (trims the last empty vertices).
		///     Replaces all empty (in-between) vertices by the instances provided by the
		///     <paramref name="createEmptyForIdxCallback" />
		///     .
		/// </summary>
		/// <typeparam name="T">The type of the vertex.</typeparam>
		/// <param name="source">The source of an extension of <see cref="IEnumerable{IVertex}" /> interface.</param>
		/// <param name="createEmptyForIdxCallback">The empty instance factory callback.</param>
		/// <returns>
		///     Sequence of vertices without last empty ones.
		/// </returns>
		public static IEnumerable<VertexAdjacency<T>> TrimEmptyEnd<T>(this IEnumerable<VertexAdjacency<T>> source,
		                                                              Func<int, VertexAdjacency<T>> createEmptyForIdxCallback)
		{
			var emptyCount = 0;
			var index = 0;
			foreach (var item in source)
			{
				if (item.Count <= 0)
				{
					emptyCount++;
				}
				else
				{
					for (var idx = 0; idx < emptyCount; idx++)
					{
						yield return createEmptyForIdxCallback.Invoke(index);
					}

					emptyCount = 0;

					yield return item;
				}

				index++;
			}
		}
	}
}