using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.SmartExpressions;
using System;
using System.Text.RegularExpressions;
using QuickAccess.Parser;

namespace QuickAccess.ExpressionParser.Demo
{
	class Program
	{
		static void Main(string[] args)
		{
			Test("12, 23, 34");
		}

		public static void Test(string expression)
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


			var source = new StringSourceCode(new ParsingContextStreamFactory(), new SourceCodeFragmentFactory(), expression);

			var rootNode = functionArg.TryParse(source.GetFurtherContext());


			Console.WriteLine($"{rootNode != null}");
			Console.ReadLine();




		}
	}
}
