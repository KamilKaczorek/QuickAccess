using System;
using System.Data;
using QuickAccess.Infrastructure.Guards;

namespace QuickAccess.Infrastructure.ValueContract
{
    public sealed class ExternallyProvidedValue<T> : IEditableValue<T>
    {
        public static IEditableValue<T> Create(Func<T> valueProvider)
        {
            return new ExternallyProvidedValue<T>(valueProvider);
        }

        private readonly Func<T> _valueProviderCallback;
        public ExternallyProvidedValue(Func<T> valueProvider)
        {
            Guard.ArgNotNull(valueProvider, nameof(valueProvider));
            _valueProviderCallback = valueProvider;
        }

        public bool IsDefined => true;
        public T Value => _valueProviderCallback.Invoke();

        public ValueModificationResult TryModifyValue(T value) { throw new ReadOnlyException("Can't modify value - value is read only"); }

        public bool IsReadOnly => true;
    }
}