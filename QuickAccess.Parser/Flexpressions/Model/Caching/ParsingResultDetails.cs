using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    public static class ParsingResultDetails
    {
        public static readonly IParsingResultDetails Undefined = new UndefinedPair();
        public static readonly IParsingResultDetails CalculationError = new Unsuccessful(ParsingEvaluationResult.CalculationError);
        public static readonly IParsingResultDetails CalculationInProgress = new Unsuccessful(ParsingEvaluationResult.CalculationInProgress);
        public static readonly IParsingResultDetails Negative = new Unsuccessful(ParsingEvaluationResult.Negative);

        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        internal static class Cached
        {
            public static readonly IParsingResultDetails Undefined = new UndefinedPairCached();
            public static readonly IParsingResultDetails CalculationError = new UnsuccessfulCached(ParsingEvaluationResult.CalculationError);
            public static readonly IParsingResultDetails CalculationInProgress = new UnsuccessfulCached(ParsingEvaluationResult.CalculationInProgress);
            public static readonly IParsingResultDetails Negative = new UnsuccessfulCached(ParsingEvaluationResult.Negative);
        }


        [Pure]
        internal static IParsingResultDetails ToCached(IParsingResultDetails notCached)
        {
            if (notCached == null)
            {
                return null;
            }

            if (notCached.IsFromCache)
            {
                return notCached;
            }

            switch (notCached.Result)
            {
                case ParsingEvaluationResult.Undefined:
                    return Cached.Undefined;
                case ParsingEvaluationResult.CalculationInProgress:
                    return Cached.CalculationInProgress;
                case ParsingEvaluationResult.CalculationError:
                    return Cached.CalculationError;
                case ParsingEvaluationResult.Negative:
                    return Cached.Negative;
                case ParsingEvaluationResult.Positive:
                    return new SuccessfulPairCached(notCached.Product, notCached.ParsedCharactersCount);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public static IParsingResultDetails CreateSuccessful(object product, int parsedCharactersCount)
        {
            return new SuccessfulPair(product, parsedCharactersCount);
        }

        public static IParsingResultDetails CreateUnsuccessful(ParsingEvaluationResult result)
        {
            switch (result)
            {
                case ParsingEvaluationResult.Undefined:
                    return Undefined;
                case ParsingEvaluationResult.CalculationInProgress:
                    return CalculationInProgress;
                case ParsingEvaluationResult.CalculationError:
                    return CalculationError;
                case ParsingEvaluationResult.Negative:
                    return Negative;
                case ParsingEvaluationResult.Positive:
                    throw new ArgumentException($"Can't initialize unsuccessful {nameof(ParsingResultDetails)} instance with {result} result.", nameof(result));
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private sealed class UndefinedPair : IParsingResultDetails
        {
            public bool IsDefined => false;
            public object Product => null;
            public int ParsedCharactersCount => 0;
            public ParsingEvaluationResult Result => ParsingEvaluationResult.Undefined;
            public bool IsSuccessful => false;
            public bool IsFromCache => false;
        }

        private sealed class Unsuccessful : IParsingResultDetails
        {
            public Unsuccessful(ParsingEvaluationResult result)
            {
                Result = result; 
            }

            public bool IsDefined => true;
            public object Product => null;
            public int ParsedCharactersCount => 0;
            public ParsingEvaluationResult Result { get; }
            public bool IsSuccessful => false;
            public bool IsFromCache => false;
        }

        private sealed class SuccessfulPair : IParsingResultDetails
        {
            public SuccessfulPair(object product, int parsedCharactersCount)
            {
                Product = product;
                ParsedCharactersCount = parsedCharactersCount;
            }

            public object Product { get; }
            public int ParsedCharactersCount { get; }
            public ParsingEvaluationResult Result => ParsingEvaluationResult.Positive;

            public bool IsSuccessful => true;
            public bool IsFromCache => false;
            public bool IsDefined => true;
        }

        private sealed class UndefinedPairCached : IParsingResultDetails
        {
            public bool IsDefined => false;
            public object Product => null;
            public int ParsedCharactersCount => 0;
            public ParsingEvaluationResult Result => ParsingEvaluationResult.Undefined;
            public bool IsSuccessful => false;
            public bool IsFromCache => true;
        }

        private sealed class UnsuccessfulCached : IParsingResultDetails
        {
            public UnsuccessfulCached(ParsingEvaluationResult result)
            {
                Result = result;
            }

            public bool IsDefined => true;
            public object Product => null;
            public int ParsedCharactersCount => 0;
            public ParsingEvaluationResult Result { get; }
            public bool IsSuccessful => false;
            public bool IsFromCache => true;
        }

        private sealed class SuccessfulPairCached : IParsingResultDetails
        {
            public SuccessfulPairCached(object product, int parsedCharactersCount)
            {
                Product = product;
                ParsedCharactersCount = parsedCharactersCount;
            }

            public object Product { get; }
            public int ParsedCharactersCount { get; }
            public ParsingEvaluationResult Result => ParsingEvaluationResult.Positive;

            public bool IsSuccessful => true;
            public bool IsFromCache => true;
            public bool IsDefined => true;
        }
    }
}