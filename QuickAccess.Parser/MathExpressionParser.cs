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
using System.Collections.Generic;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The math expression parser.
    /// It allows to parse expression defined by the following grammar rules:
    /// Expression = OperatorsChainExp = UnaryExpression, [BinaryOperator, OpChainExp]
    /// UnaryOperatorExpression = UnaryOperator, UnaryExpression
    /// BinaryOperator = ? one of binary operators provided by ITermDefinitionsRepository instance ?
    /// UnaryOperator = ? one of unary operators provided by ITermDefinitionsRepository instance ?
    /// UnaryExpression = ('(', Expression, ')') | UnaryOperatorExpression | Function | Value | Variable
    /// Function  = FunctionName, '(', [ArgList], ')'
    /// FunctionName = Name & ? one of defined functions in IGrammarProductsFactory ?
    /// Variable = Name & ? one of defined variables in IGrammarProductsFactory ?
    /// Value = ? value parsed by the IGrammarProductsFactory ?
    /// Name = Letter, [{Letter|Digit}]
    /// ArgList = [Expression, {separator, Expression}]
    /// </summary>
    /// <seealso cref="IExpressionParser" />
    public sealed class MathExpressionParser : IExpressionParser
    {
        private readonly ITermDefinitionsRepository _repository;
        private readonly IGrammarProductsFactory _grammarProductsFactory;
        private readonly IEqualityComparer<char> _charsComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathExpressionParser"/> class.
        /// </summary>
        /// <param name="termDefinitionsRepository">The term definitions repository.</param>
        /// <param name="grammarProductsFactory">The grammar products factory.</param>
        /// <param name="charsComparer">The chars comparer.</param>
        public MathExpressionParser(
            ITermDefinitionsRepository termDefinitionsRepository,
            IGrammarProductsFactory grammarProductsFactory,
            IEqualityComparer<char> charsComparer)
        {
            _repository = termDefinitionsRepository;
            _grammarProductsFactory = grammarProductsFactory;
            _charsComparer = charsComparer;
        }

        /// <inheritdoc />
        public IParsingProduct TryParse(ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            var res = ParseExpression(ctx);

            if (res == null)
            {
                return null;
            }

            ctx.ParseWhiteSpace();

            if (ctx.HasNext)
            {
                ctx.SetError(ParsingErrors.UnexpectedSymbol);

                return null;
            }

            ctx.Accept();
            return res;
        }

        // Expression = OpChainExpression
        private IParsingProduct ParseExpression(ISourceCode src)
        {
            return ParseOperatorChainExp(src);
        }

        // UnaryExpression = ('(', Expression, ')') | UnaryOperatorExpression | Function | Value | Variable
        private IParsingProduct ParseUnaryExpression(ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            ctx.ParseWhiteSpace();

            var res = ParseInBrackets(ctx, ParseExpression) ??
                      ParseUnaryOperatorExpression(ctx) ??
                      ParseFunction(ctx) ??
                      ParseValue(ctx) ??
                      ParseVariable(ctx);

            if (res != null)
            {
                ctx.ParseWhiteSpace();
                ctx.Accept();
            }

            return res;
        }

        // Value = ? value parsed by the IGrammarProductsFactory ?
        private IParsingProduct ParseValue(ISourceCode src)
        {
            return _grammarProductsFactory.ParseValue(src);
        }

        // UnaryOperatorExpression = UnaryOperator, UnaryExpression
        private IParsingProduct ParseUnaryOperatorExpression(ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            ctx.ParseWhiteSpace();

            var operFragment = ParseTerm(ctx, _repository.UnaryOperators, out var oper);

            if (operFragment == null)
            {
                return null;
            }

            var exp = ParseUnaryExpression(ctx);

            if (exp == null)
            {
                return null;
            }

            var opExp = _grammarProductsFactory.CreateOperatorNode(operFragment, oper, exp);

            if (opExp == null)
            {
                ctx.SetError(ParsingErrors.UnexpectedSymbol);
                return null;
            }
            ctx.Accept();
            return opExp;
        }

        // Variable = Name & ? one of defined variables ?
        private IParsingProduct ParseVariable(ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            var fragment = ParseName(ctx);

            if (fragment == null)
            {
                return null;
            }

            var variableNode = _grammarProductsFactory.CreateVariableNode(fragment);

            if (variableNode == null)
            {
                ctx.SetError(ParsingErrors.UndefinedVariable);    
                return null;
            }

            ctx.Accept();
            return variableNode;
        }

        // Name = Letter, [{Letter|Digit}]
        private ISourceCodeFragment ParseName(ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            if (!char.IsLetter(ctx.Next))
            {
                ctx.SetError(ParsingErrors.NameLiteralExpected);
                return null;
            }

            ctx.ParseWhile(char.IsLetterOrDigit, true);


            return ctx.GetAcceptedFragmentOrEmpty();
        }

        // Function  = Name, '(', [ArgList], ')'
        // FunctionName = ? one of defined functions in IGrammarProductsFactory ?
        private IParsingProduct ParseFunction(ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            var fragment = ParseName(ctx);

            if (fragment == null)
            {
                return null;
            }

            ctx.ParseWhiteSpace();
            var nodes = ParseInBrackets(ctx, s => ParseArgList(s));

            if (nodes == null)
            {
                return null;
            }

            var opExp = _grammarProductsFactory.CreateFunctionInvocationNode(fragment, nodes);

            if (opExp == null)
            {
                ctx.SetError(ParsingErrors.UndefinedFunction);
                return null;
            }
            ctx.Accept();
            return opExp;
        }

        // ArgList = [Expression, {separator, Expression}]
        private IList<IParsingProduct> ParseArgList(ISourceCode src, char separator = ',')
        {
            var arguments = new List<IParsingProduct>();
            using var ctx = src.GetFurtherContext();
            var first = true;
            while (true)
            {
                ctx.ParseWhiteSpace();
                var arg = ParseOperatorChainExp(ctx);

                if (arg == null)
                {
                    if (first)
                    {
                        break;
                    }

                    return null;
                }

                first = false;
                arguments.Add(arg);

                ctx.ParseWhiteSpace();


                if (ctx.HasNext && ctx.Next == separator)
                {
                    ctx.MoveNext();
                }
                else
                {
                    break;
                }
            }

            ctx.Accept();

            return arguments;
        }

        // OpChainExp = UnaryExpression, [BinaryOperator, OpChainExp]
        // BinaryOperator = ? one of operators provided by ITermDefinitionsRepository instance ?
        private IParsingProduct ParseOperatorChainExp(ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            var chain = new BinaryOperatorsChain();

            while (true)
            {
                ctx.ParseWhiteSpace();
                var expression = ParseUnaryExpression(ctx);

                if (expression == null)
                {
                    ctx.SetError(ParsingErrors.ExpressionExpected);
                    return null;
                }

                chain.Add(expression);
                ctx.ParseWhiteSpace();
                var opFragment = ParseTerm(ctx, _repository.BinaryOperators, out var oper);

                if (opFragment == null)
                {
                    break;
                }

                chain.Add(oper, opFragment);
            }

            ctx.Accept();

            return chain.EvaluateExpressionTree(_grammarProductsFactory.CreateOperatorNode);
        }

        // Term = ? one of specified terms ?
        private ISourceCodeFragment ParseTerm<T>(ISourceCode src, IReadOnlyList<T> terms, out T term)
            where T : class, ITermDefinition
        {
            using var ctx = src.GetFurtherContext();
            var idx = ctx.ParseLongestMatchingString(terms.Count, i => terms[i].Term, true, _charsComparer);

            if (idx < 0)
            {
                ctx.SetError(ParsingErrors.UnexpectedSymbol);
                term = null;
                return null;
            }

            term = terms[idx];
            return ctx.GetAcceptedFragmentOrEmpty();
        }

        // InBrackets = '('? the generic body ?')'
        private T ParseInBrackets<T>(
            ISourceCode src,
            Func<ISourceCode, T> bracketsBodyParser,
            char open = '(',
            char close = ')')
            where T : class
        {
            using var ctx = src.GetFurtherContext();
            ctx.ParseWhiteSpace();
            ctx.MoveNext();

            if (ctx.Current != open)
            {
                ctx.Rollback();
                ctx.SetError(ParsingErrors.OpenBracketExpected);
                return null;
            }

            ctx.ParseWhiteSpace();
            var res = bracketsBodyParser.Invoke(ctx);

            if (res != null)
            {
                ctx.ParseWhiteSpace();
                ctx.MoveNext();

                if (ctx.Current != close)
                {
                    ctx.SetError(ParsingErrors.CloseBracketExpected);
                    return null;
                }

                ctx.Accept();
            }

            return res;
        }
    }
}