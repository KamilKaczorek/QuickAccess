using System;

namespace QuickAccess.Parser.Flexpressions.Model.Caching
{
    public interface ICacheCurrentPositionParsingResults
    {
        IParsingResultDetails GetParsingResultOrUpdate<TProduct>(in FlexpressionId flexpressionId, Func<TProduct> getResultCallback)
            where TProduct : class;

        bool ClearParsingResult(in FlexpressionId flexpressionId);
    }
}