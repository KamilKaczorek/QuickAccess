using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickAccess.DataStructures.Common.Collections
{
    public static class ReadOnlySet
    {
        [Pure]
        public static IReadOnlySet<T> Wrap<T>(HashSet<T> actualSet)
        {
            return new ReadOnlySet<T>(actualSet);
        }

        [Pure]
        public static IReadOnlySet<T> Create<T>(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            return new ReadOnlySet<T>(new HashSet<T>(items, comparer));
        }

        [Pure]
        public static IReadOnlySet<T> Create<T>(IEnumerable<T> items)
        {
            return new ReadOnlySet<T>(new HashSet<T>(items));
        }
    }

    public class ReadOnlySet<T> : IReadOnlySet<T>
    {
        private readonly HashSet<T> _actualSet;

        public ReadOnlySet(HashSet<T> actualSet)
        {
            _actualSet = actualSet;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _actualSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _actualSet).GetEnumerator();
        }

        public int Count => _actualSet.Count;

        public IEqualityComparer<T> Comparer => _actualSet.Comparer;

        public bool Contains(T value)
        {
            return _actualSet.Contains(value);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _actualSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _actualSet.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _actualSet.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _actualSet.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return _actualSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return _actualSet.SetEquals(other);
        }
    }
}