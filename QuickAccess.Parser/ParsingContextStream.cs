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


using System;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The implementation of parsing context stream.
    /// For more details refer to <see cref="IParsingContextStream"/>.
    /// <seealso cref="StringSourceCode"/>
    /// </summary>
    /// <seealso cref="IParsingContextStream" />
    /// <seealso cref="IParsingContextStreamParent" />
    public sealed class ParsingContextStream : IParsingContextStream,
        IParsingContextStreamParent
    {
        private IParsingContextStreamParent _parent;
        private readonly int _initialOffset;
        private int _internalOffset;
        private int _acceptedPosition;
        private int _position;
        private readonly int _maxDeep;


        private int Offset => _initialOffset + _internalOffset;

        /// <inheritdoc />
        public bool HasNext => CurrentIndex + 1 < _parent.Length;

        /// <inheritdoc />
        public bool EndOfStream => CurrentIndex >= _parent.Length;

        /// <inheritdoc />
        public char Current => EndOfStream ? ParserExtensions.NullChar : _parent[CurrentIndex];

        /// <inheritdoc />
        public char Next => !HasNext ? ParserExtensions.NullChar : _parent[CurrentIndex + 1];

        private int CurrentIndex => Offset + _position;

        /// <inheritdoc />
        int IParsingContextStreamParent.Length => _parent.Length;

        /// <inheritdoc />
        char IParsingContextStreamParent.this[int idx] => _parent[idx];

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingContextStream"/> class.
        /// </summary>
        /// <param name="parent">The parent context.</param>
        /// <param name="offset">The context absolute offset within the source code.</param>
        /// <param name="maxDeep">The maximum deep of parented contexts.</param>
        public ParsingContextStream(IParsingContextStreamParent parent, int offset, int maxDeep)
        {
            _initialOffset = offset;
            _maxDeep = maxDeep;
            _internalOffset = 0;
            _parent = parent;
            Rollback();
        }

        /// <inheritdoc />
        void IParsingContextStreamParent.AcceptChildPosition(int idx)
        {
            _position = idx - Offset;
        }

        /// <inheritdoc />
        string IParsingContextStreamParent.GetString(int offset, int length)
        {
            return length <= 0 ? string.Empty : _parent.GetString(offset, length);
        }

        /// <inheritdoc />
        ISourceCodeFragment IParsingContextStreamParent.GetFragment(int offset, int length)
        {
            return _parent.GetFragment(offset, length);
        }

        /// <inheritdoc />
        void IParsingContextStreamParent.SetError(IParsingError code, int idx)
        {
            _parent.SetError(code, idx);
        }

        /// <inheritdoc />
        public IParsingContextStream GetFurtherContext()
        {
            var maxDeep = _maxDeep;
            if (_maxDeep >= 0)
            {
                if (_maxDeep == 0)
                {
                    throw new InvalidOperationException("Max size of parsing context stack exceeded.");
                }

                maxDeep--;
            }

            return new ParsingContextStream(this, CurrentIndex + 1, maxDeep);
        }

        /// <inheritdoc />
        public void Rollback()
        {
            _internalOffset = 0;
            _acceptedPosition = -1;
            _position = -1;
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (EndOfStream)
            {
                return false;
            }

            _position++;

            return !EndOfStream;
        }


        /// <inheritdoc />
        public void SetOffsetToCurrent()
        {
            _internalOffset = _position + 1;
            _position = -1;
        }

        /// <inheritdoc />
        public void Accept()
        {
            _acceptedPosition = _position;
        }

        /// <inheritdoc />
        public ISourceCodeFragment GetAcceptedFragment()
        {
            if (_acceptedPosition < 0)
            {
                return ParserExtensions.EmptyFragment;
            }

            var len = Math.Min(_acceptedPosition + 1, _parent.Length - Offset);

            return len <= 0 ? ParserExtensions.EmptyFragment : _parent.GetFragment(Offset, len);
        }

        /// <inheritdoc />
        public void SetError(IParsingError code)
        {
            _parent.SetError(code, CurrentIndex);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_acceptedPosition >= 0)
            {
                _parent.AcceptChildPosition(_acceptedPosition + Offset);
            }

            _parent = DisposedParsedContextStreamParent.Instance;
        }

        public override string ToString()
        {
            if (_parent == DisposedParsedContextStreamParent.Instance)
            {
                return "Disposed";
            }

            var len = EndOfStream ? _parent.Length - Offset : _position + 1;

            var str1 = _parent.GetString(Offset, len);


            var pos = Offset + len;

            var str2 = _parent.GetString(pos, _parent.Length - pos);

            return str1 + "»»»" + str2;
        }
    }
}