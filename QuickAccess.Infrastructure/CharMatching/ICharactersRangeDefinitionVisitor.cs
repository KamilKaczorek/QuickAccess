using System;
using QuickAccess.Infrastructure.CharMatching.Categories;
using QuickAccess.Infrastructure.Collections;
using QuickAccess.Infrastructure.Patterns.Visitor;

namespace QuickAccess.Infrastructure.CharMatching
{
    public interface ICharactersRangeDefinitionVisitor<TVisitationResult> : IVisit<TVisitationResult>
    {
        TVisitationResult VisitStandardCategory(StandardCharacterCategories category);

        TVisitationResult VisitPredicate(Func<char, bool> predicate, string description);

        TVisitationResult VisitSet(IReadOnlySet<char> characters);

        TVisitationResult VisitSingle(char character, StringComparison comparison);

        TVisitationResult VisitAny();

        TVisitationResult VisitEmpty();
    }
}