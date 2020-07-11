using System;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class QuantifierFlexpression<TConstraint> : Flexpression<TConstraint> where TConstraint : IFlexpressionConstraint
    {
        public long Min { get; }
        public long Max { get; }
        public IFlexpression<TConstraint> Content { get; }

        public bool IsEmpty => (Min == 0 && Max == 0);

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var contentVisitationResult = Content.AcceptVisitor(visitor);
            var visitationResult = visitor.VisitQuantifier(contentVisitationResult, Min, Max);
            return visitationResult;
        }

        public QuantifierFlexpression(IFlexpression<TConstraint> content, long min, long max)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));

            Constraint.ValidateQuantifierAllowed(min, max);

            if (Content is QuantifierFlexpression<TConstraint> quantifier)
            {
                Content = quantifier.Content;
                min = quantifier.Min * Min;
                max = quantifier.Max * Max;
            }

            if (min < 0)
            {
                throw new ArgumentException($"Min value {min} is smaller than 0.", nameof(min));
            }

            if (max < 0)
            {
                throw new ArgumentException($"Max value {max} is smaller than 0.", nameof(max));
            }

            if (max < min)
            {
                throw new ArgumentException($"Max value {max} is smaller than min value {min}.", nameof(max));
            }

            Min = min;
            Max = max;
        }

        /// <inheritdoc />
        public override string Name
        {
            get
            {
                var contentExpression = Content.ToString();

                if (IsEmpty)
                {
                    return FXB.Empty.ToString();
                }

                if (Min == 1 && Max == 1)
                {
                    return contentExpression;
                }

                if (Min == 0 && Max == 1)
                {
                    return $"Optional({contentExpression})";
                }

                if (Min == 0 && Max == long.MaxValue)
                {
                    return $"ZeroOrMany({contentExpression})";
                }

                if (Min == 1 && Max == long.MaxValue)
                {
                    return $"OneOrMany({contentExpression})";
                }

                if (Min == Max)
                {
                    return $"Repeated{Min}Times({contentExpression})";
                }

                return $"RepeatedBetween{Min}And{Max}Times({contentExpression})";
            }
        }
    }

    public static class QuantifierFlexpression
    {
        public static Flexpression<TConstraint> Create<TConstraint>(
            IFlexpression<TConstraint> content,
            long min,
            long max) where TConstraint : IFlexpressionConstraint
        {
            return new QuantifierFlexpression<TConstraint>(content, min, max);
        }

        public static Flexpression<TConstraint> Create<TConstraint>(
            IFlexpression<TConstraint> content,
            long count) where TConstraint : IFlexpressionConstraint
        {
            return new QuantifierFlexpression<TConstraint>(content, count, count);
        }
    }
}