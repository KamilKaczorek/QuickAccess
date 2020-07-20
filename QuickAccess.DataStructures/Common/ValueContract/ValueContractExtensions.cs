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
// Project: QuickAccess.Parser
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion
using System;
using System.Collections.Generic;
using System.Data;

namespace QuickAccess.DataStructures.Common.ValueContract
{
    public static class ValueContractExtensions
    {
		
		public static void Set<TKey, TValue>(this IModifyValue<KeyValuePair<TKey, TValue>> source,
		                                     TKey key,
		                                     TValue value)
		{
			source.Set(new KeyValuePair<TKey, TValue>(key, value));
		}

        public static void Set<TValue>(this IModifyValue<TValue> source,
                                             TValue value)
        {
			var res = source.TryModifyValue(value);

            switch (res)
            {
                case ValueModificationResult.SuccessfullyModified:
                    return;
                case ValueModificationResult.AlreadySet:
                    throw new InvalidOperationException("Value is already set, can't modify the value.");
                case ValueModificationResult.SourceReadOnly:
                    throw new ReadOnlyException("Can't modify value - value is readonly.");
                case ValueModificationResult.SourceFrozen:
					throw new ReadOnlyException("Can't modify value - value is frozen.");
				case ValueModificationResult.ValueOutOfRange:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Can't  modify value - value is out of valid range.");
                case ValueModificationResult.ValueUndefined:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Can't  modify value - value is undefined.");
				default:
                    throw new InvalidOperationException($"Can't modify value ({res}).");
            }
		}

        public static bool TrySet<TValue>(this IModifyValue<TValue> source,
                                       TValue value)
        {
            var res = source.TryModifyValue(value);

            return res == ValueModificationResult.SuccessfullyModified;
        }

		public static bool TrySet<TKey, TValue>(this IModifyValue<KeyValuePair<TKey, TValue>> source,
		                                        TKey key,
		                                        TValue value)
		{
			return source.TrySet(new KeyValuePair<TKey, TValue>(key, value));
		}

		public static TKey GetKeyOrDefault<TKey, TValue>(this IRepresentValue<KeyValuePair<TKey, TValue>> source)
		{
			return source.TryGetValue(out var pair) ? pair.Key : default;
		}

		public static TValue GetKeyValueOrDefault<TKey, TValue>(this IRepresentValue<KeyValuePair<TKey, TValue>> source)
		{
			return source.TryGetValue(out var pair) ? pair.Value : default;
		}

		public static bool TryGetValue<T>(this IRepresentValue<T> source, out T value)
		{
			if (!source.IsDefined)
			{
				value = default;
				return false;
			}

			value = source.Value;
			return source.IsDefined;
		}

		public static void Set<T1, T2, T3, T4, T5, T6, T7>(this IModifyValue<Tuple<T1, T2, T3, T4, T5, T6, T7>> source,
		                                                   T1 item1,
		                                                   T2 item2,
		                                                   T3 item3,
		                                                   T4 item4,
		                                                   T5 item5,
		                                                   T6 item6,
		                                                   T7 item7
		)
		{
			source.Set(Tuple.Create(item1, item2, item3, item4, item5, item6, item7));
		}

		public static void Set<T1, T2, T3, T4, T5, T6>(this IModifyValue<Tuple<T1, T2, T3, T4, T5, T6>> source,
		                                               T1 item1,
		                                               T2 item2,
		                                               T3 item3,
		                                               T4 item4,
		                                               T5 item5,
		                                               T6 item6
		)
		{
			source.Set(Tuple.Create(item1, item2, item3, item4, item5, item6));
		}

		public static void Set<T1, T2, T3, T4, T5>(this IModifyValue<Tuple<T1, T2, T3, T4, T5>> source,
		                                           T1 item1,
		                                           T2 item2,
		                                           T3 item3,
		                                           T4 item4,
		                                           T5 item5
		)
		{
			source.Set(Tuple.Create(item1, item2, item3, item4, item5));
		}

		public static void Set<T1, T2, T3, T4>(this IModifyValue<Tuple<T1, T2, T3, T4>> source,
		                                       T1 item1,
		                                       T2 item2,
		                                       T3 item3,
		                                       T4 item4
		)
		{
			source.Set(Tuple.Create(item1, item2, item3, item4));
		}

		public static void Set<T1, T2, T3>(this IModifyValue<Tuple<T1, T2, T3>> source,
		                                   T1 item1,
		                                   T2 item2,
		                                   T3 item3
		)
		{
			source.Set(Tuple.Create(item1, item2, item3));
		}

		public static void Set<T1, T2>(this IModifyValue<Tuple<T1, T2>> source,
		                               T1 item1,
		                               T2 item2
		)
		{
			source.Set(Tuple.Create(item1, item2));
		}

		public static bool TrySet<T1, T2, T3, T4, T5, T6, T7>(this IModifyValue<Tuple<T1, T2, T3, T4, T5, T6, T7>> source,
		                                                      T1 item1,
		                                                      T2 item2,
		                                                      T3 item3,
		                                                      T4 item4,
		                                                      T5 item5,
		                                                      T6 item6,
		                                                      T7 item7
		)
		{
			return source.TrySet(Tuple.Create(item1, item2, item3, item4, item5, item6, item7));
		}

		public static bool TrySet<T1, T2, T3, T4, T5, T6>(this IModifyValue<Tuple<T1, T2, T3, T4, T5, T6>> source,
		                                                  T1 item1,
		                                                  T2 item2,
		                                                  T3 item3,
		                                                  T4 item4,
		                                                  T5 item5,
		                                                  T6 item6
		)
		{
			return source.TrySet(Tuple.Create(item1, item2, item3, item4, item5, item6));
		}

		public static bool TrySet<T1, T2, T3, T4, T5>(this IModifyValue<Tuple<T1, T2, T3, T4, T5>> source,
		                                              T1 item1,
		                                              T2 item2,
		                                              T3 item3,
		                                              T4 item4,
		                                              T5 item5
		)
		{
			return source.TrySet(Tuple.Create(item1, item2, item3, item4, item5));
		}

		public static bool TrySet<T1, T2, T3, T4>(this IModifyValue<Tuple<T1, T2, T3, T4>> source,
		                                          T1 item1,
		                                          T2 item2,
		                                          T3 item3,
		                                          T4 item4
		)
		{
			return source.TrySet(Tuple.Create(item1, item2, item3, item4));
		}

		public static bool TrySet<T1, T2, T3>(this IModifyValue<Tuple<T1, T2, T3>> source,
		                                      T1 item1,
		                                      T2 item2,
		                                      T3 item3
		)
		{
			return source.TrySet(Tuple.Create(item1, item2, item3));
		}

		public static bool TrySet<T1, T2>(this IModifyValue<Tuple<T1, T2>> source,
		                                  T1 item1,
		                                  T2 item2
		)
		{
			return source.TrySet(Tuple.Create(item1, item2));
		}
	}
}