#region LICENSE [BSD-2-Clause]

// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2018 by Kamil Piotr Kaczorek
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


using System.Collections;
using System.Collections.Generic;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The implementation of the source code fragment.
    /// <seealso cref="ISourceCode" />
    /// </summary>
    /// <seealso cref="ISourceCodeFragment" />
    /// <seealso cref="ISourceCodeFragmentFactory" />
    /// <seealso cref="IEnumerable{T}" />
    public sealed class SourceCodeFragment : ISourceCodeFragment
    {
        private readonly IParsingContextStreamParent _parent;
        private string _cachedValue;

        /// <inheritdoc />
        public int SourcePosition { get; }

        /// <inheritdoc />
        public int Length { get; }

        /// <summary>
        /// Initializes the new instance of the <see cref="ISourceCodeFragment" /> type.
        /// </summary>
        /// <param name="parent">The source code parent.</param>
        /// <param name="offset">The absolute source code offset, where the fragment starts.</param>
        /// <param name="length">The length of the source code fragment.</param>
        public SourceCodeFragment(IParsingContextStreamParent parent, int offset, int length)
        {
            _cachedValue = null;
            _parent = parent;
            SourcePosition = offset;
            Length = length;
        }

        public IEnumerator<char> GetEnumerator()
        {
            for (var idx = 0; idx < Length; idx++)
            {
                yield return _parent[idx + SourcePosition];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            return SourcePosition;
        }

        public override string ToString()
        {
            return Length <= 0
                ? string.Empty
                : (_cachedValue ?? (_cachedValue = _parent.GetString(SourcePosition, Length)));
        }
    }
}