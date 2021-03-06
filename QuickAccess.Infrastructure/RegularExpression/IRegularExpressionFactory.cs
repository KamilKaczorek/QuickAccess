﻿#region LICENSE [BSD-2-Clause]
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
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using QuickAccess.Infrastructure.CharMatching.Categories;

namespace QuickAccess.Infrastructure.RegularExpression
{
    public interface IRegularExpressionFactory
    {

        [Pure]
        string CreateAlternation(IRegularExpressionFactoryContext ctx, IEnumerable<string> alternativeRegexs);

        [Pure]
        string CreateNamedGroup(IRegularExpressionFactoryContext ctx, string groupName, string groupContentRegex,
            out string factualGroupName);

        [Pure]
        string CreateQuantifier(IRegularExpressionFactoryContext ctx, Quantifier quantity, string quantifiedContentRegex);

        [Pure]
        string CreateNonCapturingGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex);

        [Pure]
        string CreatePositiveLookaheadGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex);

        [Pure]
        string CreateNegativeLookaheadGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex);

        [Pure]
        string CreateRecursiveGroupCall(IRegularExpressionFactoryContext ctx, string regexGroupName);

        [Pure]
        string CreateCapturingGroup(IRegularExpressionFactoryContext ctx, string groupContentRegex);

        [Pure]
        string CreateCharRange(IRegularExpressionFactoryContext ctx, StandardCharacterCategories range);

        [Pure]
        string CreateCharSet(IRegularExpressionFactoryContext ctx, IEnumerable<char> characters);

        [Pure]
        string CreateCharRange(IRegularExpressionFactoryContext ctx, char firstCharacter, char lastCharacter);

        [Pure]
        string GetWordCharacter(IRegularExpressionFactoryContext ctx);

        [Pure]
        string GetDigitCharacter(IRegularExpressionFactoryContext ctx);

        [Pure]
        string CharToRegex(char ch);

        [Pure]
        string StringToRegex(string text);

        [Pure]
        string CreateNot(IRegularExpressionFactoryContext ctx, string negatedContentRegex);
    }
}