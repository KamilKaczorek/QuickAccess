using System;
using System.Collections;
using System.Collections.Generic;

namespace QuickAccess.Parser.Product
{
    public sealed class SingleSubNode : IReadOnlyList<IParsingProduct>
    {
        private readonly IParsingProduct _node;

        public SingleSubNode(IParsingProduct node)
        {
            _node = node;
        }

        public IEnumerator<IParsingProduct> GetEnumerator()
        {
            yield return _node;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return _node;
        }

        public int Count => 1;

        public IParsingProduct this[int index]
        {
            get
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }

                return _node;
            }
        }
    }
}