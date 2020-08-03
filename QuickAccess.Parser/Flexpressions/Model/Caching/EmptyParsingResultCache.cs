using System;
using System.Collections.Generic;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    public sealed class EmptyParsingResultCache : ICacheParsingResults
    {
        public int CachedResultsCount => 0;

        private readonly HashSet<ParsingResultCacheKey> _underEvaluation = new HashSet<ParsingResultCacheKey>();

        public IParsingResultDetails GetParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId)
        {
            var key = ParsingResultCacheKey.Create(sourceCodePosition, flexpressionId);

            return _underEvaluation.Contains(key) ? ParsingResultDetails.CalculationInProgress : ParsingResultDetails.Undefined;
        }

        public IParsingResultDetails GetParsingResultOrUpdate(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in Func<IParsingResultDetails> getResultCallback)
        {
            var key = ParsingResultCacheKey.Create(sourceCodePosition, flexpressionId);

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
            var key = ParsingResultCacheKey.Create(sourceCodePosition, flexpressionId);
            return _underEvaluation.Remove(key);
        }

        public void ClearCache()
        {
            _underEvaluation.Clear();
        }
    }
}