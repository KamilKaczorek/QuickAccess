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

namespace QuickAccess.DataStructures.Algebra
{
	public static class CodeOperatorAlgebraExtensions
	{
		[Pure]
		public static T GetOperatorResultOfHighestPrioritizedAlgebra<T>(this T left, OverloadableCodeBinarySymmetricOperator binaryOperator, T right)
			where T : IDefineAlgebraicDomain<T, IDefineCodeOperatorSymmetricAlgebra<T>>
		{
			return GetHighestPrioritizedAlgebra<T, IDefineCodeOperatorSymmetricAlgebra<T>>(left, right).EvaluateOperatorResult(left, binaryOperator, right);
		}

		[Pure]
		public static T GetOperatorResultOfHighestPrioritizedAlgebra<T>(this IDefineCodeOperatorSymmetricAlgebra<T> defaultAlgebra,  OverloadableCodeBinarySymmetricOperator binaryOperator, T left, T right)
			where T : IDefineAlgebraicDomain<T, IDefineCodeOperatorSymmetricAlgebra<T>>
		{
			return defaultAlgebra.GetHighestPrioritizedAlgebra(left, right).EvaluateOperatorResult(left, binaryOperator, right);
		}

       
		[Pure]
		public static T GetOperatorResultOfHighestPrioritizedAlgebra<T>(this IDefineCodeOperatorSymmetricAlgebra<T> defaultAlgebra, OverloadableCodeUnarySymmetricOperator unaryOperator, T arg)
			where T : IDefineAlgebraicDomain<T, IDefineCodeOperatorSymmetricAlgebra<T>>
		{
			return defaultAlgebra.GetHighestPrioritizedAlgebra(arg).EvaluateOperatorResult(unaryOperator, arg);
		}

		[Pure]
		public static T GetOperatorResult<T>(this T arg, OverloadableCodeUnarySymmetricOperator unaryOperator)
			where T : IDefineAlgebraicDomain<T, IDefineCodeOperatorSymmetricAlgebra<T>>
		{
			return arg.Algebra.EvaluateOperatorResult(unaryOperator, arg);
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this TAlgebra algebra1, TAlgebra algebra2)
		where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			var arg1Prior = algebra1?.Priority ?? int.MinValue;
			var arg2Prior = algebra2?.Priority ?? int.MinValue;

			return arg1Prior >= arg2Prior ? algebra1 ?? algebra2 : algebra2;
		}

      

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, IEnumerable<TAlgebra> algebras)
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			var prior = defaultAlgebra?.Priority ?? int.MinValue;
			var res = defaultAlgebra;
			foreach (var algebra in algebras)
			{
				if (algebra != null && (algebra.Priority > prior || res == null))
				{
					res = algebra;
				}
			}

			return res;
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this TAlgebra algebra1, TAlgebra algebra2, TAlgebra algebra3)
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			return GetHighestPrioritizedAlgebra<T, TAlgebra>(GetHighestPrioritizedAlgebra<T, TAlgebra>(algebra1, algebra2), algebra3);
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, T arg1)
			where T : IDefineAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			return GetHighestPrioritizedAlgebra<T, TAlgebra>(arg1?.Algebra, defaultAlgebra);
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this T arg1, T arg2)
			where T : IDefineAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			return GetHighestPrioritizedAlgebra<T, TAlgebra>(arg1?.Algebra, arg2?.Algebra);
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, T arg1, T arg2)
			where T : IDefineAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			return GetHighestPrioritizedAlgebra<T, TAlgebra>(GetHighestPrioritizedAlgebra<T, TAlgebra>(arg1?.Algebra, arg2?.Algebra), defaultAlgebra);
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, T arg1, T arg2, T arg3)
			where T : IDefineAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			return GetHighestPrioritizedAlgebra<T, TAlgebra>(GetHighestPrioritizedAlgebra<T, TAlgebra>(arg1?.Algebra, arg2?.Algebra, arg3?.Algebra), defaultAlgebra);
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, IEnumerable<T> args)
			where T : IDefineAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			var prior = defaultAlgebra?.Priority ?? int.MinValue;
			var res = defaultAlgebra;
			foreach (var algebra in args.Where(a => a?.Algebra != null).Select(a => a.Algebra))
			{
				if (algebra.Priority > prior || res == null)
				{
					res = algebra;
				}
			}

			return res;
		}

		[Pure]
		public static TAlgebra GetHighestPrioritizedAlgebra<T, TAlgebra>(this IEnumerable<TAlgebra> algebras, Func<TAlgebra> defaultAlgebraProvidingCallback)
			where T : IDefineAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IDefineCodeOperatorSymmetricAlgebra<T>
		{
			var prior = int.MinValue;
			TAlgebra res = null;
			foreach (var algebra in algebras.Where(a => a != null))
			{
				if (algebra.Priority > prior || res == null)
				{
					res = algebra;
				}
			}

			return res ?? defaultAlgebraProvidingCallback.Invoke();
		}
	}
}