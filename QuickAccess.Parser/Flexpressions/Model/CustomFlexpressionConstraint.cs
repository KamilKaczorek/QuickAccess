using System.Diagnostics.Contracts;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public abstract class CustomFlexpressionConstraint : IFlexpressionConstraint
    {
        [Pure] public virtual DomainConstraintResult IsCharAllowed(char ch) { return DomainConstraintResult.Allowed; }
        [Pure] public virtual DomainConstraintResult IsStringAllowed(string str) { return DomainConstraintResult.Allowed; }
        [Pure] public virtual DomainConstraintResult IsQuantifierAllowed(long min, long max) { return DomainConstraintResult.Allowed; }
        [Pure] public virtual DomainConstraintResult IsUnaryOperatorAllowed(OverloadableCodeUnarySymmetricOperator unaryOperator) { return DomainConstraintResult.Allowed; }
        [Pure] public virtual DomainConstraintResult IsBinaryOperatorAllowed(OverloadableCodeBinarySymmetricOperator binaryOperator, int argsCount) { return DomainConstraintResult.Allowed; }
        [Pure] public virtual DomainConstraintResult IsOperationAllowed(string operationName, int argsCount) { return DomainConstraintResult.Allowed; }
        [Pure] public virtual DomainConstraintResult IsGroupAllowed(string groupName) { return DomainConstraintResult.Allowed; }
        [Pure] public virtual DomainConstraintResult IsPlaceholderAllowed(string groupName) { return DomainConstraintResult.Allowed; }
    }
}