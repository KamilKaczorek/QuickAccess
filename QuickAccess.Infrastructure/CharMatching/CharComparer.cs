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

namespace QuickAccess.Infrastructure.CharMatching
{
    /// <summary>
    /// The character comparer.
    /// Implements case sensitive and insensitive invariant character comparison. 
    /// <seealso cref="CharComparerToStringComparerAdapter"/>
    /// </summary>
    /// <seealso cref="IEqualityComparer{T}" />
    public sealed class CharComparer : IEqualityComparer<char>
    {
        public static readonly CharComparer OrdinalIgnoreCase = new CharComparer(StringComparison.OrdinalIgnoreCase);
        public static readonly CharComparer Ordinal = new CharComparer(StringComparison.Ordinal);
        public static readonly CharComparer InvariantCulture = new CharComparer(StringComparison.InvariantCulture);
        public static readonly CharComparer InvariantCultureIgnoreCase = new CharComparer(StringComparison.InvariantCultureIgnoreCase);
        public static readonly CharComparer CurrentCulture = new CharComparer(StringComparison.CurrentCulture);
        public static readonly CharComparer CurrentCultureIgnoreCase = new CharComparer(StringComparison.CurrentCultureIgnoreCase);

        public static CharComparer Get(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                    return CurrentCulture;
                case StringComparison.CurrentCultureIgnoreCase:
                    return CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return OrdinalIgnoreCase;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison));
            }
        }

        public StringComparison StringComparison { get; }

        public bool IgnoreCase => StringComparison == StringComparison.OrdinalIgnoreCase ||
                                  StringComparison == StringComparison.CurrentCultureIgnoreCase ||
                                  StringComparison == StringComparison.InvariantCultureIgnoreCase;

        public CultureInfo Culture
        {
            get
            {
                switch (StringComparison)
                {
                    case StringComparison.CurrentCulture:
                    case StringComparison.CurrentCultureIgnoreCase:
                        return CultureInfo.CurrentCulture;
                    case StringComparison.InvariantCulture:
                    case StringComparison.InvariantCultureIgnoreCase:
                        return CultureInfo.InvariantCulture;
                    case StringComparison.Ordinal:
                    case StringComparison.OrdinalIgnoreCase:
                        return CultureInfo.InvariantCulture;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        

        /// <summary>
        /// Initializes a new instance of the <see cref="CharComparer"/> class.
        /// </summary>
        /// <param name="ignoreCase">if set to <c>true</c> ignores case; otherwise <c>false</c> - case sensitive.</param>
        private CharComparer(bool ignoreCase)
        {
            StringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharComparer"/> class.
        /// </summary>
        private CharComparer(StringComparison stringComparison)
        {
            StringComparison = stringComparison;
        }

        public bool IsCharContainedByCollection(char c, ICollection<char> charSet)
        {
            var contains = charSet.Contains(c);

            if (contains)
            {
                return true;
            }
            
            if (!IgnoreCase)
            {
                return false;
            }

            if (StringComparison == StringComparison.OrdinalIgnoreCase)
            {
                var lo = char.ToLower(c);
                var up = char.ToUpper(c);

                return  charSet.Contains(c != lo ? lo : up);
            }
            else
            {
                var lo = char.ToLower(c, Culture);
                var up = char.ToUpper(c, Culture);

                return  charSet.Contains(c != lo ? lo : up);
            }
        }

        public bool Equals(char ch1, char ch2)
        {
            if (!IgnoreCase)
            {
                return ch1 == ch2;
            }

            if (StringComparison == StringComparison.OrdinalIgnoreCase)
            {
                return char.ToLower(ch1) == char.ToLower(ch2);
            }

            return char.ToLower(ch1, Culture) == char.ToLower(ch2, Culture);
        }
            
        public int GetHashCode(char obj)
        {
            return IgnoreCase ? char.ToLower(obj, Culture).GetHashCode() : obj.GetHashCode();
        }
    }
}