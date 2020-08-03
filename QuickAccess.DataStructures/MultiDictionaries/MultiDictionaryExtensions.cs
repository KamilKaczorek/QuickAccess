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
using System.Collections.Generic;
using System.Linq;

namespace QuickAccess.DataStructures.MultiDictionaries
{
	/// <summary>
	///     The extension methods of double dictionary defined as outer dictionary nesting inner dictionary as fallows:
	///     <code>
	/// IDictionary&lt;TOuterKey, IDictionary&lt;TInnerKey, TValue&gt;&gt;
	/// </code>
	///     Double dictionary construction gives the fastest read-access when compared to other constructions, like
	///     single dictionary with keys defined as tuples or structures (.net 4.7).
	/// </summary>
	public static class MultiDictionaryExtensions
	{
		/// <summary>
		///     Sets the value of the inner dictionary for the <paramref name="outerKey" /> of outer dictionary and
		///     <paramref name="innerKey" /> of inner dictionary.
		///     If inner dictionary doesn't exist for the specified key, the new one will be created with specified comparer.
		/// </summary>
		/// <typeparam name="TOuterKey">The type of the <paramref name="outerKey" />.</typeparam>
		/// <typeparam name="TInnerKey">The type of the <paramref name="innerKey" />.</typeparam>
		/// <typeparam name="TValue">The type of the <paramref name="value" />.</typeparam>
		/// <param name="source">
		///     The source of an extension of <see cref="IDictionary{TOuterKey, Dictionary{TInnerKey, TValue}}" />
		///     interface.
		/// </param>
		/// <param name="outerKey">The key of the outer dictionary.</param>
		/// <param name="innerKey">The key of the inner dictionary.</param>
		/// <param name="value">The value (the value of the inner dictionary).</param>
		/// <param name="innerKeyComparer">
		///     The inner dictionary keys comparer to be used when inner dictionary instance is created.
		///     If it is <c>null</c> then default comparer will be used.
		/// </param>
		/// <returns>
		///     The number of added items: <c>0</c> when item was overwritten, <c>1</c> when new item was added to the inner
		///     dictionary.
		/// </returns>
		public static int SetInnerValue<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, Dictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey,
			TValue value,
			IEqualityComparer<TInnerKey> innerKeyComparer = null)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				innerDict = innerKeyComparer != null
					? new Dictionary<TInnerKey, TValue>(innerKeyComparer)
					: new Dictionary<TInnerKey, TValue>();
				source.Add(outerKey, innerDict);
			}

			var count = innerDict.Count;
			innerDict[innerKey] = value;
			return innerDict.Count - count;
		}

		/// <summary>
		///     Ads the value of the inner dictionary for the <paramref name="outerKey" /> of outer dictionary and
		///     <paramref name="innerKey" /> of inner dictionary.
		///     If inner dictionary doesn't exist for the specified key, the new one will be created with specified comparer.
		/// </summary>
		/// <typeparam name="TOuterKey">The type of the <paramref name="outerKey" />.</typeparam>
		/// <typeparam name="TInnerKey">The type of the <paramref name="innerKey" />.</typeparam>
		/// <typeparam name="TValue">The type of the <paramref name="value" />.</typeparam>
		/// <param name="source">
		///     The source of an extension of <see cref="IDictionary{TOuterKey, Dictionary{TInnerKey, TValue}}" />
		///     interface.
		/// </param>
		/// <param name="outerKey">The key of the outer dictionary.</param>
		/// <param name="innerKey">The key of the inner dictionary.</param>
		/// <param name="value">The value (the value of the inner dictionary).</param>
		/// <param name="innerKeyComparer">
		///     The inner dictionary keys comparer to be used when inner dictionary instance is created.
		///     If it is <c>null</c> then default comparer will be used.
		/// </param>
		public static void AddInnerValue<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, Dictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey,
			TValue value,
			IEqualityComparer<TInnerKey> innerKeyComparer)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				innerDict = innerKeyComparer != null
					? new Dictionary<TInnerKey, TValue>(innerKeyComparer)
					: new Dictionary<TInnerKey, TValue>();
				source.Add(outerKey, innerDict);
			}

			innerDict.Add(innerKey, value);
		}

		/// <summary>
		///     Sets the value of the inner dictionary for the <paramref name="outerKey" /> of outer dictionary and
		///     <paramref name="innerKey" /> of inner dictionary.
		///     If inner dictionary doesn't exist for the specified key, the new one will be created using specified
		///     <paramref name="innerDictionaryFactory" /> callback.
		/// </summary>
		/// <typeparam name="TOuterKey">The type of the <paramref name="outerKey" />.</typeparam>
		/// <typeparam name="TInnerKey">The type of the <paramref name="innerKey" />.</typeparam>
		/// <typeparam name="TValue">The type of the <paramref name="value" />.</typeparam>
		/// <param name="source">
		///     The source of an extension of <see cref="IDictionary{TOuterKey, Dictionary{TInnerKey, TValue}}" />
		///     interface.
		/// </param>
		/// <param name="innerDictionaryFactory">The inner dictionary factory callback.</param>
		/// <param name="outerKey">The key of the outer dictionary.</param>
		/// <param name="innerKey">The key of the inner dictionary.</param>
		/// <param name="value">The value (the value of the inner dictionary).</param>
		/// <returns>
		///     The number of added items: <c>0</c> when item was overwritten, <c>1</c> when new item was added to the inner
		///     dictionary.
		/// </returns>
		public static int SetInnerValue<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source,
			Func<IDictionary<TInnerKey, TValue>> innerDictionaryFactory,
			TOuterKey outerKey,
			TInnerKey innerKey,
			TValue value
		)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				innerDict = innerDictionaryFactory.Invoke();
				source.Add(outerKey, innerDict);
			}

			var count = innerDict.Count;
			innerDict[innerKey] = value;
			return innerDict.Count - count;
		}

		/// <summary>
		///     Ads the value of the inner dictionary for the <paramref name="outerKey" /> of outer dictionary and
		///     <paramref name="innerKey" /> of inner dictionary.
		///     If inner dictionary doesn't exist for the specified key, the new one will be created using specified
		///     <paramref name="innerDictionaryFactory" /> callback.
		/// </summary>
		/// <typeparam name="TOuterKey">The type of the <paramref name="outerKey" />.</typeparam>
		/// <typeparam name="TInnerKey">The type of the <paramref name="innerKey" />.</typeparam>
		/// <typeparam name="TValue">The type of the <paramref name="value" />.</typeparam>
		/// <param name="source">
		///     The source of an extension of <see cref="IDictionary{TOuterKey, Dictionary{TInnerKey, TValue}}" />
		///     interface.
		/// </param>
		/// <param name="innerDictionaryFactory">The inner dictionary factory callback.</param>
		/// <param name="outerKey">The key of the outer dictionary.</param>
		/// <param name="innerKey">The key of the inner dictionary.</param>
		/// <param name="value">The value (the value of the inner dictionary).</param>
		public static int AddInnerValue<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source,
			Func<IDictionary<TInnerKey, TValue>> innerDictionaryFactory,
			TOuterKey outerKey,
			TInnerKey innerKey,
			TValue value
		)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				innerDict = innerDictionaryFactory.Invoke();
				source.Add(outerKey, innerDict);
			}

			var count = innerDict.Count;
			innerDict.Add(innerKey, value);
			return innerDict.Count - count;
		}

		/// <summary>
		///     Determines whether the source multi dictionary contains specified keys pair:
		///     - the outer dictionary contains inner dictionary for the <paramref name="outerKey" />
		///     and the inner dictionary contains <paramref name="innerKey" />.
		/// </summary>
		/// <typeparam name="TOuterKey">The type of the outerKey.</typeparam>
		/// <typeparam name="TInnerKey">The type of the innerKey.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="source">
		///     The source of an extension of
		///     <see cref="IDictionary{TOuterKey, IDictionary{TInnerKey, TValue}}" /> interface.
		/// </param>
		/// <param name="outerKey">The outerKey.</param>
		/// <param name="innerKey">The innerKey.</param>
		/// <returns>
		///     <c>true</c> if contains the specified keys pair; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsKeyPair<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				return false;
			}

			return innerDict.ContainsKey(innerKey);
		}

		/// <summary>
		///     Determines whether the source multi dictionary contains specified keys pair:
		///     - the outer dictionary contains inner dictionary for the <paramref name="outerKey" />
		///     and the inner dictionary contains <paramref name="innerKey" />.
		/// </summary>
		/// <typeparam name="TOuterKey">The type of the outerKey.</typeparam>
		/// <typeparam name="TInnerKey">The type of the innerKey.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="source">
		///     The source of an extension of
		///     <see cref="IDictionary{TOuterKey, IDictionary{TInnerKey, TValue}}" /> interface.
		/// </param>
		/// <param name="outerKey">The outerKey.</param>
		/// <param name="innerKey">The innerKey.</param>
		/// <returns>
		///     <c>true</c> if contains the specified keys pair; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsOuterInnerKeyPair<TOuterKey, TInnerKey, TValue>(
			this Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				return false;
			}

			return innerDict.ContainsKey(innerKey);
		}


		public static TValue GetInnerValue<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey)
		{
			return source[outerKey][innerKey];
		}

		public static bool TryGetInnerValue<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey,
			out TValue value)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				value = default;
				return false;
			}

			return innerDict.TryGetValue(innerKey, out value);
		}

		public static TValue GetInnerValue<TOuterKey, TInnerKey, TValue>(
			this Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey)
		{
			return source[outerKey][innerKey];
		}

		public static bool TryGetInnerValue<TOuterKey, TInnerKey, TValue>(
			this Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey,
			out TValue value)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				value = default;
				return false;
			}

			return innerDict.TryGetValue(innerKey, out value);
		}

		public static bool RemoveInner<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source,
			TOuterKey outerKey,
			TInnerKey innerKey,
			bool removeInnerIfEmpty = true)
		{
			if (!source.TryGetValue(outerKey, out var innerDict))
			{
				return false;
			}

			if (!innerDict.Remove(innerKey))
			{
				return false;
			}

			if (removeInnerIfEmpty && innerDict.Count <= 0)
			{
				source.Remove(outerKey);
			}

			return true;
		}

        public static bool RemoveInner<TOuterKey, TInnerKey, TValue>(
            this IDictionary<TOuterKey, Dictionary<TInnerKey, TValue>> source,
            TOuterKey outerKey,
            TInnerKey innerKey,
            bool removeInnerIfEmpty = true)
        {
            if (!source.TryGetValue(outerKey, out var innerDict))
            {
                return false;
            }

            if (!innerDict.Remove(innerKey))
            {
                return false;
            }

            if (removeInnerIfEmpty && innerDict.Count <= 0)
            {
                source.Remove(outerKey);
            }

            return true;
        }

		public static IEnumerable<(TOuterKey, TInnerKey)> GetOuterInnerKeyPairs<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source)
		{
			foreach (var outerKeyValue in source)
			{
				foreach (var innerKey in outerKeyValue.Value.Keys)
				{
					yield return (outerKeyValue.Key, innerKey);
				}
			}
		}

		public static IEnumerable<(TOuterKey, TInnerKey, TValue)> GetKeysValueTriples<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source)
		{
			foreach (var outerKeyValue in source)
			{
				foreach (var innerKeyvalue in outerKeyValue.Value)
				{
					yield return (outerKeyValue.Key, innerKeyvalue.Key, innerKeyvalue.Value);
				}
			}
		}

		public static IEnumerable<TValue> GetInnerValues<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source)
		{
			foreach (var innerDict in source.Values)
			{
				foreach (var value in innerDict.Values)
				{
					yield return value;
				}
			}
		}

		public static int GetInnerItemsCount<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source)
		{
			return source.Values.Sum(innerDict => innerDict.Count);
		}

		public static IEnumerable<TOuterKey> GetOuterKeysWhereInnerNullOrEmpty<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source)
		{
			foreach (var outerKeyValue in source)
			{
				if (outerKeyValue.Value == null || outerKeyValue.Value.Count <= 0)
				{
					yield return outerKeyValue.Key;
				}
			}
		}

		public static void RemoveAllWhereInnerIsNullOrEmpty<TOuterKey, TInnerKey, TValue>(
			this IDictionary<TOuterKey, IDictionary<TInnerKey, TValue>> source)
		{
			foreach (var outerKey in source.GetOuterKeysWhereInnerNullOrEmpty().ToArray())
			{
				source.Remove(outerKey);
			}
		}
	}
}