using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace QuickAccess.DataStructures.Common.Collections
{
    public static class ReadOnlyArray
    {
        public static ReadOnlyArray<T> Wrap<T>(T[] array)
        {
            return new ReadOnlyArray<T>(array);
        }
    }

    public sealed class ReadOnlyArray<T> : IReadOnlyList<T>
    {
        private readonly T[] _wrapped;

        public ReadOnlyArray(T[] wrapped)
        {
            _wrapped = wrapped;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _wrapped).GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _wrapped).GetEnumerator();
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _wrapped.Length;
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _wrapped.Length;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _wrapped[index];
        }
    }
}