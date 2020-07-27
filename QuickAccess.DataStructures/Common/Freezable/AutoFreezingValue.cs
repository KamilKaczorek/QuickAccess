#region LICENSE [BSD-2-Clause]
// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2020 by Kamil Piotr Kaczorek
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, 
//     this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, 
//     this list of conditions and the following disclaimer in the documentation and/or 
//     other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF 
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// =====================================================================
// 
// Project: QuickAccess.DataStructures
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;
using System.Runtime.InteropServices;
using QuickAccess.DataStructures.Common.ValueContract;

namespace QuickAccess.DataStructures.Common.Freezable
{
    /// <summary>
    /// Implements the value container that freezes the value from the modification after first set.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public sealed class AutoFreezingValue<T> : IEditableValue<T>
    {
        private ValueStates _state;
        private T _value;

        /// <inheritdoc />
		public bool IsReadOnly => _state.HasFlag(ValueStates.ReadOnly);

		/// <inheritdoc />
		public bool IsDefined => _state.HasFlag(ValueStates.Defined);

		public static implicit operator T(AutoFreezingValue<T> obj)
		{
			return obj.Value;
		}

        internal AutoFreezingValue(T value, ValueStates state)
        {
            _value = value;
            _state = state;
        }

        /// <inheritdoc />
        public T Value
        {
            get =>
                _state.HasFlag(ValueStates.Defined)
                    ? _value
                    : throw new InvalidOperationException("Can't access value - value is undefined.");
            set => this.Set(value);
        }

        /// <inheritdoc />
        public ValueModificationResult TryModifyValue(T value)
        {
            if (_state.HasFlag(ValueStates.ReadOnly))
            {
                return ValueModificationResult.SourceFrozen;
            }

            _state = ValueStates.Defined | ValueStates.ReadOnly;
            _value = value;

            return ValueModificationResult.SuccessfullyModified;
        }
    }

	public static class AutoFreezingValue
	{
		public static AutoFreezingValue<T> Create<T>(T currentValue, ValueStates state)
		{
			return new AutoFreezingValue<T>(currentValue, state);
		}

		public static AutoFreezingValue<T> CreateUndefined<T>()
		{
			return new AutoFreezingValue<T>(default, ValueStates.Undefined);
		}	
        
        public static AutoFreezingValue<T> CreateDefinedNotFrozen<T>(T definedValue)
        {
            return new AutoFreezingValue<T>(definedValue, ValueStates.Defined);
        }

        public static AutoFreezingValue<T> CreateDefinedFrozen<T>(T definedValue)
        {
            return new AutoFreezingValue<T>(definedValue, ValueStates.Defined | ValueStates.ReadOnly);
        }
	}
}