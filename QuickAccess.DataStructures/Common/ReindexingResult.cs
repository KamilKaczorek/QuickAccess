#region LICENSE [BSD-2-Clause]

// This code is distributed under the BSD-2-Clause license.
// =====================================================================
// 
// Copyright ©2018 by Kamil Piotr Kaczorek
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

namespace QuickAccess.DataStructures.Common
{
	/// <summary>
	/// The result of re-indexed data operation with result data and re-indexing information.
	/// </summary>
	/// <typeparam name="TData">The type of the data.</typeparam>
	public struct ReindexedDataResult<TData>
	{
		/// <summary>Initializes a new instance of the <see cref="ReindexedDataResult{TData}"/> struct.</summary>
		/// <param name="indexTranslator">The index map.</param>
		/// <param name="data">The data.</param>
		public ReindexedDataResult(IIndexTranslator indexTranslator, TData data)
		{
			IndexTranslator = indexTranslator;
			Data = data;
		}


		/// <summary>Gets the index translator.</summary>
		/// <value>The index translator.</value>
		public IIndexTranslator IndexTranslator { get; }

		/// <summary>Gets the affected data.</summary>
		/// <value>The data.</value>
		public TData Data { get; }

		/// <summary>
		/// Performs an implicit conversion from <see cref="ReindexedDataResult{TData}"/> to <see cref="TData"/>.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator TData(ReindexedDataResult<TData> source)
		{
			return source.Data;
		}
	}
}