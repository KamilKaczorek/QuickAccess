using System.Diagnostics.Contracts;
using QuickAccess.DataStructures.CodeOperatorAlgebra;
using QuickAccess.Parser.Flexpressions.Bricks;

namespace QuickAccess.Parser.Flexpressions
{
    public static class FlexpressionAlgebraExtensions
    {
        [Pure]
        public static IFlexpressionAlgebra GetHighestPrioritizedAlgebra(this IFlexpressionAlgebra defaultAlgebra, FlexpressionBrick arg1)
        {
            return defaultAlgebra.GetHighestPrioritizedAlgebra<FlexpressionBrick, IFlexpressionAlgebra>(arg1?.Algebra);
        }
    }
}