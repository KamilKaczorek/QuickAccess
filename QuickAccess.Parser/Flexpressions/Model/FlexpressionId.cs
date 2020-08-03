using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QuickAccess.Parser.Flexpressions.Model
{
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public readonly struct FlexpressionId : IEquatable<FlexpressionId>, IComparable<FlexpressionId>
    {
        private static readonly object _staticLock = new object();
        public const string UndefinedTag = "";
        public static readonly FlexpressionId Undefined = new FlexpressionId(0);
        private const uint FirstIdValue = 1;
        private static uint _localIdCounter = FirstIdValue;

        [DebuggerStepThrough]
        private static uint GenerateNextIdValue()
        {
            lock (_staticLock)
            {
                if (_localIdCounter == uint.MaxValue)
                {
                    _localIdCounter = FirstIdValue;
                }

                var id = _localIdCounter;
                ++_localIdCounter;

                return id;
            }
        }

        private readonly uint _id;

        public bool IsUndefined => _id == 0;
        public bool IsDefined => _id != 0;

        [Pure]
        public uint UnderlyingValue
        {
            [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
            get => _id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static FlexpressionId Parse(in string fxId)
        {
            if (string.IsNullOrWhiteSpace(fxId))
            {
                return Undefined;
            }

            return uint.TryParse(fxId, out var value)
                ? new FlexpressionId(value)
                : Undefined;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static FlexpressionId Generate()
        {
            var id = GenerateNextIdValue();
            return new FlexpressionId(id);
        }

        private FlexpressionId(uint id)
        {
            _id = id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public bool Equals([NotNull]FlexpressionId other)
        {
            return _id == other._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is FlexpressionId fxId && fxId._id == _id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator ==(in FlexpressionId left, in FlexpressionId right)
        {
            return left._id == right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator !=(in FlexpressionId left, in FlexpressionId right)
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
        public static bool operator <(in FlexpressionId left, in FlexpressionId right)
        {
            return left._id < right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator >(in FlexpressionId left, in FlexpressionId right)
        {
            return left._id > right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator <=(in FlexpressionId left, in FlexpressionId right)
        {
            return left._id <= right._id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static bool operator >=(in FlexpressionId left, in FlexpressionId right)
        {
            return left._id >= right._id;
        }
    }
}