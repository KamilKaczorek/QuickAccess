#region LICENSE [BSD-2-Clause]
// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2019 by Kamil Piotr Kaczorek
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

namespace QuickAccess.DataStructures.CodeOperatorAlgebra
{
	public static class OverloadableCodeOperatorsGroups
	{
		public const OverloadableCodeOperators Unary = OverloadableCodeOperators.Increment | OverloadableCodeOperators.Decrement | OverloadableCodeOperators.Plus | OverloadableCodeOperators.Minus | OverloadableCodeOperators.LogicalNegation |
		                                                 OverloadableCodeOperators.BitwiseComplement | OverloadableCodeOperators.TrueOperator | OverloadableCodeOperators.FalseOperator;

		public const OverloadableCodeOperators Bitwise = OverloadableCodeOperators.And | OverloadableCodeOperators.XOr | OverloadableCodeOperators.Or;
		public const OverloadableCodeOperators Multiplicative = OverloadableCodeOperators.Mul | OverloadableCodeOperators.Div | OverloadableCodeOperators.Mod;
		public const OverloadableCodeOperators Additive = OverloadableCodeOperators.Sum | OverloadableCodeOperators.Sub;
		public const OverloadableCodeOperators Shift = OverloadableCodeOperators.LeftShift | OverloadableCodeOperators.RightShift;
		public const OverloadableCodeOperators Relational = OverloadableCodeOperators.LessThan | OverloadableCodeOperators.LessThanOrEqual | OverloadableCodeOperators.GreaterThan | OverloadableCodeOperators.GreaterThanOrEqual;
		public const OverloadableCodeOperators Equality = OverloadableCodeOperators.Equal | OverloadableCodeOperators.Unequal;

		public const OverloadableCodeOperators Binary = Bitwise | Multiplicative | Additive | Shift | Relational | Equality;

		public const OverloadableCodeOperators Symmetric = OverloadableCodeOperators.Mul | OverloadableCodeOperators.Div | OverloadableCodeOperators.Mod | OverloadableCodeOperators.Sum | OverloadableCodeOperators.Sub | OverloadableCodeOperators.And | OverloadableCodeOperators.XOr | OverloadableCodeOperators.Or | OverloadableCodeOperators.Increment | OverloadableCodeOperators.Decrement | OverloadableCodeOperators.Plus | OverloadableCodeOperators.Minus | OverloadableCodeOperators.LogicalNegation | OverloadableCodeOperators.BitwiseComplement;

		public const OverloadableCodeOperators AllOperators = Unary | Binary;


		[Pure]
		public static bool IsOneOf(this OverloadableCodeOperator source, OverloadableCodeOperators flags)
		{
			return ((int) source & (int) flags) != 0;
		}

		[Pure]
		public static bool IsNotAnyOf(this OverloadableCodeOperator source, OverloadableCodeOperators flags)
		{
			return ((int) source & (int) flags) == 0;
		}

		[Pure]
		public static IEnumerable<OverloadableCodeOperator> ToOverloadableCSharpOperatorSequence(
			this OverloadableCodeOperators source)
		{
			return Enum.GetValues(typeof(OverloadableCodeOperator))
			           .Cast<OverloadableCodeOperator>()
			           .Where(v => ((int) v & (int) source) != 0);
		}

		[Pure]
		public static OverloadableCodeOperators ToFlags(this IEnumerable<OverloadableCodeOperator> source)
		{
			return (OverloadableCodeOperators)source.Cast<int>().Aggregate(0, (acc, src) => acc | src);
		}

		[Pure]
		public static bool Contains(this OverloadableCodeOperators source, OverloadableCodeOperator value)
		{
			return ((int)source & (int)value) != 0;
		}

		[Pure]
		public static OverloadableCodeOperators Add(this OverloadableCodeOperators source, OverloadableCodeOperator value)
		{
			return source | (OverloadableCodeOperators) value;
		}

		[Pure]
		public static OverloadableCodeOperators Remove(this OverloadableCodeOperators source, OverloadableCodeOperator value)
		{
			return source & (OverloadableCodeOperators) ~value;
		}

		[Pure]
		public static OverloadableCodeOperators Add(this OverloadableCodeOperators source, OverloadableCodeOperators value)
		{
			return source | value;
		}

		[Pure]
		public static OverloadableCodeOperators Remove(this OverloadableCodeOperators source, OverloadableCodeOperators value)
		{
			return source & ~value;
		}

		[Pure]
		public static OverloadableCodeOperators Add(this OverloadableCodeOperator source, params OverloadableCodeOperator[] values)
		{
			return ToFlags(values).Add(source);
		}
		

		[Pure]
		public static bool ContainsAny(this OverloadableCodeOperators source, OverloadableCodeOperators flags)
		{
			return (source & flags) != OverloadableCodeOperators.None;
		}

		[Pure]
		public static bool ContainsAll(this OverloadableCodeOperators source, OverloadableCodeOperators flags)
		{
			return (source & flags) == flags;
		}
	}
}