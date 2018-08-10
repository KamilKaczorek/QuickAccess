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
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickAccess.DataStructures.Common
{
	public static class EnumerableExtensions
	{
		[Pure]
		public static IEnumerable<string> AsStrings<T>(this IEnumerable<T> source)
		{
			return source.Select(item => item?.ToString());
		}

		[Pure]
		public static IEnumerable<string> AsStrings(this string source)
		{
			return source.Select(item => item.ToString());
		}

		[Pure]
		public static T[][] GetMultidimentionalClone<T>(this IReadOnlyList<T[]> source)
		{
			var res = new T[source.Count][];

			for (var idx = 0; idx < source.Count; idx++)
			{
				res[idx] = (T[])source[idx].Clone();
			}

			return res;
		}

		[Pure]
		public static T[][][] GetMultidimentionalClone<T>(this IReadOnlyList<IReadOnlyList<T[]>> source)
		{
			var res = new T[source.Count][][];

			for (var idx = 0; idx < source.Count; idx++)
			{
				res[idx] = source[idx].GetMultidimentionalClone();
			}

			return res;
		}

		[Pure]
		public static T[][][][] GetMultidimentionalClone<T>(this IReadOnlyList<IReadOnlyList<IReadOnlyList<T[]>>> source)
		{
			var res = new T[source.Count][][][];

			for (var idx = 0; idx < source.Count; idx++)
			{
				res[idx] = source[idx].GetMultidimentionalClone();
			}

			return res;
		}

		[Pure]
		public static T[][][][][] GetMultidimentionalClone<T>(this IReadOnlyList<IReadOnlyList<IReadOnlyList<IReadOnlyList<T[]>>>> source)
		{
			var res = new T[source.Count][][][][];

			for (var idx = 0; idx < source.Count; idx++)
			{
				res[idx] = source[idx].GetMultidimentionalClone();
			}

			return res;
		}

		[Pure]
		public static EmptyEnumerable<T> Empty<T>() => EmptyEnumerable<T>.Instance;

		[Pure]
		public static IEnumerable<T> EnumerateNotNullOrReturnEmpty<T>(IEnumerable<T> source)
		{
			return source ?? EmptyEnumerable<T>.Instance;
		}

		[Pure]
		public static IReadOnlyList<T> EnumerateNotNullOrReturnEmpty<T>(IReadOnlyList<T> source)
		{
			return source ?? EmptyEnumerable<T>.Instance;
		}

		[Pure]
		public static IReadOnlyCollection<T> EnumerateNotNullOrReturnEmpty<T>(IReadOnlyCollection<T> source)
		{
			return source ?? EmptyEnumerable<T>.Instance;
		}

		[Pure]
		public static IEnumerator<T> EnumerateNotNullOrReturnEmpty<T>(IEnumerator<T> source)
		{
			return source ?? EmptyEnumerable<T>.Instance;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source is List<T> list)
			{
				list.ForEach(action);
				return;
			}

			foreach (var item in source)
			{
				action.Invoke(item);
			}
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(this ValueTuple<T> source)
		{
			yield return source.Item1;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(this ValueTuple<T, T> source)
		{
			yield return source.Item1;
			yield return source.Item2;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(this ValueTuple<T, T, T> source)
		{
			yield return source.Item1;
			yield return source.Item2;
			yield return source.Item3;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(this ValueTuple<T, T, T, T> source)
		{
			yield return source.Item1;
			yield return source.Item2;
			yield return source.Item3;
			yield return source.Item4;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(this ValueTuple<T, T, T, T, T> source)
		{
			yield return source.Item1;
			yield return source.Item2;
			yield return source.Item3;
			yield return source.Item4;
			yield return source.Item5;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(this ValueTuple<T, T, T, T, T, T> source)
		{
			yield return source.Item1;
			yield return source.Item2;
			yield return source.Item3;
			yield return source.Item4;
			yield return source.Item5;
			yield return source.Item6;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(this ValueTuple<T, T, T, T, T, T, T> source)
		{
			yield return source.Item1;
			yield return source.Item2;
			yield return source.Item3;
			yield return source.Item4;
			yield return source.Item5;
			yield return source.Item6;
			yield return source.Item7;
		}

	

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3, T item4, T item5, T item6, T item7, T item8, T item9, T item10)
		{
			yield return item1;
			yield return item2;
			yield return item3;
			yield return item4;
			yield return item5;
			yield return item6;
			yield return item7;
			yield return item8;
			yield return item9;
			yield return item10;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3, T item4, T item5, T item6, T item7, T item8, T item9)
		{
			yield return item1;
			yield return item2;
			yield return item3;
			yield return item4;
			yield return item5;
			yield return item6;
			yield return item7;
			yield return item8;
			yield return item9;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3, T item4, T item5, T item6, T item7, T item8)
		{
			yield return item1;
			yield return item2;
			yield return item3;
			yield return item4;
			yield return item5;
			yield return item6;
			yield return item7;
			yield return item8;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3, T item4, T item5, T item6, T item7)
		{
			yield return item1;
			yield return item2;
			yield return item3;
			yield return item4;
			yield return item5;
			yield return item6;
			yield return item7;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3, T item4, T item5, T item6)
		{
			yield return item1;
			yield return item2;
			yield return item3;
			yield return item4;
			yield return item5;
			yield return item6;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3, T item4, T item5)
		{
			yield return item1;
			yield return item2;
			yield return item3;
			yield return item4;
			yield return item5;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3, T item4)
		{
			yield return item1;
			yield return item2;
			yield return item3;
			yield return item4;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2, T item3)
		{
			yield return item1;
			yield return item2;
			yield return item3;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1 , T item2)
		{
			yield return item1;
			yield return item2;
		}

		[Pure]
		public static IEnumerable<T> Enumerate<T>(T item1)
		{
			yield return item1;
		
		}

		[Pure]		
		public static ReindexedDataResult<T[]> GetSortedWithReindexingResult<T>(this IEnumerable<T> source,
		                                                                     IComparer<T> comparer = null)
		{
			var items = source.ToArray();

			return SortWithReindexingResult(items, comparer).ToReindexedDataResult(items);
		}

		[Pure]
		public static int GetLargestDistance(this IEnumerable<int> source)
		{
			var first = true;
			var min = 0;
			var max = min;
			foreach (var i in source)
			{
				if (first)
				{
					first = false;
					min = i;
					max = i;
					continue;
				}

				if (i > max)
				{
					max = i;
					continue;
				}

				if (i < min)
				{
					min = i;
				}
			}

			return max - min;
		}

		[Pure]
		public static uint GetLargestDistance(this IEnumerable<uint> source)
		{
			var first = true;
			var min = 0U;
			var max = min;
			foreach (var i in source)
			{
				if (first)
				{
					first = false;
					min = i;
					max = i;
					continue;
				}

				if (i > max)
				{
					max = i;
					continue;
				}

				if (i < min)
				{
					min = i;
				}
			}

			return max - min;
		}

		[Pure]
		public static long GetLargestDistance(this IEnumerable<long> source)
		{
			var first = true;
			var min = 0L;
			var max = min;
			foreach (var i in source)
			{
				if (first)
				{
					first = false;
					min = i;
					max = i;
					continue;
				}

				if (i > max)
				{
					max = i;
					continue;
				}

				if (i < min)
				{
					min = i;
				}
			}

			return max - min;
		}

		[Pure]
		public static ulong GetLargestDistance(this IEnumerable<ulong> source)
		{
			var first = true;
			var min = 0UL;
			var max = min;
			foreach (var i in source)
			{
				if (first)
				{
					first = false;
					min = i;
					max = i;
					continue;
				}

				if (i > max)
				{
					max = i;
					continue;
				}

				if (i < min)
				{
					min = i;
				}
			}

			return max - min;
		}

		[Pure]
		public static float GetLargestDistance(this IEnumerable<float> source)
		{
			var first = true;
			var min = 0F;
			var max = min;
			foreach (var i in source)
			{
				if (first)
				{
					first = false;
					min = i;
					max = i;
					continue;
				}

				if (i > max)
				{
					max = i;
					continue;
				}

				if (i < min)
				{
					min = i;
				}
			}

			return max - min;
		}

		[Pure]
		public static double GetLargestDistance(this IEnumerable<double> source)
		{
			var first = true;
			var min = 0D;
			var max = min;
			foreach (var i in source)
			{
				if (first)
				{
					first = false;
					min = i;
					max = i;
					continue;
				}

				if (i > max)
				{
					max = i;
					continue;
				}

				if (i < min)
				{
					min = i;
				}
			}

			return max - min;
		}

		[Pure]
		public static decimal GetLargestDistance(this IEnumerable<decimal> source)
		{
			var first = true;
			var min = 0M;
			var max = min;
			foreach (var i in source)
			{
				if (first)
				{
					first = false;
					min = i;
					max = i;
					continue;
				}

				if (i > max)
				{
					max = i;
					continue;
				}

				if (i < min)
				{
					min = i;
				}
			}

			return max - min;
		}

		[Pure]
		public static IEnumerable<int> AddToEachItem(this IEnumerable<int> source, int value)
		{
			return source.Select(i => i + value);
		}

		[Pure]
		public static IEnumerable<uint> AddToEachItem(this IEnumerable<uint> source, uint value)
		{
			return source.Select(i => i + value);
		}

		[Pure]
		public static IEnumerable<long> AddToEachItem(this IEnumerable<long> source, long value)
		{
			return source.Select(i => i + value);
		}

		[Pure]
		public static IEnumerable<ulong> AddToEachItem(this IEnumerable<ulong> source, ulong value)
		{
			return source.Select(i => i + value);
		}

		[Pure]
		public static IEnumerable<float> AddToEachItem(this IEnumerable<float> source, float value)
		{
			return source.Select(i => i + value);
		}

		[Pure]
		public static IEnumerable<double> AddToEachItem(this IEnumerable<double> source, double value)
		{
			return source.Select(i => i + value);
		}

		[Pure]
		public static IEnumerable<decimal> AddToEachItem(this IEnumerable<decimal> source, decimal value)
		{
			return source.Select(i => i + value);
		}

		[Pure]
		public static IEnumerable<int> MultiplyEachItemBy(this IEnumerable<int> source, int value)
		{
			return source.Select(i => i*value);
		}

		[Pure]
		public static IEnumerable<uint> MultiplyEachItemBy(this IEnumerable<uint> source, uint value)
		{
			return source.Select(i => i*value);
		}

		[Pure]
		public static IEnumerable<long> MultiplyEachItemBy(this IEnumerable<long> source, long value)
		{
			return source.Select(i => i*value);
		}

		[Pure]
		public static IEnumerable<ulong> MultiplyEachItemBy(this IEnumerable<ulong> source, ulong value)
		{
			return source.Select(i => i*value);
		}

		[Pure]
		public static IEnumerable<float> MultiplyEachItemBy(this IEnumerable<float> source, float value)
		{
			return source.Select(i => i*value);
		}

		[Pure]
		public static IEnumerable<double> MultiplyEachItemBy(this IEnumerable<double> source, double value)
		{
			return source.Select(i => i*value);
		}

		[Pure]
		public static IEnumerable<decimal> MultiplyEachItemBy(this IEnumerable<decimal> source, decimal value)
		{
			return source.Select(i => i*value);
		}


		
		
		[Pure]
		public static int XOr(this IEnumerable<int> source, int seed = 0)
		{
			return source.Aggregate(seed, (a, b) => a ^ b);
		}
		[Pure]
		public static int Or(this IEnumerable<int> source, int seed = 0)
		{
			return source.Aggregate(seed, (a, b) => a | b);
		}
		[Pure]
		public static int And(this IEnumerable<int> source, int seed = ~0)
		{
			return source.Aggregate(seed, (a, b) => a & b);
		}
		[Pure]
		public static uint XOr(this IEnumerable<uint> source, uint seed = 0U)
		{
			return source.Aggregate(seed, (a, b) => a ^ b);
		}
		[Pure]
		public static uint Or(this IEnumerable<uint> source, uint seed = 0U)
		{
			return source.Aggregate(seed, (a, b) => a | b);
		}
		[Pure]
		public static uint And(this IEnumerable<uint> source, uint seed = ~0U)
		{
			return source.Aggregate(seed, (a, b) => a & b);
		}
		[Pure]
		public static long XOr(this IEnumerable<long> source, long seed = 0L)
		{
			return source.Aggregate(seed, (a, b) => a ^ b);
		}
		[Pure]
		public static long Or(this IEnumerable<long> source, long seed = 0L)
		{
			return source.Aggregate(seed, (a, b) => a | b);
		}
		[Pure]
		public static long And(this IEnumerable<long> source, long seed = ~0L)
		{
			return source.Aggregate(seed, (a, b) => a & b);
		}
		[Pure]
		public static ulong XOr(this IEnumerable<ulong> source, ulong seed = 0UL)
		{
			return source.Aggregate(seed, (a, b) => a ^ b);
		}
		[Pure]
		public static ulong Or(this IEnumerable<ulong> source, ulong seed = 0UL)
		{
			return source.Aggregate(seed, (a, b) => a | b);
		}
		[Pure]
		public static ulong And(this IEnumerable<ulong> source, ulong seed = ~0UL)
		{
			return source.Aggregate(seed, (a, b) => a & b);
		}


		public static IndexTranslator SortWithReindexingResult<T>(this T[] items, IComparer<T> comparer = null)
		{
			var len = items.Length;
			var srcByDestIndexes = new int[len];

			for (var idx = 0; idx < len; ++idx)
			{
				srcByDestIndexes[idx] = idx;
			}

			Array.Sort(items, srcByDestIndexes, comparer);

			return new IndexTranslator(srcByDestIndexes);
		}

		public static IndexTranslator SortWithReindexingResult<T>(List<T> items, IComparer<T> comparer = null)
		{
			var len = items.Count;
			var srcByDestIndexes = new int[len];
			
			for (var idx = 0; idx < len; ++idx)
			{
				srcByDestIndexes[idx] = idx;
			}

			var array = new T[len];
			items.CopyTo(array);

			Array.Sort(array, srcByDestIndexes, comparer);

			var destBySrcIndexes = new int[len];
			for (var destIndex = 0; destIndex < len; destIndex++)
			{
				var srcIndex = srcByDestIndexes[destIndex];
				destBySrcIndexes[srcIndex] = destIndex;
				items[destIndex] = array[destIndex];				
			}

			return new IndexTranslator(srcByDestIndexes, destBySrcIndexes);
		}
		

		public static T ExtractFirstOrDefault<T>(this IList<T> source)
		{
			if (source.Count <= 0)
			{
				return default;
			}

			var res = source[0];

			source.RemoveAt(0);

			return res;
		}

		public static T ExtractLastOrDefault<T>(this IList<T> source)
		{
			var idx = source.Count - 1;

			if (idx < 0)
			{
				return default;
			}

			var res = source[idx];

			source.RemoveAt(idx);

			return res;
		}

		public static T ExtractLast<T>(this IList<T> source)
		{			
			var res = source.Last();

			source.RemoveAt(source.Count-1);

			return res;
		}

		public static T ExtractFirst<T>(this IList<T> source)
		{			
			var res = source.First();

			source.RemoveAt(0);

			return res;
		}

		public static TValue Extract<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
		{
			var val = source[key];
			source.Remove(key);
			return val;
		}

		public static bool TryExtract<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, out TValue value)
		{
			if (!source.TryGetValue(key, out value))
			{
				return false;
			}

			source.Remove(key);
			return true;
		}
	}
}