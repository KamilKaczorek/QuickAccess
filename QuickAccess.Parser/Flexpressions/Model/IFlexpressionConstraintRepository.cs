using System;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpressionConstraintRepository
    {
        IFlexpressionConstraint Get<TConstraint>()
            where TConstraint : IFlexpressionConstraint;

        IFlexpressionConstraint Get(Type domainConstraintType);
    }
}