using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using QuickAccess.DataStructures.Common.Patterns.Specifications;

namespace QuickAccess.DataStructures.Common.Guards
{
    public static class Guard
    {
        public const string ExceptionPrefix = "GUARD:";

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetMessage(string message)
        {
            return $"{ExceptionPrefix} {message}";
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("=> halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CodeNeverReached()
        {
            throw new InvalidOperationException(GetMessage("Code that was not meant to be reached was executed."));
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgNotNull<T>(T value, string parameterName)
            where T : class
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:notnull => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgNull<T>(T value, string parameterName)
            where T : class
        {
            if (!ReferenceEquals(value, null))
            {
                throw new ArgumentException($"{parameterName} is expected to be null but is '{value}'.", parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgNotNullAndSatisfies<T>(T value, string parameterName, Func<T, bool> predicate, string message = null)
            where T : class
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (!predicate.Invoke(value))
            {
                throw new ArgumentException(GetMessage(message ?? $"Unexpected argument value ({value}) <= validation predicate failed."), parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgSatisfies<T>(T value, string parameterName, Func<T, bool> predicate, string message = null)
        {
            if (!predicate.Invoke(value))
            {
                throw new ArgumentException(GetMessage(message ?? $"Unexpected argument value ({value}) <= validation predicate failed."), parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgNotNullAndSatisfies<T>(T value, string parameterName, ISpecification<T> specification, string message = null)
            where T : class
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (!specification.IsSatisfiedBy(value))
            {
                throw new ArgumentException(GetMessage(message ?? $"Unexpected argument value ({value}) <= specification '{specification.Descriptor.Name}' is not satisfied by argument."), parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgSatisfies<T>(T value, string parameterName, ISpecification<T> specification, string message = null)
        {
            if (!specification.IsSatisfiedBy(value))
            {
                throw new ArgumentException(GetMessage(message ?? $"Unexpected argument value ({value}) <= specification '{specification.Descriptor.Name}' is not satisfied by argument."), parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgNotNullNorEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (!value.Any())
            {
                if (value is string)
                {
                    throw new ArgumentException(GetMessage($"String ({parameterName}) is empty, expected not empty string."), parameterName);
                }

                throw new ArgumentException(GetMessage($"Sequence ({parameterName}) is empty, expected not empty sequence."), parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:notnull => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgNullOrEmpty<T>(IEnumerable<T> value, string parameterName)
        {
            if (!ReferenceEquals(value, null) && value.Any())
            {
                throw new ArgumentException(
                    $"{parameterName} is expected to be null or empty but contains items ({value}).",
                    parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgNotNullNorWhiteSpace(string value, string parameterName)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(GetMessage($"String ({parameterName}) is empty, expected not empty and not whitespace string."), parameterName);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(GetMessage($"String ({parameterName}) is whitespace '{value.Replace("\t", @"\t").Replace(" ", @"\s").Replace("\n", @"\n").Replace("\r", @"\r")}', expected not empty, not whitespace string."), parameterName);
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgInRange<T>(T value, string parameterName, T minIncl, T maxIncl)
            where T : IComparable<T>
        {
            ArgNotNullNorEmpty(parameterName, parameterName);

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.CompareTo(minIncl) < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, GetMessage($"Value ({value}) is smaller than expected min='{minIncl}'."));
            }

            if (value.CompareTo(maxIncl) > 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, GetMessage($"Value ({value}) is greater than expected max='{maxIncl}'."));
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgEqualOrGreaterThan<T>(T value, string parameterName, T minIncl)
            where T : IComparable<T>
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.CompareTo(minIncl) < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, GetMessage($"Value ({value}) is smaller than expected min='{minIncl}'."));
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgEqualOrSmallerThan<T>(T value, string parameterName, T maxIncl)
            where T : IComparable<T>
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.CompareTo(maxIncl) > 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, GetMessage($"Value ({value}) is greater than expected max='{maxIncl}'."));
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgGreaterThan<T>(T value, string parameterName, T minIncl)
            where T : IComparable<T>
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.CompareTo(minIncl) <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, GetMessage($"Value ({value}) is not greater than '{minIncl}'."));
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgSmallerThan<T>(T value, string parameterName, T maxIncl)
            where T : IComparable<T>
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(parameterName);
            }

            if (value.CompareTo(maxIncl) >= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, GetMessage($"Value ({value}) is not smaller than '{maxIncl}'."));
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:true => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgFalse(bool value, string parameterName)
        {
            if(value)
            {
                throw new ArgumentOutOfRangeException(parameterName, true, GetMessage("Value is true but is expected to be false."));
            }
        }

        [DebuggerHidden]
        [DebuggerStepThrough]
        [ContractAnnotation("value:false => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgTrue(bool value, string parameterName)
        {
            if (!value)
            {
                throw new ArgumentOutOfRangeException(parameterName, true, GetMessage("Value is false but is expected to be true."));
            }
        }
    }
}
