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
using System.Collections.Generic;
using System.Linq;

namespace QuickAccess.Parser.Flexpressions.Bricks
{
	public abstract class CompositeFlexpressionBrick : FlexpressionBrick
	{
		public IReadOnlyList<FlexpressionBrick> Items => Bricks; 

		protected readonly FlexpressionBrick[] Bricks;

		/// <inheritdoc />
		public override bool IsEmpty => Bricks.All(b => b.IsEmpty);

		protected CompositeFlexpressionBrick(IFlexpressionAlgebra algebra, FlexpressionBrick b1, FlexpressionBrick b2, Func<CompositeFlexpressionBrick, bool> canFlattenPredicate)
		 :base(algebra.GetHighestPrioritizedAlgebra(b1, b2))
		{
			var flatLength = GetSourceLength(b1,canFlattenPredicate) + GetSourceLength(b2,canFlattenPredicate);
			Bricks = new FlexpressionBrick[flatLength];
			var pos = 0;
			AddBrick(Bricks, b1,canFlattenPredicate, ref pos);
			AddBrick(Bricks, b2,canFlattenPredicate, ref pos);
		}

		protected CompositeFlexpressionBrick(IFlexpressionAlgebra algebra, FlexpressionBrick b1, FlexpressionBrick b2, FlexpressionBrick b3, Func<CompositeFlexpressionBrick, bool> canFlattenPredicate)
			:base(algebra.GetHighestPrioritizedAlgebra(b1, b2, b3))
		{
			var flatLength = GetSourceLength(b1,canFlattenPredicate) + GetSourceLength(b2,canFlattenPredicate) + GetSourceLength(b3,canFlattenPredicate);
			Bricks = new FlexpressionBrick[flatLength];
			var pos = 0;
			AddBrick(Bricks, b1,canFlattenPredicate, ref pos);
			AddBrick(Bricks, b2,canFlattenPredicate, ref pos);
			AddBrick(Bricks, b3,canFlattenPredicate, ref pos);
		}

		protected CompositeFlexpressionBrick(IFlexpressionAlgebra algebra, FlexpressionBrick[] bricks, Func<CompositeFlexpressionBrick, bool> canFlattenPredicate)
			:base(algebra.GetHighestPrioritizedAlgebra(bricks))
		{
			var flatLength = bricks.Sum(b => GetSourceLength(b, canFlattenPredicate));
			Bricks = new FlexpressionBrick[flatLength];

			var pos = 0;
			var len = bricks.Length;
			for (var idx = 0; idx < len; ++idx)
			{
				AddBrick(Bricks, bricks[idx], canFlattenPredicate, ref pos);
			}
		}

		private static int GetSourceLength(FlexpressionBrick brick, Func<CompositeFlexpressionBrick, bool> canFlattenPredicate)
		{
			return canFlattenPredicate != null && brick is CompositeFlexpressionBrick cb && canFlattenPredicate.Invoke(cb) ? cb.Bricks.Length : 1;
		}

		private void AddBrick(FlexpressionBrick[] source, FlexpressionBrick added, Func<CompositeFlexpressionBrick, bool> canFlattenPredicate, ref int pos)
		{
			if (canFlattenPredicate != null && added is CompositeFlexpressionBrick cb && canFlattenPredicate.Invoke(cb))
			{
				var len = cb.Bricks.Length;
				Array.Copy(cb.Bricks, 0, source, pos, len);
				pos += len;
			}
			else
			{
				source[pos] = added;
				++pos;
			}
		}

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, FlexpressionBrick content, bool recursion, bool freeze)
		{
			foreach (var parsingBrick in Bricks)
			{
				ApplyRuleDefinition(parsingBrick, name, content, recursion, freeze);
			}
		}

		/// <inheritdoc />
		public override bool Equals(FlexpressionBrick other)
		{
			return other is CompositeFlexpressionBrick cb && cb.GetType() == GetType() && cb.Bricks.Length == Bricks.Length &&
			       Bricks.Select((b, i) => cb.Bricks[i].Equals(b)).All(res => res);
		}
	}
}