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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.SmartExpressions;

namespace QuickAccess.Parser.Tests
{

	public interface ITestCompiler
	{

	}

	[TestClass]
	public class ParserBuilderIntegrationTest
	{
		[TestMethod]
		[DataRow("Abc();", true)]
		[DataRow("Abc();\t", true)]
		[DataRow("Abc(); ", true)]
		[DataRow("Abc\t();", true)]
		[DataRow("Abc ();", true)]
		[DataRow("Abc\t (\t )\t ;", true)]
		[DataRow("abcdefghijklmnopqrstuvwxyz();", true)]
		[DataRow("ABCDEFGHIJKLMNOPQRSTUVWXYZ();", true)]
		[DataRow("abc(ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890);", true)]
		[DataRow("abc(abcdefghijklmnopqrstuvwxyz1234567890);", true)]
		[DataRow("A1234567890();", true)]
		[DataRow("Abc1( );", true)]
		[DataRow("A1(\t);", true)]
		[DataRow("a123(1234567890);", true)]
		[DataRow("a1b2c3(55);", true)]
		[DataRow("def(1234567890.1234567890);", true)]
		[DataRow("ghi(1,2);", true)]
		[DataRow("jka(\t1,\t\t 2);", true)]
		[DataRow("l(\t1,\t\t 2);", true)]
		[DataRow("Mno(1.2,2.2);", true)]
		[DataRow("Pqr(1, 2, 3, 4, 5.3, 3123, 321.123);", true)]
		[DataRow("Abc(1;2);", false)]
		[DataRow("Abc;", false)]
		[DataRow("abc(;", false)]
		[DataRow("def);", false)]
		[DataRow("123();", false)]
		[DataRow("abc(,);", false)]
		[DataRow("abc( , );", false)]
		public void ON_ToRegularExpressionString_WHEN_FunctionInvocationRegularExpression_SHOULD_ReturnRegexThatParsesGivenExpression(string expression, bool expressionParsed)
		{
			var name = (SX.Letter + (SX.Digit | SX.Letter).ZeroOrMore()).DefinesRule("Name");
			var intNumber = SX.Digit.OneOrMore().DefinesRule("Integer");
			var floatNumber = (intNumber + "." + intNumber).DefinesRule("Float");
			var functionArg = (floatNumber | intNumber | name).DefinesRule("FunctionArg");
			var functionArgList = (functionArg & ("," & functionArg).ZeroOrMore()).DefinesRule("FunctionArgList");
			var functionInvocation = (name & "(" & ~functionArgList & ")" & ';').DefinesRule("FunctionInvocation");

			var ctx = RegularExpressionBuildingContext.CreateStandard();
			var regularExpressionString = functionInvocation.ToRegularExpressionString(ctx);

			var regex = new Regex(regularExpressionString, RegexOptions.Compiled);

			var res = regex.IsMatch(expression);

			Assert.AreEqual(expressionParsed, res);


		

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