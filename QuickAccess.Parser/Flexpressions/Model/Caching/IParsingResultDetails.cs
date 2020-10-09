using QuickAccess.Infrastructure.ValueContract;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    public readonly struct ParsingResultProductPair<TProduct>
    {
        private ParsingResultProductPair(ParsingEvaluationResult result, TProduct product)
        {
            Result = result;
            Product = product;
        }

        public TProduct Product { get; }
        public ParsingEvaluationResult Result { get; }
        public bool IsSuccessful => Result == ParsingEvaluationResult.Positive;

    }

    public interface IParsingResultDetails : ICanBeUndefined
    {
        object Product { get; }
        int ParsedCharactersCount { get; }
        ParsingEvaluationResult Result { get; }
        bool IsSuccessful { get; }
        bool IsFromCache { get; }
    }
}