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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	/// Data structure that contains vertices (source, destination) pair.
	/// <seealso cref="Edge{TVertexKey,TEdgeData}"/>
	/// </summary>
	/// <typeparam name="TVertexKey">The type of the vertex key.</typeparam>
	[Serializable]
	public struct VerticesPair<TVertexKey> : IEquatable<VerticesPair<TVertexKey>>, IEnumerable<TVertexKey>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="VerticesPair{TVertexKey}" /> structure with specified source and destination vertices.
		/// </summary>
		/// <param name="source">The source vertex.</param>
		/// <param name="destination">The destination vertex.</param>
		public VerticesPair(TVertexKey source, TVertexKey destination)
		{
			Source = source;
			Destination = destination;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VerticesPair{TVertexKey}" /> structure copying values from <paramref name="other"/>.
		/// </summary>
		/// <param name="other">The other.</param>
		public VerticesPair(VerticesPair<TVertexKey> other)
		{
			Source = other.Source;
			Destination = other.Destination;
		}		

		/// <summary>
		/// Gets the source vertex key.
		/// </summary>
		/// <value>
		/// The source vertex key.
		/// </value>
		[Pure]
		public TVertexKey Source { get; }

		/// <summary>
		/// Gets the destination vertex key.
		/// </summary>
		/// <value>
		/// The destination vertex key.
		/// </value>
		[Pure]
		public TVertexKey Destination { get; }

		private static EqualityComparer<TVertexKey> Comparer => EqualityComparer<TVertexKey>.Default;

		/// <inheritdoc />
		[Pure]
		public bool Equals(VerticesPair<TVertexKey> other)
		{
			return Comparer.Equals(Source, other.Source) && Comparer.Equals(Destination, other.Destination);
		}

		/// <inheritdoc />
		[Pure]
		public IEnumerator<TVertexKey> GetEnumerator()
		{
			yield return Source;
			yield return Destination;
		}

		/// <inheritdoc />
		[Pure]
		public override bool Equals(object obj)
		{
			return obj is VerticesPair<TVertexKey> pair && Equals(pair);
		}

		/// <inheritdoc />
		[Pure]
		public override int GetHashCode()
		{
			unchecked
			{
				return (Comparer.GetHashCode(Source) * 397) ^ Comparer.GetHashCode(Destination);
			}
		}

		/// <inheritdoc />
		[Pure]
		IEnumerator IEnumerable.GetEnumerator()
		{
			yield return Source;
			yield return Destination;
		}

		/// <summary>
		/// Creates edge for the current vertices with specified value.
		/// </summary>
		/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
		/// <param name="edgeData">The edge data.</param>
		/// <returns>The edge.</returns>
		[Pure]
		public Edge<TVertexKey, TEdgeData> ToEdge<TEdgeData>(TEdgeData edgeData)
		{
			return new Edge<TVertexKey, TEdgeData>(this, edgeData);
		}

		/// <summary>
		/// Returns reversed vertices pair (with swapped <see cref="Source"/> and <see cref="Destination"/>).
		/// </summary>
		/// <returns>Reversed vertices pair.</returns>
		[Pure]
		public VerticesPair<TVertexKey> Reverse()
		{
			return new VerticesPair<TVertexKey>(Destination, Source);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{Source} → {Destination}";
		}

		
	}
}