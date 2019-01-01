using System;
using System.Linq;

namespace QuickAccess.Parser.SmartExpressions
{
	public class StandardParsingBrickAlgebra : IParsingBrickAlgebra
	{
		
		public ParsingBrick DefineRule(ParsingBrick parsingBrick, string ruleName)
		{
			return new CapturingGroupBrick(parsingBrick, ruleName);
		}

		public ParsingBrick CreateQuantifierBrick(ParsingBrick parsingBrick, long min, long max)
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
				return new QuantifierBrick(qb.Content, 0, qb.Max);
			}

			return new QuantifierBrick(parsingBrick, min, max);
		}

		private static ParsingBrick Concatenate(ParsingBrick left, ParsingBrick right)
		{
			if (left.IsEmpty)
			{
				return right;
			}

			if (right.IsEmpty)
			{
				return left;
			}

			return new ConcatenationBrick(left, right);
		}

		private static ParsingBrick Concatenate(ParsingBrick left, ParsingBrick middle, ParsingBrick right)
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

			return new ConcatenationBrick(left, middle, right);
		}

		private static ParsingBrick CreateOption(ParsingBrick left, ParsingBrick right)
		{
			if (left.Equals(right))
			{
				return left;
			}

			if (left is OptionsBrick lc && right is OptionsBrick rc)
			{
				return new OptionsBrick(lc.Items.Concat(rc.Items.Where(i => !lc.Items.Contains(i))).ToArray());
			}

			if (left is OptionsBrick l && l.Items.Any(i => i.Equals(right)))
			{
				return left;
			}

			if (right is OptionsBrick r && r.Items.Any(i => i.Equals(left)))
			{
				return right;
			}

			return new OptionsBrick(left, right);
		}

		public ParsingBrick EvaluateOperatorResult(BinaryOperator binaryOperator, ParsingBrick left, ParsingBrick right)
		{
			switch (binaryOperator)
			{
				case BinaryOperator.Mul:
					return Concatenate(left, SX.Anything, right);
				case BinaryOperator.Mod:
					return Concatenate(left, SX.CustomSequence, right);
				case BinaryOperator.Add:
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
					throw new NotSupportedException($"Operator {binaryOperator} is not supported for two arguments of type {nameof(ParsingBrick)}.");
			}
		}

		public ParsingBrick EvaluateOperatorResult(UnaryOperator unaryOperator, ParsingBrick arg)
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
					throw new NotSupportedException($"Operator {unaryOperator} is not supported for argument of type {nameof(ParsingBrick)}.");
			}
		}

		public ParsingBrick EvaluatePattern(ParsingBrick parsingBrick, string pattern)
		{
			throw new NotImplementedException();
		}
	}


	

		

}
