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
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The interface of the binary operators chain builder that builds the binary operators expression list from the 
    /// elements provided according to the pattern:  (expression [{operator expression}]).
    /// It gives functionality to convert provided flat operators expression chain to the operators binary tree, 
    /// respecting the operators priority.
    /// </summary>
    public interface IBinaryOperatorsChain
    {
        /// <summary>
        /// Determines whether the chain is valid -> contains at least one expression and last item is an expression.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this chain is valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Gets a value indicating whether the chain is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this chain is empty; otherwise, <c>false</c>.
        /// </value>
        bool IsEmpty { get; }

        /// <summary>
        /// Adds the specified expression to the chain.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when last item is an expression.</exception>
        void Add(IParsingProduct expression);

        /// <summary>
        /// Adds the specified binary operator to the chain.
        /// </summary>
        /// <param name="operatorTermDefinition">The operator term definition.</param>
        /// <param name="codeFragment">The code fragment.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when missing expression node
        /// or
        /// last item is an operator
        /// </exception>
        void Add(IBinaryOperatorTermDefinition operatorTermDefinition, ISourceCodeFragment codeFragment);

        /// <summary>
        /// Builds the expression tree from operators chain respecting the operators priority.
        /// Precondition: the chain list is valid (<see cref="BinaryOperatorsChain.IsValid"/> returns <c>true</c>).
        /// </summary>
        /// <param name="createOperatorNode">The callback to create operator node for specified operator and two arguments (expressions)</param>
        /// <returns>The created expression tree node.</returns>
        IParsingProduct EvaluateExpressionTree(
            Func<ISourceCodeFragment, IBinaryOperatorTermDefinition, IParsingProduct, IParsingProduct,
                IParsingProduct> createOperatorNode);
    }
}