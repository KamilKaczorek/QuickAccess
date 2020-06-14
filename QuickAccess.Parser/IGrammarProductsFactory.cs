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

using System.Collections.Generic;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The interface of the grammar products factory.
    /// Defines the contract to provide definitions of functions, operators, variables and values to the parser.
    /// The instance is used by the parser to create final expression nodes.
    /// <seealso cref="ITermDefinitionsRepository"/>
    /// </summary>
    public interface IGrammarProductsFactory
    {
        /// <summary>
        /// Creates the binary operator node.
        /// </summary>
        /// <param name="operatorTerm">The binary operator term.</param>
        /// <param name="operator">The operator term definition.</param>
        /// <param name="exp1">The left expression.</param>
        /// <param name="exp2">The right expression.</param>
        /// <returns>The binary operator expression or <c>null</c> if operator not found.</returns>
        IParsingProduct CreateOperatorNode(
            ISourceCodeFragment operatorTerm,
            IBinaryOperatorTermDefinition @operator,
            IParsingProduct exp1,
            IParsingProduct exp2);

        /// <summary>
        /// Creates the unary operator node.
        /// </summary>
        /// <param name="operatorTerm">The unary operator term.</param>
        /// <param name="operator">The unary operator definition.</param>
        /// <param name="exp">The argument.</param>
        /// <returns>The unary operator expression or <c>null</c> if operator not found.</returns>
        IParsingProduct CreateOperatorNode(
            ISourceCodeFragment operatorTerm,
            IUnaryOperatorTermDefinition @operator,
            IParsingProduct exp);

        /// <summary>
        /// Creates the function invocation node.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="arguments">The function arguments.</param>
        /// <returns>The function expression or <c>null</c> if function not found.</returns>
        IParsingProduct CreateFunctionInvocationNode(
            ISourceCodeFragment functionName,
            IEnumerable<IParsingProduct> arguments);

        /// <summary>
        /// Creates the variable node.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns>The variable node or <c>null</c> if variable not defined.</returns>
        IParsingProduct CreateVariableNode(
            ISourceCodeFragment variable);

        /// <summary>
        /// Parses the value and returns the value node.
        /// This contract abstracts the value types from the parser.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns>The value node or <c>null</c> if value not parsed.</returns>
        IParsingProduct ParseValue(ISourceCode src);
    }
}