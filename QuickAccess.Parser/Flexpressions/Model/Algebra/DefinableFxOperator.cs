using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model.Algebra
{
    public static class OperatorSelection
    {
        private static Binary BinaryArgument => Binary.Argument;
        private static Unary UnaryArgument => Unary.Argument;

        public delegate DefinableFxBinaryOperator BinarySelector(Binary x, Binary y);
        public delegate Unary UnarySelector(Unary x);

        public static OverloadableCodeBinarySymmetricOperator ToCodeOperator(this DefinableFxBinaryOperator source)
        {
            var res = (OverloadableCodeBinarySymmetricOperator)source;
            return res;
        }

        public static OverloadableCodeUnarySymmetricOperator ToCodeOperator(this DefinableFxUnaryOperator source)
        {
            var res = (OverloadableCodeUnarySymmetricOperator)source;
            return res;
        }

        public static string GetSymbol(this DefinableFxBinaryOperator source)
        {
            var op = (OverloadableCodeBinarySymmetricOperator)source;
            return op.GetSymbol();
        }

        public static string GetSymbol(this DefinableFxUnaryOperator source)
        {
            var op = (OverloadableCodeUnarySymmetricOperator)source;
            return op.GetSymbol();
        }

        public static DefinableFxBinaryOperator ToDefinableFxOperator(this OverloadableCodeBinarySymmetricOperator source)
        {
            if (source == OverloadableCodeBinarySymmetricOperator.And ||
                source == OverloadableCodeBinarySymmetricOperator.Or)
            {
                throw new ArgumentOutOfRangeException(nameof(source), 
                    $"Binary operator '{source.GetSymbol()}' ({source}) is not definable flexpression operator.");
            }

            var res = (DefinableFxBinaryOperator)source;
            return res;
        }

        public static DefinableFxUnaryOperator ToDefinableFxOperator(this OverloadableCodeUnarySymmetricOperator source)
        {
            var res = (DefinableFxUnaryOperator)source;
            return res;
        }

        public static DefinableFxBinaryOperator EvaluateOperator(
            this BinarySelector operatorSelector)
        {
            var res = operatorSelector.Invoke(BinaryArgument, BinaryArgument);
            return res;
        }

        public static DefinableFxUnaryOperator EvaluateOperator(
            this UnarySelector operatorSelector)
        {
            var res = operatorSelector.Invoke(UnaryArgument).Operator;
            return res;
        }

        public static Func<Flexpression, Flexpression, Flexpression> GetOperatorDefinitionOrNull(
            this IReadOnlyDictionary<DefinableFxBinaryOperator, Func<Flexpression, Flexpression, Flexpression>> source,
            BinarySelector operatorSelector)
        {
            return source.TryGetValue(operatorSelector.EvaluateOperator(), out var value) ? value : null;
        }

        public static Func<Flexpression, Flexpression> GetOperatorDefinitionOrNull(
            this IReadOnlyDictionary<DefinableFxUnaryOperator, Func<Flexpression, Flexpression>> source,
            UnarySelector operatorSelector)
        {
            return source.TryGetValue(operatorSelector.EvaluateOperator(), out var value) ? value : null;
        }

        public static Func<Flexpression, Flexpression, Flexpression> GetOperatorDefinitionOrNull(
            this IReadOnlyDictionary<DefinableFxBinaryOperator, Func<Flexpression, Flexpression, Flexpression>> source,
            DefinableFxBinaryOperator @operator)
        {
            return source.TryGetValue(@operator, out var value) ? value : null;
        }

        public static Func<Flexpression, Flexpression> GetOperatorDefinitionOrNull(
            this IReadOnlyDictionary<DefinableFxUnaryOperator, Func<Flexpression, Flexpression>> source,
            DefinableFxUnaryOperator @operator)
        {
            return source.TryGetValue(@operator, out var value) ? value : null;
        }

        public sealed class Binary
        {
            public static readonly Binary Argument = new Binary();

            private Binary()
            {
            }

            public static DefinableFxBinaryOperator operator +(Binary x, Binary y)
            {
                return DefinableFxBinaryOperator.Sum;
            }

            public static DefinableFxBinaryOperator operator -(Binary x, Binary y)
            {
                return DefinableFxBinaryOperator.Sub;
            }

            public static DefinableFxBinaryOperator operator ^(Binary x, Binary y)
            {
                return DefinableFxBinaryOperator.XOr;
            }

            public static DefinableFxBinaryOperator operator %(Binary x, Binary y)
            {
                return DefinableFxBinaryOperator.Mod;
            }

            public static DefinableFxBinaryOperator operator *(Binary x, Binary y)
            {
                return DefinableFxBinaryOperator.Mul;
            }

            public static DefinableFxBinaryOperator operator /(Binary x, Binary y)
            {
                return DefinableFxBinaryOperator.Div;
            }
        }


        public sealed class Unary
        {
            public static readonly Unary Argument = new Unary(null);

            private static readonly Unary OpUnaryNegation = new Unary(DefinableFxUnaryOperator.Minus);
            private static readonly Unary OpUnaryPlus = new Unary(DefinableFxUnaryOperator.Plus);
            private static readonly Unary OpBitwiseComplement = new Unary(DefinableFxUnaryOperator.BitwiseComplement);
            private static readonly Unary OpLogicalNot = new Unary(DefinableFxUnaryOperator.LogicalNegation);
            private static readonly Unary OpDecrement = new Unary(DefinableFxUnaryOperator.Decrement);
            private static readonly Unary OpIncrement = new Unary(DefinableFxUnaryOperator.Increment);
            
            private readonly DefinableFxUnaryOperator? _operator;
            
            public DefinableFxUnaryOperator Operator
            {
                get
                {
                    if (_operator == null)
                    {
                        throw new InvalidOperationException("Operator selector doesn't contain operator definition - use unary operator expression to define operator.");
                    }

                    return _operator.Value;
                }
            }

            private Unary(DefinableFxUnaryOperator? @operator) { _operator = @operator; }

            [Pure, DebuggerStepThrough, DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining)]
            private Unary GetSingleOperationResult(Unary result)
            {
                const string doubleOperationErrorMessage = "An operator selection can only consist of one operator expression, but already combines the two:";

                if (_operator != null)
                {
                    throw new InvalidOperationException($"{doubleOperationErrorMessage} {Operator.GetSymbol()} and {result.Operator.GetSymbol()}.");
                }

                return result;
            }

            public static Unary operator +(Unary x)
            {
                return x.GetSingleOperationResult(OpUnaryPlus);
            }

            public static Unary operator -(Unary x)
            {
                return x.GetSingleOperationResult(OpUnaryNegation);
            }

            public static Unary operator ~(Unary x)
            {
                return x.GetSingleOperationResult(OpBitwiseComplement);
            }

            public static Unary operator !(Unary x)
            {
                return x.GetSingleOperationResult(OpLogicalNot);
            }

            public static Unary operator --(Unary x)
            {
                return x.GetSingleOperationResult(OpDecrement);
            }

            public static Unary operator ++(Unary x)
            {
                return x.GetSingleOperationResult(OpIncrement);
            }
        }
    }

    public enum DefinableFxBinaryOperator
    {
        /// <summary><c>x*y</c></summary>
        Mul = OverloadableCodeOperator.Mul,
        /// <summary><c>x/y</c></summary>
        Div = OverloadableCodeOperator.Div,
        /// <summary><c>x%y</c></summary>
        Mod = OverloadableCodeOperator.Mod,
        /// <summary><c>x+y</c></summary>
        Sum = OverloadableCodeOperator.Sum,
        /// <summary><c>x-y</c></summary>
        Sub = OverloadableCodeOperator.Sub,
        /// <summary><c>x^y</c></summary>
        XOr = OverloadableCodeOperator.XOr,
    }

    public enum DefinableFxUnaryOperator
    {
        /// <summary>
        /// <c>x++</c>
        /// </summary>
        Increment = OverloadableCodeOperator.Increment,

        /// <summary>
        /// <c>x--</c>
        /// </summary>
        Decrement = OverloadableCodeOperator.Decrement,

        /// <summary>
        /// <c>+x</c>
        /// </summary>
        Plus = OverloadableCodeOperator.Plus,

        /// <summary>
        /// <c>-x</c>
        /// </summary>
        Minus = OverloadableCodeOperator.Minus,

        /// <summary>
        /// <c>!x</c>
        /// </summary>
        LogicalNegation = OverloadableCodeOperator.LogicalNegation,

        /// <summary>
        /// <c>~x</c>
        /// </summary>
        BitwiseComplement = OverloadableCodeOperator.BitwiseComplement
    }

   
}
