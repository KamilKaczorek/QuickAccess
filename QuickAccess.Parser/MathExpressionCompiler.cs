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
using QuickAccess.DataStructures.MultiDictionaries;

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
        private readonly List<BinaryOperatorTermDefinition> _binaryOperators = new List<BinaryOperatorTermDefinition>();
        private readonly Dictionary<string, Dictionary<int, Func<object[], object>>> _functionsByNameByArgCount;
        private readonly IExpressionParser _parser;
        private readonly List<UnaryOperatorTermDefinition> _unaryOperators = new List<UnaryOperatorTermDefinition>();
        private readonly Dictionary<string, Func<object>> _variables;

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
            charComparer = charComparer ?? CharComparer.CaseSensitive;
            var comparer = charComparer.ToStringComparer();

            _functionsByNameByArgCount = new Dictionary<string, Dictionary<int, Func<object[], object>>>(comparer);

            _variables = new Dictionary<string, Func<object>>(comparer);

            _parser = parserFactory.Create(this, this, charComparer);
        }


        /// <inheritdoc />
        public IExecutableExpressionNode Compile(ISourceCode source)
        {
            return (IExecutableExpressionNode) _parser.TryParse(source);
        }


        /// <inheritdoc />
        IParsedExpressionNode IGrammarProductsFactory.CreateOperatorNode(
            ISourceCodeFragment operatorTerm,
            IBinaryOperatorTermDefinition oper,
            IParsedExpressionNode exp1,
            IParsedExpressionNode exp2)
        {
            return GetFunction(operatorTerm, "biranyoperator", new[] {exp1, exp2});
        }

        /// <inheritdoc />
        IParsedExpressionNode IGrammarProductsFactory.CreateOperatorNode(
            ISourceCodeFragment operatorTerm,
            IUnaryOperatorTermDefinition oper,
            IParsedExpressionNode exp)
        {
            return GetFunction(operatorTerm, "unaryoperator", new[] {exp});
        }

        /// <inheritdoc />
        IParsedExpressionNode IGrammarProductsFactory.CreateFunctionInvocationNode(
            ISourceCodeFragment functionName,
            IEnumerable<IParsedExpressionNode> arguments)
        {
            return GetFunction(functionName, "function", arguments);
        }

        /// <inheritdoc />
        IParsedExpressionNode IGrammarProductsFactory.CreateVariableNode(ISourceCodeFragment variable)
        {
            if (!_variables.TryGetValue(variable.ToString(), out var v))
            {
                return null;
            }

            return new MathExpressionParameterlessFunctionNode("variable", variable, v);
        }

        /// <inheritdoc />
        IParsedExpressionNode IGrammarProductsFactory.ParseValue(ISourceCode src)
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
            _variables.Add(name, () => func.Invoke());
        }

        /// <summary>
        /// Adds the function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        /// <param name="paramsCount">The number of function parameters.</param>
        public void DefineFunction(string name, Func<object[], object> function, int paramsCount)
        {
            if (!_functionsByNameByArgCount.TryGetValue(name, out var funByArgCount))
            {
                funByArgCount = new Dictionary<int, Func<object[], object>>();
                _functionsByNameByArgCount[name] = funByArgCount;
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
        /// Adds the 2 parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T, T> function)
        {
            DefineFunction(name, args => function.Invoke((T) args[0], (T) args[1]), 2);
        }

        /// <summary>
        /// Adds the single parameter function definition to the compiler definitions.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="function">The function delegate that evaluates the function value.</param>
        public void DefineFunction<T>(string name, Func<T, T> function)
        {
            DefineFunction(name, args => function.Invoke((T) args[0]), 1);
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
        private IParsedExpressionNode ParseUnsignedNumber(ISourceCode src)
        {
            var fragment = src.ParseUnsignedNumber(out var value, out var isFloat);

            if (fragment == null)
            {
                return null;
            }

            return new MathExpressionValueNode(
                "value",
                fragment,
                value,
                isFloat ? "ufloat" : "uint");
        }

        private IParsedExpressionNode GetFunction(
            ISourceCodeFragment functionName,
            string expressionTypeId,
            IEnumerable<IParsedExpressionNode> arguments)
        {
            if (!_functionsByNameByArgCount.TryGetValue(functionName.ToString(), out var functionByArgsCount))
            {
                return null;
            }

            var args = arguments?.ToArray() ?? Array.Empty<IParsedExpressionNode>();
            if (!functionByArgsCount.TryGetValue(args.Length, out var func))
            {
                return null;
            }

            return new MathExpressionFunctionNode(expressionTypeId, functionName, args, func);
        }
    }


    public sealed class ValuesCompiler : IValueCompiler,
        IValueParser
    {
        /// <inheritdoc />
        public ICompiledValue Compile(ParsedValue parsedValue)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ICompiledValue Compile(ParsedValue parsedValue, string valueTypeId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ParsedValue TryParse(ISourceCode src)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ParsedValue TryParse(ISourceCode src, string valueTypeId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ParsedValue TryParse(ISourceCode src, IEnumerable<string> valueTypesIds)
        {
            throw new NotImplementedException();
        }
    }

    public interface IValueCompiler
    {
        ICompiledValue Compile(ParsedValue parsedValue);
        ICompiledValue Compile(ParsedValue parsedValue, string valueTypeId);
    }

    public interface IValueCompilerDefinition
    {
        IEnumerable<string> SupportedTypesIds { get; }
        ICompiledValue Compile(ParsedValue parsedValue);
    }

    public interface IValueConverterDefinition
    {
        IEnumerable<string> PossibleInputTypeIds { get; }
        IEnumerable<string> PossibleOutputTypeIds { get; }
        bool IsImplicit { get; }
        ICompiledValue Convert(ICompiledValue value, string destTypeId);
    }

   

    public sealed class ValueConvertersRepository
    {
        //Double dictionary construction gives the fastest read-access when compared to other constructions, like
        // single dictionary with keys defined as tuples or structures (.net 4.7).
        private readonly Dictionary<string, Dictionary<string, IValueConverterDefinition>> _converters =
            new Dictionary<string, Dictionary<string, IValueConverterDefinition>>();

        private readonly Dictionary<string, int> _typeIndexByTypeId = new Dictionary<string, int>();

        public void Add(IValueConverterDefinition converter)
        {
            foreach (var inputTypeId in converter.PossibleInputTypeIds)
            {
                foreach (var outputTypeId in converter.PossibleOutputTypeIds)
                {
                    _converters.SetInnerValue(inputTypeId, outputTypeId, converter, null);
                }
            }
        }

        public bool ContainsConverter(string sourceTypeId, string destinationTypeId, bool implicitOnly = false)
        {
            if (!implicitOnly)
            {
                return _converters.ContainsOuterInnerKeyPair(sourceTypeId, destinationTypeId);
            }

            if (!_converters.TryGetInnerValue(sourceTypeId, destinationTypeId, out var converter))
            {
                return false;
            }

            return converter.IsImplicit;
        }

        public ICompiledValue Convert(ICompiledValue input, string destinationTypeId, bool implicitOnly = false)
        {
            if (!_converters.TryGetInnerValue(input.ValueTypeId, destinationTypeId, out var converter))
            {
                return null;
            }

            if (implicitOnly && !converter.IsImplicit)
            {
                return null;
            }

            return converter.Convert(input, destinationTypeId);
        }

      
    }

    public sealed class CompiledValue : ICompiledValue
    {
        /// <inheritdoc />
        public string ValueTypeId { get; }

        /// <inheritdoc />
        public object Value { get; }

        public CompiledValue(string valueTypeId, object value)
        {
            ValueTypeId = valueTypeId;
            Value = value;
        }
    }

    public interface ICompiledValue
    {
        string ValueTypeId { get; }
        object Value { get; }
    }

    public interface IValueParser
    {
        ParsedValue TryParse(ISourceCode src);
        ParsedValue TryParse(ISourceCode src, string valueTypeId);
        ParsedValue TryParse(ISourceCode src, IEnumerable<string> valueTypesIds);
    }

    public sealed class ParsedValue
    {
        public string ValueTypeId { get; }

        public ISourceCodeFragment ValueFragment { get; }

        public static ParsedValue Create(ISourceCodeFragment valueFragment, string valueTypeId)
        {
            if (valueFragment == null)
            {
                return null;
            }

            return new ParsedValue(valueFragment, valueTypeId);
        }

        private ParsedValue(ISourceCodeFragment valueFragment, string valueTypeId)
        {
            ValueFragment = valueFragment ?? throw new ArgumentNullException(nameof(valueFragment));
            ValueTypeId = valueTypeId ?? throw new ArgumentNullException(nameof(valueTypeId));
        }
    }

    public interface IValueTypeDefinition
    {
        IEnumerable<string> SupportedTypesIds { get; }
        bool Supports(string typeId);
        ParsedValue TryParse(ISourceCode src);
    }
}