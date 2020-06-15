using System;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.Parser.Flexpressions.Bricks;
using @Pure = System.Diagnostics.Contracts.PureAttribute;

namespace QuickAccess.Parser.Product
{
    [Serializable]
    public sealed class ExpressionTypeDescriptor : IEquatable<ExpressionTypeDescriptor>
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
        /// Expression class is defined by <see cref="CapturingGroupBrick"/>.
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

        [@Pure]
        public ExpressionTypeDescriptor WithName(string name)
        {
            var useSame = string.Equals(name, Name, StringComparison.Ordinal);

            return useSame
                ? this
                : ExpressionTypeDescriptor.Create(name, ValueTypeId, DefinesExpressionClass);
        }

        [@Pure]
        public ExpressionTypeDescriptor WithValueType(string valueTypeId)
        {
            var useSame = string.Equals(valueTypeId, ValueTypeId, StringComparison.Ordinal);

            return useSame
                ? this
                : ExpressionTypeDescriptor.Create(Name, valueTypeId, DefinesExpressionClass);
        }

        [@Pure]
        public ExpressionTypeDescriptor WithDefinesExpressionClass(bool definesExpressionClass)
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
        public bool Equals(ExpressionTypeDescriptor other)
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
    }
}