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
using System.Linq;

namespace QuickAccess.DataStructures.Common.CharMatching
{
    /// <summary>
    /// Implements the string comparison based on character equality comparer.
    /// </summary>
    /// <seealso cref="IEqualityComparer{T}" />
    public sealed class CharComparerToStringComparerAdapter : IEqualityComparer<string>
    {
        private readonly IEqualityComparer<char> _charComparer;
        private readonly StringComparison? _comparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharComparerToStringComparerAdapter"/> class.
        /// </summary>
        /// <param name="charComparer">The character comparer to compare strings.</param>
        public CharComparerToStringComparerAdapter(IEqualityComparer<char> charComparer)
        {
            _charComparer = charComparer;
            _comparison = (_charComparer as CharComparer)?.StringComparison;
        }

        public CharComparerToStringComparerAdapter(StringComparison comparison)
        {
            _comparison = comparison;
        }

        public bool Equals(string x, string y)
        {
            if (_comparison != null)
            {
                return string.Equals(x, y, _comparison.Value);
            }


            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.Length != y.Length)
            {
                return false;
            }

            for (var idx = 0; idx < x.Length; idx++)
            {
                if (!_charComparer.Equals(x[idx], y[idx]))
                {
                    return false;
                }
            }

            return true;
        }


        public int GetHashCode(string obj)
        {
            if (_comparison != null)
            {
                return obj.GetHashCode(_comparison.Value);
            }

            if (obj.Length == 0)
            {
                return 0;
            }

            if (obj.Length == 1)
            {
                return _charComparer.GetHashCode(obj[0]);
            }

            return HashCode.Combine(obj.Length, _charComparer.GetHashCode(obj[0]), _charComparer.GetHashCode(obj[^1]));
        }
    }
}