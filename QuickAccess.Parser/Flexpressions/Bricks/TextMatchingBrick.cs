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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions.Bricks
{
	public sealed class TextMatchingBrick : FlexpressionBrick
	{
		public string Text { get; }

		private static readonly HashSet<char> SpecialRegexCharacters = new HashSet<char>{'\\','^','$','.','|','?','*','+','(',')','{','}'};

		public static bool IsRegexSpecial(char ch)
		{
			return SpecialRegexCharacters.Contains(ch);
		}

		public static bool IsRegexSpecialOrTab(char ch)
		{
			return SpecialRegexCharacters.Contains(ch) || ch == '\t';
		}

		public static string CharToRegex(char ch)
		{
			return IsRegexSpecial(ch) ? $@"\{ch}" : ch == '\t' ? @"\t" : ch.ToString();
		}

		public static string StringToRegex(string text)
		{
			var specialCount = text.Count(IsRegexSpecialOrTab);
			var sb = new StringBuilder(specialCount+text.Length);

			foreach (var ch in text)
			{
				sb.Append(CharToRegex(ch));
			}

			return sb.ToString();
		}

		/// <inheritdoc />
		public override bool IsEmpty => string.IsNullOrEmpty(Text);

		public TextMatchingBrick(IFlexpressionAlgebra algebra, string text)
			: base(algebra)
		{
			Text = text;
		}

		/// <inheritdoc />
		public override bool Equals(FlexpressionBrick other)
		{
			if (IsEmpty && (other?.IsEmpty ?? false))
			{
				return true;
			}

			return other is TextMatchingBrick cb && Text.Equals(cb.Text);
		}

		/// <inheritdoc />
		protected override IParsingProduct TryParseInternal(IParsingContextStream ctx)
		{
			return ctx.ParseText(Text) ? ctx.Accept().CreateTermForAcceptedFragment(FXB.ExpressionTypes.TextTerm) : null;
		}

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, FlexpressionBrick content, bool recursion, bool freeze)
		{
		}

        /// <inheritdoc />
		public override string ToRegularExpressionString(RegularExpressionBuildingContext ctx)
		{
			return ctx.Factory.StringToRegex(Text);
		}

		/// <inheritdoc />
		public override MatchingLevel RegularExpressionMatchingLevel => MatchingLevel.Exact;

        public override string ToString()
        {
            return $"'{Text}'";
        }
    }
}