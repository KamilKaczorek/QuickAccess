using System;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpression : IAcceptFlexpressionsVisitor, IEquatable<IFlexpression>
    {
        FlexpressionId LocalId { get; }
        string Name { get; }
    }

    

}