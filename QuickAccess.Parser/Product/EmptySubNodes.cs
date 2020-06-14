using System;
using System.Collections;
using System.Collections.Generic;

namespace QuickAccess.Parser.Product
{
    public sealed class EmptySubNodes : IReadOnlyList<IParsingProduct>
    {
        public static EmptySubNodes Instance = new EmptySubNodes();

        private EmptySubNodes()
        {
        }

        public IEnumerator<IParsingProduct> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }

        public int Count => 0;

        public IParsingProduct this[int index] => throw new IndexOutOfRangeException();
    }
}