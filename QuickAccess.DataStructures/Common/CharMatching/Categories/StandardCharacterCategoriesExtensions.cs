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
using QuickAccess.DataStructures.Common.Collections;

namespace QuickAccess.DataStructures.Common.CharMatching.Categories
{


    public static class StandardCharacterCategoriesExtensions
    {
        public const int NegativePatternsShift = 16;

        public const int NegativePatternMask = 0x7FFF_0000;
        public const int PositivePatternMask = 0x0000_7FFF;

        public const string OperatorCharacters = @"+-*/%=^|~\&";
        public const string PunctuationMarks = ".!?`'\",;…—";
        public const string SpecialSigns = @"@#$£";

        
        private static readonly StandardCharacterCategories[] StdCategoryByCharCode;
        private static readonly CharacterCategoryType[] CategoryTypeByCharCode;

        private static readonly int MaxPreEvaluatingCharCode = (int)Math.Min(65535u, char.MaxValue);

        public static ReadOnlyArray<StandardCharacterCategories> StandardCategoryByCharacterCode;
        public static ReadOnlyArray<CharacterCategoryType> CategoryTypeByCharacterCode;

        static StandardCharacterCategoriesExtensions()
        {
            CategoryTypeByCharCode = new CharacterCategoryType[MaxPreEvaluatingCharCode + 1];
            StdCategoryByCharCode = new StandardCharacterCategories[MaxPreEvaluatingCharCode + 1];

            for (var idx = MaxPreEvaluatingCharCode; idx >= 0; --idx)
            {
                var ch = (char) idx;
                (CategoryTypeByCharCode[idx], StdCategoryByCharCode[idx]) = EvaluateCategory(ch);
            }

            StandardCategoryByCharacterCode = ReadOnlyArray.Wrap(StdCategoryByCharCode);
            CategoryTypeByCharacterCode = ReadOnlyArray.Wrap(CategoryTypeByCharCode);
        }

        private static Tuple<CharacterCategoryType, StandardCharacterCategories> EvaluateCategory(char ch)
        {
            var stdPattern = EvaluateStandardPattern(ch);
            var charCategoryType = CharacterCategoryType.Other;

            if (StandardCharacterCategories.WordCharacterOrWhiteSpace.HasFlag(stdPattern))
            {
                charCategoryType = CharacterCategoryType.Standard;
            }
            else if (EvaluateIsControlCharacter(ch))
            {
                charCategoryType = CharacterCategoryType.Control;
            }
            else if (ch.IsAny(Brackets.Bracket))
            {
                charCategoryType = CharacterCategoryType.Bracket;
            }
            else if (OperatorCharacters.IndexOf(ch) >= 0)
            {
                charCategoryType = CharacterCategoryType.Operator;
            }
            else if (PunctuationMarks.IndexOf(ch) >= 0)
            {
                charCategoryType = CharacterCategoryType.PunctuationMark;
            }
            else if (SpecialSigns.IndexOf(ch) >= 0)
            {
                charCategoryType = CharacterCategoryType.SpecialSign;
            }
            

            return Tuple.Create(charCategoryType, stdPattern);
        }

        [Pure]
        public static CharacterCategoryType GetCharacterCategoryType(this char source)
        {
            var code = (int) source;
            var category = code <= MaxPreEvaluatingCharCode
                ? CategoryTypeByCharCode[code]
                : EvaluateCategory(source).Item1;

            return category;
        }

        [Pure]
		public static bool IsWordChar(this char source)
        {
            return IsCharacterFromPatternInternal(source, StandardCharacterCategories.WordCharacter, true);
        }

        [Pure]
        public static bool IsMatch(this StandardCharacterCategories pattern, char character)
        {
            var matches = IsCharacterFromPatternInternal(character, pattern, pattern.GetValidPatternType() == StandardCharactersPatternType.Positive);
           
            return matches;
        }

        public static bool IsMatchAny(this StandardCharacterCategories pattern, IEnumerable<char> characters)
        {
            return IsAnyCharacterFromPattern(characters, pattern);
        }

        [Pure]
        public static IEnumerable<char> Matches(this StandardCharacterCategories pattern, IEnumerable<char> characters)
        {
            var positive = pattern.GetValidPatternType() == StandardCharactersPatternType.Positive;

            return characters.Where(character => IsCharacterFromPatternInternal(character, pattern, positive));
        }

        [Pure]
        public static bool HasAll(this StandardCharacterCategories pattern, StandardCharacterCategories contained)
        {
            return (pattern & contained) == contained;
        }

        public static bool HasAny(this StandardCharacterCategories pattern, StandardCharacterCategories contained)
        {
            return (pattern & contained) != StandardCharacterCategories.None;
        }

        [Pure]
		public static bool IsAny(this char character, StandardCharacterCategories pattern)
        {
            var matches = IsCharacterFromPatternInternal(character, pattern, pattern.GetValidPatternType() == StandardCharactersPatternType.Positive);
           
            return matches;
        }


        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StandardCharacterCategories CategorizeByStandardPattern(this char ch, bool positive = true)
        {
            var code = (int) ch;
            var positiveCategory = code <= MaxPreEvaluatingCharCode
                ? StdCategoryByCharCode[code]
                : EvaluateStandardPattern(ch);

            return positive 
                ? positiveCategory 
                : positiveCategory.Not();
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden, DebuggerStepThrough]
        public static StandardCharacterCategories Not(this StandardCharacterCategories pattern)
        {
            var mask = (int) pattern;
            var negative = (mask & NegativePatternsShift) >> NegativePatternsShift;
            var positive = (mask & PositivePatternMask) << NegativePatternsShift;

            var negated = (StandardCharacterCategories)(negative | positive);

            return negated;
        }

      
        [Pure]
        public static bool IsAnyCharacterFromPattern(IEnumerable<char> characters, StandardCharacterCategories pattern)
        {
            if (pattern == StandardCharacterCategories.None)
            {
                return false;
            }

            var positive = pattern.GetValidPatternType() == StandardCharactersPatternType.Positive;

            var contains = characters.Any(character => IsCharacterFromPatternInternal(character, pattern, positive));
            return contains;
        }

        [Pure]
        public static bool IsValid(this StandardCharacterCategories pattern)
        {
            var type = pattern.GetPatternType();
            return type != StandardCharactersPatternType.Invalid;
        }

        [Pure]
        public static bool IsEmpty(this StandardCharacterCategories pattern)
        {
            return pattern == StandardCharacterCategories.None;
        }

        [Pure]
        public static bool IsDefined(this StandardCharacterCategories pattern)
        {
            return pattern != StandardCharacterCategories.None;
        }

        [Pure]
        public static IDefineCharactersRange ToDefinition(this StandardCharacterCategories pattern)
        {
            return CharactersRangeDefinition.CreateMatching(pattern);
        }

        [Pure]
        public static string Description(this StandardCharacterCategories pattern)
        {
            return $"{pattern}";
        }

        [Pure]
        private static string ToRegexStatement(this StandardCharacterCategories fullPattern, bool positiveScope)
        {
            var pattern = positiveScope ? fullPattern : fullPattern.Not();

            var simple = pattern switch
            {
				StandardCharacterCategories.UpperLetter => positiveScope ? "[A-Z]" : "[^A-Z]",
				StandardCharacterCategories.LowerLetter => positiveScope ? "[a-z]": "[^a-z]",
                StandardCharacterCategories.Letter => positiveScope ? "[A-Za-z]": "[^A-Za-z]",
                StandardCharacterCategories.LetterOrDigit => positiveScope ? @"[A-Za-z0-9]": @"[^A-Za-z0-9]",
                StandardCharacterCategories.Digit => positiveScope ? @"\d": @"\D",
				StandardCharacterCategories.Underscore => positiveScope ? @"_": "^_",

				StandardCharacterCategories.Space => positiveScope ? @"[ ]": @"[^ ]",
                StandardCharacterCategories.Tab => positiveScope ? @"\t": @"^\t",
				StandardCharacterCategories.NewLine => positiveScope ? @"\n": @"^\n",
				StandardCharacterCategories.Return => positiveScope ? @"\r": @"^\r",
				StandardCharacterCategories.LineBreak => positiveScope ? @"[\r\n]": @"[^\r\n]",
				StandardCharacterCategories.SpaceOrTab => positiveScope ? @"[\t ]": @"[^\t ]",

                StandardCharacterCategories.WordCharacter => positiveScope ? @"\w": @"\W",
                StandardCharacterCategories.WhiteSpace => positiveScope ? @"\s":  @"\S",

                StandardCharacterCategories.WordCharacterOrWhiteSpace => positiveScope ? @"[\w\s]" : @"[^\w\s]",

                _ => null,
            };

            if (simple != null)
            {
                return simple;
            }

            var regex = positiveScope ? "[" : "[^";

            var wordCharacterFlags = pattern.EvaluateContainsFlags(StandardCharacterCategories.WordCharacter);

            if (wordCharacterFlags != FlagCheckResult.None)
            {
                if (wordCharacterFlags == FlagCheckResult.All)
                {
                    regex += @"\w";
                }
                else
                {
                    if (pattern.HasFlag(StandardCharacterCategories.UpperLetter))
                    {
                        regex += "A-Z";
                    }

                    if (pattern.HasFlag(StandardCharacterCategories.LowerLetter))
                    {
                        regex += "a-z";
                    }

                    if (pattern.HasFlag(StandardCharacterCategories.Underscore))
                    {
                        regex += "_";
                    }

                    if (pattern.HasFlag(StandardCharacterCategories.Digit))
                    {
                        regex += "0-9";
                    }
                }
            }


            var whiteSpaceFlags = pattern.EvaluateContainsFlags(StandardCharacterCategories.WhiteSpace);

            if (whiteSpaceFlags != FlagCheckResult.None)
            {
                if (whiteSpaceFlags == FlagCheckResult.All)
                {
                    regex += @"\s";
                }
                else
                {
                    if (pattern.HasFlag(StandardCharacterCategories.Tab))
                    {
                        regex += @"\t";
                    }

                    if (pattern.HasFlag(StandardCharacterCategories.Space))
                    {
                        regex += @" ";
                    }

                    if (pattern.HasFlag(StandardCharacterCategories.NewLine))
                    {
                        regex += @"\n";
                    }

                    if (pattern.HasFlag(StandardCharacterCategories.Return))
                    {
                        regex += @"\r";
                    }
                }
            }

            regex += "]";

			return regex;
        }

        [Pure]
        public static string ToRegexStatement(this StandardCharacterCategories pattern)
		{
            var positive = pattern.GetValidNotEmptyPatternType() == StandardCharactersPatternType.Positive;

            return ToRegexStatement(pattern, positive);
        }

        public static TVisitationResult Visit<TVisitationResult>(this ICharactersRangeDefinitionVisitor<TVisitationResult> visitor, IAcceptCharactersRangeDefinitionVisitor acceptor)
        {
            var visitationResult = acceptor.AcceptVisitor(visitor);
            return visitationResult;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StandardCharactersPatternType GetPatternType(this StandardCharacterCategories pattern)
        {
            if (pattern == StandardCharacterCategories.None)
            {
                return StandardCharactersPatternType.Empty;
            }

            var hasNegatives = ((int) pattern & NegativePatternMask) != 0;
            if (hasNegatives)
            {
                var hasPositives = ((int) pattern & PositivePatternMask) != 0;

                return hasPositives
                    ? StandardCharactersPatternType.Invalid
                    : StandardCharactersPatternType.Negative;
            }

            return StandardCharactersPatternType.Positive;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StandardCharactersPatternType GetValidNotEmptyPatternType(this StandardCharacterCategories pattern)
        {
            var type = pattern.GetPatternType();

            return type switch
            {
                StandardCharactersPatternType.Empty => throw new ArgumentException($"Pattern is empty ({pattern})", nameof(pattern)),
                StandardCharactersPatternType.Invalid => throw new ArgumentException($"Invalid pattern: Pattern contains mixed negative and positive patterns ({pattern})."),
                _ => type
            };
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StandardCharactersPatternType GetValidPatternType(this StandardCharacterCategories pattern)
        {
            var type = pattern.GetPatternType();

            return type != StandardCharactersPatternType.Invalid
                ? type
                : throw new ArgumentException(
                    $"Invalid pattern: Pattern contains mixed negative and positive patterns ({pattern}).");
        }
      
        [Pure, DebuggerStepThrough, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsCharacterFromPatternInternal(char character, StandardCharacterCategories pattern, bool positive)
        {
            var characterType = character.CategorizeByStandardPattern(positive);

            var matches = characterType != StandardCharacterCategories.None && pattern.HasFlag(characterType);
           
            return positive ? matches : !matches;
        }

        [Pure]
        private static StandardCharacterCategories EvaluateStandardPattern(char ch) =>
            ch switch
            {
                var c when char.IsDigit(c) => StandardCharacterCategories.Digit,
                var c when char.IsUpper(c) => StandardCharacterCategories.UpperLetter,
                var c when char.IsLower(c) => StandardCharacterCategories.LowerLetter,
                '_' => StandardCharacterCategories.Underscore,
                ' ' => StandardCharacterCategories.Space,
                '\r' => StandardCharacterCategories.Return,
                '\n' => StandardCharacterCategories.NewLine,
                '\t' => StandardCharacterCategories.Tab,
                _ => StandardCharacterCategories.NonWordCharacterOrWhiteSpace,
            };

        private static bool EvaluateIsControlCharacter(char c)
        {
            const char lastFromFirstControl = (char)0x0F;
            const char del = (char)0x7F;
            const char tab = '\t';

            var isControl = c <= lastFromFirstControl || c == del && c != tab;
            return isControl;
        }

    }


}