using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public class FlexpressionConstraintRepository : IFlexpressionConstraintRepository
    {
        private readonly ConcurrentDictionary<Type, IFlexpressionConstraint> _constraintByType = new ConcurrentDictionary<Type, IFlexpressionConstraint>();

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public IFlexpressionConstraint Get<TConstraint>()
            where TConstraint : IFlexpressionConstraint
        {
            var constraintType = typeof(TConstraint);
            if (constraintType == DefaultFlexpressionConstraint.ConstraintType)
            {
                return DefaultFlexpressionConstraint.Instance;
            }

            var constraint = _constraintByType.GetOrAdd(
                constraintType,
                pType =>
                {
                    var constructor = Guard.ArgTypeHasParameterlessConstructor(
                        pType,
                        nameof(constraintType));
                    return (IFlexpressionConstraint)constructor.Invoke(Array.Empty<object>());
                });

            return constraint;
        }

        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public IFlexpressionConstraint Get(Type constraintType)
        {
            if (constraintType == DefaultFlexpressionConstraint.ConstraintType)
            {
                return DefaultFlexpressionConstraint.Instance;
            }

            Guard.ArgTypeImplements<IFlexpressionConstraint>(constraintType, nameof(constraintType));

            var domainConstraint = _constraintByType.GetOrAdd(
                constraintType,
                pType =>
                {
                    var constructor = Guard.ArgTypeHasParameterlessConstructor(
                        pType,
                        nameof(constraintType));
                    return (IFlexpressionConstraint)constructor.Invoke(Array.Empty<object>());
                });

            return domainConstraint;
        }
    }
}