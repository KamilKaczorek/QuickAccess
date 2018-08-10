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

namespace QuickAccess.DataStructures.Common
{
	/// <summary>
	///     The extension methods of the <see cref="IIndexTranslator" /> interface.
	/// </summary>
	public static class IndexTranslatorInterfaceExtensions
	{
		/// <summary>Converts specified source translator to the re-indexed data result.</summary>
		/// <typeparam name="TData">The type of the result data.</typeparam>
		/// <param name="source">The source of extension.</param>
		/// <param name="sequence">The sequence.</param>
		/// <returns>The result instance.</returns>
		[Pure]
		public static ReindexedDataResult<TData> ToReindexedDataResult<TData>(this IIndexTranslator source, TData sequence)
		{
			return new ReindexedDataResult<TData>(source, sequence);
		}

		/// <summary>Gets the source to destination index pairs.</summary>
		/// <param name="source">The source of extension.</param>
		/// <returns>Source to destination index pairs</returns>
		[Pure]
		public static IEnumerable<KeyValuePair<int, int>> GetSrcToDestIndexPairs(this IIndexTranslator source)
		{
			var count = source.ItemsCount;
			for (var srcIndex = 0; srcIndex < count; srcIndex++)
			{
				yield return new KeyValuePair<int, int>(srcIndex, source.GetDestIndex(srcIndex));
			}
		}

		/// <summary>Gets the destination to source index pairs.</summary>
		/// <param name="source">The source of extension.</param>
		/// <returns>The destination to source index pairs.</returns>
		[Pure]
		public static IEnumerable<KeyValuePair<int, int>> GetDestToSrcIndexPairs(this IIndexTranslator source)
		{
			var count = source.ItemsCount;
			for (var dstIndex = 0; dstIndex < count; dstIndex++)
			{
				yield return new KeyValuePair<int, int>(source.GetSrcIndex(dstIndex), dstIndex);
			}
		}

		/// <summary>Gets the items in a re-indexed sequence.</summary>
		/// <seealso cref="ApplyReindexing{T}"/>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="source">The source.</param>
		/// <param name="beforeReindexing">The sequence before reindexing.</param>
		/// <returns>The re-indexed sequence.</returns>
		[Pure]
		public static IEnumerable<T> GetReindexed<T>(this IIndexTranslator source, IReadOnlyList<T> beforeReindexing)
		{
			for (var dstIndex = 0; dstIndex < beforeReindexing.Count; ++dstIndex)
			{
				yield return beforeReindexing[source.GetSrcIndex(dstIndex)];
			}
		}


		/// <summary>Applies the reindexing specified by the given translator to the given list.</summary>
		/// <seealso cref="GetReindexed{T}"/>
		///  <remarks>The complexity of this operation is O(N)</remarks>
		/// <typeparam name="T">The type of an item.</typeparam>
		/// <param name="source">The index translation source.</param>
		/// <param name="list">The list to adjust items order.</param>
		public static void ApplyReindexing<T>(this IIndexTranslator source, IList<T> list)
		{
			if (list.Count <= 0)
			{
				return;
			}

			var count = source.ItemsCount;
			var marked = new BitArray(list.Count);
			var markedCount = 0;
			for (var idx = 0; idx < count && markedCount < count; idx++)
			{
				var dstIndex = source.GetDestIndex(idx);

				if (dstIndex == idx || marked[dstIndex])
				{				
					// no index change or already copied
					continue;
				}

				var srcValue = list[idx];

				while (!marked[dstIndex])
				{														
					++markedCount;
					marked[dstIndex] = true;

					var tmp = list[dstIndex];
					list[dstIndex] = srcValue;
					srcValue = tmp;
					
					dstIndex = source.GetSrcIndex(dstIndex);	
				}
			}
		}


	}
}