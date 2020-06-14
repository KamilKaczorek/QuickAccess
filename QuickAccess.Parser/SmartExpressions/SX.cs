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
using QuickAccess.Parser.Product;
using QuickAccess.Parser.SmartExpressions.Bricks;

namespace QuickAccess.Parser.SmartExpressions
{
	public static class SX
	{
		public static ISmartExpressionAlgebra DefaultAlgebra = new StandardSmartExpressionAlgebra(-1);

		public static SmartExpressionBrick Anything => DefaultAlgebra.Anything;
		public static SmartExpressionBrick WhiteSpace => DefaultAlgebra.WhiteSpace;
		public static SmartExpressionBrick OptionalWhiteSpace => DefaultAlgebra.OptionalWhiteSpace;
		public static SmartExpressionBrick WhiteSpaceOrNewLine => DefaultAlgebra.WhiteSpaceOrNewLine;
		public static SmartExpressionBrick OptionalWhiteSpaceOrNewLine => DefaultAlgebra.OptionalWhiteSpaceOrNewLine;
		public static SmartExpressionBrick CustomSequence => DefaultAlgebra.CustomSequence;
		public static SmartExpressionBrick NewLine => DefaultAlgebra.NewLine;
		public static SmartExpressionBrick Letter => DefaultAlgebra.Letter;
		public static SmartExpressionBrick UpperLetter => DefaultAlgebra.UpperLetter;
		public static SmartExpressionBrick LowerLetter => DefaultAlgebra.LowerLetter;
		public static SmartExpressionBrick Symbol => DefaultAlgebra.Symbol;
		public static SmartExpressionBrick Digit => DefaultAlgebra.Digit;

		public const long Max = long.MaxValue;

		public static SmartExpressionBrick Empty => DefaultAlgebra.Empty;
		public static SmartExpressionBegin Start => DefaultAlgebra.Start;
		public static SmartExpressionBrick Current => DefaultAlgebra.Current;

		public static SmartExpressionBrick ToCharacter(char ch) => new CharBrick(DefaultAlgebra, ch);
		public static SmartExpressionBrick ToTextSequence(string text) => new TextMatchingBrick(DefaultAlgebra, text);

		public static SmartExpressionBrick ZeroOrMore(this string text) => ToTextSequence(text)[0, SX.Max];

		public static SmartExpressionBrick OneOrMore(this string text) => ToTextSequence(text)[1, SX.Max];

		public static SmartExpressionBrick Optional(this string text) => ToTextSequence(text)[0, 1];


        private static ExpressionTypeDescriptor CreateExpressionType(string ruleName, string valueTypeId = null)
        {
            if (string.IsNullOrEmpty(ruleName))
            {
				Guard.ArgNullOrEmpty(valueTypeId, nameof(valueTypeId));
				return ExpressionTypeDescriptor.Undefined;
            }

            return ExpressionTypeDescriptor.CreateExpressionClass(ruleName, valueTypeId);
        }

		public static SmartExpressionBrick DefinesRule(this string text, string patternName)
		{
			return DefaultAlgebra.DefineRule(ToTextSequence(text), CreateExpressionType(patternName));
		}

		public static SmartExpressionBrick DefinesSealedRule(this string text, string patternName)
		{
			return DefaultAlgebra.DefineSealedRule(ToTextSequence(text), CreateExpressionType(patternName));
		}

		public static SmartExpressionBrick ZeroOrMore(this SmartExpressionBrick source) => source[0, SX.Max];

		public static SmartExpressionBrick OneOrMore(this SmartExpressionBrick source) => source[1, SX.Max];

		public static SmartExpressionBrick Optional(this SmartExpressionBrick source) => source[0, 1];

		public static SmartExpressionBrick DefinesRule(this SmartExpressionBrick source, string patternName, string valueTypeId = null)
		{
			return DefaultAlgebra.DefineRule(source, CreateExpressionType(patternName, valueTypeId));
		}

		public static SmartExpressionBrick DefinesSealedRule(this SmartExpressionBrick source, string patternName, string valueTypeId = null)
		{
			return DefaultAlgebra.DefineSealedRule(source, CreateExpressionType(patternName, valueTypeId));
		}

        public static SmartExpressionBrick DefinesRule(this SmartExpressionBrick source, ExpressionTypeDescriptor expressionType)
        {
            return DefaultAlgebra.DefineRule(source, expressionType);
        }

        public static SmartExpressionBrick DefinesSealedRule(this SmartExpressionBrick source, ExpressionTypeDescriptor expressionType)
        {
            return DefaultAlgebra.DefineSealedRule(source, expressionType);
        }

		public static SmartExpressionBrick Exact(this string text)
		{
			return ToTextSequence(text);
		}

		public static SmartExpressionBrick Rule(this string patternName)
		{
			return DefaultAlgebra.CreateRulePlaceholder(patternName, null);
		}
	}
}