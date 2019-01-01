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
using System.Text;

namespace QuickAccess.Parser.SmartExpressions
{
	public abstract class CompositeBrick : ParsingBrick
	{
		public IReadOnlyList<ParsingBrick> Items => Bricks; 

		protected readonly ParsingBrick[] Bricks;

		/// <inheritdoc />
		public override bool IsEmpty => Bricks.All(b => b.IsEmpty);

		protected CompositeBrick(ParsingBrick b1, ParsingBrick b2, Func<CompositeBrick, bool> canFlattenPredicate)
		{
			var flatLength = GetSourceLength(b1,canFlattenPredicate) + GetSourceLength(b2,canFlattenPredicate);
			Bricks = new ParsingBrick[flatLength];
			var pos = 0;
			AddBrick(Bricks, b1,canFlattenPredicate, ref pos);
			AddBrick(Bricks, b2,canFlattenPredicate, ref pos);
		}

		protected CompositeBrick(ParsingBrick b1, ParsingBrick b2, ParsingBrick b3, Func<CompositeBrick, bool> canFlattenPredicate)
		{
			var flatLength = GetSourceLength(b1,canFlattenPredicate) + GetSourceLength(b2,canFlattenPredicate) + GetSourceLength(b3,canFlattenPredicate);
			Bricks = new ParsingBrick[flatLength];
			var pos = 0;
			AddBrick(Bricks, b1,canFlattenPredicate, ref pos);
			AddBrick(Bricks, b2,canFlattenPredicate, ref pos);
			AddBrick(Bricks, b3,canFlattenPredicate, ref pos);
		}

		protected CompositeBrick(ParsingBrick[] bricks, Func<CompositeBrick, bool> canFlattenPredicate)
		{
			var flatLength = bricks.Sum(b => GetSourceLength(b, canFlattenPredicate));
			Bricks = new ParsingBrick[flatLength];

			var pos = 0;
			var len = bricks.Length;
			for (var idx = 0; idx < len; ++idx)
			{
				AddBrick(Bricks, bricks[idx], canFlattenPredicate, ref pos);
			}
		}

		private static int GetSourceLength(ParsingBrick brick, Func<CompositeBrick, bool> canFlattenPredicate)
		{
			return canFlattenPredicate != null && brick is CompositeBrick cb && canFlattenPredicate.Invoke(cb) ? cb.Bricks.Length : 1;
		}

		private void AddBrick(ParsingBrick[] source, ParsingBrick added, Func<CompositeBrick, bool> canFlattenPredicate, ref int pos)
		{
			if (canFlattenPredicate != null && added is CompositeBrick cb && canFlattenPredicate.Invoke(cb))
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

		/// <param name="usedGroupNames"></param>
		/// <inheritdoc />
		public override string ToRegularExpressionString(Dictionary<string, int> usedGroupNames)
		{
			if (!ProvidesRegularExpression)
			{
				//return base.ToRegularExpressionString();
			}

			var sb = new StringBuilder(Bricks.Length);
			sb.Append(RegularExpressionPrefix);
			var any = false;
			foreach (var parsingBrick in Bricks)
			{
				if (any)
				{
					sb.Append(RegularExpressionSeparator);
				}
				else
				{
					any = true;
				}
				sb.Append(parsingBrick.ToRegularExpressionString(usedGroupNames));
			}
			sb.Append(RegularExpressionPostfix);
			return sb.ToString();
		}

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, ParsingBrick content, bool recursion)
		{
			foreach (var parsingBrick in Bricks)
			{
				ApplyRuleDefinition(parsingBrick, name, content, recursion);
			}
		}

		protected abstract string RegularExpressionSeparator { get; }
		protected virtual string RegularExpressionPrefix => "(?:";
		protected virtual string RegularExpressionPostfix => ")";

		/// <inheritdoc />
		public override bool ProvidesRegularExpression => RegularExpressionSeparator != null && Bricks.All(b => b.ProvidesRegularExpression);

		/// <inheritdoc />
		public override bool Equals(ParsingBrick other)
		{
			return other is CompositeBrick cb && cb.GetType() == GetType() && cb.Bricks.Length == Bricks.Length &&
			       Bricks.Select((b, i) => cb.Bricks[i].Equals(b)).All(res => res);
		}
	}
}