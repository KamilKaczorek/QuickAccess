using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using QuickAccess.Parser.Flexpressions.Bricks;

namespace QuickAccess.Parser.Flexpressions
{
    public static class FlexpressionAlgebraExtensions
    {
        [Pure]
        public static IFlexpressionAlgebra GetHighestPrioritizedAlgebra(this IFlexpressionAlgebra defaultAlgebra, FlexpressionBrick arg1)
        {
            if (defaultAlgebra == null)
            {
                return arg1?.Algebra ?? FXB.DefaultAlgebra;
            }

            if (arg1?.Algebra != null && arg1.Algebra.Priority >= defaultAlgebra.Priority)
            {
                return arg1.Algebra;
            }

            return defaultAlgebra;
        }

        [Pure]
        public static IFlexpressionAlgebra GetHighestPrioritizedAlgebra(this IFlexpressionAlgebra defaultAlgebra, IEnumerable<FlexpressionBrick> args)
        {
            var best = args.Select(p => p?.Algebra).Where(p => p != null).OrderBy(p => p.Priority).LastOrDefault() ?? defaultAlgebra ?? FXB.DefaultAlgebra;


            return defaultAlgebra;
        }

        [Pure]
        public static IFlexpressionAlgebra GetHighestPrioritizedAlgebra(this IFlexpressionAlgebra defaultAlgebra, FlexpressionBrick arg1, FlexpressionBrick arg2)
        {
            var alg = defaultAlgebra.GetHighestPrioritizedAlgebra(arg1).GetHighestPrioritizedAlgebra(arg2);

            return alg;
        }

        [Pure]
        public static IFlexpressionAlgebra GetHighestPrioritizedAlgebra(this IFlexpressionAlgebra defaultAlgebra, FlexpressionBrick arg1, FlexpressionBrick arg2, FlexpressionBrick arg3)
        {
            var alg = defaultAlgebra.GetHighestPrioritizedAlgebra(arg1).GetHighestPrioritizedAlgebra(arg2).GetHighestPrioritizedAlgebra(arg3);

            return alg;
        }
    }
}