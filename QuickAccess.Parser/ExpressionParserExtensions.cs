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
using System.Linq;
using QuickAccess.Infrastructure.Collections;
using QuickAccess.Parser.Flexpressions;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser
{
	public static class ExpressionParserExtensions
	{
        internal const ParsingOptions DefaultParsingOptions = ParsingOptions.Cache;

        /// <summary>
        /// Tries to parse the expression specified by the given parsing source with <see cref="DefaultParsingOptions"/> as <see cref="ParsingOptions"/>.
        /// If successful, returns not <c>null</c> expression node.
        /// If compilation is failed sets the error and returns <c>null</c>.
        /// </summary>
        /// <param name="source">The source of extension.</param>
        /// <param name="src">The parsing source.</param>
        /// <returns>The not <c>null</c> expression node if successful; otherwise <c>null</c>.</returns>
        public static IParsingProduct TryParse(this IExpressionParser source, ISourceCode src)
        {
            return source.TryParse(src, DefaultParsingOptions);
        }

		public static IParsingProduct TryAggregateParse(this IEnumerable<IExpressionParser> source, ISourceCode src, ParsingOptions options = DefaultParsingOptions)
		{
			if (source is IReadOnlyCollection<IExpressionParser> collection)
			{
				return TryAggregateParse(collection, src, options);
			}

			List<IParsingProduct> nodes = null;
            using var ctx = src.GetFurtherContext();
            foreach (var parser in source)
            {
                var res = parser.TryParse(src, options);

                if (res == null)
                {
                    return null;
                }

                nodes ??= new List<IParsingProduct>();
                nodes.Add(res);
            }

            ctx.Accept();
            return ctx.CreateExpressionForAcceptedFragment(FXB.ExpressionTypes.Concatenation, nodes);
        }

		public static IParsingProduct TryAggregateParse(this IReadOnlyCollection<IExpressionParser> parsers, ISourceCode src, ParsingOptions options = DefaultParsingOptions)
        {
            using var ctx = src.GetFurtherContext();
            var nodes = new IParsingProduct[parsers.Count];
            var idx = 0;
            foreach (var parser in parsers)
            {
                var res = parser.TryParse(ctx, options);

                if (res == null)
                {
                    return null;
                }

                nodes[idx] = res;
                idx++;
            }

            ctx.Accept();
            return ctx.CreateExpressionForAcceptedFragment(FXB.ExpressionTypes.Concatenation, nodes);
        }

		public static IParsingProduct TryAlternativeParse(
			this IEnumerable<IExpressionParser> parsers,
			ISourceCode src,
			ParsingAlternationType parsingAlternationType = ParsingAlternationType.TakeFirst, 
            ParsingOptions options = DefaultParsingOptions)
		{
			if (parsingAlternationType == ParsingAlternationType.TakeFirst)
			{
				return parsers.Select(p => p.TryParse(src, options)).FirstNotNullOrDefault();
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
				var res = parser.TryParse(src, options);

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