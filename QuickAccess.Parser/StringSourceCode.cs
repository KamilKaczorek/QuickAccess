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
using QuickAccess.Parser.Flexpressions.Model.Caching;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The parsing context of the source code provided by the string object.
    /// </summary>
    /// <seealso cref="ISourceCode" />
    /// <seealso cref="IParsingContextStreamParent" />
    public class StringSourceCode : ISourceCode,
        IParsingContextStreamParent
    {
        private readonly IParsingContextStreamFactory _contextStreamFactory;
        private readonly ISourceCodeFragmentFactory _sourceCodeFragmentFactory;
        private readonly string _codeBuffer;
        private readonly int _bufferOffset;
        private readonly int _length;
        private readonly int _maxContextStackSize;
        private IParsingError _error;
        private int _errorPos;

        private int _accepted = -1;

        char IParsingContextStreamParent.this[int idx] => _codeBuffer[idx];

        int IParsingContextStreamParent.Length => _length;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSourceCode"/> class.
        /// </summary>
        /// <param name="contextStreamFactory">The context stream factory.</param>
        /// <param name="sourceCodeFragmentFactory">The source code fragment factory.</param>
        /// <param name="sourceCodeBuffer">The buffer that contains the source code.</param>
        /// <param name="maxContextStackSize">
        /// Maximum size of the context stack - defines the maximum depth of the parented parsing context chain.
        /// </param>
        /// <param name="offset">
        /// The string offset to the source code, defines where the source code begins.
        /// By default is <c>0</c>.
        /// </param>
        /// <param name="length">
        /// The length of the source code - defines where the source code ends.
        /// By default is <c>-1</c> - the source code ends with a source string.
        /// </param>
        public StringSourceCode(IParsingContextStreamFactory contextStreamFactory, ISourceCodeFragmentFactory sourceCodeFragmentFactory, string sourceCodeBuffer, int offset = 0, int length = -1, int maxContextStackSize = -1)
        {
            _errorPos = -1;
            _codeBuffer = sourceCodeBuffer;
            _sourceCodeFragmentFactory = sourceCodeFragmentFactory;
            _contextStreamFactory = contextStreamFactory;
            _maxContextStackSize = maxContextStackSize;
            _length = length < 0 ? _codeBuffer.Length - offset : length;
            _bufferOffset = offset;
        }

        /// <inheritdoc />
        void IParsingContextStreamParent.AcceptChildPosition(int idx)
        {
            _accepted = idx;
        }


        /// <inheritdoc />
        string IParsingContextStreamParent.GetString(int offset, int length)
        {
            return length <= 0 ? string.Empty : _codeBuffer.Substring(offset, length);
        }

        /// <inheritdoc />
        ISourceCodeFragment IParsingContextStreamParent.GetFragment(int offset, int length)
        {
            length = Math.Min(length, _length - offset);

            return _sourceCodeFragmentFactory.Create(this, offset, length);
        }

        /// <inheritdoc />
        void IParsingContextStreamParent.SetError(IParsingError code, int idx)
        {
            if (idx < _errorPos)
            {
                return;
            }

            _errorPos = idx;
            _error = code;
        }

        /// <inheritdoc />
        public IParsingContextStream GetFurtherContext()
        {
            return _contextStreamFactory.Create(this, _bufferOffset, _maxContextStackSize,
                new ParsingResultCache());
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <returns></returns>
        public ParsedSourceError GetError()
        {
            if (_accepted >= _errorPos)
            {
                return null;
            }


            return new ParsedSourceError(
                _error,
                _sourceCodeFragmentFactory.Create(this, _bufferOffset, _errorPos - _bufferOffset),
                _sourceCodeFragmentFactory.Create(this, _errorPos, _length - (_errorPos - _bufferOffset)));
        }
    }
}