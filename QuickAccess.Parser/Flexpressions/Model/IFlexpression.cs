using System;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpression<out TConstraint> : IFlexpression
        where TConstraint : IFlexpressionConstraint
    {
    }

    public interface IFlexpression
    {
        Type ConstraintType { get; }
        long LocalId { get; }
        string Name { get; }
        TVisitationResult AcceptVisitor<TVisitationResult>(IVisitFlexpressions<TVisitationResult> visitor);
    }
}