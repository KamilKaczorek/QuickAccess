using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using QuickAccess.DataStructures.Common.Guards;

namespace QuickAccess.Parser.Flexpressions.Model
{
    public class FlexpressionConstraintRepository : IFlexpressionConstraintRepository
    {
        private readonly ConcurrentDictionary<Type, IFlexpressionConstraint> _constraintByType = new ConcurrentDictionary<Type, IFlexpressionConstraint>();

        [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public IFlexpressionConstraint Resolve<TConstraint>()
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

        [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public IFlexpressionConstraint Resolve(Type constraintType)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public IFlexpressionConstraint GetOrDefine(Type constraintType, Func<Type, IFlexpressionConstraint> factoryCallback)
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
                    var implementation = factoryCallback.Invoke(pType);
                    Guard.ArgImplements(implementation, nameof(implementation), constraintType);
                    return implementation;
                });

            return domainConstraint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public bool IsDefined(Type constraintType)
        {
            if (constraintType == DefaultFlexpressionConstraint.ConstraintType)
            {
                return true;
            }

            return _constraintByType.ContainsKey(constraintType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public void Define(Type constraintType, IFlexpressionConstraint implementation)
        {
            if (constraintType == DefaultFlexpressionConstraint.ConstraintType)
            {
                if (implementation != DefaultFlexpressionConstraint.Instance)
                {
                    throw new ArgumentException($"Can't redefine {DefaultFlexpressionConstraint.ConstraintType}.");
                }
            }

            Guard.ArgTypeImplements<IFlexpressionConstraint>(constraintType, nameof(constraintType));
            Guard.ArgImplements(implementation, nameof(implementation), constraintType);

            _constraintByType[constraintType] = implementation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
        public IDisposable DefineScoped(Type constraintType, IFlexpressionConstraint implementation)
        {
            Guard.ArgTypeImplements<IFlexpressionConstraint>(constraintType, nameof(constraintType));
            Guard.ArgImplements(implementation, nameof(implementation), constraintType);

            var scope = new ConstraintScope(constraintType, implementation, _constraintByType);

            return scope;
        }

        private sealed class ConstraintScope : IDisposable
        {
            private readonly Type _constraintType;
            private readonly IFlexpressionConstraint _previous;
            private readonly IFlexpressionConstraint _current;
            private ConcurrentDictionary<Type, IFlexpressionConstraint> _dictionary;

            public ConstraintScope(Type constraintType, IFlexpressionConstraint implementation, ConcurrentDictionary<Type, IFlexpressionConstraint> dictionary)
            {
                 dictionary.TryGetValue(constraintType, out _previous);
                _constraintType = constraintType;
                _current = implementation;
                _dictionary = dictionary;
                _dictionary[_constraintType] = _current;
            }

            public void Dispose()
            {
                var dict = Interlocked.Exchange(ref _dictionary, null);
                if (dict == null)
                {
                    return; 
                }

                if (_previous != null)
                {
                    dict.TryUpdate(_constraintType, _previous, _current);
                    return;
                }

                if (dict.TryRemove(_constraintType, out var flexpressionConstraint) && !ReferenceEquals(flexpressionConstraint, _current))
                {
                    dict.TryAdd(_constraintType, flexpressionConstraint);
                }
            }
        }
    }
}