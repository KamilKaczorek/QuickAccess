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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickAccess.DataStructures.Common.CharMatching;

namespace QuickAccess.DataStructures.Common.RegularExpression
{
    public class StandardRegularExpressionFactoryContext : IRegularExpressionFactoryContext
    {
        private readonly IDefineSpecialCharacters _specialCharactersDefinition;
		private readonly Dictionary<string, int> _usedGroupNamesCounters = new Dictionary<string, int>();
		private readonly Dictionary<string, string> _regexGroupNameToOriginalGroupName = new Dictionary<string, string>();

        public StandardRegularExpressionFactoryContext(IDefineSpecialCharacters specialCharactersDefinition)
        {
            _specialCharactersDefinition = specialCharactersDefinition;
        }

        public string GetOriginalGroupName(string regexGroupName)
		{
			return _regexGroupNameToOriginalGroupName.TryGetValue(regexGroupName, out var original) ? original : regexGroupName;
		}

		/// <inheritdoc />
		public void Reset()
		{
			_usedGroupNamesCounters.Clear();
			_regexGroupNameToOriginalGroupName.Clear();
		}

		/// <inheritdoc />
		public string GetUniqueAndValidGroupNameFor(string name)
		{
			var groupName = RemoveSpecialCharactersAndReplacesSpacesByUnderscore(name);
			string res;

			if (!_usedGroupNamesCounters.TryGetValue(groupName, out var counter))
			{
				counter = 0;
				res = groupName;
			}
			else
			{
				do res = $"{groupName}_{counter++}"; while (_usedGroupNamesCounters.ContainsKey(res));
			}

			_usedGroupNamesCounters[groupName] = counter;

			if (name != res)
			{
				_regexGroupNameToOriginalGroupName.Add(res, name);
			}

			return res;
		}

		internal string RemoveSpecialCharactersAndReplacesSpacesByUnderscore(string text)
		{
			var specialCount = text.Count(_specialCharactersDefinition.IsSpecialCharacter);

			if (specialCount == 0)
			{
				return text;
			}

			if (specialCount == text.Length)
			{
				return string.Empty;
			}

			var sb = new StringBuilder(text.Length-specialCount);

			var isPrevWhiteSpace = true;
			foreach (var ch in text.Where(ch => !_specialCharactersDefinition.IsSpecialCharacter(ch)))
			{
				var isWhiteSpace = char.IsWhiteSpace(ch);

				if (!isPrevWhiteSpace || !isWhiteSpace)
				{
					sb.Append(isWhiteSpace ? '_' : ch);
				}
				
				isPrevWhiteSpace = isWhiteSpace;
			}

			return sb.ToString();
		}
	}
}