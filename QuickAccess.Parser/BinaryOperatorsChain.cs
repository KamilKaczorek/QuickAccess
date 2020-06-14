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
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
	/// <summary>
	///     The implementation of the binary operators chain builder that builds the binary operators expression list from the
	///     elements provided according to the pattern:  (expression [{operator expression}]).
	///     It gives functionality to convert provided flat operators expression sequence to the operators binary tree,
	///     respecting the operators precedence.
	/// </summary>
	public sealed class BinaryOperatorsChain : IBinaryOperatorsChain
	{
		private ExpressionListNode _first;
		private ListNode _last;

		/// <inheritdoc />
		public bool IsValid => !IsEmpty && _last.IsExpression;

		/// <inheritdoc />
		public bool IsEmpty => _first == null;

		/// <inheritdoc />
		public void Add(IParsingProduct expression)
		{
			if (IsEmpty)
			{
				_first = new ExpressionListNode(expression);
				_last = _first;
				return;
			}

			_last = _last.AddNext(expression);
		}

		/// <inheritdoc />
		public void Add(IBinaryOperatorTermDefinition operatorTermDefinition, ISourceCodeFragment codeFragment)
		{
			if (IsEmpty)
			{
				throw new InvalidOperationException(
					"No expression => can't chain operator, first item must be an expression.");
			}

			_last = _last.AddNext(operatorTermDefinition, codeFragment);
		}

		/// <inheritdoc />
		public IParsingProduct EvaluateExpressionTree(
			Func<ISourceCodeFragment, IBinaryOperatorTermDefinition, IParsingProduct, IParsingProduct,
				IParsingProduct> createOperatorNode)
		{
			if (IsEmpty)
			{
				throw new InvalidOperationException(
					"Can't evaluate expression tree of operators chain - the chain is empty.");
			}

			if (!IsValid)
			{
				throw new InvalidOperationException(
					"Can't evaluate expression tree of operators chain - the chain is not completed - chain must end with an expression.");
			}

			return _first.EvaluateExpressionTree(createOperatorNode);
		}

		// Abstract method access is faster than interface method access (ratio 2:3).
		private abstract class ListNode
		{
			public abstract bool IsExpression { get; }

			public abstract ListNode AddNext(IParsingProduct expression);

			public abstract ListNode AddNext(
				IBinaryOperatorTermDefinition definition,
				ISourceCodeFragment codeFragment);
		}

		private sealed class ExpressionListNode : ListNode
		{
			private readonly IParsingProduct _expression;

			public OperatorListNode Next { get; private set; }
			public override bool IsExpression => true;

			private bool IsLast => Next == null;
			private ExpressionListNode NextExpressionNode => Next?.Next;
			private IParsingProduct NextExpression => NextExpressionNode?._expression;

			public ExpressionListNode(IParsingProduct expression, OperatorListNode next = null)
			{
				_expression = expression ?? throw new ArgumentNullException(nameof(expression));
				Next = next;
			}

			public override ListNode AddNext(IParsingProduct expression)
			{
				throw new InvalidOperationException(
					"Last item is an expression => can't chain expression to expression, operator expected.");
			}

			public override ListNode AddNext(IBinaryOperatorTermDefinition definition, ISourceCodeFragment codeFragment)
			{
				Next = new OperatorListNode(definition, codeFragment);
				return Next;
			}

			public IParsingProduct EvaluateExpressionTree(
				Func<
					ISourceCodeFragment,
					IBinaryOperatorTermDefinition,
					IParsingProduct,
					IParsingProduct, IParsingProduct> createOperatorNode)
			{
				if (IsLast)
				{
					//e.g.: 1
					return _expression;
				}

				var oper = Next;

				if (NextExpressionNode.IsLast)
				{
					//e.g.: 1 + 2
					return oper.CreateOperatorTreeNode(_expression, NextExpression, createOperatorNode);
				}

				if (oper.HasLowerPrecedenceThanNext)
				{
					//e.g.: 1 + 2 * 3... => 1 + (2*3...) => node('+', 1, recurrence(2*3...))
					return oper.CreateOperatorTreeNode(
						_expression,
						NextExpressionNode.EvaluateExpressionTree(createOperatorNode),
						createOperatorNode);
				}

				//e.g.: 1 * 2 + 3... => (1*2) + 3... => recurrence(node('*', 1, 2), 3...)
				var head = new ExpressionListNode(
					oper.CreateOperatorTreeNode(_expression, NextExpression, createOperatorNode),
					NextExpressionNode.Next);
				return head.EvaluateExpressionTree(createOperatorNode);
			}
		}

		private sealed class OperatorListNode : ListNode
		{
			private readonly ISourceCodeFragment _codeFragment;
			private readonly IBinaryOperatorTermDefinition _definition;

			private int NextOperatorPrecedence => Next?.Next?.Precedence ??
			                                      throw new NullReferenceException(
				                                      "Can't access next operator priority, the current operator is the last one.");

			private int Precedence => _definition.Precedence;

			public ExpressionListNode Next { get; private set; }

			public override bool IsExpression => false;

			public bool HasLowerPrecedenceThanNext => Precedence < NextOperatorPrecedence;

			public OperatorListNode(IBinaryOperatorTermDefinition definition, ISourceCodeFragment codeFragment)
			{
				_definition = definition ?? throw new ArgumentNullException(nameof(definition));
				_codeFragment = codeFragment ?? throw new ArgumentNullException(nameof(codeFragment));
			}

			public override ListNode AddNext(IParsingProduct expression)
			{
				Next = new ExpressionListNode(expression);
				return Next;
			}

			public override ListNode AddNext(IBinaryOperatorTermDefinition definition, ISourceCodeFragment codeFragment)
			{
				throw new InvalidOperationException(
					"Last item is an operator => can't chain operator to operator, expression expected.");
			}

			public IParsingProduct CreateOperatorTreeNode(
				IParsingProduct exp1,
				IParsingProduct exp2,
				Func<
					ISourceCodeFragment,
					IBinaryOperatorTermDefinition,
					IParsingProduct,
					IParsingProduct, IParsingProduct> createOperatorNode
			)
			{
				return createOperatorNode(_codeFragment, _definition, exp1, exp2);
			}
		}
	}
}