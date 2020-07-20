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
// Project: QuickAccess.DataStructures.Tests
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com

#endregion

using System;
using Moq.Language.Flow;

namespace QuickAccess.DataStructures.UnitTests.TestUtils
{
	public delegate void TryGetCallback<in TKey, TValue>(TKey key, out TValue amount); // needed for Callback

	public delegate bool TryGetReturn<in TKey, TValue>(TKey key, out TValue item);

	public sealed class TryGetCallbackAssembler<TKey, TValue>
	{
		private readonly Func<TKey, TValue> _outCallback;
		private readonly Func<TKey, bool> _returnCallback;

		public TValue Default = default;

		public TryGetCallback<TKey, TValue> Callback =>
			(TKey i, out TValue dt) =>
			{
				var res = _returnCallback.Invoke(i);
				dt = res ? _outCallback.Invoke(i) : Default;
			};

		public TryGetReturn<TKey, TValue> Return =>
			(TKey i, out TValue dt) =>
			{
				var res = _returnCallback.Invoke(i);
				dt = res ? _outCallback.Invoke(i) : Default;
				return res;
			};

		public TryGetCallbackAssembler(Func<TKey, bool> returnCallback, Func<TKey, TValue> outCallback)
		{
			_returnCallback = returnCallback;
			_outCallback = outCallback;
		}
	}

	public static class TryGetCallbackAssembler
	{
		public static TryGetCallbackAssembler<TKey, TValue> Create<TKey, TValue>(Func<TKey, bool> returnCallback,
		                                                                         Func<TKey, TValue> outCallback)
		{
			return new TryGetCallbackAssembler<TKey, TValue>(returnCallback, outCallback);
		}

		public static IReturnsResult<T> TryGetCallback<T, TKey, TValue>(this ISetup<T, bool> source,
		                                                                TryGetCallbackAssembler<TKey, TValue> callback)
			where T : class
		{
			return source.Callback(callback.Callback).Returns(callback.Return);
		}

		public static IReturnsResult<T> TryGetCallback<T, TKey, TValue>(this ISetup<T, bool> source,
		                                                                Func<TKey, bool> returnCallback,
		                                                                Func<TKey, TValue> outCallback)
			where T : class
		{
			var callback = new TryGetCallbackAssembler<TKey, TValue>(returnCallback, outCallback);
			return source.Callback(callback.Callback).Returns(callback.Return);
		}
	}
}