using System;
using System.Linq;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common.CharMatching;
using QuickAccess.Parser.Flexpressions.Bricks;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions
{
    

	public class StandardFlexpressionAlgebra : IFlexpressionAlgebra
	{
		/// <inheritdoc />
		public Type BaseDomainType => typeof(FlexpressionBrick);

		/// <inheritdoc />
		public int Priority { get; }

		/// <inheritdoc />
		public bool IsDomainSupported(Type domainType)
		{
			return typeof(FlexpressionBrick).IsAssignableFrom(domainType);
		}

		/// <inheritdoc />
		public OverloadableCodeOperators SupportedOperators => (OverloadableCodeOperatorsGroups.Symmetric & OverloadableCodeOperatorsGroups.Binary) 
		                                                         | OverloadableCodeOperators.BitwiseComplement 
		                                                         | OverloadableCodeOperators.LogicalNegation 
		                                                         | OverloadableCodeOperators.Minus;


		/// <inheritdoc />
		public bool IsUnaryOperatorSupported(OverloadableCodeUnarySymmetricOperator unaryOperator)
		{
			return ((int) unaryOperator & (int) SupportedOperators) != 0;
		}

		/// <inheritdoc />
		public bool IsBinaryOperatorSupported(OverloadableCodeBinarySymmetricOperator binaryOperator)
		{
			return ((int) binaryOperator & (int) SupportedOperators) != 0;
		}

		/// <inheritdoc />
		public string GetBinaryOperatorDescription(OverloadableCodeBinarySymmetricOperator binaryOperator)
		{
            return binaryOperator switch
            {
                OverloadableCodeBinarySymmetricOperator.Mul => "Concatenates left and right expression with 'Anything' in between.",
                OverloadableCodeBinarySymmetricOperator.Div => "Concatenates left and right expression with 'next line' in between.",
                OverloadableCodeBinarySymmetricOperator.Mod => "Concatenates left and right expression placing 'custom pattern' in between.",
                OverloadableCodeBinarySymmetricOperator.Sum => "Concatenates left and right expression.",
                OverloadableCodeBinarySymmetricOperator.Sub => "Concatenates left and right expression, where the right one is defined as positive lookahead.",
                OverloadableCodeBinarySymmetricOperator.And => "Concatenates left and right expression with optional, multiple white space in between.",
                OverloadableCodeBinarySymmetricOperator.XOr => "Concatenates left and right expression with (non optional) white space in between.",
                OverloadableCodeBinarySymmetricOperator.Or => "Creates alternation from left and right expression.",
                _ => throw new NotSupportedException($"Operator {binaryOperator} is not supported."),
            };
        }

		/// <inheritdoc />
		public string GetUnaryOperatorDescription(OverloadableCodeUnarySymmetricOperator unaryOperator)
		{
            return unaryOperator switch
            {
                OverloadableCodeUnarySymmetricOperator.BitwiseComplement => "Wraps expression with quantifier 0-1",
                OverloadableCodeUnarySymmetricOperator.LogicalNegation => "Creates negated expression.",
                OverloadableCodeUnarySymmetricOperator.Minus => "Creates positive lookahead from expression.",
                _ => throw new NotSupportedException($"Operator {unaryOperator} is not supported."),
            };
        }

		/// <inheritdoc />
		public bool TryEvaluateOperatorResult(object left,
		                                      OverloadableCodeBinarySymmetricOperator binaryOperator,
		                                      object right,
		                                      out object result)
		{
			if (left is FlexpressionBrick lb && right is FlexpressionBrick rb)
			{
				result = EvaluateOperatorResult(lb, binaryOperator, rb);
				return true;
			}

			result = default;
			return false;
		}

		/// <inheritdoc />
		public bool TryEvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator unaryOperator, object arg, out object result)
		{
			if (arg is FlexpressionBrick sb)
			{
				result = EvaluateOperatorResult(unaryOperator, sb);
				return true;
			}

			result = default;
			return false;
		}

		public FlexpressionBrick Anything => CreateRulePlaceholder(FXB.StandardRuleName.Anything, null);

		/// <inheritdoc />
		public FlexpressionBrick Empty => EmptyParsingBrick.Instance;

		/// <inheritdoc />
		public FlexpressionBegin Start => new FlexpressionBegin(this);
		/// <inheritdoc />
		public FlexpressionBrick WhiteSpace => CreateRulePlaceholder(FXB.StandardRuleName.WhiteSpace, (Start | ' ' | '\t').OneOrMore());
        /// <inheritdoc />
		public FlexpressionBrick WhiteSpaceOrNewLine => CreateRulePlaceholder(FXB.StandardRuleName.WhiteSpaceOrNewLine, (Start | ' ' | '\t' | '\n' | '\r').OneOrMore());

		/// <inheritdoc />
		public FlexpressionBrick OptionalWhiteSpace => CreateRulePlaceholder(FXB.StandardRuleName.OptionalWhiteSpace, ~WhiteSpace);
        /// <inheritdoc />
		public FlexpressionBrick OptionalWhiteSpaceOrNewLine => CreateRulePlaceholder(FXB.StandardRuleName.OptionalWhiteSpaceOrNewLine, ~WhiteSpaceOrNewLine);

		/// <inheritdoc />
		public FlexpressionBrick CustomSequence => CreateRulePlaceholder(FXB.StandardRuleName.CustomSequence, Empty);
		/// <inheritdoc />
		public FlexpressionBrick NewLine => CreateRulePlaceholder(FXB.StandardRuleName.NewLine, OptionalWhiteSpace + (Start | '\n' | '\r').OneOrMore());
		/// <inheritdoc />
		public FlexpressionBrick Letter => CreateRulePlaceholder(FXB.StandardRuleName.Letter, new StandardCharacterRangeBrick(this, StandardCharactersRange.Letter));
		/// <inheritdoc />
		public FlexpressionBrick UpperLetter => CreateRulePlaceholder(FXB.StandardRuleName.UpperLetter, new StandardCharacterRangeBrick(this, StandardCharactersRange.UpperLetter));
		/// <inheritdoc />
		public FlexpressionBrick LowerLetter => CreateRulePlaceholder(FXB.StandardRuleName.LowerLetter, new StandardCharacterRangeBrick(this, StandardCharactersRange.LowerLetter));
		/// <inheritdoc />
		public FlexpressionBrick Symbol => CreateRulePlaceholder(FXB.StandardRuleName.Symbol, null);
		/// <inheritdoc />
		public FlexpressionBrick Digit => CreateRulePlaceholder(FXB.StandardRuleName.Digit, new StandardCharacterRangeBrick(this, StandardCharactersRange.Digit));
		/// <inheritdoc />
		public FlexpressionBrick Current => new CurrentRulePlaceholderBrick(this);
		
		public StandardFlexpressionAlgebra(int priority)
		{
			Priority = priority;
		}

		public FlexpressionBrick DefineRule(FlexpressionBrick parsingBrick, ExpressionTypeDescriptor expressionType, bool blockFromRuleOverwriting)
        {
            return new CapturingGroupBrick(expressionType, this, parsingBrick, blockFromRuleOverwriting);
		}

		/// <inheritdoc />
		public FlexpressionBrick DefineSealedRule(FlexpressionBrick content, ExpressionTypeDescriptor expressionType)
		{
			return DefineRule(content, expressionType, blockFromRuleOverwriting: true);
		}

        /// <inheritdoc />
		public FlexpressionBrick CreateRulePlaceholder(string ruleName, FlexpressionBrick defaultExpression)
		{
			return new RulePlaceholderBrick(this.GetHighestPrioritizedAlgebra(defaultExpression), ruleName, defaultExpression);
		}

        /// <inheritdoc />
        public FlexpressionBrick CreateQuantifierBrick(FlexpressionBrick content, long min, long max)
		{
			EvaluateArguments(ref content);

			if (content == null || content == EmptyParsingBrick.Instance)
			{
				return content;
			}

			if (min == 1 && max == 1)
			{
				return content;
			}

			if (min == 0 && max == 1 && content is QuantifierBrick qb && qb.Min <= 1)
			{
				return new QuantifierBrick(content.Algebra, qb.Content, 0, qb.Max);
			}

			return new QuantifierBrick(content.Algebra, content, min, max);
		}

		/// <inheritdoc />
		public FlexpressionBrick DefineRule(FlexpressionBrick content, ExpressionTypeDescriptor expressionType)
		{
			return DefineRule(content, expressionType, blockFromRuleOverwriting: false);
		}

		public FlexpressionBrick EvaluateOperatorResult(FlexpressionBrick left, OverloadableCodeBinarySymmetricOperator binaryOperator, FlexpressionBrick right)
		{
			EvaluateArguments(ref left, ref right);

            return binaryOperator switch
            {
                OverloadableCodeBinarySymmetricOperator.Mul => Concatenate(left, FXB.Anything, right),
                OverloadableCodeBinarySymmetricOperator.Div => Concatenate(left, FXB.NewLine, right),
                OverloadableCodeBinarySymmetricOperator.Mod => Concatenate(left, FXB.CustomSequence, right),
                OverloadableCodeBinarySymmetricOperator.Sum => Concatenate(left, right),
                OverloadableCodeBinarySymmetricOperator.And => Concatenate(left, FXB.OptionalWhiteSpace, right),
                OverloadableCodeBinarySymmetricOperator.XOr => Concatenate(left, FXB.WhiteSpace, right),
                OverloadableCodeBinarySymmetricOperator.Or => CreateAlternation(left, right),
                _ => throw new NotSupportedException($"{nameof(FlexpressionBrick)} doesn't support binary operator '{binaryOperator.GetSymbol()}' ({binaryOperator})."),
            };
        }

        private const long MaxQuantification = int.MaxValue;
		
		public FlexpressionBrick EvaluateOperatorResult(OverloadableCodeUnarySymmetricOperator unaryOperator, FlexpressionBrick arg)
		{
			EvaluateArguments(ref arg);

            return unaryOperator switch
            {
                OverloadableCodeUnarySymmetricOperator.BitwiseComplement => CreateQuantifierBrick(arg, 0, 1),
                OverloadableCodeUnarySymmetricOperator.Increment => CreateQuantifierBrick(arg, 1, MaxQuantification),

                _ => throw new NotSupportedException($"{nameof(FlexpressionBrick)} doesn't support unary operator '{unaryOperator.GetSymbol()}' ({unaryOperator})."),
            };
        }

		private FlexpressionBrick Concatenate(FlexpressionBrick left, FlexpressionBrick right)
		{
			if (left.IsEmpty)
			{
				return right;
			}

			if (right.IsEmpty)
			{
				return left;
			}

			return new ConcatenationBrick(this, left, right);
		}

		private FlexpressionBrick Concatenate(FlexpressionBrick left, FlexpressionBrick middle, FlexpressionBrick right)
		{
			EvaluateArguments(ref middle);

			if (left.IsEmpty)
			{
				return Concatenate(middle, right);
			}

			if (middle.IsEmpty)
			{
				return Concatenate(left, right);
			}

			if (right.IsEmpty)
			{
				return Concatenate(left, middle);
			}

			return new ConcatenationBrick(this, left, middle, right);
		}

		private FlexpressionBrick CreateAlternation(FlexpressionBrick left, FlexpressionBrick right)
		{
			if (left.Equals(right))
			{
				return left;
			}

			if (left is AlternationBrick lc && right is AlternationBrick rc)
			{
				return new AlternationBrick(this, lc.Items.Concat(rc.Items.Where(i => !lc.Items.Contains(i))).ToArray());
			}

			if (left is AlternationBrick l && l.Items.Any(i => i.Equals(right)))
			{
				return left;
			}

			if (right is AlternationBrick r && r.Items.Any(i => i.Equals(left)))
			{
				return right;
			}

			return new AlternationBrick(this, left, right);
		}

		private void EvaluateArguments(ref FlexpressionBrick arg1)
		{
			if (arg1 is CapturingGroupBrick cb)
			{
				arg1 = cb.Clone(this, cb.GroupName);
			}
		}

		private void EvaluateArguments(ref FlexpressionBrick arg1, ref FlexpressionBrick arg2)
		{
			EvaluateArguments(ref arg1);
			EvaluateArguments(ref arg2);
		}
	}
}
