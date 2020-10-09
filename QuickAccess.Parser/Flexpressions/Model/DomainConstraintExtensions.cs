using System.Diagnostics;
using System.Runtime.CompilerServices;
using QuickAccess.Infrastructure.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public static class DomainConstraintExtensions
    {
        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateCharAllowed(this IFlexpressionConstraint source, char ch)
        {
            var res = source.IsCharAllowed(ch);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                case FXConstraintResult.OperationNotAllowed:
                    throw new DomainConstraintBrokenException($"Char operator is not allowed by domain constraint ({source.GetType().Name}).");
                default:
                    throw new DomainConstraintBrokenException($"Char operator not allowed for the argument '{ch}'  by domain constraint ({source.GetType().Name}).");
            }
        }
        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]public static void ValidateStringAllowed(this IFlexpressionConstraint source, string str)
        {
            var res = source.IsStringAllowed(str);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                case FXConstraintResult.OperationNotAllowed:
                    throw new DomainConstraintBrokenException($"String operator is not allowed by domain constraint ({source.GetType().Name}).");
                default:
                    throw new DomainConstraintBrokenException($"String operator not allowed for the argument '{str}'  by domain constraint ({source.GetType().Name}).");
            }
        }
        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]public static void ValidateQuantifierAllowed(this IFlexpressionConstraint source, long min, long max)
        {
            var res = source.IsQuantifierAllowed(min, max);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                case FXConstraintResult.OperationNotAllowed:
                    throw new DomainConstraintBrokenException($"Quantifier operator ([]) is not allowed by domain constraint ({source.GetType().Name}).");
                default:
                    throw new DomainConstraintBrokenException($"Quantifier operator not allowed for the range [{min}, {max}]  by domain constraint ({source.GetType().Name}).");
            }
        }

        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]public static void ValidateUnaryOperatorAllowed(this IFlexpressionConstraint source, OverloadableCodeUnarySymmetricOperator unaryOperator)
        {
            var res = source.IsUnaryOperatorAllowed(unaryOperator);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                default:
                    throw new DomainConstraintBrokenException($"Unary operator '{unaryOperator.GetSymbol()}' ({unaryOperator}) is not allowed  by domain constraint ({source.GetType().Name}).");
            }
        }

        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]public static void ValidateBinaryOperatorAllowed(this IFlexpressionConstraint source, OverloadableCodeBinarySymmetricOperator binaryOperator, int argsCount)
        {
            var res = source.IsBinaryOperatorAllowed(binaryOperator, argsCount);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                case FXConstraintResult.OperationNotAllowed:
                    throw new DomainConstraintBrokenException($"Binary operator '{binaryOperator.GetSymbol()}' ({binaryOperator}) is not allowed  by domain constraint ({source.GetType().Name}).");
                default:
                    throw new DomainConstraintBrokenException($"Binary operator '{binaryOperator.GetSymbol()}' ({binaryOperator}) is not allowed for the number of args={argsCount} by domain constraint ({source.GetType().Name}).");
            }
        }


        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]public static void ValidateOperationAllowed(this IFlexpressionConstraint source, string operationName, int argsCount)
        {
            var res = source.IsOperationAllowed(operationName, argsCount);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                case FXConstraintResult.OperationNotAllowed:
                    throw new DomainConstraintBrokenException($"Operations are not allowed by domain constraint ({source.GetType().Name}).");
                default:
                    throw new DomainConstraintBrokenException($"Operation '{operationName}' is not allowed (number of args={argsCount})  by domain constraint ({source.GetType().Name}).");
            }
        }

        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidateGroupAllowed(this IFlexpressionConstraint source, string groupName)
        {
            var res = source.IsGroupAllowed(groupName);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                case FXConstraintResult.OperationNotAllowed:
                    throw new DomainConstraintBrokenException($"Groups are not allowed by domain constraint ({source.GetType().Name}).");
                default:
                    throw new DomainConstraintBrokenException($"Group name '{groupName}' is not allowed by domain constraint ({source.GetType().Name}).");
            }
        }

        [DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]public static void ValidatePlaceholderAllowed(this IFlexpressionConstraint source, string groupName)
        {
            var res = source.IsGroupAllowed(groupName);

            switch (res)
            {
                case FXConstraintResult.Allowed:
                    return;
                case FXConstraintResult.OperationNotAllowed:
                    throw new DomainConstraintBrokenException("Placeholders are not allowed.");
                default:
                    throw new DomainConstraintBrokenException($"Placeholder group name '{groupName}' is not allowed.");
            }
        }
    }
}