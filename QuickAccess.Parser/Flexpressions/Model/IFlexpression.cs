using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpression<out TConstraint> : IFlexpression
        where TConstraint : IFlexpressionConstraint
    {
    }

    public interface IFlexpression 
        : IEquatable<IFlexpression>
    {
        Type ConstraintType { get; }
        FlexpressionId LocalId { get; }
        string Name { get; }
        TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor);
    }

    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct FlexpressionId : IEquatable<FlexpressionId>, IComparable<FlexpressionId>
    {
        public const string UndefinedTag = "";
        public static FlexpressionId Undefined = new FlexpressionId(0);
        private const int FirstIdValue = 1;
        private static int _localIdCounter = FirstIdValue;

        [DebuggerStepThrough]
        private static int GenerateNextIdValue()
        {
            var id = Interlocked.Increment(ref _localIdCounter);
            return id;
        }

        private readonly int _id;

        public bool IsUndefined => _id == 0;
        public bool IsDefined => _id != 0;

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static FlexpressionId Parse(string fxId)
        {
            if (string.IsNullOrWhiteSpace(fxId))
            {
                return Undefined;
            }

            return int.TryParse(fxId, out var value)
                ? new FlexpressionId(value)
                : Undefined;
        }

        [Pure, DebuggerStepThrough]
        public static FlexpressionId Generate()
        {
            var id = GenerateNextIdValue();
            return new FlexpressionId(id);
        }

        private FlexpressionId(int id)
        {
            _id = id;
        }

        [Pure, DebuggerStepThrough]
        public bool Equals(FlexpressionId other)
        {
            return _id == other._id;
        }

        [Pure, DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is FlexpressionId fxId && fxId._id == _id;
        }

        [Pure, DebuggerStepThrough]
        public override int GetHashCode()
        {
            return _id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator ==(FlexpressionId left, FlexpressionId right)
        {
            return left._id == right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator !=(FlexpressionId left, FlexpressionId right)
        {
            return left._id != right._id;
        }

        [Pure, DebuggerStepThrough]
        public override string ToString()
        {
            var res = IsUndefined ? UndefinedTag : $"{_id}";
            return res;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public int CompareTo(FlexpressionId other)
        {
            return _id.CompareTo(other._id);
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator <(FlexpressionId left, FlexpressionId right)
        {
            return left._id < right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator >(FlexpressionId left, FlexpressionId right)
        {
            return left._id > right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator <=(FlexpressionId left, FlexpressionId right)
        {
            return left._id <= right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator >=(FlexpressionId left, FlexpressionId right)
        {
            return left._id >= right._id;
        }
    }
}