using System;
using System.Collections.Generic;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    public sealed class EmptyParsingResultCache : ICacheParsingResults
    {
        public int CachedResultsCount => 0;

        private readonly HashSet<ulong> _underEvaluation = new HashSet<ulong>();

        public IParsingResultDetails GetParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId)
        {
            var key = ParsingResultCacheKey.EncodeToLong(sourceCodePosition, flexpressionId);

            return _underEvaluation.Contains(key) ? ParsingResultDetails.CalculationInProgress : ParsingResultDetails.Undefined;
        }

        public IParsingResultDetails GetParsingResultOrUpdate(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in Func<IParsingResultDetails> getResultCallback)
        {
            var key = ParsingResultCacheKey.EncodeToLong(sourceCodePosition, flexpressionId);

            if (!_underEvaluation.Add(key))
            {
                return ParsingResultDetails.CalculationInProgress;
            }

            try
            {
                var res = getResultCallback();

                return res ?? ParsingResultDetails.Negative;
            }
            finally
            {
                _underEvaluation.Remove(key);
            }
        }

        public void SetParsingResult(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in IParsingResultDetails result)
        {
        }

        public bool ClearParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId)
        {
            var key = ParsingResultCacheKey.EncodeToLong(sourceCodePosition, flexpressionId);
            return _underEvaluation.Remove(key);
        }

        public void ClearCache()
        {
            _underEvaluation.Clear();
        }
    }
}