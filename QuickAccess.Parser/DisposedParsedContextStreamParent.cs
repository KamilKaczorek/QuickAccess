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
using System.Diagnostics;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The implementation of <see cref="IParsingContextStreamParent"/> that can be used by the implementation of 
    /// <see cref="IParsingContextStream"/> to provide protection against operation invocation after dispose.
    /// </summary>
    /// <seealso cref="IParsingContextStreamParent" />
    public sealed class DisposedParsedContextStreamParent : IParsingContextStreamParent
    {
        private const string DisposedObjectName = "ParsedContextStream";
        public static readonly IParsingContextStreamParent Instance = new DisposedParsedContextStreamParent();

        /// <summary>
        /// Throws if disposed instance.
        /// </summary>
        /// <param name="parsedContextStreamParent">The parsed context stream parent.</param>
        /// <exception cref="ObjectDisposedException">Thrown if specified object is <see cref="Instance"/>.</exception>
        public static void ThrowIfDisposedInstance(IParsingContextStreamParent parsedContextStreamParent)
        {
            if (ReferenceEquals(Instance, parsedContextStreamParent))
            {
                throw new ObjectDisposedException(DisposedObjectName);
            }
        }


        /// <summary>
        /// Prevents a default instance of the <see cref="DisposedParsedContextStreamParent"/> class from being created.
        /// Use <see cref="Instance"/> to have instance of this class.
        /// </summary>
        private DisposedParsedContextStreamParent()
        {
        }

        [DebuggerHidden]
        public char this[int idx] => throw new ObjectDisposedException(DisposedObjectName);

        [DebuggerHidden]
        public int Length => throw new ObjectDisposedException(DisposedObjectName);

        [DebuggerHidden]
        public void AcceptChildPosition(int idx)
        {
            throw new ObjectDisposedException(DisposedObjectName);
        }
        [DebuggerHidden]
        public string GetString(int offset, int length)
        {
            throw new ObjectDisposedException(DisposedObjectName);
        }

        [DebuggerHidden]
        public ISourceCodeFragment GetFragment(int offset, int length)
        {
            throw new ObjectDisposedException(DisposedObjectName);
        }

        [DebuggerHidden] 
        public void SetError(IParsingError code, int idx)
        {
            throw new ObjectDisposedException(DisposedObjectName);
        }
    }
}