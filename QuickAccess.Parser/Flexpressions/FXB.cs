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
using System.Diagnostics.CodeAnalysis;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.Parser.Flexpressions.Bricks;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions
{
	public static class FX
    {
        public const long InfiniteCount = long.MaxValue;
    }

	public static class FXB
	{
        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        public static class StandardRuleName
        {
            public const string Anything = "*";
            public const string Empty = "EMPTY";
            public const string WhiteSpace = "WhSp";
            public const string WhiteSpaceOrNewLine = "WhSpOrNwLn";
            public const string OptionalWhiteSpace = "OptWhSp";
            public const string OptionalWhiteSpaceOrNewLine = "OptWhSpOrNwLn";
            public const string CustomSequence = "CustomSeq";
            public const string NewLine = "NwLn";
            public const string Letter = "Letter";
            public const string UpperLetter = "UpLetter";
            public const string LowerLetter = "LwLetter";
            public const string Symbol = "Symbol";
            public const string Digit = "Digit";
        }

        public static class ExpressionTypes
        {
            public static readonly ExpressionTypeDescriptor Concatenation = ExpressionTypeDescriptor.Create("_CONCAT_");
            public static readonly ExpressionTypeDescriptor CharTerm = ExpressionTypeDescriptor.Create("_CHAR_", "_CHAR_");
            public static readonly ExpressionTypeDescriptor TextTerm = ExpressionTypeDescriptor.Create("_TXT_", "_TXT_");
        }

        public static IFlexpressionAlgebra DefaultAlgebra = new StandardFlexpressionAlgebra(-1);

		public static FlexpressionBrick Anything => DefaultAlgebra.Anything;
		public static FlexpressionBrick WhiteSpace => DefaultAlgebra.WhiteSpace;
		public static FlexpressionBrick OptionalWhiteSpace => DefaultAlgebra.OptionalWhiteSpace;
		public static FlexpressionBrick WhiteSpaceOrNewLine => DefaultAlgebra.WhiteSpaceOrNewLine;
		public static FlexpressionBrick OptionalWhiteSpaceOrNewLine => DefaultAlgebra.OptionalWhiteSpaceOrNewLine;
		public static FlexpressionBrick CustomSequence => DefaultAlgebra.CustomSequence;
		public static FlexpressionBrick NewLine => DefaultAlgebra.NewLine;
		public static FlexpressionBrick Letter => DefaultAlgebra.Letter;
		public static FlexpressionBrick UpperLetter => DefaultAlgebra.UpperLetter;
		public static FlexpressionBrick LowerLetter => DefaultAlgebra.LowerLetter;
		public static FlexpressionBrick Symbol => DefaultAlgebra.Symbol;
		public static FlexpressionBrick Digit => DefaultAlgebra.Digit;


		public static FlexpressionBrick Empty => DefaultAlgebra.Empty;
		public static FlexpressionBegin Start => DefaultAlgebra.Start;
		public static FlexpressionBrick Current => DefaultAlgebra.Current;

		public static FlexpressionBrick ToCharacter(char ch) => new CharBrick(DefaultAlgebra, ch);
		public static FlexpressionBrick ToTextSequence(string text) => new TextMatchingBrick(DefaultAlgebra, text);

		public static FlexpressionBrick ZeroOrMore(this string text) => ToTextSequence(text)[Quantifier.Unlimited];

		public static FlexpressionBrick OneOrMore(this string text) => ToTextSequence(text)[Quantifier.AtLeastOne];

		public static FlexpressionBrick Optional(this string text) => ToTextSequence(text)[Quantifier.ZeroOrOne];


        private static ExpressionTypeDescriptor CreateExpressionType(string ruleName, string valueTypeId = null)
        {
            if (String.IsNullOrEmpty(ruleName))
            {
				Guard.ArgNullOrEmpty(valueTypeId, nameof(valueTypeId));
				return ExpressionTypeDescriptor.Undefined;
            }

            return ExpressionTypeDescriptor.CreateExpressionClass(ruleName, valueTypeId);
        }

		public static FlexpressionBrick DefinesRule(this string text, string patternName)
		{
			return DefaultAlgebra.DefineRule(ToTextSequence(text), CreateExpressionType(patternName));
		}

		public static FlexpressionBrick DefinesSealedRule(this string text, string patternName)
		{
			return DefaultAlgebra.DefineSealedRule(ToTextSequence(text), CreateExpressionType(patternName));
		}

		public static FlexpressionBrick ZeroOrMore(this FlexpressionBrick source) => source[Quantifier.Unlimited];

		public static FlexpressionBrick OneOrMore(this FlexpressionBrick source) => source[Quantifier.AtLeastOne];

		public static FlexpressionBrick Optional(this FlexpressionBrick source) => source[Quantifier.ZeroOrOne];

		public static FlexpressionBrick DefinesRule(this FlexpressionBrick source, string patternName, string valueTypeId = null)
		{
			return (source.Algebra ?? DefaultAlgebra).DefineRule(source, CreateExpressionType(patternName, valueTypeId));
		}

		public static FlexpressionBrick DefinesSealedRule(this FlexpressionBrick source, string patternName, string valueTypeId = null)
		{
			return (source.Algebra ?? DefaultAlgebra).DefineSealedRule(source, CreateExpressionType(patternName, valueTypeId));
		}

        public static FlexpressionBrick DefinesRule(this FlexpressionBrick source, ExpressionTypeDescriptor expressionType)
        {
            return (source.Algebra ?? DefaultAlgebra).DefineRule(source, expressionType);
        }

        public static FlexpressionBrick DefinesSealedRule(this FlexpressionBrick source, ExpressionTypeDescriptor expressionType)
        {
            return (source.Algebra ?? DefaultAlgebra).DefineSealedRule(source, expressionType);
        }

		public static FlexpressionBrick Exact(this string text)
		{
			return ToTextSequence(text);
		}

		public static FlexpressionBrick Rule(this string patternName)
		{
			return DefaultAlgebra.CreateRulePlaceholder(patternName, null);
		}
    }
}
