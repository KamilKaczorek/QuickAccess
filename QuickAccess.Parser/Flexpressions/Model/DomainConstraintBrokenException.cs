using System;

namespace QuickAccess.Parser.Flexpressions.Model
{
    [Serializable]
    public sealed class DomainConstraintBrokenException : InvalidOperationException
    {
        public DomainConstraintBrokenException(string message)
            : base(message)
        {
        }
    }
}