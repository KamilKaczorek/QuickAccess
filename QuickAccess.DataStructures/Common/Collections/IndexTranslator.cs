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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System;

namespace QuickAccess.DataStructures.Common.Collections
{
	/// <summary>
	///     The index translator.
	///     It is used as re-indexing operation result.
	///     It provides map between an old and new indexes of items.
	///     Useful in sorting operation when the index information of an item is relevant.
	///     <seealso cref="EnumerableExtensions.SortWithReindexingResult{T}(T[],System.Collections.Generic.IComparer{T})" />
	/// </summary>
	public sealed class IndexTranslator : IIndexTranslator
	{
		private readonly int[] _destBySrcIndexMap;
		private readonly int[] _srcByDestIndexMap;

		/// <summary>Initializes a new instance of the <see cref="IndexTranslator" /> class.</summary>
		/// <param name="srcByDestIndex">Source index by destination index map.</param>
		/// <param name="destBySrcIndexMap">Destination index by source index map.</param>
		public IndexTranslator(int[] srcByDestIndex, int[] destBySrcIndexMap)
		{
			_srcByDestIndexMap = srcByDestIndex;
			_destBySrcIndexMap = destBySrcIndexMap;
		}

		/// <summary>Initializes a new instance of the <see cref="IndexTranslator" /> class.</summary>
		/// <param name="srcByDestIndex">Source index by destination index map.</param>
		public IndexTranslator(int[] srcByDestIndex)
		{
			var len = srcByDestIndex.Length;
			_srcByDestIndexMap = srcByDestIndex;
			_destBySrcIndexMap = new int[len];

			for (var destIndex = 0; destIndex < len; destIndex++)
			{
				_destBySrcIndexMap[_srcByDestIndexMap[destIndex]] = destIndex;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="IndexTranslator" /> class.</summary>
		/// <param name="count">The number of items.</param>
		/// <param name="getSrcIndexCallback">The callback to get source index for specified destination index.</param>
		public IndexTranslator(int count, Func<int, int> getSrcIndexCallback)
		{
			_srcByDestIndexMap = new int[count];
			_destBySrcIndexMap = new int[count];

			for (var destIndex = 0; destIndex < count; destIndex++)
			{
				var srcIndex = getSrcIndexCallback(destIndex);
				_srcByDestIndexMap[destIndex] = srcIndex;
				_destBySrcIndexMap[srcIndex] = destIndex;
			}
		}

		/// <inheritdoc />
		public int ItemsCount => _srcByDestIndexMap.Length;

		/// <inheritdoc />
		public int GetDestIndex(int sourceIndex)
		{
			return _destBySrcIndexMap[sourceIndex];
		}

		/// <inheritdoc />
		public int GetSrcIndex(int destIndex)
		{
			return _srcByDestIndexMap[destIndex];
		}		
	}
}