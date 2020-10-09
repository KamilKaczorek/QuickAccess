using System.Collections.Generic;
using System.Linq;

namespace QuickAccess.Infrastructure.RegularExpression
{
    public static class RegularExpressionFactoryExtensions
    {
        public static string CharsToRegex(this IRegularExpressionFactory source, IEnumerable<char> chars)
        {
            return string.Join(string.Empty, chars.Select(source.CharToRegex));
        }
    }
}