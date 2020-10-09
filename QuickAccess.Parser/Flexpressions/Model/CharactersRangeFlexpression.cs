using QuickAccess.Infrastructure.CharMatching;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public sealed class CharactersRangeFlexpression : Flexpression
    {
        public CharactersRangeFlexpression(ICharactersRangeDefinition range) { Range = range; }

        public static Flexpression Create(ICharactersRangeDefinition range)
        {
            return new CharactersRangeFlexpression(range);
        }

        public override string Name => Range.Description;

        public ICharactersRangeDefinition Range { get; }

        public override TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor)
        {
            var visitationResult = visitor.VisitCharactersRange(Range);
            return visitationResult;
        }
    }
}