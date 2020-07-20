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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The interface of the source code fragment.
    /// <seealso cref="ISourceCode"/>
    /// </summary>
    /// <seealso cref="IEnumerable{T}" />
    public interface ISourceCodeFragment : IReadOnlyList<char>
    {
        /// <summary>
        /// Gets the absolute offset of a fragment within a source code.
        /// </summary>
        /// <value>
        /// The source position.
        /// </value>
        int SourcePosition { get; }
    }

    public sealed class StringCharacters : IReadOnlyList<char>
    {
        private readonly string _text;

        public StringCharacters(string text)
        {
            _text = text;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return _text.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _text.GetEnumerator();
        }

        public int Count => _text.Length;

        public char this[int index] => _text[index];

        public override string ToString()
        {
            return _text;
        }
    }

    public sealed class SourceCodeFragmentContentComparer : IEqualityComparer<IReadOnlyList<char>>
    {
        private readonly IEqualityComparer<char> _comparer;

        public SourceCodeFragmentContentComparer(IEqualityComparer<char> comparer = null)
        {
            _comparer = comparer ?? CharComparer.CaseSensitive;
        }

        public bool Equals(IReadOnlyList<char> x, IReadOnlyList<char> y)
        {
            if (x is null || y is null)
            {
                return false;
            }

            var count = x.Count;

            if (count != y.Count)
            {
                return false;
            }

            for (var idx = count-1; idx >= 0; --idx)
            {
                if (!_comparer.Equals(x[idx], y[idx]))
                {
                    return false;
                }
            }

            return true;
        }

        private int GetHashCode(IReadOnlyList<char> fragment, int index, int mul)
        {
            unchecked
            {
                return _comparer.GetHashCode(fragment[index])*mul;
            }
        }

        public int GetHashCode(IReadOnlyList<char> obj)
        {
            if (obj.Count == 0)
            {
                return 0;
            }
            unchecked
            {
                switch (obj.Count)
                {
                    case 1:
                        return 313 ^ GetHashCode(obj, 0, 277);
                    case 2:
                        return 626 ^ GetHashCode(obj, 0, 277) ^ GetHashCode(obj, 1, 557);
                    case 3:
                        return 939 ^ GetHashCode(obj, 0, 277) ^ GetHashCode(obj, 1, 557) ^ GetHashCode(obj, 2, 997);
                    default:
                        return (obj.Count * 313) ^ GetHashCode(obj, 0, 277) ^ GetHashCode(obj, 1, 557) ^ GetHashCode(obj, obj.Count-1, 997);
                }
            }
        }
    }
}