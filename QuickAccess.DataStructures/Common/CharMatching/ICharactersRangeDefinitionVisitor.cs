using System;
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.DataStructures.Common.Patterns.Visitor;

namespace QuickAccess.DataStructures.Common.CharMatching
{
    public interface ICharactersRangeDefinitionVisitor<TVisitationResult> : IVisit<TVisitationResult>
    {
        TVisitationResult VisitRange(StandardCharactersRange range);

        TVisitationResult VisitPredicate(Func<char, bool> predicate, string description);

        TVisitationResult VisitSet(IReadOnlySet<char> characters);

        TVisitationResult VisitSingle(char character, StringComparison comparison);

        TVisitationResult VisitAny();

        TVisitationResult VisitEmpty();
    }
}