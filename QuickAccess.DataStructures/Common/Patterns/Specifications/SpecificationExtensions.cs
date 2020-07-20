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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace QuickAccess.DataStructures.Common.Patterns.Specifications
{
	public static class SpecificationExtensions
	{
        [Pure]
		public static Specification<T> ToSpecification<T>(this ISpecification<T> source)
		{
			return source is Specification<T> spec ? spec : new WrappedSpecification<T, T>(DefaultSpecificationAlgebra<T>.Instance, source);
		}

        [Pure]
		public static Specification<T> ToSpecification<T>(this Func<T, bool> source)
		{
			return source is Specification<T> spec ? spec : new PredicateSpecification<T>(DefaultSpecificationAlgebra<T>.Instance, source);
		}

		[Pure]
        public static IEnumerable<T> Satisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.Where(specification.IsSatisfiedBy);
        }

        [Pure]
        public static IEnumerable<T> NotSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.Where(pX => !specification.IsSatisfiedBy(pX));
        }

        [Pure]
        public static T SingleOrDefaultSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.SingleOrDefault(specification.IsSatisfiedBy);
        }

        [Pure]
        public static T SingleSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.Single(specification.IsSatisfiedBy);
        }

        [Pure]
        public static T FirstSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.First(specification.IsSatisfiedBy);
        }

        [Pure]
        public static T FirstOrDefaultSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.First(specification.IsSatisfiedBy);
        }

        [Pure]
        public static T LastSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.Last(specification.IsSatisfiedBy);
        }

        [Pure]
        public static T LastOrDefaultSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.LastOrDefault(specification.IsSatisfiedBy);
        }

        [Pure]
        public static int CountOfSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.Count(specification.IsSatisfiedBy);
        }

        [Pure]
        public static bool AnySatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.Any(specification.IsSatisfiedBy);
        }

        [Pure]
        public static bool AllSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.All(specification.IsSatisfiedBy);
        }

        [Pure]
        public static int CountOfNotSatisfying<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.Count(pX => !specification.IsSatisfiedBy(pX));
        }

        [Pure]
        public static ILookup<bool, T> ToLookupBySpecificationResult<T>(this IEnumerable<T> source, ISpecification<T> specification)
        {
            return source.ToLookup(specification.IsSatisfiedBy);
        }
    }
}