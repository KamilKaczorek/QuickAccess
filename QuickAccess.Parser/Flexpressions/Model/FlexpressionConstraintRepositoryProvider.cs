using System.Runtime.CompilerServices;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public static class FlexpressionConstraintRepositoryProvider
    {
        private static readonly IFlexpressionConstraintRepository FlexpressionConstraintRepository = new FlexpressionConstraintRepository();

        public static IFlexpressionConstraintRepository Repository
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => FlexpressionConstraintRepository;
        }
    }
}