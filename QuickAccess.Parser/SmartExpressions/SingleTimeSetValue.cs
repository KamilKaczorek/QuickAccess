#region LICENSE [BSD-2-Clause]
// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2019 by Kamil Piotr Kaczorek
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
// Project: QuickAccess.Parser
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;

namespace QuickAccess.Parser.SmartExpressions
{
	public class SingleTimeSetValue<T> : IFreezableValue<T>
	{
		private readonly object _thisLock;
		private bool _isSet;
		private T _value;

		public bool IsSynchronized => _thisLock != null;

		internal SingleTimeSetValue(
			T currentValue,
			bool isSet,
			bool synchronized,
			object syncRoot = null)
		{
			_value = currentValue;
			_isSet = isSet;
			_thisLock = synchronized ? syncRoot ?? new object() : null;
		}

		public bool IsSet
		{
			get
			{
				if (!IsSynchronized)
				{
					return _isSet;
				}

				lock (_thisLock)
				{
					return _isSet;
				}
			}
		}

		public T Value
		{
			get
			{
				if (!IsSynchronized)
				{
					return _value;
				}

				lock (_thisLock)
				{
					return _value;
				}
			}
		}

		public bool TrySet(T value)
		{
			if (!IsSynchronized)
			{
				if (_isSet)
				{
					return false;
				}

				_isSet = true;
				_value = value;
				return true;
			}

			lock (_thisLock)
			{
				if (_isSet)
				{
					return false;
				}

				_isSet = true;
				_value = value;
				return true;
			}
		}

		/// <inheritdoc />
		public bool IsFrozen => IsSet;

		public void Set(T value)
		{
			if (!TrySet(value))
			{
				throw new InvalidOperationException("Value is already set.");
			}
		}

		public static implicit operator T(SingleTimeSetValue<T> obj)
		{
			return obj.Value;
		}
	}

	public static class SingleTimeSetValue
	{
		public static SingleTimeSetValue<T> Create<T>(T currentValue = default)
		{
			return new SingleTimeSetValue<T>(currentValue, false, false);
		}

		public static SingleTimeSetValue<T> CreateSynchronized<T>(T currentValue = default, object synchObject = null)
		{
			return new SingleTimeSetValue<T>(currentValue, false, true, synchObject);
		}

		public static SingleTimeSetValue<T> CreateSet<T>(T currentValue = default)
		{
			return new SingleTimeSetValue<T>(currentValue, true, false);
		}

		public static SingleTimeSetValue<T> CreateSetSynchronized<T>(T currentValue = default, object synchObject = null)
		{
			return new SingleTimeSetValue<T>(currentValue, true, true, synchObject);
		}
	}
}