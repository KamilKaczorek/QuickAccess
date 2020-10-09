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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using QuickAccess.Infrastructure.CharMatching;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The extension methods related with parsing.
    /// Extends the operations of following interfaces:
    /// <see cref="IParsingContextStream"/>
    /// <see cref="ISourceCode"/>
    /// <see cref="IEqualityComparer{T}"/>
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// The null character
        /// </summary>
        public const char NullChar = '\0';

        /// <summary>
        /// The empty fragment of the source
        /// </summary>
        public static readonly ISourceCodeFragment EmptyFragment = null;// new EmptyParsedFragment();


        public static IEqualityComparer<string> ToStringComparer(this IEqualityComparer<char> charComparer)
        {
            if (charComparer == null)
            {
                return null;
            }

            return new CharComparerToStringComparerAdapter(charComparer);
        }

        public static int ParseWhiteSpace(this IParsingContextStream source, bool accept = false)
        {
            var count = 0;
            while (source.HasNext && char.IsWhiteSpace(source.Next))
            {
                source.MoveNext();
                count++;
            }

            if (accept && count > 0)
            {
                source.Accept();
            }

            return count;
        }

        public static int ParseDigits(this IParsingContextStream source, bool accept = false)
        {
            var count = 0;
            while (source.HasNext && char.IsDigit(source.Next))
            {
                count++;
                source.MoveNext();
            }

            if (accept && count > 0)
            {
                source.Accept();
            }

            return count;
        }

        public static int ParseWhile(this IParsingContextStream source, Func<char, bool> predicate, bool accept = false)
        {
            var count = 0;
            while (source.HasNext && predicate.Invoke(source.Next))
            {
                count++;
                source.MoveNext();
            }

            if (accept && count > 0)
            {
                source.Accept();
            }

            return count;
        }

        public static bool IsContainedBy(this char source, ICollection<char> collection,
            IEqualityComparer<char> comparer)
        {
            if (collection == null)
            {
                return false;
            }

            if (comparer == null)
            {
                return collection.Contains(source);
            }

            if (comparer is CharComparer cc)
            {
                return cc.IsCharContainedByCollection(source, collection);
            }

            return collection.Contains(source, comparer);
        }

        public static int ParseWhileOneOf(this IParsingContextStream source, ISet<char> charSet, bool accept = false, IEqualityComparer<char> charComparer = null)
        {
            var count = 0;

            while (source.HasNext && source.Next.IsContainedBy(charSet, charComparer))
            {
                count++;
                source.MoveNext();
            }

            if (accept && count > 0)
            {
                source.Accept();
            }

            return count;
        }

        public static bool ParseOneOf(this IParsingContextStream source, ISet<char> charSet, bool accept = false, IEqualityComparer<char> charComparer = null)
        {
            if (source.HasNext && source.Next.IsContainedBy(charSet, charComparer))
            {
                source.MoveNext();

                if (accept)
                {
                    source.Accept();
                }

                return true;
            }

            return false;
        }

        public static bool ParseChar(
            this IParsingContextStream source,
            char ch,
            bool accept = false,
            IEqualityComparer<char> charComparer = null)
        {
            if (source.HasNext && Equals(source.Next,ch, charComparer))
            {
                source.MoveNext();

                if (accept)
                {
                    source.Accept();
                }

                return true;
            }

            return false;
        }

        public static bool ParseChar(this IParsingContextStream source, char ch1, char ch2, bool accept = false)
        {
            if (source.HasNext && source.Next == ch1 || source.Next == ch2)
            {
                source.MoveNext();

                if (accept)
                {
                    source.Accept();
                }

                return true;
            }

            return false;
        }

        public static bool ParseText(this IParsingContextStream source, IEnumerable<char> text,
            bool accept, IEqualityComparer<char> charComparer = null)
        {
            return ParseText(source, text, charComparer, accept);
        }

        public static bool ParseText(this IParsingContextStream source, IEnumerable<char> text, IEqualityComparer<char> charComparer = null, bool accept = false)
        {
	        foreach (var ch in text)
	        {
		        if (!source.MoveNext())
		        {
			        return false;
		        }

		        var cur = source.Current;

		        if (!(charComparer?.Equals(ch, cur) ?? ch == cur))
		        {
			        return false;
		        }
	        }

	        if (accept)
	        {
				source.Accept();
	        }

	        return true;
        }

        public static bool ParseWhen(this IParsingContextStream source, Func<char, bool> predicate, bool accept = false)
        {
            if (source.HasNext && predicate.Invoke(source.Next))
            {
                source.MoveNext();

                if (accept)
                {
                    source.Accept();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the source code searching for the longest matching term text from the specified terms.
        /// It returns the index of the longest matching term or -1, if there is no matching term at the specified source position.       
        /// </summary>
        /// <param name="source">The source of an extension.</param>
        /// <param name="textSelector">The selector of term text at the specified 0 based index.</param>
        /// <param name="accept">if set to <c>true</c> accepts the position after parsed longest text.</param>
        /// <param name="charComparer">The character comparer.</param>
        /// <param name="termsCount">The number of terms.</param>
        /// <returns>The index of the longest matching text or -1, if there is no matching text at the specified source position.</returns>
        public static int ParseLongestMatchingString(this IParsingContextStream source, int termsCount, Func<int, string> textSelector, bool accept = false, IEqualityComparer<char> charComparer = null)
        {
            
            var text = Enumerable.Range(0, termsCount).Select(textSelector).ToArray();            
            var matchedTermIdx = -1;

            using (var ctx = source.GetFurtherContext())
            {
                //go through all character indexes from 0 to longest matching term
                //  for each term check if character at specified position matches
                //      if char matches and it is last character, temporary accept position and exclude term by setting to null
                //      if char doesn't match -> exclude term by setting to null
                //  if there are no more matching terms -> break
                //  if accept true and matchedTermIdx > -1 -> accept
                // return matchedTermIndex

                var charPosition = -1;
                while (ctx.MoveNext())
                {
                    charPosition++;
                    var current = ctx.Current;
                    var matchingTermsCount = 0;
                    for (var termIdx = 0; termIdx < text.Length; termIdx++)
                    {
                        var currentTerm = text[termIdx];

                        if (currentTerm != null)
                        {
                            if (currentTerm.Length <= charPosition)
                            {
                                text[termIdx] = null;
                            }

                            if (Equals(currentTerm[charPosition], current, charComparer))
                            { 
                                if (currentTerm.Length == charPosition + 1)
                                {
                                    matchedTermIdx = termIdx;
                                    ctx.Accept();
                                    text[termIdx] = null;
                                }
                                else
                                {
                                    matchingTermsCount++;
                                }
                            }
                            else
                            {
                                text[termIdx] = null;
                            }
                        }
                    }

                    if (matchingTermsCount <= 0)
                    {
                        break;
                    }
                }
            }

            if (accept && matchedTermIdx >= 0)
            {
                source.Accept();
            }

            return matchedTermIdx;
        }

        // UnsignedNumber = ( ({Digit},'.', [{Digit}]) | ({Digit}, ['.']) | '.', {Digit} ), [('E'|'e'), ['+'|'-'], {Digit}]
        public static ISourceCodeFragment ParseUnsignedNumber(this ISourceCode src, out double value, out bool isFloat)
        {
            using var ctx = src.GetFurtherContext();
            value = 0D;
      
            var intLength = ctx.ParseDigits();
            isFloat = false;
            if (ctx.ParseChar('.'))
            {
                isFloat = true;
                var fractionLength = ctx.ParseDigits();

                if (fractionLength + intLength == 0)
                {
                    ctx.SetError(ParsingErrors.DigitExpected);
                    return null;
                }
            }
            else
            {
                if (intLength == 0)
                {
                    ctx.SetError(ParsingErrors.DigitExpected);
                    return null;
                }
            }

            if (ctx.ParseChar('e', 'E'))
            {
                isFloat = true;
                var containsExpSign = ctx.ParseChar('+', '-');

                var expLength = ctx.ParseDigits();

                if (expLength == 0)
                {
                    ctx.SetError(containsExpSign ? ParsingErrors.DigitExpected : ParsingErrors.DigitOrPlusMinusExpected);
                    return null;
                }
            }

            ctx.Accept();
            var fragment = ctx.GetAcceptedFragmentOrEmpty();

            value = double.Parse(fragment.ToString(), CultureInfo.InvariantCulture);

            return fragment;
        }

        public static bool Equals(this ISourceCodeFragment source, string text, IEqualityComparer<char> charComparer = null)
        {
            if (source.Count <= 0 && string.IsNullOrEmpty(text))
            {
                return true;
            }

            if (source.Count != text.Length)
            {
                return false;
            }

            var idx = 0;
            foreach (var ch1 in source)
            {  
                var ch2 = text[idx];                
                if (!Equals(ch1, ch2, charComparer))
                {
                    return false;
                }
                idx++;
            }

            return true;
        }

        private static bool Equals(char ch1, char ch2, IEqualityComparer<char> charComparer = null)
        {
            return (charComparer ?? CharComparer.Ordinal).Equals(ch1, ch2);
        }
    }
}