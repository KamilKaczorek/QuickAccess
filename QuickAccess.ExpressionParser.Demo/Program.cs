using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using QuickAccess.Infrastructure.CharMatching.Categories;
using QuickAccess.Infrastructure.Guards;
using QuickAccess.Infrastructure.RegularExpression;
using QuickAccess.Infrastructure.ValueContract;
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

                //Test2(source, count, option);
                ParseFunctionDescriptorUsingFlexpressionBricksAndProvideRegEx(fun, count, option);


                
                st.Stop();
                var stop = DateTime.UtcNow;
                var total = stop - start;
                Console.WriteLine($"CPUTime   = {st.Elapsed}");
                Console.WriteLine($"TotalTime = {total}");
                Console.WriteLine($"---- {new string('-', option.ToString().Length)} ----");
                
                st.Reset();
            }
        }



        public static void ParseMathExpressionWithFlexpressionBricks(string expression, int performanceTestExecutionsCount = 1, ParsingOptions options = ParsingOptions.Cache)
        {
            Console.WriteLine();
            Console.WriteLine("Math expression demo using Flexpression Bricks");
            Console.WriteLine($"Expression: {expression}");

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

            for (var idx = 0; idx < performanceTestExecutionsCount; ++idx)
            {
                var rootNode = expr.TryParse(source.GetFurtherContext(), options);
            }
        }

        public static void ParseFunctionDescriptorUsingFlexpressionBricksAndProvideRegEx(string expression, int performanceTestExecutionsCount = 1, ParsingOptions options = ParsingOptions.Cache)
        {
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

            var source = new StringSourceCode(new ParsingContextStreamFactory(new ProductFactory(), FXB.DefaultAlgebra), new SourceCodeFragmentFactory(), expression);

            for (var idx = 0; idx < performanceTestExecutionsCount; ++idx)
            {
                var rootNode = functionInvocation.TryParse(source, options);

                if (rootNode == null)
                {
                    throw new InvalidOperationException($"Parsing error {source.GetError()}");
                }
            }
			
        }

        

        
		public static void TestFlex()
        {
            var b = FXSpecification.Create();

            var c = FXSpecification.Create();

            var z = b["abc"] = b.Text("asda") | b.Char('c') + b["zzz"] + c["aaa"];
            b["zzz"] = b.Text("zzz") + z;

            b["WhiteSpaceChar"] = StandardCharacterCategories.WhiteSpace;

            b["WhiteSpace"] = b["WhiteSpaceChar"][1, long.MaxValue];

            b[x => ~x] = x => x[0, 1];



            b[(x, y) => x + y] = (x, y) => x & ~b["WhiteSpace"] & y;
            b[(x, y) => x ^ y] = (x, y) => x & b["WhiteSpace"] & y;

            b[x => +x] = x => x;


            var str = b["zzz"].ToString();
            //Console.WriteLine(str);
            //Console.ReadLine();
        }

		
	}

     public readonly struct PerformanceTime : ICanBeUndefined,
        IComparable<PerformanceTime>, IComparable, IEquatable<PerformanceTime>
    {
        private readonly TimeSpan _executionsTime;
        private readonly int _numberOfExecutions;

        public static readonly PerformanceTime Undefined = new PerformanceTime(0, TimeSpan.Zero, null);


        public static PerformanceTime Create(int numberOfExecutions, TimeSpan executionsTime, string operationName = null)
        {
            Guard.ArgGreaterThan(numberOfExecutions, nameof(numberOfExecutions), 0);
            Guard.ArgEqualOrGreaterThan(executionsTime, nameof(executionsTime), TimeSpan.Zero);

            return new PerformanceTime(numberOfExecutions, executionsTime, operationName);
        }

        private PerformanceTime(int numberOfExecutions, TimeSpan executionsTime, string operationName)
        {
            _numberOfExecutions = numberOfExecutions;
            _executionsTime = executionsTime;
            _operationName = operationName;
        }

        public TimeSpan SingleExecutionTime
        {
            get
            {
                ValidateIsDefined();

                if (_executionsTime.Ticks <= 0)
                {
                    return TimeSpan.Zero;
                }

                return _executionsTime / _numberOfExecutions;
            }
        }

        public double SingleExecutionTimeInMilliseconds
        {
            get
            {
                ValidateIsDefined();

                if (_executionsTime.Ticks <= 0)
                {
                    return 0.0;
                }

                return _executionsTime.TotalMilliseconds / _numberOfExecutions;
            }
        }

        public double NumberOfExecutionsPerSecond
        {
            get
            {
                ValidateIsDefined();

                if (_executionsTime <= TimeSpan.Zero)
                {
                    return double.PositiveInfinity;
                }
                
                return _numberOfExecutions / _executionsTime.TotalSeconds;
            }
        }

        public double NumberOfExecutionsPerMillisecond
        {
            get
            {
                ValidateIsDefined();

                if (_executionsTime <= TimeSpan.Zero)
                {
                    return double.PositiveInfinity;
                }
                
                return _numberOfExecutions / _executionsTime.TotalMilliseconds;
            }
        }

        public double NumberOfExecutionsPerTicks
        {
            get
            {
                ValidateIsDefined();

                if (_executionsTime <= TimeSpan.Zero)
                {
                    return double.PositiveInfinity;
                }
                
                return _numberOfExecutions / (double)_executionsTime.Ticks;
            }
        }

        public double GetNumberOfExecutionsPer(TimeSpan timeRange)
        {
            ValidateIsDefined();

            if (_executionsTime <= TimeSpan.Zero)
            {
                return double.PositiveInfinity;
            }

            var factor = timeRange / _executionsTime;
                
            return _numberOfExecutions * factor;
        }

        [DebuggerHidden, DebuggerStepThrough]
        private void ValidateIsDefined()
        {
            if (!IsDefined)
            {
                throw new InvalidOperationException("Can't provide value - the performance time is undefined.");
            }
        }

        private readonly string _operationName;

        public string OperationName => IsDefined ? _operationName : "<UNDEFINED>";

        public bool IsDefined => _numberOfExecutions > 0;

        public int CompareTo(PerformanceTime other)
        {
            var cmp = IsDefined.CompareTo(other.IsDefined);

            if (cmp != 0)
            {
                return cmp;
            }

            return SingleExecutionTimeInMilliseconds.CompareTo(other.SingleExecutionTimeInMilliseconds);
        }

        public int CompareTo(object obj)
        {
            if(obj is null) return 1;
            return obj is PerformanceTime other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(PerformanceTime)}");
        }

        public static bool operator <(PerformanceTime left, PerformanceTime right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(PerformanceTime left, PerformanceTime right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(PerformanceTime left, PerformanceTime right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(PerformanceTime left, PerformanceTime right)
        {
            return left.CompareTo(right) >= 0;
        }

        public bool Equals(PerformanceTime other)
        {
            if (IsDefined != other.IsDefined)
            {
                return false;
            }

            if (!IsDefined)
            {
                return true;
            }

            return SingleExecutionTime.Equals(other.SingleExecutionTime);
        }

        public override bool Equals(object obj)
        {
            return obj is PerformanceTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            return SingleExecutionTime.GetHashCode();
        }

        public static bool operator ==(PerformanceTime left, PerformanceTime right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PerformanceTime left, PerformanceTime right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            if (!IsDefined)
            {
                return "<UNDEFINED>";
            }

            return $"AVGExecutionTime({OperationName})='{SingleExecutionTime}'";
        }
    }

    public class PerformanceMeasurementExecutor
    {
        public PerformanceMeasurementExecutor() : this(TimeSpan.FromMilliseconds(100))
        {
        }

        public PerformanceMeasurementExecutor(TimeSpan minimalExecutionTime)
        {
            MinimalTestTime = minimalExecutionTime;
        }

        public TimeSpan MinimalTestTime { get; }

        public PerformanceTime MeasureActionPerformance(Action action)
        {
            var stopWatch = new Stopwatch();

            var executionCount = 1;

            Thread.Sleep(0);

            while (true)
            {
                var executionTime = MeasureActionPerformance(action, executionCount, stopWatch);

                if (executionTime >= MinimalTestTime)
                {
                    return PerformanceTime.Create(executionCount, executionTime);
                }

                if (executionTime <= TimeSpan.Zero)
                {
                    executionCount *= 100;
                }
                else
                {
                    executionCount *= (int) Math.Max(10, MinimalTestTime.Ticks / executionTime.Ticks);
                }
            }
        }


        private TimeSpan MeasureActionPerformance(Action action, int executionCount, Stopwatch stopwatch)
        {
            stopwatch.Reset();

            stopwatch.Start();

            for (var idx = executionCount; idx > 0; --idx)
            {
                action.Invoke();
            }

            stopwatch.Stop();

            return stopwatch.Elapsed;
        }
    }


}
