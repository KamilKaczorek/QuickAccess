using System.Threading;

namespace QuickAccess.Parser.Flexpressions.Model
{
    internal static class FlexpressionLocalIdGenerator
    {
        private static long _localIdCounter = 0;

        public static long GetNext()
        {
            var id = Interlocked.Increment(ref _localIdCounter);

            return id;
        }
    }
}