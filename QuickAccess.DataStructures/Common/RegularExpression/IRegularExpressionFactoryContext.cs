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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion
namespace QuickAccess.DataStructures.Common.RegularExpression
{
	/// <summary>
	/// The context of the <see cref="IRegularExpressionFactory"/>.
	/// Context contains also the building state of the expression.
	/// </summary>
	public interface IRegularExpressionFactoryContext
	{
		/// <summary>Gets the unique and valid group name for the specified original group name.</summary>
		/// <remarks>
		/// Regular expression must have unique names of groups that is why, when the group is used many times in one expression the unique regex group name must be created.
		/// To get back the original group name for the specific regex group name use <seealso cref="GetOriginalGroupName"/> method.
		/// </remarks>
		/// <param name="originalGroupName">Name of the original group.</param>
		/// <returns>The regex group name.</returns>
		string GetUniqueAndValidGroupNameFor(string originalGroupName);

		/// <summary>Gets the name of the original group for the given factual regex group name created by the <see cref="GetUniqueAndValidGroupNameFor"/> method.</summary>
		/// <param name="regexGroupName">Name of the regex group crated by the <see cref="GetUniqueAndValidGroupNameFor"/> for this context.</param>
		/// <returns>The original group name for which the regex group was created.</returns>
		string GetOriginalGroupName(string regexGroupName);

		/// <summary>Determines whether the specified character is a special regex character (symbol).</summary>
		/// <param name="character">The character.</param>
		/// <returns><c>true</c> if is a special regex character; otherwise, <c>false</c>.</returns>
		bool IsSpecialCharacter(char character);

		/// <summary>Determines whether the specified character is a tab (<c>'\t'</c>) character.</summary>
		/// <param name="character">The character.</param>
		/// <returns><c>true</c> if is a tab; otherwise, <c>false</c>.</returns>
		bool IsTab(char character);

		/// <summary>Determines whether the specified character is a white space character.</summary>
		/// <param name="character">The character.</param>
		/// <returns><c>true</c> if is a white space; otherwise, <c>false</c>.</returns>
		bool IsWhiteSpaceCharacter(char character);

		/// <summary>
		/// Resets the state of the built expression.
		/// The information about group names created by the <see cref="GetUniqueAndValidGroupNameFor"/> will be lost.
		/// </summary>
		void Reset();
		
	}
}