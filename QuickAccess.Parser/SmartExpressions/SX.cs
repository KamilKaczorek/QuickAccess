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
namespace QuickAccess.Parser.SmartExpressions
{
	public static class SX
	{
		public static IParsingBrickAlgebra BrickAlgebra = new StandardParsingBrickAlgebra();
		public const long Max = long.MaxValue;

		public static ParsingBrick Empty => EmptyParsingBrick.Instance;
		public static StartParsingExpression Start => StartParsingExpression.Instance;
		public static ParsingBrick Current => new CurrentRulePlaceholderBrick();

		public static ParsingBrick ToCharacter(char ch) => new CharBrick(ch);
		public static ParsingBrick ToTextSequence(string text) => new TextMatchingBrick(text);

		public static ParsingBrick ZeroOrMore(this string text) => ToTextSequence(text)[0, SX.Max];

		public static ParsingBrick OneOrMore(this string text) => ToTextSequence(text)[1, SX.Max];

		public static ParsingBrick Optional(this string text) => ToTextSequence(text)[0, 1];

		public static ParsingBrick DefinesRule(this string text, string patternName)
		{
			return SX.BrickAlgebra.DefineRule(ToTextSequence(text), patternName);
		}

		public static RulePlaceholderBrick Anything => new RulePlaceholderBrick("Anything");
		public static RulePlaceholderBrick WhiteSpace => new RulePlaceholderBrick("WhiteSpace");
		public static RulePlaceholderBrick OptionalWhiteSpace => new RulePlaceholderBrick("OptionalWhiteSpace");
		public static RulePlaceholderBrick CustomSequence => new RulePlaceholderBrick("CustomSequence");
		public static RulePlaceholderBrick Letter => new RulePlaceholderBrick("Letter");
		public static RulePlaceholderBrick UpperLetter => new RulePlaceholderBrick("UpperLetter");
		public static RulePlaceholderBrick LowerLetter => new RulePlaceholderBrick("LowerLetter");
		public static RulePlaceholderBrick Symbol => new RulePlaceholderBrick("Symbol");
		public static RulePlaceholderBrick Digit => new RulePlaceholderBrick("Digit");

		//public static ParsingBrick CreateConcatenationOperatorPlaceholder

		public static ParsingBrick Exact(this string text)
		{
			return ToTextSequence(text);
		}

		public static ParsingBrick Regex(this string text)
		{
			return ToTextSequence(text);
		}

		public static ParsingBrick Rule(this string patternName)
		{
			return new RulePlaceholderBrick(patternName);
		}
		
	}
}