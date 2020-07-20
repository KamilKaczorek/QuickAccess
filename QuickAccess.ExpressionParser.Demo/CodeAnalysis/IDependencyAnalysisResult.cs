using System;
using System.Collections.Generic;

namespace QuickAccess.ExpressionParser.Demo.CodeAnalysis
{
    public interface IDependencyAnalysisResult
    {
        Type AnalyzedType { get; }
        int TotalReferencesCount { get; }

        IReadOnlyCollection<Type> ReferencedTypes { get; }
    }
}