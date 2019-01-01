using System;
using System.Linq;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.Parser.SmartExpressions.Bricks;

namespace QuickAccess.Parser.SmartExpressions
{
	public class StandardSmartExpressionAlgebra : ISmartExpressionAlgebra
	{
		public SmartExpressionBrick Anything => CreatePlaceholder(StandardSmartExpressionRuleNames.Anything, null);

		/// <inheritdoc />
		public SmartExpressionBrick Empty => EmptyParsingBrick.Instance;

		/// <inheritdoc />
		public SmartExpressionBegin Start => new SmartExpressionBegin(this);
		public SmartExpressionBrick WhiteSpace => CreatePlaceholder(StandardSmartExpressionRuleNames.WhiteSpace, (Start | ' ' | '\t').OneOrMore());
		public SmartExpressionBrick OptionalWhiteSpace => CreatePlaceholder(StandardSmartExpressionRuleNames.OptionalWhiteSpace, ~WhiteSpace);
		public SmartExpressionBrick CustomSequence => CreatePlaceholder(StandardSmartExpressionRuleNames.CustomSequence, Empty);
		public SmartExpressionBrick NextLine => CreatePlaceholder(StandardSmartExpressionRuleNames.NextLine, OptionalWhiteSpace + Environment.NewLine);
		public SmartExpressionBrick Letter => CreatePlaceholder(StandardSmartExpressionRuleNames.Letter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.Letter));
		public SmartExpressionBrick UpperLetter => CreatePlaceholder(StandardSmartExpressionRuleNames.UpperLetter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.UpperLetter));
		public SmartExpressionBrick LowerLetter => CreatePlaceholder(StandardSmartExpressionRuleNames.LowerLetter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.LowerLetter));
		public SmartExpressionBrick Symbol => CreatePlaceholder(StandardSmartExpressionRuleNames.Symbol, null);
		public SmartExpressionBrick Digit => CreatePlaceholder(StandardSmartExpressionRuleNames.Digit, new StandardCharacterRangeBrick(this, StandardCharactersRanges.Digit));


		public StandardSmartExpressionAlgebra(int priority)
		{
			Priority = priority;
		}

		public SmartExpressionBrick DefineRule(SmartExpressionBrick parsingBrick, string ruleName)
		{
			return new CapturingGroupBrick(this, parsingBrick, ruleName);
		}

		/// <inheritdoc />
		public SmartExpressionBrick CreatePlaceholder(string ruleName, SmartExpressionBrick defaultExpression)
		{
			return new RulePlaceholderBrick(this.GetAlgebra<SmartExpressionBrick, ISmartExpressionAlgebra>(defaultExpression), ruleName, defaultExpression);
		}

		/// <inheritdoc />
		public SmartExpressionBrick Current => new CurrentRulePlaceholderBrick(this);
		

		public SmartExpressionBrick CreateQuantifierBrick(SmartExpressionBrick parsingBrick, long min, long max)
		{
			if (parsingBrick == null || parsingBrick == EmptyParsingBrick.Instance)
			{
				return parsingBrick;
			}

			if (min == 1 && max == 1)
			{
				return parsingBrick;
			}

			if (min == 0 && max == 1 && parsingBrick is QuantifierBrick qb && qb.Min <= 1)
			{
				return new QuantifierBrick(this, qb.Content, 0, qb.Max);
			}

			return new QuantifierBrick(this, parsingBrick, min, max);
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

		private SmartExpressionBrick CreateOption(SmartExpressionBrick left, SmartExpressionBrick right)
		{
			if (left.Equals(right))
			{
				return left;
			}

			if (left is OptionsBrick lc && right is OptionsBrick rc)
			{
				return new OptionsBrick(this, lc.Items.Concat(rc.Items.Where(i => !lc.Items.Contains(i))).ToArray());
			}

			if (left is OptionsBrick l && l.Items.Any(i => i.Equals(right)))
			{
				return left;
			}

			if (right is OptionsBrick r && r.Items.Any(i => i.Equals(left)))
			{
				return right;
			}

			return new OptionsBrick(this, left, right);
		}

		/// <inheritdoc />
		public int Priority { get; }

		public SmartExpressionBrick EvaluateOperatorResult(SmartExpressionBrick left, BinaryOperator binaryOperator, SmartExpressionBrick right)
		{
			switch (binaryOperator)
			{
				case BinaryOperator.Mul:
					return Concatenate(left, SX.Anything, right);
				case BinaryOperator.Div:
					return Concatenate(left, SX.NextLine, right);
				case BinaryOperator.Mod:
					return Concatenate(left, SX.CustomSequence, right);
				case BinaryOperator.Sum:
					return Concatenate(left, right);
				case BinaryOperator.Sub:
					throw new NotImplementedException();
				case BinaryOperator.And:
					return Concatenate(left, SX.OptionalWhiteSpace, right);
				case BinaryOperator.XOr:
					return Concatenate(left, SX.WhiteSpace, right);
				case BinaryOperator.Or:
					return CreateOption(left, right);
				default:
					throw new NotSupportedException($"Operator {binaryOperator} is not supported for two arguments of type {nameof(SmartExpressionBrick)}.");
			}
		}

		
		public SmartExpressionBrick EvaluateOperatorResult(UnaryOperator unaryOperator, SmartExpressionBrick arg)
		{
			switch (unaryOperator)
			{
				case UnaryOperator.SingleMinus:
					throw new NotImplementedException();
				case UnaryOperator.LogicalNot:
					throw new NotImplementedException();
				case UnaryOperator.BinaryNot:
					return CreateQuantifierBrick(arg, 0, 1);
				default:
					throw new NotSupportedException($"Operator {unaryOperator} is not supported for argument of type {nameof(SmartExpressionBrick)}.");
			}
		}

		public SmartExpressionBrick EvaluatePattern(SmartExpressionBrick parsingBrick, string pattern)
		{
			throw new NotImplementedException();
		}
	}


	

		

}
