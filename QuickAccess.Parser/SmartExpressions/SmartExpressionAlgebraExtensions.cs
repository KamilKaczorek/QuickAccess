using System.Diagnostics.Contracts;
using QuickAccess.DataStructures.CodeOperatorAlgebra;

namespace QuickAccess.Parser.SmartExpressions
{
    public static class SmartExpressionAlgebraExtensions
    {
        [Pure]
        public static ISmartExpressionAlgebra GetHighestPrioritizedAlgebra(this ISmartExpressionAlgebra defaultAlgebra, SmartExpressionBrick arg1)
        {
            return defaultAlgebra.GetHighestPrioritizedAlgebra<SmartExpressionBrick, ISmartExpressionAlgebra>(arg1?.Algebra);
        }
    }
}