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

namespace QuickAccess.DataStructures.Common.Patterns.Specifications
{
	public sealed class WrappedSpecification<TWrapped, T> : Specification<T>
		where T : TWrapped
	{
		private readonly ISpecification<TWrapped> _wrapped;

		public WrappedSpecification(ISpecificationAlgebra<T> algebra, ISpecification<TWrapped> wrapped) : base(algebra, wrapped.Descriptor)
		{
			_wrapped = wrapped;
		}

		/// <inheritdoc />
		public override bool IsGeneralizationOf(ISpecificationInfo specificationInfo)
		{
			return _wrapped.IsGeneralizationOf(specificationInfo);
		}

		/// <inheritdoc />
		public override bool IsSatisfiedBy(T candidate)
		{
			return _wrapped.IsSatisfiedBy(candidate);
		}

		/// <inheritdoc />
		public override Specification<T> GetCustomNegation()
		{
			if (_wrapped is Specification<TWrapped> wrapped)
			{
				var custom = wrapped.GetCustomNegation();

				if (custom != null)
				{
					return new WrappedSpecification<TWrapped, T>(Algebra, custom);
				}
			}

			return _wrapped is ISpecification<T> spec ? new NotSpecification<T>(Algebra, spec) : null;
		}

		/// <inheritdoc />
		public override bool IsDeMorganSimplificationCandidate()
		{
			return _wrapped is Specification<TWrapped> spec && spec.IsDeMorganSimplificationCandidate();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return _wrapped.ToString();
		}
	}
}