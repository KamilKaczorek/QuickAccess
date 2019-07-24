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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickAccess.Parser
{
	/// <summary>
	/// The interface of the stream enumerator for the specific parsing context, designed for the left side recursive descent parser.
	/// When instance created, the position is set to the parent stream position - 1.
	/// It allows to provide the stack of parented contexts via <see cref="ISourceCode.GetFurtherContext" /> method.
	/// The instance should be created using <see cref="IParsingContextStreamFactory" /> contract.
	/// <example>
	/// private IParsedFragment ParseUnsignedIntValue(IParsedSource src)
	/// {
	/// using (var ctx = src.GetFurtherContext())
	/// {
	/// if (ctx.ParseDigits() == 0)
	/// {
	/// ctx.SetError(ParsingErrors.DigitExpected);
	/// // on Dispose the current position will be ignored.
	/// return null;
	/// }
	/// ctx.Accept();
	/// //on Dispose the position of a parent context will be advanced to the accepted position.
	/// return ctx.GetAcceptedFragment();
	/// }
	/// </example></summary>
	/// <seealso cref="QuickAccess.Parser.ISourceCode" />
	/// <seealso cref="System.IDisposable" />
	/// <seealso cref="ISourceCode" />
	/// <seealso cref="IEnumerator{T}" />
	public interface IParsingContextStream : ISourceCode, IDisposable, IEquatable<IParsingContextStream>, IComparable<IParsingContextStream>
	{
		/// <summary>
		/// Gets a value indicating whether the stream contains next character. 
		/// (the current character is not last one and the current position is not end of stream).
		/// </summary>
		/// <value>
		///   <c>true</c> if it contains next character; otherwise, <c>false</c>.
		/// </value>
		[Pure]
		bool HasNext { get; }

		/// <summary>
		/// Gets a value indicating whether the current position is at the end of the stream.
		/// </summary>
		/// <value>
		///   <c>true</c> if the current position reached end of the stream; otherwise, <c>false</c>.
		/// </value>
		[Pure]
		bool EndOfStream { get; }

		/// <summary>
		/// Gets the next character.
		/// If there is no next character the <c>'\0'</c> character will be returned.
		/// </summary>
		/// <value>
		/// The next character or <c>'\0'</c>.
		/// </value>
		[Pure]
		char Next { get; }

		/// <summary>
		/// Gets the character at the current position of the context stream.
		/// <seealso cref="Next"/>
		/// <remarks>
		/// After initialization the current index points to the position before the first character in the context stream, 
		/// (regular enumerator behavior - see also <see cref="IEnumerator"/>).
		/// </remarks>
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		[Pure]
		char Current { get; }

		/// <summary>
		/// Advances the current stream position to the next character.        
		/// </summary>
		/// <returns>
		/// <c>true</c> if the current position was successfully advanced to the next character; 
		/// otherwise <c>false</c> - end of stream.
		/// </returns>
		bool MoveNext();

		/// <summary>
		/// Rollbacks the context to the initial position, which is before the first character in the context stream.
		/// Resets the internal offset value, which was set by <see cref="SetOffsetToCurrent"/> method.
		/// Removes acceptance mark, so all previous acceptances will have no effect.
		/// </summary>
		void Rollback();

		/// <summary>
		/// Advances the internal offset of the current stream context to the current position.
		/// The <see cref="GetAcceptedFragment"/> will return the fragment from the offset position.
		/// <remarks>
		/// Execution of the <see cref="Rollback"/> method resets the internal offset to the initial position.
		/// </remarks>
		/// </summary>
		void SetOffsetToCurrent();

		/// <summary>
		/// Accepts parsed fragment of the current stream from the offset position to the current position.
		/// On <see cref="IDisposable.Dispose"/> the parent context position will be set to accepted position, unless the 
		/// acceptance will be rolled back by <see cref="IEnumerator.Reset"/> method.
		/// The <see cref="GetAcceptedFragment"/> will return accepted fragment.
		/// <seealso cref="SetOffsetToCurrent"/>
		/// <seealso cref="ParsingContextStreamExtensions.AcceptAndGetFragment"/>
		/// </summary>
		void Accept();

		/// <summary>
		/// Gets the accepted fragment of the current stream, from the offset position to the accepted position.
		/// <seealso cref="Accept"/>
		/// <seealso cref="SetOffsetToCurrent"/>
		/// <seealso cref="ParsingContextStreamExtensions.AcceptAndGetFragment"/>
		/// </summary>
		/// <returns>
		/// Accepted fragment. If the fragment was not accepted, then empty fragment.
		/// </returns>
		[Pure]
		ISourceCodeFragment GetAcceptedFragment();

		/// <summary>
		/// Sets the parsing error at current position.
		/// </summary>
		/// <param name="error">The parsing error.</param>
		void SetError(IParsingError error);

		/// <summary>Gets the accepted position.</summary>
		/// <value>The accepted position.</value>
		[Pure]
		int AcceptedPosition { get; }
	}
}