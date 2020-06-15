#region LICENSE [BSD-2-Clause]
// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2019 by Kamil Piotr Kaczorek
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
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.Parser.Flexpressions;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
	public static class ExpressionParserExtensions
	{
		public static IParsingProduct TryAggregateParse(this IEnumerable<IExpressionParser> source, ISourceCode src)
		{
			if (source is IReadOnlyCollection<IExpressionParser> collection)
			{
				return TryAggregateParse(collection, src);
			}

			List<IParsingProduct> nodes = null;
            using var ctx = src.GetFurtherContext();
            foreach (var parser in source)
            {
                var res = parser.TryParse(src);

                if (res == null)
                {
                    return null;
                }

                nodes ??= new List<IParsingProduct>();
                nodes.Add(res);
            }

            ctx.Accept();
            return ctx.CreateExpressionForAcceptedFragment(FX.ExpressionTypes.Concatenation, nodes);
        }

		public static IParsingProduct TryAggregateParse(this IReadOnlyCollection<IExpressionParser> parsers, ISourceCode src)
        {
            using var ctx = src.GetFurtherContext();
            var nodes = new IParsingProduct[parsers.Count];
            var idx = 0;
            foreach (var parser in parsers)
            {
                var res = parser.TryParse(ctx);

                if (res == null)
                {
                    return null;
                }

                nodes[idx] = res;
                idx++;
            }

            ctx.Accept();
            return ctx.CreateExpressionForAcceptedFragment(FX.ExpressionTypes.Concatenation, nodes);
        }

		public static IParsingProduct TryAlternativeParse(
			this IEnumerable<IExpressionParser> parsers,
			ISourceCode src,
			ParsingAlternationType parsingAlternationType = ParsingAlternationType.TakeFirst)
		{
			if (parsingAlternationType == ParsingAlternationType.TakeFirst)
			{
				return parsers.Select(p => p.TryParse(src)).FirstNotNullOrDefault();
			}

			if (parsingAlternationType != ParsingAlternationType.TakeLongest &&
			    parsingAlternationType != ParsingAlternationType.TakeShortest)
			{
				throw new NotSupportedException($"{parsingAlternationType} is not supported.");
			}

			var shortest = parsingAlternationType == ParsingAlternationType.TakeShortest;
			var replaceCmp = shortest ? -1 : 1;

			IParsingContextStream selectedCtx = null;
			IParsingProduct result = null;
		    
			foreach (var parser in parsers)
			{
				var ctx = src.GetFurtherContext();
				var res = parser.TryParse(src);

				if (res != null && (result == null || replaceCmp == ctx.CompareTo(selectedCtx)))
				{
					selectedCtx?.Dispose();

					selectedCtx = ctx;
					result = res;
				}					    
			}

			selectedCtx?.Accept();
			return result;
		}
	}
}