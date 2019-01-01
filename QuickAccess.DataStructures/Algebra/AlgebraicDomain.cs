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

using System.Collections.Generic;
using System.Linq;

namespace QuickAccess.DataStructures.Algebra
{
	public static class AlgebraicDomain
	{
		public static T Calculate<T>(this T left, BinaryOperator binaryOperator, T right)
			where T : IAlgebraicDomain<T, IAlgebra<T>>
		{
			return GetAlgebra<T, IAlgebra<T>>(left, right).EvaluateOperatorResult(left, binaryOperator, right);
		}

		public static T Calculate<T>(this IAlgebra<T> defaultAlgebra, T left,  BinaryOperator binaryOperator, T right)
			where T : IAlgebraicDomain<T, IAlgebra<T>>
		{
			return defaultAlgebra.GetAlgebra(left, right).EvaluateOperatorResult(left, binaryOperator, right);
		}

		public static T Calculate<T>(this IAlgebra<T> defaultAlgebra, UnaryOperator unaryOperator, T arg)
			where T : IAlgebraicDomain<T, IAlgebra<T>>
		{
			return defaultAlgebra.GetAlgebra(arg).EvaluateOperatorResult(unaryOperator, arg);
		}

		public static TAlgebra GetAlgebra<T, TAlgebra>(TAlgebra algebra1, TAlgebra algebra2)
		where TAlgebra : class, IAlgebra<T>
		{
			var arg1Prior = algebra1?.Priority ?? int.MinValue;
			var arg2Prior = algebra2?.Priority ?? int.MinValue;

			return arg1Prior >= arg2Prior ? algebra1 ?? algebra2 : algebra2;
		}


		public static TAlgebra GetAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, IEnumerable<TAlgebra> algebras)
			where TAlgebra : class, IAlgebra<T>
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

		public static TAlgebra GetAlgebra<T, TAlgebra>(TAlgebra algebra1, TAlgebra algebra2, TAlgebra algebra3)
			where TAlgebra : class, IAlgebra<T>
		{
			return GetAlgebra<T, TAlgebra>(GetAlgebra<T, TAlgebra>(algebra1, algebra2), algebra3);
		}

		public static TAlgebra GetAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, T arg1)
			where T : IAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IAlgebra<T>
		{
			return GetAlgebra<T, TAlgebra>(arg1?.Algebra, defaultAlgebra);
		}

		public static TAlgebra GetAlgebra<T, TAlgebra>(T arg1, T arg2)
			where T : IAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IAlgebra<T>
		{
			return GetAlgebra<T, TAlgebra>(arg1?.Algebra, arg2?.Algebra);
		}

		public static TAlgebra GetAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, T arg1, T arg2)
			where T : IAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IAlgebra<T>
		{
			return GetAlgebra<T, TAlgebra>(GetAlgebra<T, TAlgebra>(arg1?.Algebra, arg2?.Algebra), defaultAlgebra);
		}

		public static TAlgebra GetAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, T arg1, T arg2, T arg3)
			where T : IAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IAlgebra<T>
		{
			return GetAlgebra<T, TAlgebra>(GetAlgebra<T, TAlgebra>(arg1?.Algebra, arg2?.Algebra, arg3?.Algebra), defaultAlgebra);
		}

		public static TAlgebra GetAlgebra<T, TAlgebra>(this TAlgebra defaultAlgebra, IEnumerable<T> args)
			where T : IAlgebraicDomain<T, TAlgebra>
			where TAlgebra : class, IAlgebra<T>
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
	}
}