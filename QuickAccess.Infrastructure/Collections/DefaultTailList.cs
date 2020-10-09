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

namespace QuickAccess.Infrastructure.Collections
{
	/// <summary>
	///     The list that consists of two parts: regular defined items and undefined tail, for which the default item is
	///     returned.
	///     The first part contains allocated memory for each item, the tail contains only single instance that is return for
	///     each tail index.
	///     It allows to save memory and reduce number of instances when the list contains empty elements at the end.
	///     It is used by graph structures, especially compacted graph, to not use list capacity by empty vertices.
	/// </summary>
	/// <typeparam name="T">The type of item.</typeparam>
	/// <seealso cref="System.Collections.Generic.IList{T}" />
	/// <seealso cref="System.Collections.Generic.IReadOnlyList{T}" />
	/// <seealso cref="FakeList{T}"/>
	public sealed class DefaultTailList<T> : IList<T>, IReadOnlyList<T>
	{
		private readonly List<T> _source;

		/// <summary>Gets the default item that is returned as every item of undefined tail.</summary>
		/// <value>The default item.</value>
		public T DefaultItem { get; }

		/// <summary>
		///     Gets the specified allocated items count (<see cref="Count" /> - default tail count).
		/// </summary>
		/// <value>
		///     The number of allocated items.
		/// </value>
		public int DefinedInstancesCount => _source.Count;

		/// <summary>
		///     Initializes a new instance of the <see cref="DefaultTailList{T}" /> class.
		/// </summary>
		/// <param name="source">The reference to source list container.</param>
		/// <param name="defaultItem">The default item.</param>
		/// <param name="count">The total number of items (allocated + tail).</param>
		internal DefaultTailList(List<T> source, T defaultItem, int count = 0)
		{
			_source = source ?? new List<T>();
			DefaultItem = defaultItem;
			Count = count;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="DefaultTailList{T}" /> class.
		/// </summary>
		/// <param name="defaultItem">The default item.</param>
		/// <param name="count">The number of default items in the list.</param>
		/// <param name="definedItemsCapacity">The initial capacity for allocated items.</param>
		public DefaultTailList(T defaultItem, int count = 0, int definedItemsCapacity = 0)
		{
			_source = new List<T>(definedItemsCapacity);
			DefaultItem = defaultItem;
			Count = count;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="DefaultTailList{T}" /> class copying data from specified list.
		/// </summary>
		/// <param name="other">The other list.</param>
		public DefaultTailList(DefaultTailList<T> other)
			: this(other._source.ToList(), other.DefaultItem, other.Count)
		{
		}

		/// <summary>
		///     Gets or sets the item at the specified index.
		///     If an item is defined it returns instance of defined item at specified index.
		///     If an item is not defined but the <paramref name="index" /> is smaller than <see cref="Count" /> then returns
		///     defined
		///     default value (the tail value).
		/// </summary>
		/// <value>The item of <see cref="T" /> type.</value>
		/// <param name="index">The index.</param>
		/// <returns>The item at specified index.</returns>
		public T this[int index]
		{
			get => index >= _source.Count && index < Count ? DefaultItem : _source[index];
			set
			{
				if (index >= _source.Count && index < Count)
				{
					if (EqualityComparer<T>.Default.Equals(value, DefaultItem))
					{
						return;
					}

					ExtendSourceTo(index + 1);
				}

				_source[index] = value;
			}
		}

		/// <summary>
		///     Gets the number of elements contained in the current list
		///     (the number of allocated items + count of default tail items).
		///     <seealso cref="DefinedInstancesCount" />
		/// </summary>
		public int Count { get; private set; }

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public void Add(T item)
		{
			if (EqualityComparer<T>.Default.Equals(item, DefaultItem))
			{
				++Count;
				return;
			}

			ExtendSourceTo(Count);
			_source.Add(item);
			Count = _source.Count;
		}

		/// <inheritdoc />
		public void Clear()
		{
			Count = 0;
			_source.Clear();
		}

		/// <inheritdoc />
		public bool Contains(T item)
		{
			if (Count > _source.Count && EqualityComparer<T>.Default.Equals(item, DefaultItem))
			{
				return true;
			}

			return _source.Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(T[] array, int arrayIndex)
		{
			_source.CopyTo(array, arrayIndex);

			for (var idx = _source.Count; idx < Count; ++idx)
			{
				array[idx + arrayIndex] = DefaultItem;
			}
		}

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator()
		{
			return _source.Concat(GetTailEnumerator()).GetEnumerator();
		}

		/// <inheritdoc />
		public int IndexOf(T item)
		{
			var idx = _source.IndexOf(item);

			if (idx >= 0)
			{
				return idx;
			}

			if (Count > _source.Count && EqualityComparer<T>.Default.Equals(item, DefaultItem))
			{
				return _source.Count;
			}

			return -1;
		}

		/// <inheritdoc />
		public void Insert(int index, T item)
		{
			if (index >= _source.Count && index < Count)
			{
				if (EqualityComparer<T>.Default.Equals(item, DefaultItem))
				{
					return;
				}

				ExtendSourceTo(index + 1);
			}

			_source.Insert(index, item);
		}

		/// <inheritdoc />
		public bool Remove(T item)
		{
			if (_source.Remove(item))
			{
				--Count;
				return true;
			}

			if (Count > _source.Count && EqualityComparer<T>.Default.Equals(item, DefaultItem))
			{
				--Count;
				return true;
			}

			return false;
		}

		/// <inheritdoc />
		public void RemoveAt(int index)
		{
			if (index >= _source.Count && index < Count)
			{
				--Count;
				return;
			}

			_source.RemoveAt(index);
			--Count;
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _source.Concat(GetTailEnumerator()).GetEnumerator();
		}

		/// <summary>
		///     Extends the count to the specified value, if the new count is not larger than current one, the operation is
		///     ignored and <c>0</c> is returned.
		/// </summary>
		/// <param name="newCount">The new count value.</param>
		/// <returns>The difference between new and old value.</returns>
		public int ExtendCountTo(int newCount)
		{
			var diff = newCount - Count;

			if (diff <= 0)
			{
				return 0;
			}

			Count = newCount;

			return diff;
		}

		private void ExtendSourceTo(int newCount)
		{
			var diff = newCount - _source.Count;

			if (diff <= 0)
			{
				return;
			}

			_source.Capacity = Math.Max(_source.Capacity, newCount);
			_source.AddRange(Enumerable.Repeat(DefaultItem, diff));
		}

		/// <summary>
		///     Determines whether it contains defined instance at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns><c>true</c> if it contains defined instance at the specified index; otherwise, <c>false</c>.</returns>
		public bool ContainsDefinedInstanceAt(int index)
		{
			return index >= 0 && index < _source.Count;
		}

		/// <summary>
		///     Gets the tail sequence.
		/// </summary>
		/// <returns>Tail sequence.</returns>
		public IEnumerable<T> GetTailEnumerator()
		{
			return Enumerable.Repeat(DefaultItem, Count - _source.Count);
		}
	}
}