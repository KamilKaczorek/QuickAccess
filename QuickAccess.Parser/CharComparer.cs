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

namespace QuickAccess.Parser
{
    /// <summary>
    /// The character comparer.
    /// Implements case sensitive and insensitive invariant character comparison. 
    /// <seealso cref="CharComparerToStringComparerAdapter"/>
    /// </summary>
    /// <seealso cref="IEqualityComparer{T}" />
    public sealed class CharComparer : IEqualityComparer<char>
    {
        public static readonly IEqualityComparer<char> CaseInsensitive = new CharComparer(true);
        public static readonly IEqualityComparer<char> CaseSensitive = new CharComparer(false);

        private readonly bool _ignoreCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharComparer"/> class.
        /// </summary>
        /// <param name="ignoreCase">if set to <c>true</c> ignores case; otherwise <c>false</c> - case sensitive.</param>
        private CharComparer(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
        }

        public bool IsCharContainedByCollection(char c, ICollection<char> charSet)
        {
            var contains = charSet.Contains(c);

            if (contains)
            {
                return true;
            }
            
            if (!_ignoreCase)
            {
                return false;
            }

            return charSet.Contains(char.ToLowerInvariant(c)) || charSet.Contains(char.ToUpperInvariant(c));
        }

        public bool Equals(char ch1, char ch2)
        {
            return _ignoreCase ? char.ToLowerInvariant(ch1) == char.ToLowerInvariant(ch2) :  ch1 == ch2;       
        }
            
        public int GetHashCode(char obj)
        {
            return char.ToLowerInvariant(obj).GetHashCode();
        }
    }
}