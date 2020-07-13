using System;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public interface IFlexpressionConstraintRepository
    {
        IFlexpressionConstraint Resolve<TConstraint>()
            where TConstraint : IFlexpressionConstraint;

        IFlexpressionConstraint Resolve(Type constraintType);

        IFlexpressionConstraint GetOrDefine(Type constraintType, Func<Type, IFlexpressionConstraint> factoryCallback);

        bool IsDefined(Type constraintType);

        void Define(Type constraintType, IFlexpressionConstraint implementation);

        IDisposable DefineScoped(Type constraintType, IFlexpressionConstraint implementation);

    }
}