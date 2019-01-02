using System;
using System.Linq;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common;
using QuickAccess.Parser.SmartExpressions.Bricks;

namespace QuickAccess.Parser.SmartExpressions
{
	public class StandardSmartExpressionAlgebra : ISmartExpressionAlgebra
	{
		/// <inheritdoc />
		public int Priority { get; }

		public SmartExpressionBrick Anything => CreatePlaceholder(StandardSmartExpressionRuleNames.Anything, null);

		/// <inheritdoc />
		public SmartExpressionBrick Empty => EmptyParsingBrick.Instance;

		/// <inheritdoc />
		public SmartExpressionBegin Start => new SmartExpressionBegin(this);
		/// <inheritdoc />
		public SmartExpressionBrick WhiteSpace => CreatePlaceholder(StandardSmartExpressionRuleNames.WhiteSpace, (Start | ' ' | '\t').OneOrMore());
		/// <inheritdoc />
		public SmartExpressionBrick OptionalWhiteSpace => CreatePlaceholder(StandardSmartExpressionRuleNames.OptionalWhiteSpace, ~WhiteSpace);
		/// <inheritdoc />
		public SmartExpressionBrick CustomSequence => CreatePlaceholder(StandardSmartExpressionRuleNames.CustomSequence, Empty);
		/// <inheritdoc />
		public SmartExpressionBrick NextLine => CreatePlaceholder(StandardSmartExpressionRuleNames.NextLine, OptionalWhiteSpace + Environment.NewLine);
		/// <inheritdoc />
		public SmartExpressionBrick Letter => CreatePlaceholder(StandardSmartExpressionRuleNames.Letter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.Letter));
		/// <inheritdoc />
		public SmartExpressionBrick UpperLetter => CreatePlaceholder(StandardSmartExpressionRuleNames.UpperLetter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.UpperLetter));
		/// <inheritdoc />
		public SmartExpressionBrick LowerLetter => CreatePlaceholder(StandardSmartExpressionRuleNames.LowerLetter, new StandardCharacterRangeBrick(this, StandardCharactersRanges.LowerLetter));
		/// <inheritdoc />
		public SmartExpressionBrick Symbol => CreatePlaceholder(StandardSmartExpressionRuleNames.Symbol, null);
		/// <inheritdoc />
		public SmartExpressionBrick Digit => CreatePlaceholder(StandardSmartExpressionRuleNames.Digit, new StandardCharacterRangeBrick(this, StandardCharactersRanges.Digit));
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
		public SmartExpressionBrick CreatePlaceholder(string ruleName, SmartExpressionBrick defaultExpression)
		{
			return new RulePlaceholderBrick(this.GetAlgebra<SmartExpressionBrick, ISmartExpressionAlgebra>(defaultExpression), ruleName, defaultExpression);
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

		public SmartExpressionBrick EvaluateOperatorResult(SmartExpressionBrick left, BinaryOperator binaryOperator, SmartExpressionBrick right)
		{
			EvaluateArguments(ref left, ref right);

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
					return CreateAlternation(left, right);
				default:
					throw new NotSupportedException($"Operator {binaryOperator} is not supported for two arguments of type {nameof(SmartExpressionBrick)}.");
			}
		}
		
		public SmartExpressionBrick EvaluateOperatorResult(UnaryOperator unaryOperator, SmartExpressionBrick arg)
		{
			EvaluateArguments(ref arg);

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
