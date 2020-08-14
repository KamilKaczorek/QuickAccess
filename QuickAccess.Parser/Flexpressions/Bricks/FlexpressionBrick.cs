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
using System.Runtime.CompilerServices;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common;
using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.Flexpressions.Model;
using QuickAccess.Parser.Flexpressions.Model.Caching;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions.Bricks
{
    public abstract class FlexpressionBrick
		:  IExpressionParser,
            IRepresentRegularExpression, 
            IDefineAlgebraicDomain<FlexpressionBrick, IFlexpressionAlgebra>, 
            IEquatable<FlexpressionBrick>
    {
        protected internal virtual bool CanCacheParsingResult => true;

        public const string AnonymousNamePrefix = "Anonym";

		protected FlexpressionBrick(IFlexpressionAlgebra algebra)
        {
            Id = FlexpressionId.Generate();

			Algebra = algebra ?? FXB.DefaultAlgebra;
		}

		public FlexpressionId Id { get; }
		public virtual string Name => $"{AnonymousNamePrefix}{Id}";

		/// <inheritdoc />
		public IFlexpressionAlgebra Algebra { get; }

		public FlexpressionBrick this[in int minCount, in int maxCount] =>
			Algebra.CreateQuantifierBrick(this, Quantifier.Create(minCount, maxCount));

		public FlexpressionBrick this[in int count] => Algebra.CreateQuantifierBrick(this, Quantifier.Create(count));

        public FlexpressionBrick this[in CountValue minCount, in CountValue maxCount] =>
            Algebra.CreateQuantifierBrick(this, Quantifier.Create(minCount, maxCount));

        
        
        public FlexpressionBrick this[in CountValue count] => Algebra.CreateQuantifierBrick(this, Quantifier.Create(count));

        public FlexpressionBrick this[in Quantifier quantity] => Algebra.CreateQuantifierBrick(this, quantity);

        /// <inheritdoc />
        public virtual MatchingLevel RegularExpressionMatchingLevel => MatchingLevel.None;

		public virtual bool IsEmpty => false;

        protected abstract void ApplyRuleDefinition(string name, FlexpressionBrick content, bool recursion, bool freeze);

        /// <summary>Tries the parse internal.</summary>
        /// <param name="parsingContext">The source.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected abstract IParsingProduct TryParseInternal(IParsingContextStream parsingContext, ParsingOptions options);

        /// <inheritdoc />
        public abstract bool Equals(FlexpressionBrick other);

        public static FlexpressionBrick operator *(FlexpressionBrick left, FlexpressionBrick right)
		{
			return FXB.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.Mul, left, right);
		}

		public static FlexpressionBrick operator /(FlexpressionBrick left, FlexpressionBrick right)
		{
			return FXB.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.Div, left, right);
		}

		public static FlexpressionBrick operator %(FlexpressionBrick left, FlexpressionBrick right)
		{
			return FXB.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.Mod, left, right);
		}

		public static FlexpressionBrick operator +(FlexpressionBrick left, FlexpressionBrick right)
		{
			return FXB.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.Sum, left, right);
		}

		public static FlexpressionBrick operator &(FlexpressionBrick left, FlexpressionBrick right)
		{
			return FXB.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.And, left, right);
		}

		public static FlexpressionBrick operator ^(FlexpressionBrick left, FlexpressionBrick right)
		{
			return FXB.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.XOr, left, right);
		}

		public static FlexpressionBrick operator |(FlexpressionBrick left, FlexpressionBrick right)
		{
			return FXB.DefaultAlgebra.GetOperatorResultOfHighestPrioritizedAlgebra(OverloadableCodeBinarySymmetricOperator.Or, left, right);
		}

		public static FlexpressionBrick operator ++(FlexpressionBrick arg)
		{
			return FXB.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.Increment, arg);
		}

		public static FlexpressionBrick operator +(FlexpressionBrick arg)
		{
			return FXB.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.Plus, arg);
		}

		public static FlexpressionBrick operator -(FlexpressionBrick arg)
		{
			return FXB.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.Minus, arg);
		}

		public static FlexpressionBrick operator ~(FlexpressionBrick arg)
		{
			return FXB.DefaultAlgebra.EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator.BitwiseComplement, arg);
		}

		public static implicit operator FlexpressionBrick(string x)
		{
			return FXB.ToTextSequence(x);
		}

		public static implicit operator FlexpressionBrick(char x)
		{
			return FXB.ToCharacter(x);
		}

		public void ApplyCustomRule(string name, FlexpressionBrick content)
		{
			ApplyRuleDefinition(name, content, recursion: false, freeze: false);
		}

		public void ApplyCustomSealedRule(string name, FlexpressionBrick content)
		{
			ApplyRuleDefinition(name, content, recursion: false, freeze: true);
		}

		protected static void ApplyRuleDefinition(FlexpressionBrick target,
		                                          string name,
		                                          FlexpressionBrick content,
		                                          bool recursion,
		                                          bool freeze)
		{
			target.ApplyRuleDefinition(name, content, recursion, freeze);
		}

		public virtual string ToRegularExpressionString(RegularExpressionBuildingContext ctx)
		{
			var matchingLevel = RegularExpressionMatchingLevel;
			if (matchingLevel != MatchingLevel.None)
			{
				throw new InvalidOperationException(
					$"{nameof(RegularExpressionMatchingLevel)} returns {matchingLevel} but {nameof(ToRegularExpressionString)} method is not overloaded.");
			}

			throw new NotSupportedException($"Conversion to regular expression is not supported for {GetType()}.");
		}

        /// <inheritdoc />
        public IParsingProduct TryParse(ISourceCode sourceCode, ParsingOptions options)
        {
            var canCache = CanCacheParsingResult;
            var res = canCache && options.HasFlag(ParsingOptions.Cache) 
                ? TryParseCache(sourceCode, options) 
                : TryParseNoCache(sourceCode, options);

            if (res != null && options.HasFlag(ParsingOptions.PrintSteps))
            {
				Console.WriteLine($"{GetType().Name}, {Id}, {Name} Passed {sourceCode}");
            }

            return res;
        }

        
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParsingProduct TryParseCache(ISourceCode sourceCode, ParsingOptions options)
        {
            using var ctx = sourceCode.GetFurtherContext();
            // ReSharper disable once AccessToDisposedClosure
            var res = ctx.GetParsingResultOrUpdate(Id, () => TryParseInternal(ctx, options));

            if (!res.IsSuccessful)
            {
                if (res.Result == ParsingEvaluationResult.CalculationInProgress)
                {
                    throw new InvalidOperationException($"Circular definition found {Name}");
                }
         
				ctx.SetError(new ParsingError(1, $"{GetType().Name} expected at: '{ctx}'; {Name}"));

                return null;
            }

            ctx.Accept();
            var product = (IParsingProduct) res.Product;
            return product;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParsingProduct TryParseNoCache(ISourceCode sourceCode, ParsingOptions options)
        {
            using var ctx = sourceCode.GetFurtherContext();

            // ReSharper disable once AccessToDisposedClosure
            var res = TryParseInternal(ctx, options);

            if (res != null)
            {
                ctx.Accept();
            }

            return res;
        }
    }
}