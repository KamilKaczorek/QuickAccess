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
using System.Collections.Generic;

namespace QuickAccess.Parser.SmartExpressions
{
	public abstract class ParsingBrick : IRegularExpressionProvider, IEquatable<ParsingBrick>
	{
		public ParsingBrick this[string pattern] => SX.BrickAlgebra.EvaluatePattern(this, pattern);

		public ParsingBrick this[long min, long max] => SX.BrickAlgebra.CreateQuantifierBrick(this, min, max);

		public ParsingBrick this[long count] => SX.BrickAlgebra.CreateQuantifierBrick(this, count, count);

		public virtual bool IsEmpty => false;
		

		public static ParsingBrick operator &(ParsingBrick left, ParsingBrick right)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(BinaryOperator.And, left, right);
		}

		public static ParsingBrick operator +(ParsingBrick left, ParsingBrick right)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(BinaryOperator.Add, left, right);
		}

		public static ParsingBrick operator -(ParsingBrick left, ParsingBrick right)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(BinaryOperator.Sub, left, right);
		}

		public static ParsingBrick operator *(ParsingBrick left, ParsingBrick right)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(BinaryOperator.Mul, left, right);
		}

		public static ParsingBrick operator |(ParsingBrick left, ParsingBrick right)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(BinaryOperator.Or, left, right);
		}

		public static ParsingBrick operator ^(ParsingBrick left, ParsingBrick right)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(BinaryOperator.XOr, left, right);
		}

		public static ParsingBrick operator %(ParsingBrick left, ParsingBrick right)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(BinaryOperator.Mod, left, right);
		}

		public static ParsingBrick operator ++(ParsingBrick arg)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(UnaryOperator.DoublePlus, arg);
		}

		public static ParsingBrick operator --(ParsingBrick arg)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(UnaryOperator.DoubleMinus, arg);
		}

		public static ParsingBrick operator +(ParsingBrick arg)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(UnaryOperator.SinglePlus, arg);
		}

		public static ParsingBrick operator -(ParsingBrick arg)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(UnaryOperator.SingleMinus, arg);
		}

		public static ParsingBrick operator !(ParsingBrick arg)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(UnaryOperator.LogicalNot, arg);
		}
		
		public static ParsingBrick operator ~(ParsingBrick arg)
		{
			return SX.BrickAlgebra.EvaluateOperatorResult(UnaryOperator.BinaryNot, arg);
		}

		public static ParsingBrick operator /(ParsingBrick left, string tagName)
		{
			return SX.BrickAlgebra.DefineRule(left, tagName);
		}

		public static implicit operator ParsingBrick(string x)
		{
			return SX.ToTextSequence(x);
		}

		public static implicit operator ParsingBrick(char x)
		{
			return SX.ToCharacter(x);
		}


		public void ApplyCustomRule(string name, ParsingBrick content)
		{
			ApplyRuleDefinition(name, content, recursion:false);
		}

		protected abstract void ApplyRuleDefinition(string name, ParsingBrick content, bool recursion);

		protected static void ApplyRuleDefinition(ParsingBrick target,
		                                          string name,
		                                          ParsingBrick content, bool recursion)
		{
			target.ApplyRuleDefinition(name, content, recursion);
		}
		
		public ParsingBrick ZeroOrMore() => this[0, SX.Max];

		public ParsingBrick OneOrMore() => this[1, SX.Max];

		public ParsingBrick Optional() => this[0, 1];

		public ParsingBrick DefinesRule(string patternName)
		{
			return SX.BrickAlgebra.DefineRule(this, patternName);
		}

		public abstract string ExpressionId { get; }

		public virtual string ToRegularExpressionString(Dictionary<string, int> usedGroupNames)  
		{
			if (ProvidesRegularExpression)
			{
				throw new InvalidOperationException($"{nameof(ProvidesRegularExpression)} returns true but {nameof(ToRegularExpressionString)} method is not overloaded.");
			}

			throw new NotSupportedException($"Conversion to regular expression is not supported for {GetType()}.");
		}

		public virtual bool ProvidesRegularExpression => false;

		/// <inheritdoc />
		public abstract bool Equals(ParsingBrick other);
	}
}