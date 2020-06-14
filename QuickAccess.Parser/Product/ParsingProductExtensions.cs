using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;

namespace QuickAccess.Parser.Product
{
    public static class ParsingProductExtensions
    {
        [Pure]
        public static IReadOnlyList<IParsingProduct> ToList(this IParsingProduct source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.ProductType == ParsingProductType.Empty)
            {
                return EmptySubNodes.Instance;
            }

            return new SingleSubNode(source);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<IParsingProduct> Concatenate(this IParsingProduct source, params IParsingProduct[] other)
        {
            return Concatenate(source.ToList(), (IReadOnlyCollection<IParsingProduct>)other);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IParsingProduct> WhereNotEmpty(this IEnumerable<IParsingProduct> source)
        {
            return source.Where(pX => pX.ProductType != ParsingProductType.Empty);
        }

        [Pure]
        public static IReadOnlyList<IParsingProduct> WithoutEmptyProducts(
            this IReadOnlyCollection<IParsingProduct> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var count = source.WhereNotEmpty().Count();

            if (count <= 0)
            {
                return EmptySubNodes.Instance;
            }

            if (count == 1)
            {
                return new SingleSubNode(source.WhereNotEmpty().Single());
            }

            var items = new IParsingProduct[count];

            var idx = 0;
            foreach (var x in source.WhereNotEmpty())
            {
                items[idx] = x;
                idx++;
            }

            return items;
        }

        [Pure]
        public static IReadOnlyList<IParsingProduct> Concatenate(this IReadOnlyCollection<IParsingProduct> source,
            IReadOnlyCollection<IParsingProduct> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var count = source.WhereNotEmpty().Count() + other.WhereNotEmpty().Count();

            if (count <= 0)
            {
                return EmptySubNodes.Instance;
            }

            if (count == 1)
            {
                return new SingleSubNode(source.WhereNotEmpty().SingleOrDefault() ?? other.WhereNotEmpty().Single());
            }

            var items = new IParsingProduct[count];

            var idx = 0;
            foreach (var x in source.WhereNotEmpty())
            {
                items[idx] = x;
                idx++;
            }

            foreach (var y in other.WhereNotEmpty())
            {
                items[idx] = y;
                idx++;
            }

            return items;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<IParsingProduct> Concatenate(this IReadOnlyCollection<IParsingProduct> source, params IParsingProduct[] other)
        {
            return Concatenate(source, (IReadOnlyCollection<IParsingProduct>)other);
        }
    }
}