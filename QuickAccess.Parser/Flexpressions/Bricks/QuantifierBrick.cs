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
// Project: QuickAccess.Parser
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions.Bricks
{
	public sealed class QuantifierBrick : FlexpressionBrick
	{
		public Quantifier Quantity { get; }
		public FlexpressionBrick Content { get; }

		public override bool IsEmpty => Quantity.IsEmpty || Content.IsEmpty;

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, FlexpressionBrick content, bool recursion, bool freeze)
		{
			ApplyRuleDefinition(Content, name, content, recursion, freeze);
		}

		public QuantifierBrick(IFlexpressionAlgebra algebra, FlexpressionBrick content, in Quantifier quantity)
		: base(algebra.GetHighestPrioritizedAlgebra(content))
		{
			Content = content ?? throw new ArgumentNullException(nameof(content));
            Quantity = quantity.ToDefinedRange();
        }

		/// <inheritdoc />
		//public override string ExpressionId => $"${Content.ExpressionId}${{{Min}${Max}}}$";

		public override string ToRegularExpressionString(RegularExpressionBuildingContext ctx)
		{
			return ctx.Factory.CreateQuantifier(ctx.Context, Quantity, Content.ToRegularExpressionString(ctx));
		}		

		/// <inheritdoc />
		public override bool Equals(FlexpressionBrick other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other) || (other.IsEmpty && IsEmpty))
			{
				return true;
			}

			return other is QuantifierBrick qb && Quantity.Equals(qb.Quantity) && qb.Content.Equals(Content);
		}

		/// <inheritdoc />
		protected override IParsingProduct TryParseInternal(IParsingContextStream ctx, ParsingOptions options)
        {
            var min = Quantity.Min.MaxUInt;
            var max = Quantity.Max.MaxUInt;

			if (min == 1 && max == 1)
			{
				return Content.TryParse(ctx, options);
			}

			if (min == 0 && max == 1)
			{
				return Content.TryParse(ctx, options) ?? new EmptyNode(ctx);
			}

			List<IParsingProduct> nodes = null;

            var infinite = Quantity.Max.IsInfinite;

			for (var idx = 0; idx <= max || infinite; idx++)
			{
				var res = idx < max ? Content.TryParse(ctx, options) : null;

				if (res == null)
				{
					if (idx >= min)
                    {
                        ctx.Accept();
						return nodes != null
							? ctx.CreateExpressionForAcceptedFragment(FXB.ExpressionTypes.Concatenation, nodes)
							: new EmptyNode(ctx);
					}
					
					return null;
				}

				nodes ??= new List<IParsingProduct>((int)Math.Min(Math.Max(min, 16), max));

				if (!EmptyNode.IsEmptyNode(res))
				{
					nodes.Add(res);
				}
			}

			Guard.CodeNeverReached();
            return null;
        }

		/// <inheritdoc />
		public override string ToString()
		{
			if (IsEmpty)
			{
				return FXB.Empty.ToString();
			}

            var contentExpression = Content.ToString();


            var min = Quantity.Min.MaxUInt;
            var max = Quantity.Max.MaxUInt;

			if (min == 1 && max == 1)
			{
				return contentExpression;
			}

			if (min == 0 && max == 1)
			{
				return $"Optional({contentExpression})";
			}

			if (min == 0 && Quantity.Max.IsInfinite)
			{
				return $"ZeroOrMany({contentExpression})";
			}

			if (min == 1 && Quantity.Max.IsInfinite)
			{
				return $"OneOrMany({contentExpression})";
			}

			if (min == max)
			{
				return $"Repeated{min}Times({contentExpression})";
			}

			return $"RepeatedBetween{min}And{max}Times({contentExpression})";
		}
	}
}