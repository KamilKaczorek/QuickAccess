using System.Collections.Generic;
using QuickAccess.Infrastructure.Algebra;
using QuickAccess.Infrastructure.CharMatching;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpressionSpecification
    {
        bool IsSatisfiedByChar(ICharactersRangeDefinition charactersRangeDefinition);

        bool IsSatisfiedByString(string str);

        bool IsSatisfiedByQuantifier(bool isContentSatisfied, long min, long max);

        bool IsSatisfiedByUnaryOperator(OverloadableCodeUnarySymmetricOperator unaryOperator, bool isArgSatisfied);

        bool IsSatisfiedByBinaryOperator(OverloadableCodeBinarySymmetricOperator binaryOperator, IEnumerable<bool> isArgSatisfied, int argsCount);

        bool IsSatisfiedByOperation(string operationName, IEnumerable<bool> isArgSatisfied, int argsCount);

        bool IsSatisfiedByGroup(string groupName, bool isContentSatisfied);

        bool IsSatisfiedByPlaceholder(string groupName);

        bool IsSatisfiedByCustom(IFlexpression customFlexpression);
    }
}