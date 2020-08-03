using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using QuickAccess.DataStructures.Common.Guards;
using QuickAccess.DataStructures.MultiDictionaries;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    //*
    public sealed class ParsingResultCache : ICacheParsingResults
    {
        public static readonly int DefaultMaxCacheCapacity = 104857600;
        public const int MinCacheCapacity = 1024;

        public int MaxCacheCapacity { get; }

        private readonly Dictionary<uint, Dictionary<FlexpressionId, IParsingResultDetails>> _cachedResults = new Dictionary<uint, Dictionary<FlexpressionId, IParsingResultDetails>>();
        public ParsingResultCache()
        {
            MaxCacheCapacity = DefaultMaxCacheCapacity;
        }

        public ParsingResultCache(int maxCacheCapacity)
        {
            MaxCacheCapacity = Math.Max(MinCacheCapacity, maxCacheCapacity);
        }

        public int CachedResultsCount => _cachedResults.Count;

        IParsingResultDetails ICacheParsingResults.GetParsingResult(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId)
        {
            if (flexpressionId.IsUndefined)
            {
                return ParsingResultDetails.Undefined;
            }

            return _cachedResults.TryGetInnerValue(sourceCodePosition, flexpressionId, out var result)
                ? result
                : ParsingResultDetails.Undefined;
        }

        public IParsingResultDetails GetParsingResultOrUpdate(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in Func<IParsingResultDetails> getResultCallback) 
        {
            Guard.ArgSatisfies(flexpressionId, nameof(flexpressionId), p => p.IsDefined, p => "Undefined flexpression id.");

            var parsingResult = _cachedResults.TryGetInnerValue(sourceCodePosition, flexpressionId, out var res) 
                ? res 
                : AddToCache(sourceCodePosition, flexpressionId, getResultCallback);

            return parsingResult;
        }

        public void SetParsingResult(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in IParsingResultDetails product) 
        {
            Guard.ArgSatisfies(flexpressionId, nameof(flexpressionId), p => p.IsDefined, p => "Undefined flexpression id.");

            var res = product ?? ParsingResultDetails.Cached.Negative;
            AddToCache(sourceCodePosition, flexpressionId, res);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddToCache(uint sourceCodePosition, FlexpressionId flexpressionId, IParsingResultDetails res)
        {
            _cachedResults.SetInnerValue(sourceCodePosition, flexpressionId, ParsingResultDetails.ToCached(res));
        }

        public bool ClearParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId)
        {
            var removed = _cachedResults.RemoveInner(sourceCodePosition, flexpressionId, true);
            return removed;
        }

        public void ClearCache() { _cachedResults.Clear(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParsingResultDetails AddToCache(in uint srcPos, in FlexpressionId flexpressionId, in Func<IParsingResultDetails> getResultCallback)
        {
            if (_cachedResults.Count == MaxCacheCapacity)
            {
                _cachedResults.Clear();
            }

            AddToCache(srcPos, flexpressionId, ParsingResultDetails.Cached.CalculationInProgress);

            try
            {
                var product = getResultCallback.Invoke();
                var res = product ?? ParsingResultDetails.Negative;

                AddToCache(srcPos, flexpressionId, res);

                return res;
            }
            catch
            {
                AddToCache(srcPos, flexpressionId, ParsingResultDetails.Cached.CalculationError);
                throw;
            }
        }
    }

    /*/

    public sealed class ParsingResultCache : ICacheParsingResults
    {
        public static readonly int DefaultMaxCacheCapacity = 104857600;
        public const int MinCacheCapacity = 1024;

        public int MaxCacheCapacity { get; }


        private readonly Dictionary<ulong, IParsingResultProductPair> _cachedResults = new Dictionary<ulong, IParsingResultProductPair>(MinCacheCapacity);

        public ParsingResultCache()
        {
            MaxCacheCapacity = DefaultMaxCacheCapacity;
        }

        public ParsingResultCache(int maxCacheCapacity)
        {
            MaxCacheCapacity = Math.Max(MinCacheCapacity, maxCacheCapacity);
        }

        public int CachedResultsCount => _cachedResults.Count;

        IParsingResultProductPair ICacheParsingResults.GetParsingResult(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId)
        {
            if (flexpressionId.IsUndefined)
            {
                return ParsingResultDetails.Undefined;
            }

            var key = ParsingResultCacheKey.GetFromCodePosAndId(sourceCodePosition, flexpressionId);

            return _cachedResults.TryGetValue(key, out var result)
                ? result
                : ParsingResultDetails.Undefined;
        }

        public IParsingResultProductPair GetParsingResultOrUpdate<TProduct>(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in Func<TProduct> getResultCallback) where TProduct : class
        {
            Guard.ArgSatisfies(flexpressionId, nameof(flexpressionId), p => p.IsDefined, p => "Undefined flexpression id.");

            var key = ParsingResultCacheKey.GetFromCodePosAndId(sourceCodePosition, flexpressionId);

            var parsingResult = _cachedResults.TryGetValue(key, out var res) ? res : AddToCache(key, getResultCallback);
            return parsingResult;
        }

        public void SetParsingResult<TProduct>(
            in uint sourceCodePosition,
            in FlexpressionId flexpressionId,
            in TProduct product) where TProduct : class
        {
            Guard.ArgSatisfies(flexpressionId, nameof(flexpressionId), p => p.IsDefined, p => "Undefined flexpression id.");
            var key = ParsingResultCacheKey.GetFromCodePosAndId(sourceCodePosition, flexpressionId);

            var res = product != null ? ParsingResultDetails.CreateSuccessful(product) : ParsingResultDetails.Negative;
            _cachedResults[key] = res;
        }

        public bool ClearParsingResult(in uint sourceCodePosition, in FlexpressionId flexpressionId)
        {
            var key = ParsingResultCacheKey.GetFromCodePosAndId(sourceCodePosition, flexpressionId);
            var removed = _cachedResults.Remove(key);
            return removed;
        }

        public void ClearCache() { _cachedResults.Clear(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IParsingResultProductPair AddToCache<TProduct>(in ulong key, in Func<TProduct> getResultCallback)
        where TProduct : class
        {
            if (_cachedResults.Count == MaxCacheCapacity)
            {
                _cachedResults.Clear();
            }

            _cachedResults[key] = ParsingResultDetails.CalculationInProgress;

            try
            {
                var product = getResultCallback.Invoke();
                var res = product != null ? ParsingResultDetails.CreateSuccessful(product): ParsingResultDetails.Negative;
                _cachedResults[key] = res;
                return res;
            }
            catch
            {
                _cachedResults[key] = ParsingResultDetails.CalculationError;
                throw;
            }
        }
    }
    //*/

}
