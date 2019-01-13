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

namespace QuickAccess.DataStructures.Common.Collections
{
	public sealed class DefaultTailReadOnlyList<T> : IReadOnlyList<T>
	{
		private readonly T[] _source;

		public T OutOfTheSourceValue { get; }

		public int SpecifiedInstancesCount => _source?.Length ?? 0;

		public DefaultTailReadOnlyList(T[] source, int count, T outOfTheSourceValue)
		{
			_source = source != null && source.Length <= 0 ? null : source;
			Count = count;
			OutOfTheSourceValue = outOfTheSourceValue;
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new IndexOutOfRangeException();
				}

				return index < _source.Length
					? _source[index]
					: OutOfTheSourceValue;
			}
		}

		public int Count { get; }

		public IEnumerator<T> GetEnumerator()
		{
			var srcCount = _source?.Length ?? 0;
			for (var idx = 0; idx < Count; idx++)
			{
				yield return idx < srcCount
					// ReSharper disable once PossibleNullReferenceException
					? _source[idx]
					: OutOfTheSourceValue;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool ContainsSpecifiedInstanceAt(int index)
		{
			return _source != null && index >= 0 && index < SpecifiedInstancesCount;
		}
	}
}