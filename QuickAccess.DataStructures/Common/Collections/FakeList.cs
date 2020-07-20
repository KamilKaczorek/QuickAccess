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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace QuickAccess.DataStructures.Common.Collections
{
	/// <summary>
	///     The fake list, that returns single specified value for each list element.
	/// </summary>
	/// <typeparam name="T">The type of list item.</typeparam>
	/// <seealso cref="System.Collections.Generic.IReadOnlyList{T}" />
	public sealed class FakeList<T> : IReadOnlyList<T>
	{
		/// <summary>Gets the value returned for each list element.</summary>
		/// <value>The value returned for each list element.</value>
		public T Value { get; }

		/// <summary>Initializes a new instance of the <see cref="FakeList{T}" /> class.</summary>
		/// <param name="value">The value returned for each list element.</param>
		/// <param name="count">The initial list count.</param>
		public FakeList(T value, int count)
		{
			Count = count;
			Value = value;
		}

		/// <inheritdoc />
		/// <remarks>For each item it returns defined <see cref="Value" />.</remarks>
		public IEnumerator<T> GetEnumerator()
		{
			return Enumerable.Repeat(Value, Count).GetEnumerator();
		}

		/// <inheritdoc />
		/// <remarks>For each item it returns defined <see cref="Value" />.</remarks>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public int Count { get; set; }

		/// <inheritdoc />
		/// <remarks>When the index is in valid range, it will always return defined <see cref="Value" />.</remarks>
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new IndexOutOfRangeException(Count > 0
						? $"Specified index ({index}) is out of the valid range (0, {Count - 1})"
						: "Can't access item of empty list.");
				}

				return Value;
			}
		}

		/// <summary>Increases list count by one.</summary>
		public void Add()
		{
			++Count;
		}

		/// <summary>If the list <see cref="Count" /> is larger than <c>0</c> decreases list count by one.</summary>
		/// <returns><c>true</c> if the count was decreased.</returns>
		public bool Remove()
		{
			if (Count <= 0)
			{
				return false;
			}

			--Count;
			return true;
		}

		/// <summary>Increases the list <see cref="Count" /> by the specified value.</summary>
		/// <param name="rangeLength">Length of the range.</param>
		public void AddRange(int rangeLength)
		{
			Count += rangeLength;
		}
	}
}