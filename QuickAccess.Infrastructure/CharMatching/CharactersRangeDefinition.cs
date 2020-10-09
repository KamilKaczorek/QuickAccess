using System;
using System.Collections.Generic;
using System.Linq;
using QuickAccess.Infrastructure.CharMatching.Categories;
using QuickAccess.Infrastructure.Collections;
using QuickAccess.Infrastructure.Guards;

namespace QuickAccess.Infrastructure.CharMatching
{
    public static class CharactersRangeDefinition
    {
        public static readonly ICharactersRangeDefinition Any = new AnyChar();
        public static readonly ICharactersRangeDefinition Empty = new EmptyChar();

        public static ICharactersRangeDefinition CreateMatching(StandardCharacterCategories range)
        {
            Guard.ArgSatisfies(range, nameof(range), p => !p.IsEmpty(), p => $"Specified category is empty ({p}).");
            Guard.ArgSatisfies(range, nameof(range), p => p.IsValid(), p => $"Specified category is not supported ({p})");

            return new StandardCategory(range);
        }

        public static ICharactersRangeDefinition CreateMatching(Func<char, bool> predicate, string description = null)
        {
            Guard.ArgNotNull(predicate, nameof(predicate));

            return new Predicate(predicate, description);
        }

        public static ICharactersRangeDefinition CreateMatching(HashSet<char> characters)
        {
            Guard.ArgNotNull(characters, nameof(characters));

            return new CharSet(characters.AsReadOnly());
        }

        public static ICharactersRangeDefinition CreateMatching(char character, StringComparison comparison = StringComparison.Ordinal)
        {
            return comparison != StringComparison.Ordinal
                ? (ICharactersRangeDefinition)new SingleChar(character, comparison)
                : new SingleCharDefaultComparer(character);
        }

        public static ICharactersRangeDefinition CreateMatching(char char1, char char2, params char[] characters)
        {
            var charSet = new HashSet<char> { char1, char2 };

            if (characters != null && characters.Length > 0)
            {
                charSet.UnionWith(characters);
            }

            return new CharSet(charSet.AsReadOnly());
        }

        public static ICharactersRangeDefinition CreateMatching(IEnumerable<char> characters, StringComparison comparison = StringComparison.Ordinal)
        {
            Guard.ArgNotNull(characters, nameof(characters));
            var charSet = comparison != StringComparison.Ordinal ? new HashSet<char>(characters, CharComparer.Get(comparison)) : new HashSet<char>(characters);

            if (charSet.Count == 0)
            {
                return Empty;
            }

            if (charSet.Count == 1)
            {
                return CreateMatching(charSet.Single(), comparison);
            }

            return new CharSet(charSet.AsReadOnly());
        }

        private sealed class AnyChar : ICharactersRangeDefinition
        {
            public string Description => "<AnyChar>";
            public bool IsMatch(char character) { return true; }

            public bool IsMatchAny(IEnumerable<char> characters) { return characters.Any(); }

            public IEnumerable<char> Matches(IEnumerable<char> characters) { return characters; }

            public override string ToString() { return Description; }
            public TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor)
            {
                return visitor.VisitAny();
            }

            public CharactersRangeDefinitionType DefinitionType => CharactersRangeDefinitionType.Any;
        }

        private sealed class EmptyChar : ICharactersRangeDefinition
        {
            public string Description => "<NotAnyChar>";
            public bool IsMatch(char character) { return false; }

            public bool IsMatchAny(IEnumerable<char> characters) { return false; }

            public IEnumerable<char> Matches(IEnumerable<char> characters) { yield break; }

            public override string ToString() { return Description; }
            public TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor)
            {
                return visitor.VisitEmpty();
            }

            public CharactersRangeDefinitionType DefinitionType => CharactersRangeDefinitionType.Empty;
        }

        private sealed class StandardCategory : ICharactersRangeDefinition
        {
            private readonly StandardCharacterCategories _category;

            public StandardCategory(StandardCharacterCategories category)
            {
                _category = category;
            }

            public string Description => $"{_category.ToRegexStatement()}";
            public bool IsMatch(char character) { return _category.IsMatch(character); }

            public bool IsMatchAny(IEnumerable<char> characters) { return _category.IsMatchAny(characters); }

            public IEnumerable<char> Matches(IEnumerable<char> characters) { return _category.Matches(characters); }

            public override string ToString() { return Description; }
            public TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor)
            {
                return visitor.VisitStandardCategory(_category);
            }

            public CharactersRangeDefinitionType DefinitionType => CharactersRangeDefinitionType.StandardCategory;
        }

        private sealed class Predicate : ICharactersRangeDefinition
        {
            private readonly Func<char, bool> _predicate;

            public Predicate(Func<char, bool> predicate, string description)
            {
                _predicate = predicate;
                Description = description ?? "<Predicate>";
            }

            public string Description { get; }
            public bool IsMatch(char character) { return _predicate.Invoke(character); }

            public bool IsMatchAny(IEnumerable<char> characters) { return characters.Any(_predicate); }

            public IEnumerable<char> Matches(IEnumerable<char> characters) { return characters.Where(_predicate); }

            public override string ToString() { return Description; }
            public TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor)
            {
                return visitor.VisitPredicate(_predicate, Description);
            }

            public CharactersRangeDefinitionType DefinitionType => CharactersRangeDefinitionType.Predicate;
        }

        private sealed class CharSet : ICharactersRangeDefinition
        {
            private readonly IReadOnlySet<char> _characters;

            public CharSet(IReadOnlySet<char> characters) { _characters = characters; }

            public string Description => _characters.Count < 40 ? $"[{_characters}]" : "<CharSet>";

            public bool IsMatch(char character) { return _characters.Contains(character); }

            public bool IsMatchAny(IEnumerable<char> characters) { return _characters.Overlaps(characters); }

            public IEnumerable<char> Matches(IEnumerable<char> characters) { return characters.Where(IsMatch); }

            public override string ToString() { return Description; }
            public TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor)
            {
                return visitor.VisitSet(_characters);
            }

            public CharactersRangeDefinitionType DefinitionType => CharactersRangeDefinitionType.Set;
        }

        private sealed class SingleCharDefaultComparer : ICharactersRangeDefinition
        {
            private readonly char _character;

            public SingleCharDefaultComparer(char character)
            {
                _character = character;
            }

            public string Description => $"'{_character}'";

            public bool IsMatch(char character)
            {
                return _character == character;
            }

            public bool IsMatchAny(IEnumerable<char> characters) { return characters.Contains(_character); }

            public IEnumerable<char> Matches(IEnumerable<char> characters) { return characters.Where(IsMatch); }

            public override string ToString() { return Description; }
            public TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor)
            {
                return visitor.VisitSingle(_character, StringComparison.Ordinal);
            }

            public CharactersRangeDefinitionType DefinitionType => CharactersRangeDefinitionType.Single;
        }

        private sealed class SingleChar : ICharactersRangeDefinition
        {
            private readonly char _character;
            private readonly StringComparison _comparison;

            public SingleChar(char character, StringComparison comparison)
            {
                _character = character;
                _comparison = comparison;
            }

            public string Description => $"'{_character}'";

            public bool IsMatch(char character)
            {
                return CharComparer.Get(_comparison).Equals(_character, character);
            }

            public bool IsMatchAny(IEnumerable<char> characters) { return characters.Contains(_character, CharComparer.Get(_comparison)); }

            public IEnumerable<char> Matches(IEnumerable<char> characters) { return characters.Where(IsMatch); }

            public override string ToString() { return Description; }
            public TVisitationResult AcceptVisitor<TVisitationResult>(ICharactersRangeDefinitionVisitor<TVisitationResult> visitor)
            {
                return visitor.VisitSingle(_character, _comparison);
            }

            public CharactersRangeDefinitionType DefinitionType => CharactersRangeDefinitionType.Single;
        }
    }
}