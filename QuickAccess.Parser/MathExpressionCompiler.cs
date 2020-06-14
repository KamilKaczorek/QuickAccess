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
using System.Collections.Generic;
using System.Linq;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The math expression compiler (final engine).
    /// Allows to compile the math expression using specified <see cref="IExpressionParserFactory" />.
    /// </summary>
    /// <seealso cref="IGrammarProductsFactory" />
    /// <seealso cref="ITermDefinitionsRepository" />
    public sealed class MathExpressionCompiler : IGrammarProductsFactory,
        ITermDefinitionsRepository,
        ICompiler
    {
        public static class ExpressionTypeId
        {
            public const string UnaryOperator = "math_un_oper";
            public const string BinaryOperator = "math_bin_oper";
            public const string Function = "math_function";
            public const string Value = "math_value";
            public const string Variable = "math_var";
            public const string Constant = "math_const";
        }

        private readonly List<BinaryOperatorTermDefinition> _binaryOperators = new List<BinaryOperatorTermDefinition>();
        private readonly Dictionary<IReadOnlyList<char>, Dictionary<int, Func<object[], object>>> _functionsByNameByArgCount;
        private readonly IExpressionParser _parser;
        private readonly List<UnaryOperatorTermDefinition> _unaryOperators = new List<UnaryOperatorTermDefinition>();
        private readonly Dictionary<IReadOnlyList<char>, Func<object>> _variables;
        private readonly Dictionary<IReadOnlyList<char>, object> _constants;

        /// <inheritdoc />
        IReadOnlyList<IBinaryOperatorTermDefinition> ITermDefinitionsRepository.BinaryOperators => _binaryOperators;

        /// <inheritdoc />
        IReadOnlyList<IUnaryOperatorTermDefinition> ITermDefinitionsRepository.UnaryOperators => _unaryOperators;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathExpressionCompiler" /> class.
        /// </summary>
        /// <param name="charComparer">The character comparer.</param>
        /// <param name="parserFactory">The parser factory.</param>
        public MathExpressionCompiler(IEqualityComparer<char> charComparer, IExpressionParserFactory parserFactory)
        {
            charComparer ??= CharComparer.CaseSensitive;
            var comparer = new SourceCodeFragmentContentComparer(charComparer);

            _functionsByNameByArgCount = new Dictionary<IReadOnlyList<char>, Dictionary<int, Func<object[], object>>>(comparer);

            _variables = new Dictionary<IReadOnlyList<char>, Func<object>>(comparer);
            _constants = new Dictionary<IReadOnlyList<char>, object>(comparer);

            _parser = parserFactory.Create(this, this, charComparer);
        }


        /// <inheritdoc />
        public IExecutableExpressionNode Compile(ISourceCode source)
        {
            return (IExecutableExpressionNode) _parser.TryParse(source);
        }


        /// <inheritdoc />
        IParsingProduct IGrammarProductsFactory.CreateOperatorNode(
            ISourceCodeFragment operatorTerm,
            IBinaryOperatorTermDefinition @operator,
            IParsingProduct exp1,
            IParsingProduct exp2)
        {
            return GetFunction(operatorTerm, ExpressionTypeId.BinaryOperator, new[] {exp1, exp2});
        }

        /// <inheritdoc />
        IParsingProduct IGrammarProductsFactory.CreateOperatorNode(
            ISourceCodeFragment operatorTerm,
            IUnaryOperatorTermDefinition @operator,
            IParsingProduct exp)
        {
            return GetFunction(operatorTerm, ExpressionTypeId.UnaryOperator, new[] {exp});
        }

        /// <inheritdoc />
        IParsingProduct IGrammarProductsFactory.CreateFunctionInvocationNode(
            ISourceCodeFragment functionName,
            IEnumerable<IParsingProduct> arguments)
        {
            return GetFunction(functionName, ExpressionTypeId.Function, arguments);
        }

        /// <inheritdoc />
        IParsingProduct IGrammarProductsFactory.CreateVariableNode(ISourceCodeFragment variable)
        {
            if (_variables.TryGetValue(variable, out var v))
            {
                return new MathExpressionParameterlessFunctionNode(ExpressionTypeId.Variable, variable, v);
            }

            if (_constants.TryGetValue(variable, out var c))
            {
                return new MathExpressionValueNode(
                    ExpressionTypeId.Value,
                    variable,
                    c,
                    "float");
            }

            return null;

        }

        /// <inheritdoc />
        IParsingProduct IGrammarProductsFactory.ParseValue(ISourceCode src)
        {
            return ParseUnsignedNumber(src);
        }

        /// <summary>
        /// Adds variable definition to the compiler definitions.
        /// </summary>
        /// <typeparam name="T">The type of variable</typeparam>
        /// <param name="name">The variable name.</param>
        /// <param name="func">The function to evaluate variable value.</param>
        public void DefineVariable<T>(string name, Func<T> func)
        {
            var chars = new StringCharacters(name);

            if (_variables.ContainsKey(chars) || _constants.ContainsKey(chars))
            {
                throw new ArgumentException($"Variable or constant of the specified name '{name}' is already defined.", nameof(name));
            }

            _variables[chars] = (() => func.Invoke());
        }

        /// <summary>
        /// Adds variable definition to the compiler definitions.
        /// </summary>
        /// <typeparam name="T">The type of variable</typeparam>
        /// <param name="name">The variable name.</param>
        /// <param name="constValue">The constant value.</param>
        public void DefineConstant<T>(string name, T constValue)
        {
            var chars = new StringCharacters(name);

            if (_variables.ContainsKey(chars) || _constants.ContainsKey(chars))
            {
                throw new ArgumentException($"Variable or constant of the specified name '{name}' is already defined.", nameof(name));
            }

            _constants[chars] = constValue;
        }

        /// <summary>
        /// Adds the function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        /// <param name="paramsCount">The number of function parameters.</param>
        public void DefineFunction(string name, Func<object[], object> function, int paramsCount)
        {
            
            var c = new StringCharacters(name);
            if (!_functionsByNameByArgCount.TryGetValue(c, out var funByArgCount))
            {
                funByArgCount = new Dictionary<int, Func<object[], object>>();
                _functionsByNameByArgCount[c] = funByArgCount;
            }
            else
            {
                if (funByArgCount.ContainsKey(paramsCount))
                {
                    throw new ArgumentException($"Function '{name}'({paramsCount} params) is already defined.", nameof(name));
                }
            }

            funByArgCount[paramsCount] = function;
        }

        /// <summary>
        /// Adds the binary operator definition to the compiler definitions.
        /// </summary>
        /// <typeparam name="T">The type of operator result and arguments.</typeparam>
        /// <param name="name">The operator name.</param>
        /// <param name="function">The function to evaluate operator value.</param>
        /// <param name="priority">The operator priority - higher value means higher priority.</param>
        public void DefineOperator<T>(string name, Func<T, T, T> function, int priority)
        {
            DefineFunction(name, args => function.Invoke((T) args[0], (T) args[1]), 2);

            _binaryOperators.Add(new BinaryOperatorTermDefinition(name, priority));
        }

        /// <summary>
        /// Adds the unary operator definition to the compiler definitions.
        /// </summary>
        /// <typeparam name="T">The type of operator result and argument.</typeparam>
        /// <param name="name">The operator name.</param>
        /// <param name="function">The function to evaluate operator value.</param>
        public void DefineOperator<T>(string name, Func<T, T> function)
        {
            DefineFunction(name, args => function.Invoke((T) args[0]), 1);

            _unaryOperators.Add(new UnaryOperatorTermDefinition(name));
        }

        /// <summary>
        /// Adds the 8 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T, T, T, T, T, T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T)a[0], (T)a[1], (T)a[2], (T)a[3], (T)a[4], (T)a[5], (T)a[6], (T)a[7]), 8);
        }

        /// <summary>
        /// Adds the 7 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T, T, T, T, T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T)a[0], (T)a[1], (T)a[2], (T)a[3], (T)a[4], (T)a[5], (T)a[6]), 7);
        }

        /// <summary>
        /// Adds the 6 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T, T, T, T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T)a[0], (T)a[1], (T)a[2], (T)a[3], (T)a[4], (T)a[5]), 6);
        }

        /// <summary>
        /// Adds the 5 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T, T, T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T)a[0], (T)a[1], (T)a[2], (T)a[3], (T)a[4]), 5);
        }

        /// <summary>
        /// Adds the 4 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T, T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T)a[0], (T)a[1], (T)a[2], (T)a[3]), 4);
        }

        /// <summary>
        /// Adds the 3 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T) a[0], (T) a[1], (T)a[2]), 3);
        }

        /// <summary>
        /// Adds the 2 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T) a[0], (T) a[1]), 2);
        }

        /// <summary>
        /// Adds the single parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T> function)
        {
            DefineFunction(name, a => function.Invoke((T) a[0]), 1);
        }

        /// <summary>
        /// Adds the parameterless function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T> function)
        {
            DefineFunction(name, args => function.Invoke(), 0);
        }

        // UnsignedNumber = ( ({Digit},'.', [{Digit}]) | ({Digit}, ['.']) | '.', {Digit} ), [('E'|'e'), ['+'|'-'], {Digit}]
        private IParsingProduct ParseUnsignedNumber(ISourceCode src)
        {
            var fragment = src.ParseUnsignedNumber(out var value, out var isFloat);

            if (fragment == null)
            {
                return null;
            }

            return new MathExpressionValueNode(
                ExpressionTypeId.Value,
                fragment,
                value,
                isFloat ? @"u_float" : @"uint");
        }

        private IParsingProduct GetFunction(
            ISourceCodeFragment functionName,
            string expressionTypeId,
            IEnumerable<IParsingProduct> arguments)
        {
            if (!_functionsByNameByArgCount.TryGetValue(functionName, out var functionByArgsCount))
            {
                return null;
            }

            var args = arguments?.ToArray() ?? Array.Empty<IParsingProduct>();
            if (!functionByArgsCount.TryGetValue(args.Length, out var func))
            {
                return null;
            }

            return new MathExpressionFunctionNode(ExpressionTypeDescriptor.CreateExpressionClass(expressionTypeId), functionName, args, func);
        }
    }
}