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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;

namespace QuickAccess.DataStructures.Common.CharMatching
{
    public static class CharactersRangeExtensions
    {
        public const int NegativePatternsShift = 16;

        public const int NegativePatternMask = 0x7FFF_0000;
        public const int PositivePatternMask = 0x0000_7FFF;



        [Pure]
		public static bool IsWordChar(this char source)
		{
			return char.IsLetter(source) || char.IsDigit(source) || source == '_';
		}

        [Pure]
        public static bool IsMatch(this StandardCharactersRange range, char character)
        {
            var matches = IsCharacterFromRangeInternal(character, range, range.GetValidRangeType() == StandardCharactersRangeType.Positive);
           
            return matches;
        }

        public static bool IsMatchAny(this StandardCharactersRange range, IEnumerable<char> characters)
        {
            return IsAnyCharacterFromRange(characters, range);
        }

        [Pure]
        public static IEnumerable<char> Matches(this StandardCharactersRange range, IEnumerable<char> characters)
        {
            var positive = range.GetValidRangeType() == StandardCharactersRangeType.Positive;

            return characters.Where(character => IsCharacterFromRangeInternal(character, range, positive));
        }

        [Pure]
        public static bool HasAll(this StandardCharactersRange range, StandardCharactersRange contained)
        {
            return (range & contained) == contained;
        }

        public static bool HasAny(this StandardCharactersRange range, StandardCharactersRange contained)
        {
            return (range & contained) != StandardCharactersRange.None;
        }

        [Pure]
		public static bool IsFromRange(this char character, StandardCharactersRange range)
        {
            var matches = IsCharacterFromRangeInternal(character, range, range.GetValidRangeType() == StandardCharactersRangeType.Positive);
           
            return matches;
        }

        [Pure]
        public static StandardCharactersRange StandardRange(this char ch, bool positive = true) =>
            ch switch
            {
				var c when char.IsDigit(c) => positive ? StandardCharactersRange.Digit : StandardCharactersRange.NonDigit,
				var c when char.IsUpper(c) => positive ? StandardCharactersRange.UpperLetter : StandardCharactersRange.NonUpperLetter,
				var c when char.IsLower(c) => positive ? StandardCharactersRange.LowerLetter : StandardCharactersRange.NonLowerLetter,
				'_' => positive ? StandardCharactersRange.Underscore : StandardCharactersRange.NonUnderscore,
				' ' => positive ? StandardCharactersRange.Space : StandardCharactersRange.NonSpace,
				'\r' => positive ? StandardCharactersRange.Return : StandardCharactersRange.NonReturn,
				'\n' => positive ? StandardCharactersRange.NewLine : StandardCharactersRange.NonNewLine,
				'\t' => positive ?StandardCharactersRange.Tab : StandardCharactersRange.NonTab, 
				_ => positive ? StandardCharactersRange.NonWordCharacterOrWhiteSpace : StandardCharactersRange.WordCharacterOrWhiteSpace,
            };

        
            
        [Pure]
        public static StandardCharactersRange Not(this StandardCharactersRange range)
        {
            var negative = ((int) range & NegativePatternsShift) >> NegativePatternsShift;
            var positive = ((int) range & PositivePatternMask) << NegativePatternsShift;

            var negated = (StandardCharactersRange)(negative | positive);

            return negated;
        }

      
        [Pure]
        public static bool IsAnyCharacterFromRange(IEnumerable<char> characters, StandardCharactersRange range)
        {
            if (range == StandardCharactersRange.None)
            {
                return false;
            }

            var positive = range.GetValidRangeType() == StandardCharactersRangeType.Positive;

            var contains = characters.Any(character => IsCharacterFromRangeInternal(character, range, positive));
            return contains;
        }

        [Pure]
        public static bool IsValid(this StandardCharactersRange range)
        {
            var type = range.GetRangeType();
            return type != StandardCharactersRangeType.Invalid;
        }

        [Pure]
        public static bool IsEmpty(this StandardCharactersRange range)
        {
            return range == StandardCharactersRange.None;
        }

        [Pure]
        public static bool IsDefined(this StandardCharactersRange range)
        {
            return range != StandardCharactersRange.None;
        }

        [Pure]
        public static IDefineCharactersRange ToDefinition(this StandardCharactersRange range)
        {
            return CharactersRangeDefinition.CreateMatching(range);
        }

        [Pure]
        public static string Description(this StandardCharactersRange range)
        {
            return $"{range}";
        }

        [Pure]
        private static string ToRegexStatement(this StandardCharactersRange fullRange, bool positiveScope)
        {
            var range = positiveScope ? fullRange : fullRange.Not();

            var simple = range switch
            {
				StandardCharactersRange.UpperLetter => positiveScope ? "[A-Z]" : "[^A-Z]",
				StandardCharactersRange.LowerLetter => positiveScope ? "[a-z]": "[^a-z]",
                StandardCharactersRange.Letter => positiveScope ? "[A-Za-z]": "[^A-Za-z]",
                StandardCharactersRange.LetterOrDigit => positiveScope ? @"[A-Za-z0-9]": @"[^A-Za-z0-9]",
                StandardCharactersRange.Digit => positiveScope ? @"\d": @"\D",
				StandardCharactersRange.Underscore => positiveScope ? @"_": "^_",

				StandardCharactersRange.Space => positiveScope ? @"[ ]": @"[^ ]",
                StandardCharactersRange.Tab => positiveScope ? @"\t": @"^\t",
				StandardCharactersRange.NewLine => positiveScope ? @"\n": @"^\n",
				StandardCharactersRange.Return => positiveScope ? @"\r": @"^\r",
				StandardCharactersRange.LineBreak => positiveScope ? @"[\r\n]": @"[^\r\n]",
				StandardCharactersRange.SpaceOrTab => positiveScope ? @"[\t ]": @"[^\t ]",

                StandardCharactersRange.WordCharacter => positiveScope ? @"\w": @"\W",
                StandardCharactersRange.WhiteSpace => positiveScope ? @"\s":  @"\S",

                StandardCharactersRange.WordCharacterOrWhiteSpace => positiveScope ? @"[\w\s]" : @"[^\w\s]",

                _ => null,
            };

            if (simple != null)
            {
                return simple;
            }

            var regex = positiveScope ? "[" : "[^";

            var wordCharacterFlags = range.EvaluateContainsFlags(StandardCharactersRange.WordCharacter);

            if (wordCharacterFlags != FlagCheckResult.None)
            {
                if (wordCharacterFlags == FlagCheckResult.All)
                {
                    regex += @"\w";
                }
                else
                {
                    if (range.HasFlag(StandardCharactersRange.UpperLetter))
                    {
                        regex += "A-Z";
                    }

                    if (range.HasFlag(StandardCharactersRange.LowerLetter))
                    {
                        regex += "a-z";
                    }

                    if (range.HasFlag(StandardCharactersRange.Underscore))
                    {
                        regex += "_";
                    }

                    if (range.HasFlag(StandardCharactersRange.Digit))
                    {
                        regex += "0-9";
                    }
                }
            }


            var whiteSpaceFlags = range.EvaluateContainsFlags(StandardCharactersRange.WhiteSpace);

            if (whiteSpaceFlags != FlagCheckResult.None)
            {
                if (whiteSpaceFlags == FlagCheckResult.All)
                {
                    regex += @"\s";
                }
                else
                {
                    if (range.HasFlag(StandardCharactersRange.Tab))
                    {
                        regex += @"\t";
                    }

                    if (range.HasFlag(StandardCharactersRange.Space))
                    {
                        regex += @" ";
                    }

                    if (range.HasFlag(StandardCharactersRange.NewLine))
                    {
                        regex += @"\n";
                    }

                    if (range.HasFlag(StandardCharactersRange.Return))
                    {
                        regex += @"\r";
                    }
                }
            }

            regex += "]";

			return regex;
        }

        [Pure]
        public static string ToRegexStatement(this StandardCharactersRange range)
		{
            var positive = range.GetValidNotEmptyRangeType() == StandardCharactersRangeType.Positive;

            return ToRegexStatement(range, positive);
        }

        public static TVisitationResult Visit<TVisitationResult>(this ICharactersRangeDefinitionVisitor<TVisitationResult> visitor, IAcceptCharactersRangeDefinitionVisitor acceptor)
        {
            var visitationResult = acceptor.AcceptVisitor(visitor);
            return visitationResult;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StandardCharactersRangeType GetRangeType(this StandardCharactersRange range)
        {
            if (range == StandardCharactersRange.None)
            {
                return StandardCharactersRangeType.Empty;
            }

            var hasNegatives = ((int) range & 0xFFFF_0000) != 0;
            if (hasNegatives)
            {
                var hasPositives = ((int) range & 0x0000_FFFF) != 0;

                return hasPositives
                    ? StandardCharactersRangeType.Invalid
                    : StandardCharactersRangeType.Negative;
            }

            return StandardCharactersRangeType.Positive;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StandardCharactersRangeType GetValidNotEmptyRangeType(this StandardCharactersRange range)
        {
            var type = range.GetRangeType();

            return type switch
            {
                StandardCharactersRangeType.Empty => throw new ArgumentException($"Range is empty ({range})", nameof(range)),
                StandardCharactersRangeType.Invalid => throw new ArgumentException($"Invalid range: Range contains mixed negative and positive patterns ({range})."),
                _ => type
            };
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StandardCharactersRangeType GetValidRangeType(this StandardCharactersRange range)
        {
            var type = range.GetRangeType();

            return type != StandardCharactersRangeType.Invalid
                ? type
                : throw new ArgumentException(
                    $"Invalid range: Range contains mixed negative and positive patterns ({range}).");
        }
      
        [Pure, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsCharacterFromRangeInternal(char character, StandardCharactersRange range, bool positive)
        {
            var characterType = character.StandardRange(positive);

            var matches = characterType != StandardCharactersRange.None && range.HasFlag(characterType);
           
            return positive ? matches : !matches;
        }
    }


}