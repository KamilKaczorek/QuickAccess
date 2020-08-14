using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using QuickAccess.DataStructures.Common.ValueContract;

namespace QuickAccess.DataStructures.Common
{
    [StructLayout(LayoutKind.Auto)]
    [Serializable]
    public readonly struct CountValue : ICanBeUndefined, IEquatable<CountValue>, IComparable<CountValue>, IComparable
    {
        private const uint InfiniteValue = uint.MaxValue;
        private const uint UndefinedValue = 0;
        private const uint ZeroValue = 1;
        private const uint OneValue = 2;
        

        public const string UndefinedString = "NAN";
        public const string InfiniteString = "INF";

        public static readonly CountValue Infinite = new CountValue(InfiniteValue);
        public static readonly CountValue Undefined = new CountValue(UndefinedValue);
        public static readonly CountValue Zero = new CountValue(ZeroValue);
        public static readonly CountValue One = new CountValue(OneValue);

        private readonly uint _valuePlusOne;
        public static CountValue Create(int count) => new CountValue(count);

        public static CountValue Parse(string text, StringComparison comparison = StringComparison.Ordinal)
        {
            if (string.Equals(UndefinedString, text, comparison))
            {
                return Undefined;
            }

            if (string.Equals(InfiniteString, text, comparison))
            {
                return Infinite;
            }

            var count = int.Parse(text);

            return Create(count);
        }

        public static bool TryParse(string text, out CountValue value, StringComparison comparison = StringComparison.Ordinal)
        {
            if (string.Equals(UndefinedString, text, comparison))
            {
                value = Undefined;
                return true;
            }

            if (string.Equals(InfiniteString, text, comparison))
            {
                value = Infinite;
                return true;
            }

            if (int.TryParse(text, out var count))
            {
                value = Create(count);
                return true;
            }

            value = Undefined;
            return false;
        }

        private CountValue(in int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be equal or greater than 0 but is {count}");
            }

            _valuePlusOne = (uint)count + 1;
        }

        private CountValue(uint valuePlusOne)
        {
            _valuePlusOne = valuePlusOne;
        }

        public bool IsInfinite => _valuePlusOne == InfiniteValue;
        public bool IsZero => _valuePlusOne == ZeroValue;
        public bool IsDefined => _valuePlusOne != UndefinedValue;
        public bool IsLimited => _valuePlusOne != InfiniteValue && _valuePlusOne != UndefinedValue;

        public int MaxInt
        {
            get
            {
                if (_valuePlusOne == UndefinedValue)
                {
                    throw new InvalidOperationException("Can't provide count value: the count value is undefined.");
                }

                if (_valuePlusOne == InfiniteValue)
                {
                    return int.MaxValue;
                }

                return (int) (_valuePlusOne - 1);
            }
        }

        public uint MaxUInt
        {
            get
            {
                if (_valuePlusOne == UndefinedValue)
                {
                    throw new InvalidOperationException("Can't provide count value: the count value is undefined.");
                }

                if (_valuePlusOne == InfiniteValue)
                {
                    return uint.MaxValue;
                }

                return _valuePlusOne - 1;
            }
        }

        public long MaxLong
        {
            get
            {
                if (_valuePlusOne == UndefinedValue)
                {
                    throw new InvalidOperationException("Can't provide count value: the count value is undefined.");
                }

                if (_valuePlusOne == InfiniteValue)
                {
                    return long.MaxValue;
                }

                return (_valuePlusOne - 1);
            }
        }

        public ulong MaxULong
        {
            get
            {
                if (_valuePlusOne == UndefinedValue)
                {
                    throw new InvalidOperationException("Can't provide count value: the count value is undefined.");
                }

                if (_valuePlusOne == InfiniteValue)
                {
                    return ulong.MaxValue;
                }

                return (_valuePlusOne - 1);
            }
        }


        [Pure]
        public Quantifier RangeTo(CountValue max)
        {
            return Quantifier.Create(this, max);
        }

        public override string ToString()
        {
            if (_valuePlusOne == UndefinedValue)
            {
                return UndefinedString;
            }

            if (_valuePlusOne == InfiniteValue)
            {
                return InfiniteString;
            }

            return $"{_valuePlusOne-1}";
        }


        public bool Equals(CountValue other)
        {
            return _valuePlusOne == other._valuePlusOne;
        }

        public override bool Equals(object obj)
        {
            return obj is CountValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int) _valuePlusOne;
        }

        public static bool operator ==(CountValue left, CountValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CountValue left, CountValue right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(CountValue other)
        {
            return _valuePlusOne.CompareTo(other._valuePlusOne);
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            return obj is CountValue other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(CountValue)}");
        }

        public static bool operator <(CountValue left, CountValue right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(CountValue left, CountValue right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(CountValue left, CountValue right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(CountValue left, CountValue right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}