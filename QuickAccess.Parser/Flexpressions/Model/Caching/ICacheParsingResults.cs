using System;
using System.Diagnostics.Contracts;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    public interface ICacheParsingResults
    {
        int CachedResultsCount { get; }

        [Pure]
        IParsingResultDetails GetParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId);

        IParsingResultDetails GetParsingResultOrUpdate(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in Func<IParsingResultDetails> getResultCallback);

        void SetParsingResult(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in IParsingResultDetails result);

        bool ClearParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId);
        void ClearCache();
    }
}