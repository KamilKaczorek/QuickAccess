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
using static QuickAccess.DataStructures.Common.CharMatching.CharactersRangeExtensions;

namespace QuickAccess.DataStructures.Common.CharMatching
{
	[Flags]
	public enum StandardCharactersRange
	{
		None = 0,

		UpperLetter = 0x01,
		LowerLetter = 0x02,
		Digit = 0x04,
		Underscore = 0x08,

		Space = 0x10,
		Tab = 0x20,
        NewLine = 0x40,
		Return = 0x80,

        NonUpperLetter = UpperLetter << NegativePatternsShift,
        NonLowerLetter = LowerLetter << NegativePatternsShift,
        NonDigit = Digit << NegativePatternsShift,
        NonUnderscore = Underscore << NegativePatternsShift,

        NonSpace = Space << NegativePatternsShift,
        NonTab = Tab << NegativePatternsShift,
        NonNewLine = NewLine << NegativePatternsShift,
        NonReturn = Return << NegativePatternsShift,

		LineBreak = NewLine | Return,
		SpaceOrTab = Tab | Space,
		WhiteSpace = SpaceOrTab | LineBreak,
		Letter = UpperLetter | LowerLetter,
		LetterOrDigit = Letter | Digit,
		WordCharacter = LetterOrDigit | Underscore,
		WordCharacterOrWhiteSpace = WordCharacter | WhiteSpace,

        NonLineBreak = NonNewLine | NonReturn,
        NonSpaceOrTab = NonTab | NonSpace,
        NonWhiteSpace = NonSpaceOrTab | NonLineBreak,
        NonLetter = NonUpperLetter | NonLowerLetter,
        NonLetterOrDigit = NonLetter | NonDigit,
        NonWordCharacter = NonLetterOrDigit | NonUnderscore,
        NonWordCharacterOrWhiteSpace = NonWordCharacter | NonWhiteSpace,
	}
}