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

namespace QuickAccess.Parser
{
    /// <summary>
    /// The interface of the parsed expression node (the grammar production).
    /// </summary>
    public interface IParsedExpressionNode
    {
        /// <summary>
        /// Gets the identifier of the type of an expression.
        /// </summary>
        /// <value>
        /// The expression type identifier.
        /// </value>
        string ExpressionTypeId { get; }

        /// <summary>
        /// Gets the value type identifier.
        /// </summary>
        /// <value>
        /// The value type identifier.
        /// </value>
        string ValueTypeId { get; }

        /// <summary>
        /// Gets the fragment of the source code that defines this expression.
        /// </summary>
        /// <value>
        /// The source code fragment.
        /// </value>
        ISourceCodeFragment Fragment { get; }

        /// <summary>
        /// Gets the sub nodes (parameters)
        /// </summary>
        /// <value>
        /// The sub nodes.
        /// </value>
        IReadOnlyList<IParsedExpressionNode> SubNodes { get; }
    }
}