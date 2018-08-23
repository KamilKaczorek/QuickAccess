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
using System.Diagnostics.Contracts;

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     The interface of read only graph.
	///     It contains connectivity definition provided in an index form.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <seealso cref="IReadOnlyGraph{TEdgeData, TSymbol}" />
	public interface IReadOnlyGraph<TEdgeData>
	{
		/// <summary>
		///   Occurs when graph connectivity changed.
		/// <remarks>
		/// An event handler is added only if graph is not read only (or is not frozen).
		/// When the graph is read only (or frozen) then adding of event handler to this event is ignored.
		/// </remarks>
		/// </summary>
		/// <seealso cref="GraphConnectivityChangeType"/>
		/// <seealso cref="GraphConnectivityChangedEventArgs"/>
		event EventHandler<GraphConnectivityChangedEventArgs> ConnectivityChanged;

		/// <summary>Gets a value indicating whether this instance is read only.</summary>
		/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
		[Pure]
		bool IsReadOnly { get; }

		/// <summary>
		///     Gets the graph connectivity definition.
		/// </summary>
		/// <value>
		///     The connectivity definition.
		/// </value>
		[Pure]
		GraphConnectivityDefinition<TEdgeData> Connectivity { get; }
	}

	/// <summary>
	///     The interface of read only symbol graph (where vertices are provided as symbols).
	///     It contains connectivity definition provided in an index form.
	/// </summary>
	/// <typeparam name="TEdgeData">The type of the edge data.</typeparam>
	/// <typeparam name="TSymbol">The type of the vertex symbol.</typeparam>
	/// <seealso cref="IReadOnlyGraph{TEdgeData}" />
	public interface IReadOnlyGraph<TEdgeData, TSymbol> : IReadOnlyGraph<TEdgeData>
	{
		/// <summary>
		///     Gets the vertex symbol to index converter  that allows to translate vertex symbol into vertex index and vice versa.
		///     <seealso cref="ISymbolToIndexReadOnlyConverter{TSymbol}" />
		/// </summary>
		/// <value>
		///     The symbol to index converter.
		/// </value>
		[Pure]
		ISymbolToIndexReadOnlyConverter<TSymbol> SymbolToIndexConverter { get; }
	}
}