using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace QuickAccess.Infrastructure
{
    public static class EnumExtensions
    {
        [Pure]
        public static FlagCheckResult EvaluateContainsFlags(this Enum source, Enum flags)
        {
            var typeCode = source.GetTypeCode();

            switch (typeCode)
            {
                case TypeCode.Byte:
                    return EvaluateContainsInt((byte)(object)source, (byte)(object)flags);
                case TypeCode.SByte:
                    return EvaluateContainsInt((sbyte)(object)source, (sbyte)(object)flags);
                case TypeCode.Int16:
                    return EvaluateContainsInt((short)(object)source, (short)(object)flags);
                case TypeCode.UInt16:
                    return EvaluateContainsInt((ushort)(object)source, (ushort)(object)flags);
                case TypeCode.Int32:
                    return EvaluateContainsInt((int)(object)source, (int)(object)flags);
                case TypeCode.Int64:
                    return EvaluateContainsLong((long)(object)source, (long)(object)flags);
                case TypeCode.UInt32:
                    return EvaluateContainsUInt((uint)(object)source, (uint)(object)flags);
                case TypeCode.UInt64:
                    return EvaluateContainsULong((ulong)(object)source, (ulong)(object)flags);
                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FlagCheckResult EvaluateContainsInt(int srcInt, int flagsInt)
        {
            var res = srcInt & flagsInt;

            return res == 0
                ? FlagCheckResult.None
                : res == flagsInt
                    ? FlagCheckResult.All
                    : FlagCheckResult.NotAll;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FlagCheckResult EvaluateContainsUInt(uint srcInt, uint flagsInt)
        {
            var res = srcInt & flagsInt;

            return res == 0
                ? FlagCheckResult.None
                : res == flagsInt
                    ? FlagCheckResult.All
                    : FlagCheckResult.NotAll;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FlagCheckResult EvaluateContainsLong(long srcInt, long flagsInt)
        {
            var res = srcInt & flagsInt;

            return res == 0
                ? FlagCheckResult.None
                : res == flagsInt
                    ? FlagCheckResult.All
                    : FlagCheckResult.NotAll;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FlagCheckResult EvaluateContainsULong(ulong srcInt, ulong flagsInt)
        {
            var res = srcInt & flagsInt;

            return res == 0
                ? FlagCheckResult.None
                : res == flagsInt
                    ? FlagCheckResult.All
                    : FlagCheckResult.NotAll;
        }
    }
}