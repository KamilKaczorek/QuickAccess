#region LICENSE [BSD-2-Clause]
// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2019 by Kamil Piotr Kaczorek
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
using System.Diagnostics.Contracts;

namespace QuickAccess.Parser
{
	/// <summary>
	/// Extensions of <seealso cref="IParsingContextStream"/>.
	/// </summary>
	public static class ParsingContextStreamExtensions
	{
		/// <summary>
		/// Accepts the source context (by <see cref="IParsingContextStream.Accept"/>) and returns accepted fragment (<see cref="IParsingContextStream.GetAcceptedFragmentOrEmpty"/>).
		/// <seealso cref="IParsingContextStream.Accept"/>
        /// <seealso cref="IParsingContextStream.GetAcceptedFragmentOrEmpty"/>
        /// <seealso cref="GetAcceptedFragment"/>
        /// <seealso cref="TryGetAcceptedFragment"/>
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>The accepted fragment.</returns>
		public static ISourceCodeFragment AcceptAndGetFragment(this IParsingContextStream source)
		{
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

			source.Accept();
			return source.GetAcceptedFragmentOrEmpty();
		}

        /// <summary>
        /// Determines whether the specific parsing context has accepted fragment.
        /// <seealso cref="IParsingContextStream.Accept"/>
        /// <seealso cref="IParsingContextStream.GetAcceptedFragmentOrEmpty"/>
        /// <seealso cref="AcceptAndGetFragment"/>
        /// <seealso cref="TryGetAcceptedFragment"/>
        /// </summary>
        /// <returns>
        /// <c>true</c> if the fragment was accepted; otherwise <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsAccepted(this IParsingContextStream source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var isAccepted = source.AcceptedPosition < 0;

            return isAccepted;
        }

        /// <summary>
        /// Gets the accepted fragment of the current stream, from the offset position to the accepted position.
        /// <seealso cref="IParsingContextStream.Accept"/>
        /// <seealso cref="IParsingContextStream.GetAcceptedFragmentOrEmpty"/>
        /// <seealso cref="AcceptAndGetFragment"/>
        /// <seealso cref="TryGetAcceptedFragment"/>
        /// </summary>
        /// <returns>
        /// Accepted fragment. If the fragment was not accepted, then throws <see cref="ArgumentException"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when given fragment is not accepted.</exception>
        [Pure]
        public static ISourceCodeFragment GetAcceptedFragment(this IParsingContextStream source)
        {
            if (source == null)
            {
				throw new ArgumentNullException(nameof(source));
            }

            if (source.AcceptedPosition < 0)
            {
				throw new ArgumentException("Unable to get accepted fragment: the parsing context is not accepted.", nameof(source));
            }

            return source.GetAcceptedFragmentOrEmpty();
        }

        /// <summary>
        /// Tries to get the accepted fragment of the current stream, from the offset position to the accepted position.
        /// <seealso cref="IParsingContextStream.Accept"/>
        /// <seealso cref="IParsingContextStream.GetAcceptedFragmentOrEmpty"/>
        /// <seealso cref="AcceptAndGetFragment"/>
        /// <seealso cref="GetAcceptedFragment"/>
        /// </summary>
        /// <param name="source">The parsing context.</param>
        /// <param name="acceptedFragment">The accepted fragment.</param>
        /// <returns>
        /// <c>true</c> if the fragment was accepted; otherwise <c>false</c>.
        /// </returns>
        [Pure]
        public static bool TryGetAcceptedFragment(this IParsingContextStream source, out ISourceCodeFragment acceptedFragment)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.AcceptedPosition < 0)
            {
                acceptedFragment = null;
                return false;
            }

            acceptedFragment =  source.GetAcceptedFragmentOrEmpty();
            return true;
        }
	}
}