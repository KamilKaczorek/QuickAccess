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
using System.Threading;
using System.Threading.Tasks;

namespace QuickAccess.Parser
{
    /// <summary>
    /// The expression node that represents the parameterless math function.
    /// </summary>
    /// <seealso cref="ParsedExpressionNode" />
    /// <seealso cref="IExecutableExpressionNode" />
    public sealed class MathExpressionParameterlessFunctionNode : TerminalExpressionNode, IExecutableExpressionNode
    {
        private readonly Func<object> _function;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathExpressionParameterlessFunctionNode"/> class.
        /// </summary>
        /// <param name="expressionTypeId">The expression type identifier.</param>
        /// <param name="functionNameFragment">The function name fragment.</param>
        /// <param name="function">The function to evaluate function value.</param>
        /// <param name="valueTypeId">The value type identifier.</param>
        public MathExpressionParameterlessFunctionNode(
            string expressionTypeId,
            ISourceCodeFragment functionNameFragment,
            Func<object> function,
            string valueTypeId = null)
            : base(expressionTypeId, functionNameFragment, valueTypeId)
        {
            _function = function;            
        }

        /// <inheritdoc />
        public object Execute()
        {
            return _function.Invoke();
        }

        /// <inheritdoc />
        public  Task<object> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_function.Invoke());
        }
    }
}