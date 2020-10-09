using System.Diagnostics.Contracts;
using QuickAccess.Infrastructure.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public abstract class CustomFXConstraint : IFlexpressionConstraint
    {
        [Pure] public virtual FXConstraintResult IsCharAllowed(char ch) { return FXConstraintResult.Allowed; }
        [Pure] public virtual FXConstraintResult IsStringAllowed(string str) { return FXConstraintResult.Allowed; }
        [Pure] public virtual FXConstraintResult IsQuantifierAllowed(long min, long max) { return FXConstraintResult.Allowed; }
        [Pure] public virtual FXConstraintResult IsUnaryOperatorAllowed(OverloadableCodeUnarySymmetricOperator unaryOperator) { return FXConstraintResult.Allowed; }
        [Pure] public virtual FXConstraintResult IsBinaryOperatorAllowed(OverloadableCodeBinarySymmetricOperator binaryOperator, int argsCount) { return FXConstraintResult.Allowed; }
        [Pure] public virtual FXConstraintResult IsOperationAllowed(string operationName, int argsCount) { return FXConstraintResult.Allowed; }
        [Pure] public virtual FXConstraintResult IsGroupAllowed(string groupName) { return FXConstraintResult.Allowed; }
        [Pure] public virtual FXConstraintResult IsPlaceholderAllowed(string groupName) { return FXConstraintResult.Allowed; }
    }
}