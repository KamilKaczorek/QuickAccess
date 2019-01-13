using System;
using System.Linq;
using QuickAccess.DataStructures.CodeOperatorAlgebra;
using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.SmartExpressions.Bricks;

namespace QuickAccess.Parser.SmartExpressions
{
	public class StandardSmartExpressionAlgebra : ISmartExpressionAlgebra
	{
		/// <inheritdoc />
		public Type BaseDomainType => typeof(SmartExpressionBrick);

		/// <inheritdoc />
		public int Priority { get; }

		/// <inheritdoc />
		public bool IsDomainSupported(Type domainType)
		{
			return typeof(SmartExpressionBrick).IsAssignableFrom(domainType);
		}

		/// <inheritdoc />
		public OverloadableCodeOperators SupportedOperators => (OverloadableCodeOperatorsGroups.Symmetric & OverloadableCodeOperatorsGroups.Binary) 
		                                                         | OverloadableCodeOperators.BitwiseComplement 
		                                                         | OverloadableCodeOperators.LogicalNegation 
		                                                         | OverloadableCodeOperators.Minus;

		/// <inheritdoc />
		public bool IsUnaryOperatorSupported(OverloadableCodeSymmetricUnaryOperator unaryOperator)
		{
			return ((int) unaryOperator & (int) SupportedOperators) != 0;
		}

		/// <inheritdoc />
		public bool IsBinaryOperatorSupported(OverloadableCodeSymmetricBinaryOperator binaryOperator)
		{
			return ((int) binaryOperator & (int) SupportedOperators) != 0;
		}

		/// <inheritdoc />
		public string GetBinaryOperatorDescription(OverloadableCodeSymmetricBinaryOperator binaryOperator)
		{
			switch (binaryOperator)
			{
				case OverloadableCodeSymmetricBinaryOperator.Mul:
					return "Concatenates left and right expression with 'Anything' in between.";
				case OverloadableCodeSymmetricBinaryOperator.Div:
					return "Concatenates left and right expression with 'next line' in between.";
				case OverloadableCodeSymmetricBinaryOperator.Mod:
					return "Concatenates left and right expression placing 'custom pattern' in between.";
				case OverloadableCodeSymmetricBinaryOperator.Sum:
					return "Concatenates left and right expression.";
				case OverloadableCodeSymmetricBinaryOperator.Sub:
					return "Concatenates left and right expression, where the right one is defined as positive lookahead.";
				case OverloadableCodeSymmetricBinaryOperator.And:
					return "Concatenates left and right expression with optional, multiple white space in between.";
				case OverloadableCodeSymmetricBinaryOperator.XOr:
					return "Concatenates left and right expression with (non optional) white space in between.";
				case OverloadableCodeSymmetricBinaryOperator.Or:
					return "Creates alternation from left and right expression.";
				default:
					throw new NotSupportedException($"Operator {binaryOperator} is not supported.");
			}
		}

		/// <inheritdoc />
		public string GetUnaryOperatorDescription(OverloadableCodeSymmetricUnaryOperator unaryOperator)
		{
			switch (unaryOperator)
			{
				case OverloadableCodeSymmetricUnaryOperator.BitwiseComplement:
					return "Wraps expression with quantifier 0-1";
				case OverloadableCodeSymmetricUnaryOperator.LogicalNegation:
					return "Creates negated expression.";
				case OverloadableCodeSymmetricUnaryOperator.Minus:
					return "Creates positive lookahead from expression.";
				default:
					throw new NotSupportedException($"Operator {unaryOperator} is not supported.");
			}
		}

		/// <inheritdoc />
		public bool TryEvaluateOperatorResult(object left,
		                                      OverloadableCodeSymmetricBinaryOperator binaryOperator,
		                                      object right,
		                                      out object result)
		{
			if (left is SmartExpressionBrick lb && right is SmartExpressionBrick rb)
			{
				result = EvaluateOperatorResult(lb, binaryOperator, rb);
				return true;
			}

			result = default;
			return false;
		}

		/// <inheritdoc />
		public bool TryEvaluateOperatorResult(OverloadableCodeSymmetricUnaryOperator unaryOperator, object arg, out object result)
		{
			if (arg is SmartExpressionBrick sb)
			{
				result = EvaluateOperatorResult(unaryOperator, sb);
				return true;
			}

			result = default;
			return false;
		}

		public SmartExpressionBrick Anything => CreateRulePlaceholder(StandardSmartExpressionRuleNames.Anything, null);

		/// <inheritdoc />
		public SmartExpressionBrick Empty => EmptyParsingBrick.Instance;

		/// <inheritdoc />
		public SmartExpressionBegin Start => new SmartExpressionBegin(this);
		/// <inheritdoc />
		public SmartExpressionBrick WhiteSpace => CreateRulePlaceholder(StandardSmartExpressionRuleNames.WhiteSpace, (Start | ' ' | '\t').OneOrMore());
		/// <inheritdoc />
		public SmartExpressionBrick OptionalWhiteSpace => CreateRulePlaceholder(StandardSmartExpressionRuleNames.OptionalWhiteSpace, ~WhiteSpace);
		/// <inheritdoc />
		public SmartExpressionBrick CustomSequence => CreateRulePlaceholder(StandardSmartExpressionRuleNames.CustomSequence, Empty);
		/// <inheritdoc />
		public SmartExpressionBrick NextLine => CreateRulePlaceholder(StandardSmartExpressionRuleNames.NextLine, OptionalWhiteSpace + Environment.NewLine);
		/// <inheritdoc />
		public SmartExpressionBrick Letter => CreateRulePlaceholder(StandardSmartExpressionRuleNames.Letter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.Letter));
		/// <inheritdoc />
		public SmartExpressionBrick UpperLetter => CreateRulePlaceholder(StandardSmartExpressionRuleNames.UpperLetter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.UpperLetter));
		/// <inheritdoc />
		public SmartExpressionBrick LowerLetter => CreateRulePlaceholder(StandardSmartExpressionRuleNames.LowerLetter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.LowerLetter));
		/// <inheritdoc />
		public SmartExpressionBrick Symbol => CreateRulePlaceholder(StandardSmartExpressionRuleNames.Symbol, null);
		/// <inheritdoc />
		public SmartExpressionBrick Digit => CreateRulePlaceholder(StandardSmartExpressionRuleNames.Digit, new StandardCharacterRangeBrick(this, StandardCharactersRanges.Digit));
		/// <inheritdoc />
		public SmartExpressionBrick Current => new CurrentRulePlaceholderBrick(this);
		
		public StandardSmartExpressionAlgebra(int priority)
		{
			Priority = priority;
		}

		public SmartExpressionBrick DefineRule(SmartExpressionBrick parsingBrick, string ruleName, bool blockFromRuleOverwriting)
		{
			return new CapturingGroupBrick(this, parsingBrick, ruleName, blockFromRuleOverwriting);
		}

		/// <inheritdoc />
		public SmartExpressionBrick DefineSealedRule(SmartExpressionBrick content, string ruleName)
		{
			return DefineRule(content, ruleName, blockFromRuleOverwriting: true);
		}

		/// <inheritdoc />
		public SmartExpressionBrick CreateRulePlaceholder(string ruleName, SmartExpressionBrick defaultExpression)
		{
			return new RulePlaceholderBrick(this.GetHighestPrioritizedAlgebra<SmartExpressionBrick, ISmartExpressionAlgebra>(defaultExpression), ruleName, defaultExpression);
		}

		/// <inheritdoc />
		public SmartExpressionBrick CreatePositiveLookahead(SmartExpressionBrick content)
		{
			throw new NotImplementedException();
		}

		public SmartExpressionBrick CreateQuantifierBrick(SmartExpressionBrick content, long min, long max)
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
				return new QuantifierBrick(this, qb.Content, 0, qb.Max);
			}

			return new QuantifierBrick(this, content, min, max);
		}

		/// <inheritdoc />
		public SmartExpressionBrick DefineRule(SmartExpressionBrick content, string ruleName)
		{
			return DefineRule(content, ruleName, blockFromRuleOverwriting: false);
		}

		public SmartExpressionBrick EvaluateOperatorResult(SmartExpressionBrick left, OverloadableCodeSymmetricBinaryOperator binaryOperator, SmartExpressionBrick right)
		{
			EvaluateArguments(ref left, ref right);

			switch (binaryOperator)
			{
				case OverloadableCodeSymmetricBinaryOperator.Mul:
					return Concatenate(left, SX.Anything, right);
				case OverloadableCodeSymmetricBinaryOperator.Div:
					return Concatenate(left, SX.NextLine, right);
				case OverloadableCodeSymmetricBinaryOperator.Mod:
					return Concatenate(left, SX.CustomSequence, right);
				case OverloadableCodeSymmetricBinaryOperator.Sum:
					return Concatenate(left, right);
				case OverloadableCodeSymmetricBinaryOperator.Sub:
					return Concatenate(left, CreatePositiveLookahead(right));
				case OverloadableCodeSymmetricBinaryOperator.And:
					return Concatenate(left, SX.OptionalWhiteSpace, right);
				case OverloadableCodeSymmetricBinaryOperator.XOr:
					return Concatenate(left, SX.WhiteSpace, right);
				case OverloadableCodeSymmetricBinaryOperator.Or:
					return CreateAlternation(left, right);
				default:
					throw new NotSupportedException($"Operator {binaryOperator} is not supported for two arguments of type {nameof(SmartExpressionBrick)}.");
			}
		}
		
		public SmartExpressionBrick EvaluateOperatorResult(OverloadableCodeSymmetricUnaryOperator unaryOperator, SmartExpressionBrick arg)
		{
			EvaluateArguments(ref arg);

			switch (unaryOperator)
			{
				case OverloadableCodeSymmetricUnaryOperator.Minus:
					return CreatePositiveLookahead(arg);
				case OverloadableCodeSymmetricUnaryOperator.LogicalNegation:
					throw new NotImplementedException();
				case OverloadableCodeSymmetricUnaryOperator.BitwiseComplement:
					return CreateQuantifierBrick(arg, 0, 1);
				default:
					throw new NotSupportedException($"Operator {unaryOperator} is not supported for argument of type {nameof(SmartExpressionBrick)}.");
			}
		}

		private SmartExpressionBrick Concatenate(SmartExpressionBrick left, SmartExpressionBrick right)
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

		private SmartExpressionBrick Concatenate(SmartExpressionBrick left, SmartExpressionBrick middle, SmartExpressionBrick right)
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

		private SmartExpressionBrick CreateAlternation(SmartExpressionBrick left, SmartExpressionBrick right)
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

		private void EvaluateArguments(ref SmartExpressionBrick arg1)
		{
			if (arg1 is CapturingGroupBrick cb)
			{
				arg1 = cb.Clone(this, cb.GroupName);
			}
		}

		private void EvaluateArguments(ref SmartExpressionBrick arg1, ref SmartExpressionBrick arg2)
		{
			EvaluateArguments(ref arg1);
			EvaluateArguments(ref arg2);
		}
	}
}
