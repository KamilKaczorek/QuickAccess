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
using QuickAccess.DataStructures.CodeOperatorAlgebra;

namespace QuickAccess.DataStructures.Common.Patterns.Specifications
{
	public sealed class DefaultSpecificationAlgebra<T> : ISpecificationAlgebra<T>
	{
		public static readonly ISpecificationAlgebra<T> Instance = new DefaultSpecificationAlgebra<T>();

		/// <inheritdoc />
		public Type BaseDomainType => typeof(ISpecification<T>);

		/// <inheritdoc />
		public int Priority => 0;

		/// <inheritdoc />
		public bool IsDomainSupported(Type domainType)
		{
			return typeof(ISpecification<T>).IsAssignableFrom(domainType);
		}

		/// <inheritdoc />
		public OverloadableCodeOperators SupportedOperators => OverloadableCodeOperators.And | OverloadableCodeOperators.Or |
		                                                       OverloadableCodeOperators.XOr |
		                                                       OverloadableCodeOperators.LogicalNegation;

		/// <inheritdoc />
		public bool IsUnaryOperatorSupported(OverloadableCodeSymmetricUnaryOperator unaryOperator)
		{
			return ((int) unaryOperator & (int) SupportedOperators) != 0;
		}

		/// <inheritdoc />
		public bool IsBinaryOperatorSupported(OverloadableCodeSymmetricBinaryOperator binaryOperator)
		{
			return ((int) binaryOperator & (int) SupportedOperators) != 0;
		}

		/// <inheritdoc />
		public string GetBinaryOperatorDescription(OverloadableCodeSymmetricBinaryOperator binaryOperator)
		{
			switch (binaryOperator)
			{
				case OverloadableCodeSymmetricBinaryOperator.And : return $"Creates composite specification where the overall result is a disjunction of results of aggregated specifications.";
				case OverloadableCodeSymmetricBinaryOperator.Or : return  $"Creates composite specification where the overall result is a conjunction of results of aggregated specifications.";
				case OverloadableCodeSymmetricBinaryOperator.XOr : return  $"Creates composite specification where the overall result is a exclusive OR of results of aggregated specifications.";
				default:throw new NotSupportedException($"Operator {binaryOperator} is not supported.");
			}
		}

		/// <inheritdoc />
		public string GetUnaryOperatorDescription(OverloadableCodeSymmetricUnaryOperator unaryOperator)
		{
			switch (unaryOperator)
			{
				case OverloadableCodeSymmetricUnaryOperator.LogicalNegation :
					return
						"Creates specification where the specification result will be negation of a result of aggregated specification.";
				default:throw new NotSupportedException($"Operator {unaryOperator} is not supported.");
			}
		}

		/// <inheritdoc />
		public bool TryEvaluateOperatorResult(object left,
		                                      OverloadableCodeSymmetricBinaryOperator binaryOperator,
		                                      object right,
		                                      out object result)
		{
			if (left is ISpecification<T> ls && right is ISpecification<T> rs)
			{
				result = EvaluateOperatorResult(ls.ToSpecification(), binaryOperator, rs.ToSpecification());
				return true;
			}

			result = default;
			return false;
		}

		/// <inheritdoc />
		public bool TryEvaluateOperatorResult(OverloadableCodeSymmetricUnaryOperator unaryOperator, object arg, out object result)
		{
			if (arg is ISpecification<T> spec)
			{
				result = EvaluateOperatorResult(unaryOperator, spec.ToSpecification());
				return true;
			}

			result = default;
			return false;
		}

		/// <inheritdoc />
		public Specification<T> EvaluateOperatorResult(Specification<T> left,
		                                               OverloadableCodeSymmetricBinaryOperator binaryOperator,
		                                               Specification<T> right)
		{
			return new CompositeSpecification<T>(this, binaryOperator.ToCompositeSpecificationOperator(), left, right);
		}

		/// <inheritdoc />
		public Specification<T> EvaluateOperatorResult(OverloadableCodeSymmetricUnaryOperator unaryOperator, Specification<T> arg)
		{
			switch (unaryOperator)
			{
				case OverloadableCodeSymmetricUnaryOperator.LogicalNegation :
					return GetNegation(arg);
				default:throw new NotSupportedException($"Operator {unaryOperator} is not supported.");
			}
		}

		/// <inheritdoc />
		public Specification<T> GetNegation(ISpecification<T> arg)
		{
			return (arg is Specification<T> spec ? spec.GetCustomNegation() : null) ?? new NotSpecification<T>(this, arg);
		}
	}
}