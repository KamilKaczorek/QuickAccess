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
// Project: QuickAccess.Parser.Tests
// 
// Author: Kamil Piotr Kaczorek
// http://kamil.scienceontheweb.net
// e-mail: kamil.piotr.kaczorek@gmail.com
#endregion

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickAccess.Parser.SmartExpressions;

namespace QuickAccess.Parser.Tests
{

	[TestClass]
	public class ParserBuilderIntegrationTest
	{
		[TestMethod]
		public void Test()
		{
			
			var d = 2.2;
			var name = (SX.Letter + (SX.Digit | SX.Letter).ZeroOrMore()) / "Name";
			var intNumber = SX.Digit.OneOrMore() / "Integer";
			var floatNumber = (intNumber + "." + intNumber) / "Float";
			var whiteSpace = (" ".Exact() / "Space" | "\t".Exact() / "Tab").OneOrMore() / "WhiteSpace";
			var optWs = (" ".Exact() / "Space" | "\t".Exact() / "Tab").ZeroOrMore();
			var functionArg = (floatNumber | intNumber | name) / "FunctionArg";
			var functionArgList = (functionArg & ("," & SX.Current).ZeroOrMore()) / "FunctionArgList";
			var functionInvocation = (name & "(" & ~functionArgList & ")" & ';') / "FunctionInvocation";

		


			functionInvocation.ApplyCustomRule(SX.WhiteSpace.RuleName, whiteSpace);
			functionInvocation.ApplyCustomRule(SX.OptionalWhiteSpace.RuleName, optWs);
			functionInvocation.ApplyCustomRule(SX.Digit.RuleName, SX.Start | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' | '0');
			functionInvocation.ApplyCustomRule(SX.Letter.RuleName, SX.Start | 'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'g' | 'h' | 'i' | 'j');

			var gn = new Dictionary<string, int>();

			var nregex = functionArg.ToRegularExpressionString(gn);
			var regex = functionInvocation.ToRegularExpressionString(gn);
			Console.WriteLine(regex);

			// * anything
			// + nothing
			// - positive lookahead			
			// ^ white space
			// 
			// Parsing
			// Syntactic validation
			// Tree build
			// Semantic validation
			// Compilation

		}
	}
}