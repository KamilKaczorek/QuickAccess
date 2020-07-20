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

namespace QuickAccess.DataStructures.Common.Patterns.Specifications
{
	public sealed class CandidateConverterSpecification<TSource, TDestination> : Specification<TDestination>
	{
		private readonly Func<TDestination, TSource> _convertCandidateCallback;
		private readonly ISpecification<TSource> _original;

		public CandidateConverterSpecification(ISpecificationAlgebra<TDestination> algebra, ISpecification<TSource> original, Func<TDestination, TSource> convertCandidateCallback) : base(algebra, original.Descriptor)
		{
			_original = original;
			_convertCandidateCallback = convertCandidateCallback;
		}

		/// <inheritdoc />
		public override bool IsGeneralizationOf(ISpecificationInfo specificationInfo)
		{
			return _original.IsGeneralizationOf(specificationInfo);
		}

		/// <inheritdoc />
		public override bool IsSatisfiedBy(TDestination candidate)
		{
			return _original.IsSatisfiedBy(_convertCandidateCallback.Invoke(candidate));
		}

		/// <inheritdoc />
		public override Specification<TDestination> GetCustomNegation()
		{
			if (_original is Specification<TSource> wrapped)
			{
				var custom = wrapped.GetCustomNegation();

				if (custom != null)
				{
					return new CandidateConverterSpecification<TSource, TDestination>(Algebra, custom, _convertCandidateCallback);
				}
			}

			return null;
		}

		/// <inheritdoc />
		public override bool IsDeMorganSimplificationCandidate()
		{
			return _original is Specification<TSource> spec && spec.IsDeMorganSimplificationCandidate();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return _original.ToString();
		}
	}
}