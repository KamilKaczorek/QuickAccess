using System;
using System.Collections.Generic;
using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    public sealed class ParsingResultCache : ICacheParsingResults
    {
        public static readonly int DefaultMaxCacheCapacity = 104857600;
        public const int MinCacheCapacity = 1024;

        public int MaxCacheCapacity { get; }

        private readonly Dictionary<ulong, IParsingResultDetails> _cachedResults = new Dictionary<ulong, IParsingResultDetails>(MinCacheCapacity);

        public ParsingResultCache()
        {
            MaxCacheCapacity = DefaultMaxCacheCapacity;
        }

        public ParsingResultCache(in int maxCacheCapacity)
        {
            MaxCacheCapacity = Math.Max(MinCacheCapacity, maxCacheCapacity);
        }

        public int CachedResultsCount => _cachedResults.Count;

        public IParsingResultDetails GetParsingResult(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId)
        {
            if (flexpressionId.IsUndefined)
            {
                return ParsingResultDetails.Undefined;
            }

            var key = ParsingResultCacheKey.EncodeToLong(sourceCodePosition, flexpressionId);

            return _cachedResults.TryGetValue(key, out var result)
                ? result
                : ParsingResultDetails.Undefined;
        }

        public IParsingResultDetails GetParsingResultOrUpdate(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in Func<IParsingResultDetails> getResultCallback) 
        {
            var key = ParsingResultCacheKey.EncodeToLong(sourceCodePosition, flexpressionId);

            if (key == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(flexpressionId), flexpressionId, "Undefined flexpression id.");
            }

            if (_cachedResults.TryGetValue(key, out var res))
            {
                return res;
            }


            if (_cachedResults.Count == MaxCacheCapacity)
            {
                _cachedResults.Clear();
            }

            _cachedResults[key] = ParsingResultDetails.Cached.CalculationInProgress;

            try
            {
                var product = getResultCallback.Invoke();
                res = product ?? ParsingResultDetails.Negative;

                _cachedResults[key] = ParsingResultDetails.ToCached(res);

                return res;
            }
            catch
            {
                _cachedResults[key] = ParsingResultDetails.Cached.CalculationError;
                throw;
            }
        }

        public void SetParsingResult(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in IParsingResultDetails product) 
        {
            Guard.ArgSatisfies(flexpressionId, nameof(flexpressionId), p => p.IsDefined, p => "Undefined flexpression id.");


            var res = product ?? ParsingResultDetails.Cached.Negative;

            var key = ParsingResultCacheKey.EncodeToLong(sourceCodePosition, flexpressionId);

            _cachedResults[key] = ParsingResultDetails.ToCached(res);
        }

        public bool ClearParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId)
        {
            var key = ParsingResultCacheKey.EncodeToLong(sourceCodePosition, flexpressionId);

            var removed = _cachedResults.Remove(key);
            return removed;
        }

        public void ClearCache() { _cachedResults.Clear(); }
    }
}
