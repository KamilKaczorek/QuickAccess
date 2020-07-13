using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class DefaultFlexpressionConstraint : IFlexpressionConstraint
    {
        public static readonly IFlexpressionConstraint Instance = new DefaultFlexpressionConstraint();
        public static readonly Type ConstraintType = typeof(DefaultFlexpressionConstraint);

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsCharAllowed(char ch) { return FXConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsStringAllowed(string str) { return FXConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsQuantifierAllowed(long min, long max) { return FXConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsUnaryOperatorAllowed(OverloadableCodeUnarySymmetricOperator unaryOperator) { return FXConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsBinaryOperatorAllowed(OverloadableCodeBinarySymmetricOperator binaryOperator, int argsCount) { return FXConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsOperationAllowed(string operationName, int argsCount) { return FXConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsGroupAllowed(string groupName) { return FXConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public FXConstraintResult IsPlaceholderAllowed(string groupName) { return FXConstraintResult.Allowed; }
    }
}