using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace QuickAccess.DataStructures.Common.Collections
{
    /// <summary>
    /// Generic collection that guarantees the uniqueness of its elements, as defined
    /// by some comparer. It also supports basic set operations such as Union, Intersection, 
    /// Complement and Exclusive Complement.
    /// </summary>
    public interface IReadOnlySet<T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Get items comparer.
        /// </summary>
        [Pure]
        IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Determines whether this set contains specific <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if set is contains given <paramref name="value"/>, otherwise <c>false</c>.</returns>
        [Pure]
        bool Contains(T value);

        /// <summary>
        /// Check if this set is a subset of other.
        /// </summary>
        /// <param name="other">The sequence of elements from other set.</param>
        /// <returns><c>true</c> if set is a subset of <paramref name="other"/>, otherwise <c>false</c>.</returns>
        [Pure]
        bool IsSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this set is a superset of other.
        /// </summary>
        /// <param name="other">The sequence of elements from other set.</param>
        /// <returns><c>true</c> if set is a superset of <paramref name="other"/>, otherwise <c>false</c>.</returns>
        [Pure]
        bool IsSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this set is a superset of other, but not the same as it.
        /// </summary>
        /// <param name="other">The sequence of elements from other set.</param>
        /// <returns><c>true</c> if set is a proper superset of <paramref name="other"/>, otherwise <c>false</c>.</returns>
        [Pure]
        bool IsProperSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this set is a subset of other, but not the same as it.
        /// </summary>
        /// <param name="other">The sequence of elements from other set.</param>
        /// <returns><c>true</c> if set is a proper subset of <paramref name="other"/>, otherwise <c>false</c>.</returns>
        [Pure]
        bool IsProperSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Check if this set has any elements in common with other.
        /// </summary>
        /// <param name="other">The sequence of elements from other set.</param>
        /// <returns><c>true</c> if set overlaps <paramref name="other"/>, otherwise <c>false</c>.</returns>
        [Pure]
        bool Overlaps(IEnumerable<T> other);

        /// <summary>
        /// Check if this set contains the same and only the same elements as other.
        /// </summary>
        /// <param name="other">The sequence of elements from other set.</param>
        /// <returns><c>true</c> if set equals to <paramref name="other"/>, otherwise <c>false</c>.</returns>
        [Pure]
        bool SetEquals(IEnumerable<T> other);
    }
}