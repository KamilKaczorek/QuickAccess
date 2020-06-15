using QuickAccess.Parser.Flexpressions.Bricks;
using QuickAccess.Parser.Product;

namespace QuickAccess.Parser.Flexpressions
{
    public interface IDefineBaseFlexpressions
    {
        FlexpressionBrick CreateQuantifierBrick(FlexpressionBrick content, long min, long max);
        FlexpressionBrick DefineRule(FlexpressionBrick content, ExpressionTypeDescriptor expressionType);
        FlexpressionBrick DefineSealedRule(FlexpressionBrick content, ExpressionTypeDescriptor expressionType);
        FlexpressionBrick CreateRulePlaceholder(string ruleName, FlexpressionBrick defaultExpression);

        FlexpressionBrick Current { get; }

        FlexpressionBegin Start { get; }

        FlexpressionBrick Anything { get; }
        FlexpressionBrick Empty { get; }
        FlexpressionBrick WhiteSpace { get; }
        FlexpressionBrick WhiteSpaceOrNewLine { get; }
        FlexpressionBrick OptionalWhiteSpace { get; }
        FlexpressionBrick OptionalWhiteSpaceOrNewLine { get; }
        FlexpressionBrick CustomSequence { get; }
        FlexpressionBrick NewLine { get; }
        FlexpressionBrick Letter { get; }
        FlexpressionBrick UpperLetter { get; }
        FlexpressionBrick LowerLetter { get; }
        FlexpressionBrick Symbol { get; }
        FlexpressionBrick Digit { get; }
    }
}