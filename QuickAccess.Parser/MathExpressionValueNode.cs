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

using System.Threading;
using System.Threading.Tasks;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
	/// <summary>
    /// The math expression value node.
    /// </summary>
    /// <seealso cref="ParsingProduct" />
    /// <seealso cref="IExecutableExpressionNode" />
    public sealed class MathExpressionValueNode : TerminalExpressionNode,
        IExecutableExpressionNode
    {
        private readonly object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathExpressionValueNode"/> class.
        /// </summary>
        /// <param name="expressionTypeId">The expression type identifier.</param>
        /// <param name="valueFragment">The source code fragment that represents the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="valueTypeId">The value type identifier.</param>
        public MathExpressionValueNode(string expressionTypeId, ISourceCodeFragment valueFragment, object value, string valueTypeId = null)
            : base(ExpressionTypeDescriptor.CreateExpressionClass(expressionTypeId, valueTypeId), valueFragment)
        {
            _value = value;
        }

        /// <inheritdoc />
        public object Execute()
        {
            return _value;
        }

        /// <inheritdoc />
        public Task<object> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_value);
        }
    }
}