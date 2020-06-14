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
        public static string GetSymbol(this OverloadableCodeBinarySymmetricOperator source)
        {
            return source.ToCodeOperator().GetSymbol();
        }

        [Pure]
        public static string GetSymbol(this OverloadableCodeUnarySymmetricOperator source)
        {
            return source.ToCodeOperator().GetSymbol();
        }

		[Pure]
		public static string ToCodeRepresentation(this OverloadableCodeBinarySymmetricOperator source, string arg = null, string right = null)
		{
			return source.ToCodeOperator().ToCodeRepresentation(arg, right);
		}

		[Pure]
		public static string ToCodeRepresentation(this OverloadableCodeUnarySymmetricOperator source, string arg = null, string right = null)
		{
			return source.ToCodeOperator().ToCodeRepresentation(arg, right);
		}

		[Pure]
		public static string ToCodeRepresentation(this OverloadableCodeOperator source, string arg = null, string right = null, string spaceBetweenBinOperatorAndArgument = null)
		{
            var symbol = source.GetSymbol();
            var argsCount = source.GetNumberOfArguments();

            if (argsCount == 0)
            {
                return symbol;
            }

            if (argsCount == 1)
            {
                var isPost = (source == OverloadableCodeOperator.Increment ||
                              source == OverloadableCodeOperator.Decrement);
                return isPost ? $"{arg}{symbol}" : $"{symbol}{arg}";
            }

            return
                $"{arg}{spaceBetweenBinOperatorAndArgument}{source.GetSymbol()}{spaceBetweenBinOperatorAndArgument}{right}";
        }

		[Pure]
		public static int GetNumberOfArguments(this OverloadableCodeOperator source)
		{
            if (source == OverloadableCodeOperator.TrueOperator || source == OverloadableCodeOperator.FalseOperator)
            {
                return 0;
            }

            if (source.IsOneOf(Unary))
            {
                return 1;
            }

            if (source.IsOneOf(Binary))
            {
                return 2;
            }

            throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }

		[Pure]
		public static string GetSymbol(this OverloadableCodeOperator source)
		{
            var res = source switch
            {
                OverloadableCodeOperator.Increment => "++",
                OverloadableCodeOperator.Decrement => "--",
                OverloadableCodeOperator.Plus => "+",
                OverloadableCodeOperator.Minus => "-",
                OverloadableCodeOperator.LogicalNegation => "!",
                OverloadableCodeOperator.BitwiseComplement => "~",
                OverloadableCodeOperator.TrueOperator => "true",
                OverloadableCodeOperator.FalseOperator => "false",
                OverloadableCodeOperator.Mul => "*",
                OverloadableCodeOperator.Div => "/",
                OverloadableCodeOperator.Mod => "%",
                OverloadableCodeOperator.Sum => "+",
                OverloadableCodeOperator.Sub => "-",
                OverloadableCodeOperator.LeftShift => "<<",
                OverloadableCodeOperator.RightShift => ">>",
                OverloadableCodeOperator.LessThan => "<",
                OverloadableCodeOperator.LessThanOrEqual => "<=",
                OverloadableCodeOperator.GreaterThan => ">",
                OverloadableCodeOperator.GreaterThanOrEqual => ">=",
                OverloadableCodeOperator.Equal => "==",
                OverloadableCodeOperator.Unequal => "!=",
                OverloadableCodeOperator.And => "&",
                OverloadableCodeOperator.XOr => "^",
                OverloadableCodeOperator.Or => "|",
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
            };

            return res;
        }

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
		public static bool IsOneOf(this OverloadableCodeBinarySymmetricOperator source, OverloadableCodeOperators flags)
		{
			return ((int) source & (int) flags) != 0;
		}

		[Pure]
		public static bool IsNotAnyOf(this OverloadableCodeBinarySymmetricOperator source, OverloadableCodeOperators flags)
		{
			return ((int) source & (int) flags) == 0;
		}

		[Pure]
		public static bool IsOneOf(this OverloadableCodeUnarySymmetricOperator source, OverloadableCodeOperators flags)
		{
			return ((int) source & (int) flags) != 0;
		}

		[Pure]
		public static bool IsNotAnyOf(this OverloadableCodeUnarySymmetricOperator source, OverloadableCodeOperators flags)
		{
			return ((int) source & (int) flags) == 0;
		}

		[Pure]
		public static OverloadableCodeBinarySymmetricOperator ToBinarySymmetricOperator(this OverloadableCodeOperator source)
		{
			return source.IsOneOf(Binary|Symmetric)
				? (OverloadableCodeBinarySymmetricOperator) source
				: throw new ArgumentException($"The {source} is not binary symmetric operator.", nameof(source));
		}

		[Pure]
		public static OverloadableCodeUnarySymmetricOperator ToUnarySymmetricOperator(this OverloadableCodeOperator source)
		{
			return source.IsOneOf(Unary|Symmetric)
				? (OverloadableCodeUnarySymmetricOperator) source
				: throw new ArgumentException($"The {source} is not unary symmetric operator.", nameof(source));
		}

		[Pure]
		public static OverloadableCodeOperator ToCodeOperator(this OverloadableCodeBinarySymmetricOperator source)
		{
			return (OverloadableCodeOperator) source;
		}

		[Pure]
		public static OverloadableCodeOperator ToCodeOperator(this OverloadableCodeUnarySymmetricOperator source)
		{
			return (OverloadableCodeOperator) source;
		}

		[Pure]
		public static IEnumerable<OverloadableCodeOperator> ToOverloadableCodeOperatorSequence(
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