using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using QuickAccess.DataStructures.Common.ValueContract;

namespace QuickAccess.DataStructures.Common
{
    [StructLayout(LayoutKind.Auto)]
    [Serializable]
    public readonly struct Quantifier : IEquatable<Quantifier>, ICanBeUndefined, ICanBeEmpty
    {
        public static readonly Quantifier Undefined = new Quantifier(CountValue.Undefined, CountValue.Undefined);
        public static readonly Quantifier Zero = new Quantifier(CountValue.Zero, CountValue.Zero);
        public static readonly Quantifier ZeroOrOne = new Quantifier(CountValue.Zero, CountValue.One);
        public static readonly Quantifier Unlimited = new Quantifier(CountValue.Zero, CountValue.Infinite);
        public static readonly Quantifier AtLeastOne = new Quantifier(CountValue.Zero, CountValue.Infinite);

        [Pure]
        public static Quantifier Create(in int min, in int max)
        {
            if (min == max)
            {
                return Create(min);
            }

            return Create(CountValue.Create(min), CountValue.Create(max));
        }

        [Pure]
        public static Quantifier Create(in int specificCount)
        {
            if (specificCount == 0)
            {
                return Zero;
            }

            return Create(CountValue.Create(specificCount));
        }

        [Pure]
        public static Quantifier Create(in CountValue min, in CountValue max)
        {
            if (min == max)
            {
                return Create(min);
            }

            if (max.IsDefined && max < min)
            {
                throw new ArgumentException("max is smaller than min", nameof(max));
            }

            return new Quantifier(min, max);
        }

        [Pure]
        public static Quantifier Create(in CountValue specificCount)
        {
            if (!specificCount.IsDefined)
            {
                return Undefined;
            }

            if (specificCount.IsZero)
            {
                return Zero;
            }

            return  new Quantifier(specificCount, specificCount);
        }

        public Quantifier(in CountValue min, in CountValue max)
        {
            Min = min;
            Max = max;
        }

        public const long Infinite = long.MaxValue;

        public CountValue Min { get; }
        public CountValue Max { get; }

        public bool IsDefined => Min.IsDefined || Max.IsDefined;

        public bool IsMinAndMaxDefined => Min.IsDefined && Max.IsDefined;
        public bool IsMinAndMaxLimited => Min.IsLimited && Max.IsLimited;

        public void Deconstruct(out CountValue min, out CountValue max)
        {
            min = Min;
            max = Max;
        }

        [Pure]
        public Quantifier ToDefinedRange()
        {
            if (IsMinAndMaxDefined)
            {
                return this;
            }

            if (Min.IsDefined)
            {
                return Create(Min, CountValue.Infinite);
            }

            if (Max.IsDefined)
            {
                return Create(CountValue.Zero, Max);
            }

            return Unlimited;
        }

        [Pure]
        public bool IsInRange(int count)
        {
            if (Min.IsDefined)
            {
                if (Min.IsInfinite)
                {
                    return false;
                }

                if (count < Min.MaxInt)
                {
                    return false;
                }
            }

            if (Max.IsDefined)
            {
                if (Max.IsInfinite)
                {
                    return true;
                }

                if (count > Max.MaxInt)
                {
                    return false;
                }
            }

            return true;
        }

        [Pure]
        public bool IsInRange(uint count)
        {
            if (Min.IsDefined)
            {
                if (Min.IsInfinite)
                {
                    return false;
                }

                if (count < Min.MaxUInt)
                {
                    return false;
                }
            }

            if (Max.IsDefined)
            {
                if (Max.IsInfinite)
                {
                    return true;
                }

                if (count > Max.MaxUInt)
                {
                    return false;
                }
            }

            return true;
        }

        [Pure]
        public bool IsInRange(long count)
        {
            if (Min.IsDefined)
            {
                if (Min.IsInfinite)
                {
                    return false;
                }

                if (count < Min.MaxLong)
                {
                    return false;
                }
            }

            if (Max.IsDefined)
            {
                if (Max.IsInfinite)
                {
                    return true;
                }

                if (count > Max.MaxLong)
                {
                    return false;
                }
            }

            return true;
        }

        [Pure]
        public bool Equals(Quantifier other)
        {
            return Min.Equals(other.Min) && Max.Equals(other.Max);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            return obj is Quantifier other && Equals(other);
        }

        [Pure]
        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public static bool operator ==(Quantifier left, Quantifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Quantifier left, Quantifier right)
        {
            return !left.Equals(right);
        }

        public bool IsEmpty => !IsDefined || (Min.IsZero && Max.IsZero);
    }
}