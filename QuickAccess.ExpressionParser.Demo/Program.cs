using QuickAccess.DataStructures.Common.RegularExpression;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.DataStructures.Common.Freezable;
using QuickAccess.DataStructures.Common.ValueContract;
using QuickAccess.Parser;
using QuickAccess.Parser.Flexpressions;
using QuickAccess.Parser.Flexpressions.Bricks;
using QuickAccess.Parser.Flexpressions.Model;

namespace QuickAccess.ExpressionParser.Demo
{
	class Program
	{
		static void Main()
        {

            Test("Cos(12, 23, 34);");
			Test2("10+(12.1 - 4)*8");
            TestFlex();
        }

        

        public static void Test2(string expression)
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

            Console.WriteLine($"Regex: {regularExpressionString}");


            var source = new StringSourceCode(new ParsingContextStreamFactory(new ProductFactory(), fx), new SourceCodeFragmentFactory(),  expression);

            var rootNode = expr.TryParse(source.GetFurtherContext());

            Console.WriteLine(expr);
            Console.WriteLine($"{rootNode != null}");
            Console.ReadLine();

		} 

        public class FXSpecification<TConstraint> where TConstraint : IFlexpressionConstraint
        {
            private readonly Dictionary<string, GroupFlexpression<TConstraint>> _groupsByName;
                                
            public FXSpecification()
            {
                _groupsByName = new Dictionary<string, GroupFlexpression<TConstraint>>();
            }

            public Flexpression<TConstraint> Text(string str)
            {
                return StringFlexpression.Create<TConstraint>(str);
            }

            public Flexpression<TConstraint> DefineGroup(string groupName, IFlexpression<TConstraint> content)
            {
                var group = _groupsByName.GetExistingValueOrNew(
                    groupName,
                    pName => new GroupFlexpression<TConstraint>(pName, AutoFreezingValue.CreateUndefined<IFlexpression<TConstraint>>()));

                if (content != null)
                {
                    group.ContentContainer.Set(content);
                }

                return group;
            }

            public Flexpression<TConstraint> this[string groupName]
            {
                get => DefineGroup(groupName, null);
                set => DefineGroup(groupName, value);
            }

            public Flexpression<TConstraint> Char(char c)
            {
                return CharFlexpression.Create<TConstraint>(c);
            }
        }

        public class ParsingConstraint : CustomFXConstraint
        {

        }

        public class ParsingExt : ParsingConstraint
        {

        }

		public static void TestFlex()
        {
            var b = new FXSpecification<ParsingConstraint>();

            var c = new FXSpecification<ParsingExt>();

            b["abc"] = b.Text("asda") | b.Char('c') + b["zzz"] + c["aaa"];
            b["zzz"] = b.Text("zzz");



            var str = b["zzz"].ToString();
            Console.WriteLine(str);
            Console.ReadLine();
        }

		public static void Test(string expression)
		{
			var _ = (new CharFlexpression<DefaultFlexpressionConstraint>('c') + "dsdsa" | 'c')[1, 2];

			var name = (FXB.Letter + (FXB.Digit | FXB.Letter).ZeroOrMore()).DefinesSealedRule("Name", "String");
			var intNumber = FXB.Digit.OneOrMore().DefinesSealedRule("Integer", "Integer");
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

			var source = new StringSourceCode(new ParsingContextStreamFactory(new ProductFactory(), FXB.DefaultAlgebra), new SourceCodeFragmentFactory(), expression);

			var rootNode = functionInvocation.TryParse(source.GetFurtherContext());

			Console.WriteLine(functionInvocation);
			Console.WriteLine($"{rootNode != null}");
			Console.ReadLine();
        }
	}
}
