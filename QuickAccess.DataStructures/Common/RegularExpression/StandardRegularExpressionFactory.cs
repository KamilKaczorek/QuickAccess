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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickAccess.DataStructures.Common.RegularExpression
{
	public class StandardRegularExpressionFactory : IRegularExpressionFactory
	{
		public string CharToRegex(IRegularExpressionFactoryContext ctx, char ch)
		{
			return ctx.IsSpecialCharacter(ch) ? $@"\{ch}" : ctx.IsTab(ch) ? @"\t" : ch.ToString();
		}

		public string CharsToRegex(IRegularExpressionFactoryContext ctx, IEnumerable<char> chars)
		{
			return string.Join(string.Empty, chars.Select(ch => CharToRegex(ctx, ch)));
		}

		public string StringToRegex(IRegularExpressionFactoryContext ctx, string text)
		{
			var specialCount = text.Count(ch => ctx.IsSpecialCharacter(ch) || ctx.IsTab(ch));
			var sb = new StringBuilder(specialCount+text.Length);

			foreach (var ch in text)
			{
				sb.Append(CharToRegex(ctx, ch));
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
		public string CreateQuantifier(IRegularExpressionFactoryContext ctx, long min, long max, string quantifiedContentRegex)
		{
			if (string.IsNullOrEmpty(quantifiedContentRegex))
			{
				return string.Empty;
			}

			quantifiedContentRegex = CreateNonCapturingGroup(ctx, quantifiedContentRegex);

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

				if (min == 0 && max == long.MaxValue)
				{
					return $"{quantifiedContentRegex}*";
				}

				if (min == 1 && max == long.MaxValue)
				{
					return $"{quantifiedContentRegex}+";
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
		public string CreateCharRange(IRegularExpressionFactoryContext ctx, StandardCharactersRanges range)
		{
			if ((range & ~StandardCharactersRanges.NotWordCharacter) != StandardCharactersRanges.None)
			{
				throw new NotSupportedException($"Can't create character range: Specified range is not supported ({range})");
			}

			switch (range)
			{
				case StandardCharactersRanges.None:
				case StandardCharactersRanges.Not:
					throw new InvalidOperationException($"Can't create character range: range is empty ({range}).");
				case StandardCharactersRanges.Underscore:
					return "_";
				case StandardCharactersRanges.WordCharacter:
					return @"\w";
				case StandardCharactersRanges.NotWordCharacter:
					return @"\W";
				case StandardCharactersRanges.NotUnderscore:
					return "^_";
			}

			var regex = ((range & StandardCharactersRanges.Not) != StandardCharactersRanges.None) ? "[^" : "[";

			if ((range & StandardCharactersRanges.UpperLetter) != StandardCharactersRanges.None)
			{
				regex += "A-Z";
			}

			if ((range & StandardCharactersRanges.LowerLetter) != StandardCharactersRanges.None)
			{
				regex += "a-z";
			}

			if ((range & StandardCharactersRanges.Digit) != StandardCharactersRanges.None)
			{
				regex += "0-9";
			}

			if ((range & StandardCharactersRanges.Underscore) != StandardCharactersRanges.None)
			{
				regex += "_";
			}

			regex += "]";

			return regex;
		}

		/// <inheritdoc />
		public string CreateCharSet(IRegularExpressionFactoryContext ctx, IEnumerable<char> characters)
		{
			return $"[{CharsToRegex(ctx, characters)}]";
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
				return CharToRegex(ctx, firstCharacter);
			}

			return $"[{CharToRegex(ctx, firstCharacter)}-{CharToRegex(ctx, lastCharacter)}]";
		}

		/// <inheritdoc />
		public string GetWordCharacter(IRegularExpressionFactoryContext ctx) => @"\w";

		/// <inheritdoc />
		public string GetDigitCharacter(IRegularExpressionFactoryContext ctx) => @"[0-9]";
	}
}