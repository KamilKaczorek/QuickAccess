using QuickAccess.DataStructures.Common.RegularExpression;
using QuickAccess.Parser.SmartExpressions;
using System;
using System.Text.RegularExpressions;
using QuickAccess.Parser;

namespace QuickAccess.ExpressionParser.Demo
{
	class Program
	{
		static void Main()
		{
			Test("Cos(12, 23, 34);");
			Test2("10+(12.1 - 4)*8");
		}

        public static void Test2(string expression)
        {
            var intNumber = SX.Digit.OneOrMore().DefinesSealedRule("Integer", "Integer");
            var floatNumber = (intNumber + "." + intNumber).DefinesSealedRule("Float", "Float");
            var number = (intNumber | floatNumber).DefinesRule("Number");

            var sumOper = SX.ToCharacter('+');
            var minOper = SX.ToCharacter('-');
            var mulOper = SX.ToCharacter('*');
            var divOper = SX.ToCharacter('/');

            var oper = (sumOper | minOper | mulOper | divOper).DefinesSealedRule("Operator");

            var exprNoOperation = ("BracketExpression".Rule() | number).DefinesRule("ExpressionNoOperation");

			var operation = exprNoOperation & oper & "Expression".Rule();

            var expr = (operation | exprNoOperation).DefinesRule("Expression");

            (SX.ToCharacter('(') & expr & SX.ToCharacter(')')).DefinesSealedRule("BracketExpression");


            var ctx = RegularExpressionBuildingContext.CreateStandard();
            var regularExpressionString = expr.ToRegularExpressionString(ctx);

            Console.WriteLine($"Regex: {regularExpressionString}");


            var source = new StringSourceCode(new ParsingContextStreamFactory(), new SourceCodeFragmentFactory(), new ProductFactory(),  expression);

            var rootNode = expr.TryParse(source.GetFurtherContext());

            Console.WriteLine(expr);
            Console.WriteLine($"{rootNode != null}");
            Console.ReadLine();

		}

		public static void Test(string expression)
		{
			var name = (SX.Letter + (SX.Digit | SX.Letter).ZeroOrMore()).DefinesSealedRule("Name", "String");
			var intNumber = SX.Digit.OneOrMore().DefinesSealedRule("Integer", "Integer");
			var floatNumber = (intNumber + "." + intNumber).DefinesSealedRule("Float", "Float");
			var functionArg = (floatNumber | intNumber | name).DefinesSealedRule("FunctionArg");
			var functionArgList = (functionArg & ("," & functionArg).ZeroOrMore()).DefinesRule("FunctionArgList");
			var functionInvocation = (name & "(" & ~functionArgList & ")" & ';').DefinesSealedRule("FunctionInvocation");

			var ctx = RegularExpressionBuildingContext.CreateStandard();
			var regularExpressionString = functionInvocation.ToRegularExpressionString(ctx);

			var regex = new Regex(regularExpressionString, RegexOptions.Compiled);

			var res = regex.IsMatch(expression);

			Console.WriteLine($"Regex: {regularExpressionString}");
			Console.WriteLine($"Regex result: {res}");


			var source = new StringSourceCode(new ParsingContextStreamFactory(), new SourceCodeFragmentFactory(), new ProductFactory(), expression);

			var rootNode = functionInvocation.TryParse(source.GetFurtherContext());

			Console.WriteLine(functionInvocation);
			Console.WriteLine($"{rootNode != null}");
			Console.ReadLine();
        }
	}
}
