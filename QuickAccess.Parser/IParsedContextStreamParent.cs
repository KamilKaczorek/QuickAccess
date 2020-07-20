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

namespace QuickAccess.Parser
{
    /// <summary>
    /// The interface of the parent of <see cref="ParsingContextStream"/>.   
    /// </summary>
    public interface IParsingContextStreamParent
    {
        /// <summary>
        /// Gets the source character at the specified index.
        /// </summary>
        /// <param name="idx">The absolute index.</param>
        /// <returns>The character.</returns>
        char this[int idx] { get; }

        /// <summary>
        /// Gets the length of the source.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        int Length { get; }

        /// <summary>
        /// Accepts the child context position at specific absolute index.
        /// Please refer to <see cref="IParsingContextStream.Accept"/> for more details.
        /// </summary>
        /// <param name="idx">The absolute index.</param>
        void AcceptChildPosition(int idx);

        /// <summary>
        /// Gets the source fragment as a string.
        /// <seealso cref="GetFragment"/>
        /// </summary>
        /// <param name="offset">The absolute source offset.</param>
        /// <param name="length">The source length.</param>
        /// <returns>The source fragment string.</returns>
        string GetString(int offset, int length);

        /// <summary>
        /// Gets the source fragment.
        /// <seealso cref="GetString"/>
        /// </summary>
        /// <param name="offset">The absolute offset.</param>
        /// <param name="length">The source length.</param>
        /// <returns>The source fragment.</returns>
        ISourceCodeFragment GetFragment(int offset, int length);

        /// <summary>
        /// Sets the parsing error at specified absolute position.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="idx">The absolute index.</param>
        void SetError(IParsingError error, int idx);
    }
}