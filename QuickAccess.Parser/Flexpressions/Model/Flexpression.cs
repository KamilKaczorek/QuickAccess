using System;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common.CharMatching;
using QuickAccess.DataStructures.Common.CharMatching.Categories;

namespace QuickAccess.Parser.Flexpressions.Model
{
    [Serializable]
    public abstract class Flexpression : IFlexpression
    {
        public FlexpressionId LocalId { get; } = FlexpressionId.Generate();

        public virtual string Name => $"<{GetType().Name}_{LocalId}>";

       

        public Flexpression this[long minCount, long maxCount] =>
            QuantifierFlexpression.Create(this, minCount, maxCount);

        public Flexpression this[long count] => QuantifierFlexpression.Create(this, count);

        public static Flexpression operator *(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mul, left, right);
        }

        public static Flexpression operator /(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Div, left, right);
        }

        public static Flexpression operator %(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mod, left, right);
        }

        public static Flexpression operator +(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sum, left, right);
        }

        public static Flexpression operator -(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sub, left, right);
        }

        public static Flexpression operator &(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.And, left, right);
        }

        public static Flexpression operator ^(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.XOr, left, right);
        }

        public static Flexpression operator |(Flexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Or, left, right);
        }

        public static Flexpression operator ++(Flexpression arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Increment, arg);
        }

        public static Flexpression operator --(Flexpression arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Decrement, arg);
        }

        public static Flexpression operator +(Flexpression arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Plus, arg);
        }

        public static Flexpression operator -(Flexpression arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.Minus, arg);
        }

        public static Flexpression operator ~(Flexpression arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.BitwiseComplement, arg);
        }

        public static Flexpression operator !(Flexpression arg)
        {
            return UnaryOperatorFlexpression.Create(OverloadableCodeUnarySymmetricOperator.LogicalNegation, arg);
        }

        public static Flexpression operator *(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mul, left, right);
        }

        public static Flexpression operator /(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Div, left, right);
        }

        public static Flexpression operator %(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mod, left, right);
        }

        public static Flexpression operator +(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sum, left, right);
        }

        public static Flexpression operator -(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sub, left, right);
        }

        public static Flexpression operator &(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.And, left, right);
        }

        public static Flexpression operator ^(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.XOr, left, right);
        }

        public static Flexpression operator |(Flexpression left, IFlexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Or, left, right);
        }

        public static Flexpression operator *(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mul, left, right);
        }

        public static Flexpression operator /(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Div, left, right);
        }

        public static Flexpression operator %(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Mod, left, right);
        }

        public static Flexpression operator +(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sum, left, right);
        }

        public static Flexpression operator -(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Sub, left, right);
        }

        public static Flexpression operator &(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.And, left, right);
        }

        public static Flexpression operator ^(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.XOr, left, right);
        }

        public static Flexpression operator |(IFlexpression left, Flexpression right)
        {
            return BinaryOperatorFlexpression.Create(OverloadableCodeBinarySymmetricOperator.Or, left, right);
        }

        public static implicit operator Flexpression(string x)
        {
            return StringFlexpression.Create(x);
        }

        public static implicit operator Flexpression(char x)
        {
            return CharactersRangeFlexpression.Create(CharactersRangeDefinition.CreateMatching(x));
        }

        public static implicit operator Flexpression(Func<char, bool> x)
        {
            return CharactersRangeFlexpression.Create(CharactersRangeDefinition.CreateMatching(x));
        }

        public static implicit operator Flexpression(StandardCharacterCategories x)
        {
            return CharactersRangeFlexpression.Create(CharactersRangeDefinition.CreateMatching(x));
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
            return HashCode.Combine(LocalId);
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual bool Equals(IFlexpression other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return LocalId.Equals(other.LocalId);
        }

        public static bool operator ==(Flexpression left, Flexpression right) { return Equals(left, right); }
        public static bool operator !=(Flexpression left, Flexpression right) { return !Equals(left, right); }
    }
}