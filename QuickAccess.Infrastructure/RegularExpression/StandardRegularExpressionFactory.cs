﻿#region LICENSE [BSD-2-Clause]
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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickAccess.Infrastructure.CharMatching;
using QuickAccess.Infrastructure.CharMatching.Categories;

namespace QuickAccess.Infrastructure.RegularExpression
{
    
    public class StandardRegularExpressionFactory : IRegularExpressionFactory
	{
        private readonly IDefineSpecialCharacters _specialCharactersDefinition;

        public StandardRegularExpressionFactory(IDefineSpecialCharacters specialCharactersDefinition)
        {
            _specialCharactersDefinition = specialCharactersDefinition;
        }

        public string CharToRegex(char ch)
		{
			return _specialCharactersDefinition.IsSpecialCharacter(ch) ? $@"\{ch}" : _specialCharactersDefinition.IsTab(ch) ? @"\t" : ch.ToString();
		}

		public string StringToRegex(string text)
		{
			var specialCount = text.Count(ch => _specialCharactersDefinition.IsSpecialCharacter(ch) || _specialCharactersDefinition.IsTab(ch));
			var sb = new StringBuilder(specialCount+text.Length);

			foreach (var ch in text)
			{
				sb.Append(CharToRegex(ch));
			}

			return sb.ToString();
		}

		/// <inheritdoc />
		public string CreateNot(IRegularExpressionFactoryContext ctx, string negatedContentRegex)
		{
			return $"(?:(?:(?!{negatedContentRegex}).*)|.+(?:{negatedContentRegex}).*|.*(?:{negatedContentRegex}).+)";
		}

		/// <inheritdoc />
		public string CreateAlternation(IRegularExpressionFactoryContext ctx, IEnumerable<string> alternativeRegexs)
		{
			return CreateNonCapturingGroup(ctx, string.Join("|", alternativeRegexs));
		}

		/// <inheritdoc />
		public string CreateNamedGroup(IRegularExpressionFactoryContext ctx, string groupName, string regexContent, out string factualGroupName)
		{
			factualGroupName = ctx.GetUniqueAndValidGroupNameFor(groupName ?? string.Empty);
			
			return $"(?<{factualGroupName}>{regexContent})";
		}

		/// <inheritdoc />
		public string CreateQuantifier(IRegularExpressionFactoryContext ctx, Quantifier quantity, string quantifiedContentRegex)
		{
			if (string.IsNullOrEmpty(quantifiedContentRegex))
			{
				return string.Empty;
			}

			quantifiedContentRegex = CreateNonCapturingGroup(ctx, quantifiedContentRegex);

            var defined = quantity.ToDefinedRange();

            var min = defined.Min.MaxUInt;
            var max = defined.Max.MaxUInt;

			if (min == 1 && max == 1)
			{
				return quantifiedContentRegex;
			}

			return CreateNonCapturingGroup(ctx, GetQuantifiedContent());
		

			string GetQuantifiedContent()
			{
				if (min == 0 && max == 1)
				{
					return $"{quantifiedContentRegex}?";
				}

                if (defined.Max.IsInfinite)
                {
                    if (min == 0)
                    {
                        return $"{quantifiedContentRegex}*";
                    }

                    if (min == 1)
                    {
                        return $"{quantifiedContentRegex}+";
                    }

                    return $"{quantifiedContentRegex}{{{min},}}";
                }

                if (min == max)
				{
					return $"{quantifiedContentRegex}{{{min}}}";
				}

				return $"{quantifiedContentRegex}{{{min},{max}}}";
			}
		}

		/// <inheritdoc />
		public string CreateNonCapturingGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex)
		{
			
			return $"(?:{groupContentRegex})";
		}

		/// <inheritdoc />
		public string CreatePositiveLookaheadGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex)
		{
			return $"(?={groupContentRegex})";
		}

		/// <inheritdoc />
		public string CreateNegativeLookaheadGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex)
		{
			return $"(?!{groupContentRegex})";
		}

		/// <inheritdoc />
		public string CreateRecursiveGroupCall(IRegularExpressionFactoryContext ctx, string regexGroupName)
		{
			return $"(?&{regexGroupName})";
		}

		/// <inheritdoc />
		public string CreateCapturingGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex)
		{
			return $"({groupContentRegex})";
		}

		/// <inheritdoc />
		public string CreateCharRange(IRegularExpressionFactoryContext ctx, StandardCharacterCategories range)
        {
            return range.ToRegexStatement();
        }

		/// <inheritdoc />
		public string CreateCharSet(IRegularExpressionFactoryContext ctx, IEnumerable<char> characters)
		{
			return $"[{this.CharsToRegex(characters)}]";
		}

		/// <inheritdoc />
		public string CreateCharRange(IRegularExpressionFactoryContext ctx, char firstCharacter, char lastCharacter)
		{
			if (firstCharacter > lastCharacter)
			{
				var tmp = firstCharacter;
				firstCharacter = lastCharacter;
				lastCharacter = tmp;
			}

			if (firstCharacter == lastCharacter)
			{
				return CharToRegex(firstCharacter);
			}

			return $"[{CharToRegex(firstCharacter)}-{CharToRegex(lastCharacter)}]";
		}

		/// <inheritdoc />
		public string GetWordCharacter(IRegularExpressionFactoryContext ctx) => @"\w";

		/// <inheritdoc />
		public string GetDigitCharacter(IRegularExpressionFactoryContext ctx) => @"[0-9]";
	}
}