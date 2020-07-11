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

namespace QuickAccess.DataStructures.Common.RegularExpression
{
	public enum MatchingLevel
	{
		/// <summary>There is no regex representation of an object.</summary>
		/// 
		None = 0,

		/// <summary>The regex defines only placeholder for the object expression (e.g.: ".+" when parsing float number).</summary>
		/// 
		Placeholder = 1,

		/// <summary>The regex defines generalized pattern (not exact) (e.g. "[\d\.]+" when parsing float number)</summary>
		Generalized = 2,

		/// <summary>The regex defines exact pattern (e.g. "\d+(?:\.\d+)?" when parsing float number)</summary>
		Exact = 3
	}

	public static class MatchingLevelExtensions
	{
		public static MatchingLevel GetMinimalMatchingLevel(
			this IEnumerable<IRepresentRegularExpression> regularExpressionRepresentableItems)
		{
			return  regularExpressionRepresentableItems.Select(r => r.RegularExpressionMatchingLevel).GetMinimal();
		}

		public static MatchingLevel GetMinimal(
			this IEnumerable<MatchingLevel> source)
		{
			var min = MatchingLevel.Exact;

			foreach (var matchingLevel in source)
			{
				if (matchingLevel == MatchingLevel.None)
				{
					return MatchingLevel.None;
				}

				if (min > matchingLevel)
				{
					min = matchingLevel;
				}
			}

			return min;
		}
	}
}