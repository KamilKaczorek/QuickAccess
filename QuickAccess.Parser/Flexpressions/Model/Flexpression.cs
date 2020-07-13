using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    [Serializable]
    public abstract class Flexpression<TConstraint> : IFlexpression<TConstraint> 
        where TConstraint : IFlexpressionConstraint
    {
        public Type ConstraintType => typeof(TConstraint);
        public long LocalId { get; } = FlexpressionLocalIdGenerator.GetNext();

        public virtual string Name => GetType().Name;

        protected static IFlexpressionConstraint Constraint
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
            get
            {
                var constraint = FlexpressionConstraintRepositoryProvider.Repository.Resolve<TConstraint>();
                return constraint;
            }
        }

        public Flexpression<TConstraint> this[long minCount, long maxCount] =>
            QuantifierFlexpression.Create(this, minCount, maxCount);

        public Flexpression<TConstraint> this[long count] => QuantifierFlexpression.Create(this, count);

        public static Flexpression<TConstraint> operator *(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mul, left, right);
        }

        public static Flexpression<TConstraint> operator /(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Div, left, right);
        }

        public static Flexpression<TConstraint> operator %(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mod, left, right);
        }

        public static Flexpression<TConstraint> operator +(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sum, left, right);
        }

        public static Flexpression<TConstraint> operator -(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sub, left, right);
        }

        public static Flexpression<TConstraint> operator &(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.And, left, right);
        }

        public static Flexpression<TConstraint> operator ^(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.XOr, left, right);
        }

        public static Flexpression<TConstraint> operator |(Flexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Or, left, right);
        }

        public static Flexpression<TConstraint> operator ++(Flexpression<TConstraint> arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Increment, arg);
        }

        public static Flexpression<TConstraint> operator --(Flexpression<TConstraint> arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Decrement, arg);
        }

        public static Flexpression<TConstraint> operator +(Flexpression<TConstraint> arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Plus, arg);
        }

        public static Flexpression<TConstraint> operator -(Flexpression<TConstraint> arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Minus, arg);
        }

        public static Flexpression<TConstraint> operator ~(Flexpression<TConstraint> arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.BitwiseComplement, arg);
        }

        public static Flexpression<TConstraint> operator !(Flexpression<TConstraint> arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.LogicalNegation, arg);
        }

        public static Flexpression<TConstraint> operator *(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mul, left, right);
        }

        public static Flexpression<TConstraint> operator /(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Div, left, right);
        }

        public static Flexpression<TConstraint> operator %(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mod, left, right);
        }

        public static Flexpression<TConstraint> operator +(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sum, left, right);
        }

        public static Flexpression<TConstraint> operator -(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sub, left, right);
        }

        public static Flexpression<TConstraint> operator &(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.And, left, right);
        }

        public static Flexpression<TConstraint> operator ^(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.XOr, left, right);
        }

        public static Flexpression<TConstraint> operator |(Flexpression<TConstraint> left, IFlexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Or, left, right);
        }

        public static Flexpression<TConstraint> operator *(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mul, left, right);
        }

        public static Flexpression<TConstraint> operator /(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Div, left, right);
        }

        public static Flexpression<TConstraint> operator %(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mod, left, right);
        }

        public static Flexpression<TConstraint> operator +(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sum, left, right);
        }

        public static Flexpression<TConstraint> operator -(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sub, left, right);
        }

        public static Flexpression<TConstraint> operator &(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.And, left, right);
        }

        public static Flexpression<TConstraint> operator ^(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.XOr, left, right);
        }

        public static Flexpression<TConstraint> operator |(IFlexpression<TConstraint> left, Flexpression<TConstraint> right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Or, left, right);
        }

        public static implicit operator Flexpression<TConstraint>(string x)
        {
            return StringFlexpression.Create<TConstraint>(x);
        }

        public static implicit operator Flexpression<TConstraint>(char x)
        {
            return CharFlexpression.Create<TConstraint>(x);
        }

        public virtual TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = visitor.VisitCustom(this);
            return visitationResult;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Flexpression<TConstraint> flexpression)
            {
                var equals = flexpression.LocalId == LocalId;
                return equals;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(LocalId);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}