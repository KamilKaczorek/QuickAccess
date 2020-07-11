using System;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.Parser.Flexpressions.Bricks;
using @Pure = System.Diagnostics.Contracts.PureAttribute;

namespace QuickAccess.Parser.Product
{
    public interface IDefineExpressionBuilderAlgebra<TExpression> : IDefineCodeOperatorSymmetricAlgebra<TExpression>
    {
        TExpression DefineRule(TExpression content, ExpressionTypeDescriptor expressionType, bool isSealed);
        TExpression CreateQuantifierBrick<TDomain>(TExpression content, long min, long max);
        TExpression CreateRulePlaceholder(string ruleName, TExpression defaultExpression);

        TExpression Current { get; }
        TExpression Empty { get; }
    }

    public interface IDefineParserBuilderAlgebra<TExpression> : IDefineExpressionBuilderAlgebra<TExpression>
    {
        TExpression Anything { get; }
        TExpression WhiteSpace { get; }
        TExpression WhiteSpaceOrNewLine { get; }
        TExpression OptionalWhiteSpace { get; }
        TExpression OptionalWhiteSpaceOrNewLine { get; }
        TExpression CustomSequence { get; }
        TExpression NewLine { get; }
        TExpression Letter { get; }
        TExpression UpperLetter { get; }
        TExpression LowerLetter { get; }
        TExpression Symbol { get; }
        TExpression Digit { get; }
    }

    /// <summary>
    /// Extensions of <see cref="IDefineExpressionBuilderAlgebra{TDomain}"/> 
    /// </summary>
    public static class ExpressionBuilderAlgebraDefinitionExtensions
    {
        public static TExpression DefineRule<TExpression>(this IDefineExpressionBuilderAlgebra<TExpression> source, TExpression content, ExpressionTypeDescriptor expressionType)
        {
            return source.DefineRule(content, expressionType, false);
        }

        public static TExpression DefineSealedRule<TExpression>(this IDefineExpressionBuilderAlgebra<TExpression> source, TExpression content,
            ExpressionTypeDescriptor expressionType)
        {
            return source.DefineRule(content, expressionType, true);
        }
    }

    public interface IExpressionTypeDescriptor : IEquatable<IExpressionTypeDescriptor>
    {
        /// <summary>
        /// Gets the identifier of the type of an expression.
        /// </summary>
        /// <value>
        /// The expression type identifier.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the value type identifier.
        /// </summary>
        /// <value>
        /// The value type identifier.
        /// </value>
        string ValueTypeId { get; }

        /// <summary>
        /// Gets value indicating whether specific product defines expression class.
        /// Expression class is defined by <see cref="CapturingGroupBrick<TDomain>"/>.
        /// </summary>
        bool DefinesExpressionClass { get; }

        /// <summary>
        /// Gets value indicating whether specific descriptor describes empty expression type.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the clone of current descriptor replacing <see cref="Name"/> to the one given by the parameter.
        /// </summary>
        /// <param name="name">The name of the cloned descriptor.</param>
        /// <returns>Cloned descriptor instance with replaced name or the same as current instance if the name is not different.</returns>
        [NotNull, @Pure]
        IExpressionTypeDescriptor WithName(string name);

        /// <summary>
        /// Gets the clone of current descriptor replacing <see cref="ValueTypeId"/> to the one given by the parameter.
        /// </summary>
        /// <param name="valueTypeId">The value type id of the cloned descriptor.</param>
        /// <returns>Cloned descriptor instance with replaced type id or the same as current instance if the type id is not different.</returns>
        [NotNull, @Pure]
        IExpressionTypeDescriptor WithValueType(string valueTypeId);

        /// <summary>
        /// Gets the clone of current descriptor replacing value of <see cref="DefinesExpressionClass"/> to the one given by the parameter.
        /// </summary>
        /// <param name="definesExpressionClass">The value of the <see cref="DefinesExpressionClass"/> of the cloned descriptor.</param>
        /// <returns>Cloned descriptor instance with replaced value or the same as current instance if the value is not different.</returns>
        [NotNull, @Pure]
        IExpressionTypeDescriptor WithDefinesExpressionClass(bool definesExpressionClass);
    }

    [Serializable]
    public sealed class ExpressionTypeDescriptor : IExpressionTypeDescriptor
    {
        public const string EmptyName = "_EMPTY_";
        public const string UndefinedName = "_UNDEF_";

        private static int _anonymousCounter = 0x10;
        public static string GetAnonymousName() => $"_ANON_{Interlocked.Increment(ref _anonymousCounter):X}_";

        public static readonly ExpressionTypeDescriptor Empty = new ExpressionTypeDescriptor(EmptyName, null, false, true);
        public static readonly ExpressionTypeDescriptor Undefined = new ExpressionTypeDescriptor(UndefinedName, null, false, false);

        
        [NotNull, @Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionTypeDescriptor Create(string name, string valueTypeId = null, bool definesExpressionClass = false)
        {
            bool isEmpty;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = GetAnonymousName();
                isEmpty = false;
            }
            else
            {
                isEmpty = string.Equals(name, EmptyName, StringComparison.Ordinal);

                if (isEmpty)
                {
                    Guard.ArgNull(valueTypeId, nameof(valueTypeId));
                    Guard.ArgFalse(definesExpressionClass, nameof(definesExpressionClass));
                }
            }

            return new ExpressionTypeDescriptor(name, valueTypeId, definesExpressionClass, isEmpty);
        }

        [NotNull, @Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ExpressionTypeDescriptor CreateExpressionClass(string name, string valueTypeId = null)
        {
            return Create(name, valueTypeId, true);
        }

        /// <summary>
        /// Gets the identifier of the type of an expression.
        /// </summary>
        /// <value>
        /// The expression type identifier.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the value type identifier.
        /// </summary>
        /// <value>
        /// The value type identifier.
        /// </value>
        public string ValueTypeId { get; }

        /// <summary>
        /// Gets value indicating whether specific product defines expression class.
        /// Expression class is defined by <see cref="CapturingGroupBrick<TDomain>"/>.
        /// </summary>
        public bool DefinesExpressionClass { get; }

        /// <summary>
        /// Gets value indicating whether specific descriptor describes empty expression type.
        /// </summary>
        public bool IsEmpty { get; }

        private ExpressionTypeDescriptor(string name, string valueTypeId, bool definesExpressionClass, bool isEmpty)
        {
            Name = name;
            ValueTypeId = valueTypeId;
            DefinesExpressionClass = definesExpressionClass;
            IsEmpty = isEmpty;
        }

        [NotNull, @Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IExpressionTypeDescriptor WithName(string name)
        {
            var useSame = string.Equals(name, Name, StringComparison.Ordinal);

            return useSame
                ? this
                : ExpressionTypeDescriptor.Create(name, ValueTypeId, DefinesExpressionClass);
        }

        [NotNull, @Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IExpressionTypeDescriptor WithValueType(string valueTypeId)
        {
            var useSame = string.Equals(valueTypeId, ValueTypeId, StringComparison.Ordinal);

            return useSame
                ? this
                : ExpressionTypeDescriptor.Create(Name, valueTypeId, DefinesExpressionClass);
        }

        [NotNull, @Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IExpressionTypeDescriptor WithDefinesExpressionClass(bool definesExpressionClass)
        {
            var useSame = definesExpressionClass == DefinesExpressionClass;

            return useSame
                ? this
                : ExpressionTypeDescriptor.Create(Name, ValueTypeId, definesExpressionClass);
        }

        /// <summary>
        /// Gets the string representation of the current descriptor formatted as:
        /// "&lt;<see cref="Name"/>:<see cref="ValueTypeId"/>&gt;"; for class expression
        /// or "(<see cref="Name"/>:<see cref="ValueTypeId"/>)" for non class expression.
        /// </summary>
        /// <returns>Descriptor as text.</returns>
        [@Pure]
        public override string ToString()
        {
            var res = string.IsNullOrEmpty(ValueTypeId) ? $"{Name}" : $"{Name}:{ValueTypeId}";

            return DefinesExpressionClass ? $"<{res}>" : $"({res})";
        }

        [@Pure]
        public bool Equals(IExpressionTypeDescriptor other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && ValueTypeId == other.ValueTypeId && DefinesExpressionClass == other.DefinesExpressionClass;
        }

        [@Pure]
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ExpressionTypeDescriptor other && Equals(other);
        }

        [@Pure]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ValueTypeId != null ? ValueTypeId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DefinesExpressionClass.GetHashCode();
                return hashCode;
            }
        }

        [@Pure]
        public static bool operator ==(ExpressionTypeDescriptor left, ExpressionTypeDescriptor right)
        {
            return Equals(left, right);
        }

        [@Pure]
        public static bool operator !=(ExpressionTypeDescriptor left, ExpressionTypeDescriptor right)
        {
            return !Equals(left, right);
        }

        [@Pure]
        public static bool operator ==(ExpressionTypeDescriptor left, IExpressionTypeDescriptor right)
        {
            return Equals(left, right);
        }

        [@Pure]
        public static bool operator !=(ExpressionTypeDescriptor left, IExpressionTypeDescriptor right)
        {
            return !Equals(left, right);
        }

        [@Pure]
        public static bool operator ==(IExpressionTypeDescriptor left, ExpressionTypeDescriptor right)
        {
            return Equals(left, right);
        }

        [@Pure]
        public static bool operator !=(IExpressionTypeDescriptor left, ExpressionTypeDescriptor right)
        {
            return !Equals(left, right);
        }
    }
}