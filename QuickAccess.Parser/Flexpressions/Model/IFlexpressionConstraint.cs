using JetBrains.Annotations;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpressionConstraint
    {
        [Pure] DomainConstraintResult IsCharAllowed(char ch);
        [Pure] DomainConstraintResult IsStringAllowed(string str);
        [Pure] DomainConstraintResult IsQuantifierAllowed(long min, long max);
        [Pure] DomainConstraintResult IsUnaryOperatorAllowed(OverloadableCodeUnarySymmetricOperator unaryOperator);
        [Pure] DomainConstraintResult IsBinaryOperatorAllowed(OverloadableCodeBinarySymmetricOperator binaryOperator, int argsCount);
        [Pure] DomainConstraintResult IsOperationAllowed(string operationName, int argsCount);
        [Pure] DomainConstraintResult IsGroupAllowed(string groupName);
        [Pure] DomainConstraintResult IsPlaceholderAllowed(string groupName);
    }
}