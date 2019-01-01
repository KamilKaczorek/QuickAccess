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

using System.Collections.Generic;

namespace QuickAccess.Parser.SmartExpressions
{
	public class CapturingGroupBrick : ParsingBrick
	{
		public string GroupName { get; }
		public ParsingBrick Content { get; }

		/// <inheritdoc />
		public override bool IsEmpty => Content.IsEmpty;

		public CapturingGroupBrick(ParsingBrick content, string groupName)
		{
			Content = content;
			GroupName = groupName;
			ApplyRuleDefinition(Content, GroupName, Content, true);
		}

		/// <inheritdoc />
		protected override void ApplyRuleDefinition(string name, ParsingBrick content, bool recursion)
		{
			if (name == GroupName)
			{
				return;
			}

			ApplyRuleDefinition(Content, name, content, recursion);
		}

		/// <inheritdoc />
		public override string ExpressionId => Content.ExpressionId;

		public override string ToRegularExpressionString(Dictionary<string, int> usedGroupNames)
		{
			var groupName = GroupName;
			if (!string.IsNullOrEmpty(GroupName) && usedGroupNames != null)
			{

				if (!usedGroupNames.TryGetValue(GroupName, out var counter))
				{
					counter = 0;
				}
				else
				{
					groupName += $"_{counter}";
				}
				
				++counter;
				usedGroupNames[GroupName] = counter;

			}
			
			return string.IsNullOrEmpty(GroupName) ? $"({Content.ToRegularExpressionString(usedGroupNames)})" : $"(?<{groupName}>{Content.ToRegularExpressionString(usedGroupNames)})";
		}

		/// <inheritdoc />
		public override bool Equals(ParsingBrick other)
		{
			if (IsEmpty && (other?.IsEmpty ?? false))
			{
				return true;
			}

			return other is CapturingGroupBrick cb && cb.GroupName == GroupName && cb.Content.Equals(Content);
		}



		/// <inheritdoc />
		public override string ToString()
		{
			return $"{GroupName} ::= {Content}";
		}
	}
}