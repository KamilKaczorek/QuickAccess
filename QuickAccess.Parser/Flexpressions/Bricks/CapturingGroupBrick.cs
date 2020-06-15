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

using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions.Bricks
{
	public sealed class CapturingGroupBrick : FlexpressionBrick
	{
		/// <inheritdoc />
		public override string Name => GroupName;

        public string GroupName => _expressionType.Name;

		
		public FlexpressionBrick Content { get; }

        private readonly ExpressionTypeDescriptor _expressionType;

		/// <inheritdoc />
		public override bool IsEmpty => Content.IsEmpty;

		public CapturingGroupBrick(ExpressionTypeDescriptor expressionType, IFlexpressionAlgebra algebra, FlexpressionBrick content, bool freeze) 
			: base(algebra)
        {
			Guard.ArgNotNull(expressionType, nameof(expressionType));

            _expressionType = expressionType;
			Content = content;
            ApplyRuleDefinition(Content, GroupName, content, true, freeze);
		}

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, FlexpressionBrick content, bool recursion, bool freeze)
		{
			if (name == GroupName)
			{
				return;
			}

			ApplyRuleDefinition(Content, name, content, recursion, freeze);
		}

        public CapturingGroupBrick Clone(IFlexpressionAlgebra algebra, string groupName, bool freeze = false)
        {
            var expressionType = string.Equals(groupName, _expressionType.Name)
                ? _expressionType
                : ExpressionTypeDescriptor.Create(groupName, _expressionType.ValueTypeId, _expressionType.DefinesExpressionClass);

			return new CapturingGroupBrick(expressionType, algebra.GetHighestPrioritizedAlgebra(this), Content, freeze);
		}

		public CapturingGroupBrick Clone(IFlexpressionAlgebra algebra, bool freeze = false)
		{
			return new CapturingGroupBrick(_expressionType, algebra.GetHighestPrioritizedAlgebra(this), Content, freeze);
		}

		public override string ToRegularExpressionString(RegularExpressionBuildingContext ctx)
		{
			return string.IsNullOrEmpty(GroupName)
				? ctx.Factory.CreateCapturingGroup(ctx.Context, Content.ToRegularExpressionString(ctx))
				: ctx.Factory.CreateNamedGroup(ctx.Context, GroupName, Content.ToRegularExpressionString(ctx), out _);
		}

		/// <inheritdoc />
		public override bool Equals(FlexpressionBrick other)
		{
			if (IsEmpty && (other?.IsEmpty ?? false))
			{
				return true;
			}

			return other is CapturingGroupBrick cb && cb.GroupName == GroupName && cb.Content.Equals(Content);
		}

		/// <inheritdoc />
		protected override IParsingProduct TryParseInternal(IParsingContextStream ctx)
		{
			var res = Content.TryParse(ctx);

            if (res == null)
            {
                return null;
            }

            var overwriteExpressionType = res.ExpressionType.ValueTypeId != null && _expressionType.ValueTypeId == null;

            var expressionType = overwriteExpressionType
                ? ExpressionTypeDescriptor.Create(_expressionType.Name, res.ExpressionType.ValueTypeId,
                    _expressionType.DefinesExpressionClass) : _expressionType;

            var node = ctx.Accept().CreateExpressionForAcceptedFragment(
                expressionType,
                res.ToList());

			return node;
        }

		/// <inheritdoc />
		public override string ToString()
		{
            if (Content is CapturingGroupBrick cgb)
            {
                return $"<{GroupName}> ::= <{cgb.GroupName}>\n{Content}";
            }

			return $"<{GroupName}> ::= {Content}";
		}
	}
}