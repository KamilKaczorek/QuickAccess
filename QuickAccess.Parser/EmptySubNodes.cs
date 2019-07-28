using System;
using System.Collections;
using System.Collections.Generic;

namespace QuickAccess.Parser
{
    public sealed class EmptySubNodes : IReadOnlyList<IParsedExpressionNode>
    {
        public static EmptySubNodes Instance = new EmptySubNodes();

        private EmptySubNodes()
        {
        }

        public IEnumerator<IParsedExpressionNode> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }

        public int Count => 0;

        public IParsedExpressionNode this[int index] => throw new IndexOutOfRangeException();
    }
}