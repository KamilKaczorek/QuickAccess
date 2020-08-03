using QuickAccess.DataStructures.Common.RegularExpression;
using System;
using System.Diagnostics;
using System.Threading;
using QuickAccess.DataStructures.Common.CharMatching;
using QuickAccess.Parser;
using QuickAccess.Parser.Flexpressions;
using QuickAccess.Parser.Flexpressions.Model;

namespace QuickAccess.ExpressionParser.Demo
{
	class Program
	{
        static void Main()
        {
            var part =
                "10 + (12.1 - 4) * 8 + 20 * 30 + 8 + (3 + 5 * (2 - 1) / 12.4)-12.1213*(1- 5)*(10 + (12.1 - 4) * 8 + 20 * 30 + 8 + (3 + 5 * (2 - 1) / 12.4)-12.1213*(1- 5))";


            var source = $"{part}+{part}/{part}*{part}";

            var fun = $"FooFunction12312FunctionCosSinArg(232, 3223, 232, 111, 23.2323, 323.112, 343, 232); ";

            var count = 1000;
            var st = new Stopwatch();
            TestFlex();

            

            foreach (var option in new[] {ParsingOptions.None, ParsingOptions.Cache})
            {
                Console.WriteLine($"---- {option} ----");
                Thread.Sleep(0);
                

                st.Start();
                var start = DateTime.UtcNow;

                Test2(source, count, option);
                Test(fun, count, option);

                
                st.Stop();
                var stop = DateTime.UtcNow;
                var total = stop - start;
                Console.WriteLine($"CPUTime   = {st.Elapsed}");
                Console.WriteLine($"TotalTime = {total}");
                Console.WriteLine($"---- {new string('-', option.ToString().Length)} ----");
                
                st.Reset();
            }
        }



        public static void Test2(string expression, int count = 1, ParsingOptions options = ParsingOptions.Cache)
        {
			var fx = new StandardFlexpressionAlgebra(-1);

            var intNumber = fx.Digit.OneOrMore().DefinesSealedRule("Integer", "Integer");
            var floatNumber = (intNumber + "." + intNumber).DefinesSealedRule("Float", "Float");
            var number = (intNumber | floatNumber).DefinesRule("Number");

            var sumOper = FXB.ToCharacter('+');
            var minOper = FXB.ToCharacter('-');
            var mulOper = FXB.ToCharacter('*');
            var divOper = FXB.ToCharacter('/');

            var oper = (sumOper | minOper | mulOper | divOper).DefinesSealedRule("Operator");

            var exprNoOperation = ("BracketExpression".Rule() | number).DefinesRule("ExpressionNoOperation");

			var operation = exprNoOperation & oper & "Expression".Rule();

            var expr = (operation | exprNoOperation).DefinesRule("Expression");

            (FXB.ToCharacter('(') & expr & FXB.ToCharacter(')')).DefinesSealedRule("BracketExpression");


            var ctx = RegularExpressionBuildingContext.CreateStandard();
            var regularExpressionString = expr.ToRegularExpressionString(ctx);

            //Console.WriteLine($"Regex: {regularExpressionString}");


            var source = new StringSourceCode(new ParsingContextStreamFactory(new ProductFactory(), fx), new SourceCodeFragmentFactory(),  expression);

            for (var idx = 0; idx < count; ++idx)
            {
                var rootNode = expr.TryParse(source.GetFurtherContext(), options);
            }

            //Console.WriteLine(expr);
            //Console.WriteLine($"{rootNode != null}");
            //Console.ReadLine();

		}

        


        
        

       
        public class ParsingConstraint : CustomFXConstraint
        {

        }

        public class ParsingExt : ParsingConstraint
        {

        }

        
		public static void TestFlex()
        {
            var b = FXSpecification.Create();

            var c = FXSpecification.Create();

            var z = b["abc"] = b.Text("asda") | b.Char('c') + b["zzz"] + c["aaa"];
            b["zzz"] = b.Text("zzz") + z;

            b["WhiteSpaceChar"] = StandardCharactersRange.WhiteSpace;

            b["WhiteSpace"] = b["WhiteSpaceChar"][1, long.MaxValue];

            b[x => ~x] = x => x[0, 1];



            b[(x, y) => x + y] = (x, y) => x & ~b["WhiteSpace"] & y;
            b[(x, y) => x ^ y] = (x, y) => x & b["WhiteSpace"] & y;

            b[x => +x] = x => x;


            var str = b["zzz"].ToString();
            //Console.WriteLine(str);
            //Console.ReadLine();
        }

		public static void Test(string expression, int count = 1, ParsingOptions options = ParsingOptions.Cache)
		{

			var name = (FXB.Letter + (FXB.Digit | FXB.Letter).ZeroOrMore()).DefinesSealedRule("Name", "String");
			var intNumber = FXB.Digit.OneOrMore().DefinesSealedRule("Integer", "Integer");
			var floatNumber = (intNumber + "." + intNumber).DefinesSealedRule("Float", "Float");
			var functionArg = (floatNumber | intNumber | name).DefinesSealedRule("FunctionArg");
			var functionArgList = (functionArg & ("," & functionArg).ZeroOrMore()).DefinesRule("FunctionArgList");
			var functionInvocation = (name & "(" & ~functionArgList & ")" & ';').DefinesSealedRule("FunctionInvocation");

			var ctx = RegularExpressionBuildingContext.CreateStandard();
			//var regularExpressionString = functionInvocation.ToRegularExpressionString(ctx);

			//var regex = new Regex(regularExpressionString, RegexOptions.Compiled);

			//var res = regex.IsMatch(expression);



			var source = new StringSourceCode(new ParsingContextStreamFactory(new ProductFactory(), FXB.DefaultAlgebra), new SourceCodeFragmentFactory(), expression);

            for (var idx = 0; idx < count; ++idx)
            {
                var rootNode = functionInvocation.TryParse(source, options);

                if (rootNode == null)
                {
                    throw new InvalidOperationException($"Parsing error {source.GetError()}");
                }
            }

           
			
        }
	}
}
