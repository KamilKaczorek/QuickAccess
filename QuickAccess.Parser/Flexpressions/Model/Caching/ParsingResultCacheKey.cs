using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuickAccess.DataStructures.Common.ValueContract;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    [StructLayout(LayoutKind.Auto)]
    [Serializable]
    internal sealed class ParsingResultCacheKey : ICanBeUndefined, IEquatable<ParsingResultCacheKey>, IComparable<ParsingResultCacheKey>
    {
        public static readonly ParsingResultCacheKey Empty = new ParsingResultCacheKey(0, FlexpressionId.Undefined);

        private ParsingResultCacheKey(uint sourceCodePosition, FlexpressionId flexpressionId)
        {
            SourceCodePosition = sourceCodePosition;
            FlexpressionId = flexpressionId;
        }

        public uint SourceCodePosition { get; }
        public FlexpressionId FlexpressionId { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerHidden, DebuggerStepThrough]
        internal static ulong EncodeToLong(uint sourceCodePosition, FlexpressionId flexpressionId)
        {
            var id =  flexpressionId.UnderlyingValue;

            if (id == 0)
            {
                return 0;
            }

            ulong pos = sourceCodePosition;
            pos <<= 32;

            return pos | id;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public static ParsingResultCacheKey Create(in uint sourceCodePosition, in FlexpressionId flexpressionId)
        {
            return new ParsingResultCacheKey(sourceCodePosition, flexpressionId);
        }

        public void Deconstruct(out uint sourcePosition, out FlexpressionId flexpressionId)
        {
            sourcePosition = SourceCodePosition;
            flexpressionId = FlexpressionId;
        }

        public bool Equals(ParsingResultCacheKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return SourceCodePosition == other.SourceCodePosition && FlexpressionId.Equals(other.FlexpressionId);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ParsingResultCacheKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SourceCodePosition, FlexpressionId);
        }

        public static bool operator ==(ParsingResultCacheKey left, ParsingResultCacheKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParsingResultCacheKey left, ParsingResultCacheKey right)
        {
            return !Equals(left, right);
        }

        public bool IsDefined => FlexpressionId.IsDefined;

        public int CompareTo(ParsingResultCacheKey other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var sourceCodePositionComparison = SourceCodePosition.CompareTo(other.SourceCodePosition);
            if (sourceCodePositionComparison != 0) return sourceCodePositionComparison;
            return FlexpressionId.CompareTo(other.FlexpressionId);
        }
    }
}