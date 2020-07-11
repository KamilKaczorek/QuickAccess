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

using System;

namespace QuickAccess.DataStructures.Algebra
{
	/// <summary>
	/// Enumeration of all overloadable c# operators in precedence order (lowest value - highest priority).
	/// <seealso cref="OverloadableCodeOperatorsGroups"/>
	/// </summary>
	[Flags]
	public enum OverloadableCodeOperator
	{
		/// <summary>The <c>++</c> operator.</summary>
		Increment = OverloadableCodeOperators.Increment,
		/// <summary>The <c>--</c> operator.</summary>
		Decrement = OverloadableCodeOperators.Decrement,

		/// <summary>The unary <c>+</c> operator.</summary>
		Plus = OverloadableCodeOperators.Plus,
		/// <summary>The unary <c>-</c> operator.</summary>
		Minus =  OverloadableCodeOperators.Minus,

		/// <summary>The <c>!</c> operator.</summary>
		LogicalNegation = OverloadableCodeOperators.LogicalNegation,

		/// <summary>The <c>~</c> operator.</summary>
		BitwiseComplement = OverloadableCodeOperators.BitwiseComplement,

		/// <summary>The '<c>true</c>' operator.</summary>
		TrueOperator = OverloadableCodeOperators.TrueOperator,

		/// <summary>The '<c>false</c>' operator.</summary>
		FalseOperator = OverloadableCodeOperators.FalseOperator,

		/// <summary>The <c>*</c> operator.</summary>
		Mul = OverloadableCodeOperators.Mul,
		/// <summary>The <c>/</c> operator.</summary>
		Div = OverloadableCodeOperators.Div,
		/// <summary>The <c>%</c> operator.</summary>
		Mod = OverloadableCodeOperators.Mod,

		/// <summary>The binary <c>+</c> operator.</summary>
		Sum = OverloadableCodeOperators.Sum,

		/// <summary>The binary <c>-</c> operator.</summary>
		Sub = OverloadableCodeOperators.Sub,
		
		/// <summary>The <c>&lt;&lt;</c> operator.</summary>
		LeftShift = OverloadableCodeOperators.LeftShift,
		/// <summary>The <c>&gt;&gt;</c> operator.</summary>
		RightShift = OverloadableCodeOperators.RightShift,
		/// <summary>The <c>&lt;</c> operator.</summary>
		LessThan = OverloadableCodeOperators.LessThan,
		/// <summary>The <c>&lt;=</c> operator.</summary>
		LessThanOrEqual = OverloadableCodeOperators.LessThanOrEqual,
		/// <summary>The <c>&gt;</c> operator.</summary>
		GreaterThan = OverloadableCodeOperators.GreaterThan,
		/// <summary>The <c>&gt;=</c> operator.</summary>
		GreaterThanOrEqual = OverloadableCodeOperators.GreaterThanOrEqual,
		/// <summary>The <c>==</c> operator.</summary>
		Equal = OverloadableCodeOperators.Equal,
		/// <summary>The <c>!=</c> operator.</summary>
		Unequal = OverloadableCodeOperators.Unequal,

		/// <summary>The <c>&amp;</c> operator.</summary>
		And = OverloadableCodeOperators.And,
		/// <summary>The <c>^</c> operator.</summary>
		XOr = OverloadableCodeOperators.XOr,
		/// <summary>The <c>|</c> operator.</summary>
		Or= OverloadableCodeOperators.Or
	}
}