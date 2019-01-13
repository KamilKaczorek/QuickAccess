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
	public static class Specification
	{
		public static ISpecificationAlgebra<T> GetDefaultAlgebra<T>()
		{
			return DefaultSpecificationAlgebra<T>.Instance;
		}

		public static Specification<T> FromPredicate<T>(Func<T, bool> predicate)
		{
			return predicate.ToSpecification();
		}

		public static Specification<T> GetTrue<T>()
		{
			return new TrueSpecification<T>(GetDefaultAlgebra<T>());
		}

		public static Specification<T> GetFalse<T>()
		{
			return new FalseSpecification<T>(GetDefaultAlgebra<T>());
		}
	}

	public abstract class Specification<T> : ISpecification<T>, IEquatable<Specification<T>>, ICodeOperatorAlgebraicDomain<Specification<T>, ISpecificationAlgebra<T>>
	{
		/// <inheritdoc />
		public abstract bool IsSatisfiedBy(T candidate);

		/// <inheritdoc />
		public virtual ISpecificationAlgebra<T> Algebra { get; }

		/// <inheritdoc />
		public virtual SpecificationDescriptor Descriptor { get; }

		public bool this [T candidate] => IsSatisfiedBy(candidate);

		protected Specification(ISpecificationAlgebra<T> algebra, SpecificationDescriptor descriptor = null)
		{
			Descriptor = descriptor ?? CreateDefaultDescriptor();
			Algebra = algebra ?? new DefaultSpecificationAlgebra<T>();
		}

		/// <inheritdoc />
		public virtual bool IsGeneralizationOf(ISpecificationInfo specificationInfo)
		{
			if (specificationInfo is TrueSpecification<T>)
			{
				return true;
			}

			return Equals(specificationInfo);
		}

		/// <inheritdoc />
		public virtual bool Equals(ISpecificationInfo other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Descriptor.Equals(other.Descriptor);
		}

		/// <inheritdoc />
		public bool Equals(Specification<T> other)
		{
			return Equals((ISpecificationInfo) other);
		}

		/// <inheritdoc />
		public sealed override bool Equals(object obj)
		{
			return obj is ISpecificationInfo other && Equals(other);
		}

		public static Specification<T> operator &(Specification<T> left, Specification<T> right)
		{
			return left.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.And, right);
		}

		public static Specification<T> operator |(Specification<T> left, Specification<T> right)
		{
			return left.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.Or, right);
		}

		public static Specification<T> operator ^(Specification<T> left, Specification<T> right)
		{
			return left.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.XOr, right);
		}

		public static Specification<T> operator &(Func<T, bool> left, Specification<T> right)
		{
			return right.Algebra.EvaluateOperatorResult(left.ToSpecification(), OverloadableCodeBinarySymmetricOperator.And, right);
		}

		public static Specification<T> operator |(Func<T, bool> left, Specification<T> right)
		{
			return right.Algebra.EvaluateOperatorResult(left.ToSpecification(), OverloadableCodeBinarySymmetricOperator.Or, right);
		}

		public static Specification<T> operator ^(Func<T, bool> left, Specification<T> right)
		{
			return right.Algebra.EvaluateOperatorResult(left.ToSpecification(), OverloadableCodeBinarySymmetricOperator.XOr, right);
		}

		public static Specification<T> operator &(Specification<T> left, Func<T, bool> right)
		{
			return left.Algebra.EvaluateOperatorResult(left, OverloadableCodeBinarySymmetricOperator.And, right.ToSpecification());
		}

		public static Specification<T> operator |(Specification<T> left, Func<T, bool> right)
		{
			return left.Algebra.EvaluateOperatorResult(left, OverloadableCodeBinarySymmetricOperator.Or, right.ToSpecification());
		}

		public static Specification<T> operator ^(Specification<T> left, Func<T, bool> right)
		{
			return left.Algebra.EvaluateOperatorResult(left, OverloadableCodeBinarySymmetricOperator.XOr, right.ToSpecification());
		}

		public static Specification<T> operator !(Specification<T> arg)
		{
			return arg.GetOperatorResult(OverloadableCodeUnarySymmetricOperator.LogicalNegation);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return Descriptor.GetHashCode();
		}

		public static bool operator ==(Specification<T> left, Specification<T> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Specification<T> left, Specification<T> right)
		{
			return !Equals(left, right);
		}

		public virtual bool IsDeMorganSimplificationCandidate()
		{
			return false;
		}

		public virtual Specification<T> GetCustomNegation()
		{
			return null;
		}

		public virtual Specification<TDest> OfCandidate<TDest>()
			where TDest : T
		{
			return new WrappedSpecification<T, TDest>(Algebra.OfCandidate<TDest>(), this);
		}

		public virtual Specification<TDest> OfCandidate<TDest>(Func<TDest, T> convertCandidateCallback)	
		{
			return new CandidateConverterSpecification<T,TDest>(Algebra.OfCandidate<TDest>(), this, convertCandidateCallback);
		}

		private SpecificationDescriptor CreateDefaultDescriptor()
		{
			var candidateType = typeof(T);
			var specificationTypeName = GetType().Name;
			var specificationName = GetSpecificationNameRemovingSpecificationTag(specificationTypeName);

			return new SpecificationDescriptor(Guid.NewGuid().ToString(), specificationName, specificationTypeName, candidateType.FullName);
		}

		protected static string GetSpecificationNameRemovingSpecificationTag(string name)
		{
			return name.Replace("Specification", "");
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return Descriptor.ToString();
		}
	}
}