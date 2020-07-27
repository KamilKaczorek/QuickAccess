using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using QuickAccess.DataStructures.Algebra;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class OperatorDefinition
    {
        public OverloadableCodeBinarySymmetricOperator Operator { get; }
        private readonly OperatorDefinitionArg _left;
        private readonly OperatorDefinitionArg _right;

        public OperatorDefinition(OverloadableCodeBinarySymmetricOperator @operator, OperatorDefinitionArg left, OperatorDefinitionArg right)
        {
            Operator = @operator;
            _left = left;
            _right = right;
        }
    }

    public sealed class OperatorDefinitionArg
    {
        public static OperatorDefinition operator +(OperatorDefinitionArg x, OperatorDefinitionArg y)
        {
            return new OperatorDefinition(OverloadableCodeBinarySymmetricOperator.Sum, x, y);
        }

        public static OperatorDefinition operator -(OperatorDefinitionArg x, OperatorDefinitionArg y)
        {
            return new OperatorDefinition(OverloadableCodeBinarySymmetricOperator.Sub, x, y);
        }

        public static OperatorDefinition operator ^(OperatorDefinitionArg x, OperatorDefinitionArg y)
        {
            return new OperatorDefinition(OverloadableCodeBinarySymmetricOperator.XOr, x, y);
        }

        public static OperatorDefinition operator %(OperatorDefinitionArg x, OperatorDefinitionArg y)
        {
            return new OperatorDefinition(OverloadableCodeBinarySymmetricOperator.Mod, x, y);
        }

        public static OperatorDefinition operator *(OperatorDefinitionArg x, OperatorDefinitionArg y)
        {
            return new OperatorDefinition(OverloadableCodeBinarySymmetricOperator.Mul, x, y);
        }

        public static OperatorDefinition operator /(OperatorDefinitionArg x, OperatorDefinitionArg y)
        {
            return new OperatorDefinition(OverloadableCodeBinarySymmetricOperator.Div, x, y);
        }
    }



    [Serializable]
    public abstract class Flexpression<TConstraint> 
        : IFlexpression<TConstraint> 
        where TConstraint : IFlexpressionConstraint
    {
        public Type ConstraintType => typeof(TConstraint);
        public FlexpressionId LocalId { get; } = FlexpressionId.Generate();

        public virtual string Name => $"<{GetType().Name}_{LocalId}>";

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
            return Equals(obj as IFlexpression);
        }

        public override int GetHashCode()
        {
            return LocalId.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual bool Equals(IFlexpression other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return LocalId.Equals(other.LocalId);
        }

        public static bool operator ==(Flexpression<TConstraint> left, Flexpression<TConstraint> right) { return Equals(left, right); }
        public static bool operator !=(Flexpression<TConstraint> left, Flexpression<TConstraint> right) { return !Equals(left, right); }
    }
}