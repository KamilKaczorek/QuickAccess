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
// Project: QuickAccess.Parser
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common.RegularExpression;

namespace QuickAccess.Parser.SmartExpressions.Bricks
{
	public sealed class QuantifierBrick : SmartExpressionBrick
	{
		public long Min { get; }
		public long Max { get; }
		public SmartExpressionBrick Content { get; }

		public override bool IsEmpty => (Min == 0 && Max == 0) || Content.IsEmpty;

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, SmartExpressionBrick content, bool recursion)
		{
			ApplyRuleDefinition(Content, name, content, recursion);
		}

		public QuantifierBrick(ISmartExpressionAlgebra algebra, SmartExpressionBrick content, long min, long max)
		: base(algebra.GetAlgebra(content))
		{
			Content = content ?? throw new ArgumentNullException(nameof(content));

			if (min < 0)
			{
				throw new ArgumentException($"Min value {min} is smaller than 0.", nameof(min));
			}

			if (max < 0)
			{
				throw new ArgumentException($"Max value {max} is smaller than 0.", nameof(max));
			}

			if (max < min)
			{
				throw new ArgumentException($"Max value {max} is smaller than min value {min}.", nameof(max));
			}

			Min = min;
			Max = max;
		}

		/// <inheritdoc />
		public override string ExpressionId => $"${Content.ExpressionId}${{{Min}${Max}}}$";

		public override string ToRegularExpressionString(RegularExpressionBuildingContext ctx)
		{
			return ctx.Factory.CreateQuantifier(ctx.Context, Min, Max, Content.ToRegularExpressionString(ctx));
		}		

		/// <inheritdoc />
		public override bool Equals(SmartExpressionBrick other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			if (ReferenceEquals(this, other) || (other.IsEmpty && IsEmpty))
			{
				return true;
			}

			return other is QuantifierBrick qb && qb.Min == Min && qb.Max == Max && qb.Content.Equals(Content);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			var contentExpression = Content.ToString();

			if (IsEmpty)
			{
				return SX.Empty.ToString();
			}

			if (Min == 1 && Max == 1)
			{
				return contentExpression;
			}

			if (Min == 0 && Max == 1)
			{
				return $"Optional({contentExpression})";
			}

			if (Min == 0 && Max == long.MaxValue)
			{
				return $"ZeroOrMany({contentExpression})";
			}

			if (Min == 1 && Max == long.MaxValue)
			{
				return $"OneOrMany({contentExpression})";
			}

			if (Min == Max)
			{
				return $"Repeated{Min}Times({contentExpression})";
			}

			return $"RepeatedBetween{Min}And{Max}Times({contentExpression})";
		}
	}
}