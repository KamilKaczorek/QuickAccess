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

using QuickAccess.DataStructures.Common.RegularExpression;

namespace QuickAccess.Parser.SmartExpressions.Bricks
{
	public sealed class NegationBrick : SmartExpressionBrick
	{
		/// <inheritdoc />
		public override string Name => "Not";
		public SmartExpressionBrick Content { get; }
		/// <inheritdoc />
		public NegationBrick(ISmartExpressionAlgebra algebra, SmartExpressionBrick content) : base(algebra)
		{
			Content = content;
		}

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, SmartExpressionBrick content, bool recursion, bool freeze)
		{
			ApplyRuleDefinition(Content, name, content, recursion, freeze);
		}

		/// <inheritdoc />
		public override string ExpressionId => $"$!{Content.ExpressionId}";

		/// <inheritdoc />
		public override bool Equals(SmartExpressionBrick other)
		{
			return other is NegationBrick nb && nb.Content.Equals(Content);
		}

		/// <inheritdoc />
		public override MatchingLevel RegularExpressionMatchingLevel =>
			Content?.RegularExpressionMatchingLevel ?? MatchingLevel.None;

		/// <inheritdoc />
		public override string ToRegularExpressionString(RegularExpressionBuildingContext ctx)
		{
			return ctx.Factory.CreateNot(ctx.Context, Content.ToRegularExpressionString(ctx));
		}

		protected override IParsedExpressionNode TryParseInternal(IParsingContextStream ctx)
		{
			var res = Content.TryParse(ctx);

			return res != null ? null : new EmptyNode(ctx);
		}
	}
}