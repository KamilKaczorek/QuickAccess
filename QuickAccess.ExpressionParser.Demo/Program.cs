using QuickAccess.DataStructures.Common.RegularExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using QuickAccess.DataStructures.Algebra;
using QuickAccess.DataStructures.Common.Collections;
using QuickAccess.DataStructures.Common.Freezable;
using QuickAccess.DataStructures.Common.ValueContract;
using QuickAccess.Parser;
using QuickAccess.Parser.Flexpressions;
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


        public static class FXSpecification
        {
            public static FXSpecification<TConstraint> Create<TConstraint>()
                where TConstraint : IFlexpressionConstraint
            {
                return new FXSpecification<TConstraint>(new Dictionary<string, GroupFlexpressionDefinition<TConstraint>>());
            }

            public static FXSpecification<TConstraint> Create<TConstraint>(IEqualityComparer<string> groupNameComparer)
                where TConstraint : IFlexpressionConstraint
            {
                return new FXSpecification<TConstraint>(
                    new Dictionary<string, GroupFlexpressionDefinition<TConstraint>>(groupNameComparer));
            }

            public static FXSpecification<TConstraint> Create<TConstraint>(
                IEnumerable<IDefineGroupFlexpression<TConstraint>> predefinedOverwritableGroups,
                IEnumerable<IDefineGroupFlexpression<TConstraint>> predefinedSealedGroups,
                IEqualityComparer<string> groupNameComparer = null)
                where TConstraint : IFlexpressionConstraint
            {
                predefinedOverwritableGroups ??= Array.Empty<IDefineGroupFlexpression<TConstraint>>();
                predefinedSealedGroups ??= Array.Empty<IDefineGroupFlexpression<TConstraint>>();

                groupNameComparer ??= StringComparer.Ordinal;

                var groups = predefinedOverwritableGroups
                    .Where(p => p.IsDefined)
                    .Select(p =>
                        new GroupFlexpressionDefinition<TConstraint>(p.GroupName,
                            AutoFreezingValue.CreateDefinedNotFrozen(p.Content)))
                    .Concat(predefinedSealedGroups
                        .Where(p => p.IsDefined)
                        .Select(p =>
                            new GroupFlexpressionDefinition<TConstraint>(p.GroupName,
                                AutoFreezingValue.CreateDefinedFrozen(p.Content), isSealed:true)))
                    .ToDictionary(
                        pK => pK.GroupName,
                        pV => pV,
                        groupNameComparer);

                var res = new FXSpecification<TConstraint>(groups);
                return res;
            }

            public static FXSpecification<TConstraint> Create<TConstraint>(
                IEnumerable<IDefineGroupFlexpression<TConstraint>> predefinedGroups,
                IEqualityComparer<string> groupNameComparer = null)
                where TConstraint : IFlexpressionConstraint
            {
                var groupsBySealed = predefinedGroups.ToLookup(p => p.IsSealed);

                var res = Create(groupsBySealed[false], groupsBySealed[true], groupNameComparer);
                return res;
            }
        }


        public class FXSpecification<TConstraint> where TConstraint : IFlexpressionConstraint
        {
            private readonly Dictionary<string, GroupFlexpressionDefinition<TConstraint>> _groupsDefinitionsByName;
            private readonly Dictionary<string, GroupPlaceholder<TConstraint>> _groupsPlaceholdersByName;

            internal FXSpecification(Dictionary<string, GroupFlexpressionDefinition<TConstraint>> groupsByName)
            {
                _groupsDefinitionsByName = groupsByName ?? new Dictionary<string, GroupFlexpressionDefinition<TConstraint>>();
                _groupsPlaceholdersByName = _groupsDefinitionsByName.Values.ToDictionary(pK => pK.GroupName,
                    pV => new GroupPlaceholder<TConstraint>(pV.GroupName));
            }

            public Flexpression<TConstraint> Text(string str)
            {
                return StringFlexpression.Create<TConstraint>(str);
            }

            public Flexpression<TConstraint> DefineGroup(string groupName, IFlexpression<TConstraint> content)
            {
                var group = _groupsDefinitionsByName.GetExistingValueOrNew(
                    groupName,
                    pName => new GroupFlexpressionDefinition<TConstraint>(pName, AutoFreezingValue.CreateUndefined<IFlexpression<TConstraint>>()));

                if (content != null)
                {
                    group.ContentContainer.Set(content);
                }

                return group;
            }

            public Flexpression<TConstraint> this[string groupName]
            {
                get => _groupsPlaceholdersByName.GetExistingValueOrNew(groupName, pName => new GroupPlaceholder<TConstraint>(pName));
                set => DefineGroup(groupName, value);
            }

            public Flexpression<TConstraint> Char(char c)
            {
                return CharFlexpression.Create<TConstraint>(c);
            }

            private readonly Dictionary<OverloadableCodeBinarySymmetricOperator, Func<Flexpression<TConstraint>, Flexpression<TConstraint>, Flexpression<TConstraint>>>

                _binOperatorDefinitions = new Dictionary<OverloadableCodeBinarySymmetricOperator,Func<Flexpression<TConstraint>,Flexpression<TConstraint>,Flexpression<TConstraint>>>();

            public Func<Flexpression<TConstraint>, Flexpression<TConstraint>, Flexpression<TConstraint>> this [
                Func<OperatorDefinitionArg, OperatorDefinitionArg, OperatorDefinition> operatorSelector]
            {
                set
                {
                    var selectedOperator = operatorSelector.Invoke(new OperatorDefinitionArg(), new OperatorDefinitionArg()).Operator;
                    _binOperatorDefinitions[selectedOperator] = value;
                }
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
            var b = FXSpecification.Create<ParsingConstraint>();

            var c = FXSpecification.Create<ParsingExt>();

            b["abc"] = b.Text("asda") | b.Char('c') + b["zzz"] + c["aaa"];
            b["zzz"] = b.Text("zzz");


            b[(x, y) => x + y] = (x, y) => x & b.Text("_") & y;


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
