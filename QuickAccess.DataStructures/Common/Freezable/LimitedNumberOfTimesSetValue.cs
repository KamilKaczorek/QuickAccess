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

namespace QuickAccess.DataStructures.Common.Freezable
{
	public sealed class LimitedNumberOfTimesSetValue<T> : FreezableValueBase<T>
	{
		private readonly int _numberOfTimesCanBeSet;
		private int _numberOfTimesAlreadySet;

		public int RemainingNumberOfTimesCanBeSet => Math.Max(0, _numberOfTimesCanBeSet - _numberOfTimesAlreadySet);

		internal LimitedNumberOfTimesSetValue(
			int numberOfTimesCanBeSet
		)
		{
			Value = default;
			_numberOfTimesCanBeSet = numberOfTimesCanBeSet;
			_numberOfTimesAlreadySet = 0;
		}

		internal LimitedNumberOfTimesSetValue(
			T currentValue,
			int remainingNumberOfTimesCanBeSet
		)
		{
			Value = currentValue;
			_numberOfTimesCanBeSet = remainingNumberOfTimesCanBeSet + 1;
			_numberOfTimesAlreadySet = 1;
		}

		public override bool IsSet => _numberOfTimesAlreadySet > 0;

		public override bool TrySet(T value)
		{
			if (_numberOfTimesAlreadySet < _numberOfTimesCanBeSet)
			{
				_numberOfTimesAlreadySet++;
				Value = value;
				return true;
			}

			return false;
		}

		/// <inheritdoc />
		public override bool IsFrozen => _numberOfTimesAlreadySet >= _numberOfTimesCanBeSet;

		public static implicit operator T(LimitedNumberOfTimesSetValue<T> obj)
		{
			return obj.Value;
		}
	}

	public static class LimitedNumberOfTimesSetValue
	{
		public static LimitedNumberOfTimesSetValue<T> CreateSet<T>(T currentValue, int remainingNumberOfTimesCanBeSet)
		{
			return new LimitedNumberOfTimesSetValue<T>(currentValue, remainingNumberOfTimesCanBeSet);
		}

		public static LimitedNumberOfTimesSetValue<T> CreateSetFrozen<T>(T currentValue)
		{
			return new LimitedNumberOfTimesSetValue<T>(currentValue, 0);
		}
		
		public static LimitedNumberOfTimesSetValue<T> CreateNotSet<T>(int numberOfTimesCanBeSet)
		{
			return new LimitedNumberOfTimesSetValue<T>(numberOfTimesCanBeSet);
		}		
	}
}