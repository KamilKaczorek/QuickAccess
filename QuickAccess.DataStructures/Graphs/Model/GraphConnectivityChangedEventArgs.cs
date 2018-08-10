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

namespace QuickAccess.DataStructures.Graphs.Model
{
	/// <summary>
	///     The arguments of <see cref="IReadOnlyGraph{TEdgeData}.ConnectivityChanged" /> event.
	/// </summary>
	/// <seealso cref="EventArgs" />
	public sealed class GraphConnectivityChangedEventArgs : EventArgs
	{
		/// <summary>Gets the type of the change operation.</summary>
		/// <value>The type of the change.</value>
		public GraphConnectivityChangeType ChangeType { get; }

		/// <summary>Gets the source vertex index or first index of added vertex.</summary>
		/// <value>The source vertex index.</value>
		public int Source { get; }

		/// <summary>Gets the destination vertex index or last index of added vertex..</summary>
		/// <value>The destination vertex index.</value>
		public int Destination { get; }

		/// <summary>Gets the type of the removed edge operation.</summary>
		/// <value>The type of the removed edge operation.</value>
		public RemovedEdgeOperationType RemovedEdgeOperationType =>
			ChangeType != GraphConnectivityChangeType.EdgeRemoved
				? RemovedEdgeOperationType.None
				: Source < 0
					? RemovedEdgeOperationType.AllToVertex
					: Destination < 0
						? RemovedEdgeOperationType.AllFromVertex
						: RemovedEdgeOperationType.Single;

		/// <summary>Gets the number of added vertices.</summary>
		/// <value>
		///     The number of added vertices if the operation is of <see cref="GraphConnectivityChangeType.VertexAdded" /> type;
		///     otherwise, <c>0</c>.
		/// </value>
		public int AddedVerticesCount
		{
			get
			{
				if (ChangeType != GraphConnectivityChangeType.VertexAdded)
				{
					return 0;
				}

				return Destination - Source + 1;
			}
		}

		/// <summary>Gets the number of added vertices.</summary>
		/// <value>
		///     The number of added vertices if the operation is of <see cref="GraphConnectivityChangeType.VertexRemoved" />
		///     type; otherwise, <c>0</c>.
		/// </value>
		public int RemovedVerticesCount
		{
			get
			{
				if (ChangeType != GraphConnectivityChangeType.VertexRemoved)
				{
					return 0;
				}

				return Destination - Source + 1;
			}
		}

		private GraphConnectivityChangedEventArgs(GraphConnectivityChangeType changeType, int source = -1, int destination = -1)
		{
			ChangeType = changeType;
			Source = source;
			Destination = destination;
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.Frozen" /> operation
		///     type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateFrozen()
		{
			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.Frozen);
		}

		/// <summary>Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.Reset" /> operation type.</summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateReset()
		{
			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.Reset);
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.EdgeAdded" /> operation
		///     type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateEdgeAdded(int srcVertex, int destVertex)
		{
			if (srcVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(srcVertex));
			}

			if (destVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(destVertex));
			}

			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.EdgeAdded, srcVertex, destVertex);
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.EdgeRemoved" /> operation
		///     type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateEdgeRemoved(int srcVertex, int destVertex)
		{
			if (srcVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(srcVertex));
			}

			if (destVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(destVertex));
			}

			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.EdgeRemoved, srcVertex, destVertex);
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.EdgeDataChanged" />
		///     operation type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateEdgeDataChanged(int srcVertex, int destVertex)
		{
			if (srcVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(srcVertex));
			}

			if (destVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(destVertex));
			}

			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.EdgeDataChanged, srcVertex, destVertex);
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.EdgeRemoved" /> operation
		///     type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateAllEdgesFromRemoved(int srcVertex)
		{
			if (srcVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(srcVertex));
			}

			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.EdgeRemoved, srcVertex);
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.EdgeRemoved" /> operation
		///     type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateAllEdgesToRemoved(int destVertex)
		{
			if (destVertex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(destVertex));
			}

			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.EdgeRemoved, -1, destVertex);
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.VertexAdded" /> operation
		///     type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateVertexAdded(int firstVertexIndex, int lastVertexIndex)
		{
			if (firstVertexIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(firstVertexIndex));
			}

			if (lastVertexIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(lastVertexIndex));
			}

			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.VertexAdded, firstVertexIndex, lastVertexIndex);
		}

		/// <summary>
		///     Creates the new instance of arguments for the <see cref="GraphConnectivityChangeType.VertexRemoved" />
		///     operation type.
		/// </summary>
		/// <returns>The new instance.</returns>
		public static GraphConnectivityChangedEventArgs CreateVertexRemoved(int firstVertexIndex, int lastVertexIndex)
		{
			if (firstVertexIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(firstVertexIndex));
			}

			if (lastVertexIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(lastVertexIndex));
			}

			return new GraphConnectivityChangedEventArgs(GraphConnectivityChangeType.VertexRemoved, firstVertexIndex, lastVertexIndex);
		}

		/// <summary>
		///     Determines whether the change is related with vertex at the specified position.
		/// </summary>
		/// <param name="vertexIndex">Index of the vertex.</param>
		/// <returns><c>true</c> if the vertex specified position was affected by the operation; otherwise, <c>false</c>.</returns>
		public bool IsRelatedWithVertexAt(int vertexIndex)
		{
			switch (ChangeType)
			{
				case GraphConnectivityChangeType.Frozen:
					return false;
				case GraphConnectivityChangeType.Reset:
					return true;
				case GraphConnectivityChangeType.EdgeDataChanged:
				case GraphConnectivityChangeType.EdgeAdded:
					return vertexIndex == Source || vertexIndex == Destination;
				case GraphConnectivityChangeType.EdgeRemoved:
					if (Source >= 0 && Destination >= 0)
					{
						return vertexIndex == Source || vertexIndex == Destination;
					}

					return Source < 0 ? vertexIndex == Destination : vertexIndex == Source;
				case GraphConnectivityChangeType.VertexAdded:
				case GraphConnectivityChangeType.VertexRemoved:
					return IsAffectedByVertexCountChanged(vertexIndex) || IsAffectedByVertexCountChanged(vertexIndex);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		///     Determines whether the specified edge is affected by the change.
		/// </summary>
		/// <param name="srcVertexIndex">Index of the source vertex.</param>
		/// <param name="dstVertexIndex">Index of the destination vertex.</param>
		/// <returns><c>true</c> if the edge was affected by the operation; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public bool IsEdgeAffected(int srcVertexIndex, int dstVertexIndex)
		{
			switch (ChangeType)
			{
				case GraphConnectivityChangeType.Frozen:
					return false;
				case GraphConnectivityChangeType.Reset:
					return true;
				case GraphConnectivityChangeType.EdgeDataChanged:
				case GraphConnectivityChangeType.EdgeAdded:
					return srcVertexIndex == Source && dstVertexIndex == Destination;
				case GraphConnectivityChangeType.EdgeRemoved:
					if (Source >= 0 && Destination >= 0)
					{
						return srcVertexIndex == Source && dstVertexIndex == Destination;
					}

					return Source < 0 ? dstVertexIndex == Destination : srcVertexIndex == Source;
				case GraphConnectivityChangeType.VertexAdded:
				case GraphConnectivityChangeType.VertexRemoved:
					return IsAffectedByVertexCountChanged(srcVertexIndex) || IsAffectedByVertexCountChanged(dstVertexIndex);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private bool IsAffectedByVertexCountChanged(int vertexIndex)
		{
			return vertexIndex >= Source && vertexIndex <= Destination;
		}
	}
}