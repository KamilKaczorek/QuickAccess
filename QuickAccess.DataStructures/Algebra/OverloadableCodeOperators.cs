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

namespace QuickAccess.DataStructures.Algebra
{
	/// <summary>
	/// Enumeration of all overloadable c# operators in precedence order (lowest value - highest priority).
	/// <seealso cref="OverloadableCodeOperatorsGroups"/>
	/// </summary>
	[Flags]
	public enum OverloadableCodeOperators
	{
		/// <summary>None.</summary>
		None = 0,

		/// <summary>The <c>++</c> operator.</summary>
		Increment = 			0x00_00_00_01,
		/// <summary>The <c>--</c> operator.</summary>
		Decrement = 			0x00_00_00_02,

		/// <summary>The unary <c>+</c> operator.</summary>
		Plus = 					0x00_00_00_04,
		/// <summary>The unary <c>-</c> operator.</summary>
		Minus = 				0x00_00_00_08,

		/// <summary>The <c>!</c> operator.</summary>
		LogicalNegation = 		0x00_00_00_10,

		/// <summary>The <c>~</c> operator.</summary>
		BitwiseComplement = 	0x00_00_00_20,

		/// <summary>The '<c>true</c>' operator.</summary>
		TrueOperator = 			0x00_00_00_40,

		/// <summary>The '<c>false</c>' operator.</summary>
		FalseOperator = 		0x00_00_00_80,

		/// <summary>The <c>*</c> operator.</summary>
		Mul = 					0x00_00_10_00,
		/// <summary>The <c>/</c> operator.</summary>
		Div = 					0x00_00_20_00,
		/// <summary>The <c>%</c> operator.</summary>
		Mod = 					0x00_00_40_00,

		/// <summary>The binary <c>+</c> operator.</summary>
		Sum = 					0x00_01_00_00,

		/// <summary>The binary <c>-</c> operator.</summary>
		Sub = 					0x00_02_00_00,
		
		/// <summary>The <c>&lt;&lt;</c> operator.</summary>
		LeftShift = 			0x00_04_00_00,
		/// <summary>The <c>&gt;&gt;</c> operator.</summary>
		RightShift = 			0x00_08_00_00,
		/// <summary>The <c>&lt;</c> operator.</summary>
		LessThan = 				0x00_10_00_00,
		/// <summary>The <c>&lt;=</c> operator.</summary>
		LessThanOrEqual = 		0x00_20_00_00,
		/// <summary>The <c>&gt;</c> operator.</summary>
		GreaterThan = 			0x00_40_00_00,
		/// <summary>The <c>&gt;=</c> operator.</summary>
		GreaterThanOrEqual = 	0x00_80_00_00,
		/// <summary>The <c>==</c> operator.</summary>
		Equal = 				0x01_00_00_00,
		/// <summary>The <c>!=</c> operator.</summary>
		Unequal = 				0x02_00_00_00,

		/// <summary>The <c>&amp;</c> operator.</summary>
		And = 					0x10_00_00_00,
		/// <summary>The <c>^</c> operator.</summary>
		XOr = 					0x20_00_00_00,
		/// <summary>The <c>|</c> operator.</summary>
		Or = 					0x40_00_00_00
	}
}