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
using System.Linq;
using QuickAccess.DataStructures.CodeOperatorAlgebra;
using QuickAccess.DataStructures.Common.Collections;

namespace QuickAccess.DataStructures.Common.Patterns.Specifications
{
	public sealed class CompositeSpecification<T> : Specification<T>, ICompositeSpecification<T>
	{
		private readonly ISpecification<T>[] _arguments;


		public CompositeSpecificationOperation Operation { get; }

		/// <inheritdoc />
		public IReadOnlyList<ISpecification<T>> Arguments => _arguments;

		/// <summary>Initializes a new instance of the <see cref="CompositeSpecification{T}"/> class.</summary>
		/// <param name="algebra">The algebra.</param>
		/// <param name="operation">The operator.</param>
		/// <param name="arguments">The arguments.</param>
		public CompositeSpecification(ISpecificationAlgebra<T> algebra,
		                              CompositeSpecificationOperation operation,
		                              params ISpecification<T>[] arguments
		)
			: base(algebra.GetHighestPrioritizedAlgebra<Specification<T>, ISpecificationAlgebra<T>>(arguments.OfType<ICodeOperatorAlgebraicDomain<Specification<T>, ISpecificationAlgebra<T>>>().Select(a => a.Algebra)), null)
		{
			Operation = operation;
			_arguments = GetFlatArgumentsArray(operation, arguments);
		}

		private static ISpecification<T>[] GetFlatArgumentsArray(CompositeSpecificationOperation operation,
		                                                         ISpecification<T>[] arguments)
		{
			if (operation != CompositeSpecificationOperation.And &&
			    operation != CompositeSpecificationOperation.Or)
			{
				return arguments;
			}

			var argsCount = GetFlatArgsCount(operation, arguments);

			if (argsCount == arguments.Length)
			{
				return arguments;
			}

			var res = new ISpecification<T>[argsCount];
			var idx = 0;
			foreach (var arg in GetFlatArgs(operation, arguments))
			{
				res[idx] = arg;
				++idx;
			}

			return res;
		}

		private static int GetFlatArgsCount(CompositeSpecificationOperation operation, IEnumerable<ISpecification<T>> specifications)
		{
			return specifications.Sum(spec => spec is ICompositeSpecification<T> composite && composite.Operation == operation
				? composite.Arguments.Count
				: 1);
		}

		private static IEnumerable<ISpecification<T>> GetFlatArgs(CompositeSpecificationOperation @operator, IEnumerable<ISpecification<T>> specifications)
		{
			foreach (var spec in specifications)
			{
				if (spec is ICompositeSpecification<T> composite && composite.Operation == @operator)
				{
					foreach (var arg in composite.Arguments)
					{
						yield return arg;
					}
				}
				else
				{
					yield return spec;
				}
			}
		}

		/// <inheritdoc />
		public override bool IsSatisfiedBy(T candidate)
		{
			switch (Operation)
			{
				case CompositeSpecificationOperation.And : return _arguments.All(arg => arg.IsSatisfiedBy(candidate));
				case CompositeSpecificationOperation.Or : return  _arguments.Any(arg => arg.IsSatisfiedBy(candidate));
				case CompositeSpecificationOperation.XOr:
				{
					var count = _arguments.Length;

					if (count <= 0)
					{
						return true;
					}

					var res = _arguments[0].IsSatisfiedBy(candidate);
					for(var idx = 1; idx < _arguments.Length; idx++)
					{
						res ^= _arguments[idx].IsSatisfiedBy(candidate);
					}

					return res;
				}
				case CompositeSpecificationOperation.Single :
					return _arguments.ExactlyOne(v => v.IsSatisfiedBy(candidate));
				case CompositeSpecificationOperation.AllButOne:
					return _arguments.AllButOne(v => v.IsSatisfiedBy(candidate));
				default:throw new InvalidOperationException($"Operator {Operation} is not supported.");
			}
		}

		/// <inheritdoc />
		public override bool IsDeMorganSimplificationCandidate()
		{
			if (Operation != CompositeSpecificationOperation.And && Operation != CompositeSpecificationOperation.Or)
			{
				return false;
			}

			return Arguments.Count(a => a is Specification<T> spec && spec.IsDeMorganSimplificationCandidate()) >=
			       Arguments.Count / 2;
		}

		/// <inheritdoc />
		public override Specification<T> GetCustomNegation()
		{
			if (!IsDeMorganSimplificationCandidate())
			{
				return null;
			}

			return new CompositeSpecification<T>(Algebra, Operation == CompositeSpecificationOperation.And ? CompositeSpecificationOperation.Or : CompositeSpecificationOperation.And, _arguments.Select(a => Algebra.GetNegation(a)).Cast<ISpecification<T>>().ToArray());
		}

		/// <inheritdoc />
		public override bool IsGeneralizationOf(ISpecificationInfo specificationInfo)
		{
			if (base.IsGeneralizationOf(specificationInfo))
			{
				return true;
			}

			return Operation == CompositeSpecificationOperation.And && _arguments.Any(a => a.IsGeneralizationOf(specificationInfo))
				|| Operation == CompositeSpecificationOperation.Or && _arguments.Any(a => a.Equals(specificationInfo));
		}

		/// <inheritdoc />
		public override string ToString()
		{
			var isCodeOperator = Operation.CanBeRepresentedByCodeOperator();
			var args = _arguments.Select(arg => arg.ToString());
			var separator = isCodeOperator ? Operation.ToCodeSymmetricBinaryOperator().ToCodeRepresentation() : ", ";
			var operation = isCodeOperator ? string.Empty : Operation.ToString();
			
			return $"{operation}({string.Join(separator, args)})";
		}
	}
}