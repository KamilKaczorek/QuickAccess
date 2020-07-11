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



        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsCharAllowed(char ch) { return DomainConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsStringAllowed(string str) { return DomainConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsQuantifierAllowed(long min, long max) { return DomainConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsUnaryOperatorAllowed(OverloadableCodeUnarySymmetricOperator unaryOperator) { return DomainConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsBinaryOperatorAllowed(OverloadableCodeBinarySymmetricOperator binaryOperator, int argsCount) { return DomainConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsOperationAllowed(string operationName, int argsCount) { return DomainConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsGroupAllowed(string groupName) { return DomainConstraintResult.Allowed; }
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough] public DomainConstraintResult IsPlaceholderAllowed(string groupName) { return DomainConstraintResult.Allowed; }
    }
}