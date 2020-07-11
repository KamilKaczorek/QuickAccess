using System.Collections.Generic;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public static class FlexpressionExtensions
    {
        public static bool Satisfies(this IFlexpression source, IFlexpressionSpecification flexpressionSpecification)
        {
            var visitor = new SpecificationToVisitorAdapter(flexpressionSpecification);
            var satisfied = source.AcceptVisitor(visitor);
            return satisfied;
        }

        public static bool IsSatisfiedBy(this IFlexpressionSpecification flexpressionSpecification, IFlexpression flexpression)
        {
            var visitor = new SpecificationToVisitorAdapter(flexpressionSpecification);
            var satisfied = flexpression.AcceptVisitor(visitor);
            return satisfied;
        }

        public static TVisitationResult Visit<TVisitationResult>(this IVisitFlexpressions<TVisitationResult> visitor, IFlexpression flexpression)
        {
            var visitationResult = flexpression.AcceptVisitor(visitor);
            return visitationResult;
        }

        private sealed class SpecificationToVisitorAdapter : IVisitFlexpressions<bool>
        {
            private readonly IFlexpressionSpecification _specification;

            public SpecificationToVisitorAdapter(IFlexpressionSpecification specification)
            {
                _specification = specification;
            }

            public bool VisitChar(char ch) { return _specification.IsSatisfiedByChar(ch); }

            public bool VisitString(string str) { return _specification.IsSatisfiedByString(str); }

            public bool VisitQuantifier(bool contentVisitationResult, long min, long max) { return _specification.IsSatisfiedByQuantifier(contentVisitationResult, min, max); }

            public bool VisitUnaryOperator(OverloadableCodeUnarySymmetricOperator unaryOperator, bool argVisitationResult) { return _specification.IsSatisfiedByUnaryOperator(unaryOperator, argVisitationResult); }

            public bool VisitBinaryOperator(
                OverloadableCodeBinarySymmetricOperator binaryOperator,
                IEnumerable<bool> argsVisitationResults,
                int argsCount)
            {
                return _specification.IsSatisfiedByBinaryOperator(binaryOperator, argsVisitationResults, argsCount);
            }

            public bool VisitOperation(string operationName, IEnumerable<bool> argsVisitationResults, int argsCount) { return _specification.IsSatisfiedByOperation(operationName, argsVisitationResults, argsCount); }

            public bool VisitGroup(string groupName, bool contentVisitationResult) { return _specification.IsSatisfiedByGroup(groupName, contentVisitationResult); }

            public bool VisitPlaceholder(string targetGroupName) { return _specification.IsSatisfiedByPlaceholder(targetGroupName); }

            public bool VisitCustom(IFlexpression customFlexpression) { return _specification.IsSatisfiedByCustom(customFlexpression); }
        }
    }
}