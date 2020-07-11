using System.Collections.Generic;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpressionSpecification
    {
        bool IsSatisfiedByChar(char ch);

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