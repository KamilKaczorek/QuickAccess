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
using QuickAccess.DataStructures.Common.Collections;

namespace QuickAccess.DataStructures.Common
{
	/// <summary>
	/// It defines empty value type to use as a dummy type parameter in generic types, when the specified value is not required.
	/// It is used as edge data type of graph with empty edges (edges without data).
	/// </summary>
	/// <seealso cref="IEquatable{T}" />
	/// <seealso cref="EmptyEnumerable{T}"/>
	public struct EmptyValue : IEquatable<EmptyValue>, IComparable<EmptyValue>, ICloneable, IReadOnlyList<EmptyValue>, IConvertible
	{
		/// <summary>Creates the fake list with empty value items.</summary>
		/// <param name="count">The number of items in a list.</param>
		/// <returns>The fake list instance.</returns>
		public static FakeList<EmptyValue> CreateFakeList(int count)
		{
			return new FakeList<EmptyValue>(Value, count);
		}

		/// <summary>
		/// The empty value.
		/// </summary>
		public static readonly EmptyValue Value = new EmptyValue();

		/// <summary>
		/// Determines whether the specified type is equal to <see cref="EmptyValue"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		///   <c>true</c> if the specified type is  <see cref="EmptyValue"/>; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsEmptyValueType(Type type)
		{
			return type == typeof(EmptyValue);		
		}

		/// <summary>
		/// Determines whether the specified type is equal to <see cref="EmptyValue" />.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <returns>
		///   <c>true</c> if the specified type is  <see cref="EmptyValue" />; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsEmptyValueType<T>()
		{
			return typeof(T) == typeof(EmptyValue);
		}

		/// <inheritdoc />
		public bool Equals(EmptyValue other)
		{
			return true;
		}

		/// <inheritdoc />
		public int CompareTo(EmptyValue other)
		{
			return 0;
		}

		/// <inheritdoc />
		public IEnumerator<EmptyValue> GetEnumerator()
		{
			yield break;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is EmptyValue;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return 0;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return nameof(EmptyValue);
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			yield break;
		}

		/// <inheritdoc />
		public object Clone()
		{
			return this;
		}

		/// <inheritdoc />
		public int Count => 0;

		/// <inheritdoc />
		public EmptyValue this[int index] => throw new IndexOutOfRangeException();

		/// <inheritdoc />
		public TypeCode GetTypeCode()
		{
			return TypeCode.Empty;
		}

		/// <inheritdoc />
		public bool ToBoolean(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public byte ToByte(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public char ToChar(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public DateTime ToDateTime(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public decimal ToDecimal(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public double ToDouble(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public short ToInt16(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public int ToInt32(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public long ToInt64(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public sbyte ToSByte(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public float ToSingle(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public string ToString(IFormatProvider provider)
		{
			return ToString();
		}

		/// <inheritdoc />
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			return Activator.CreateInstance(conversionType);
		}

		/// <inheritdoc />
		public ushort ToUInt16(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public uint ToUInt32(IFormatProvider provider)
		{
			return default;
		}

		/// <inheritdoc />
		public ulong ToUInt64(IFormatProvider provider)
		{
			return default;
		}
	}
}