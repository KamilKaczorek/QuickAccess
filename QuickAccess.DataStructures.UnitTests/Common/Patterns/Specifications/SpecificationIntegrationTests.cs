using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickAccess.Infrastructure.Patterns.Specifications;

namespace QuickAccess.DataStructures.UnitTests.Common.Patterns.Specifications
{
	[TestClass]
	public class SpecificationIntegrationTests
	{
		[TestMethod]
		public void ON_IsSatisfied_WHEN_Operators_Applied_SHOULD_ProperResultEvaluated()
		{
			// Arrange
			var isDivisibleBy10 = Specification.FromPredicate<int>( x => x % 10 == 0);
			var isDivisibleBy3 = Specification.FromPredicate<int>( x => x % 3 == 0);
			var isDivisibleBy7 = Specification.FromPredicate<int>( x => x % 7 == 0);
			var isGreaterThan10 = Specification.FromPredicate<int>( x => x > 10);
			var isGreaterThan100 = Specification.FromPredicate<int>( x => x > 100);

			var spec = !(isGreaterThan10 & isDivisibleBy3 | isDivisibleBy10 & isGreaterThan100 & !isDivisibleBy7) | (x => x % 51 == 0);

			for (var x = 0; x < 150; x++)
			{
				var expectedRes = !(x > 10 && x % 3 == 0 || x % 10 == 0 && x > 100 && x % 7 != 0) || x % 51 == 0;

				// Act
				var res = spec.IsSatisfiedBy(x);

				// Assert
				Assert.AreEqual(res, expectedRes);
			}
		}

		[TestMethod]
		[DataRow(true, true, true, true)]
		[DataRow(false, false, true, false)]
		[DataRow(false, false, false, true)]
		[DataRow(false, false, false, false)]
		[DataRow(false, true, false, true)]
		public void ON_CodeOperatorAnd_WHEN_SpecifiedSpecsResults_SHOULD_ReturnExpectedResult(bool expectedResult, bool spec1Res, bool spec2Res, bool spec3Res)
		{
			// Arrange
			var spec1 = Specification.FromPredicate<int>(c => spec1Res);
			var spec2 = Specification.FromPredicate<int>(c => spec2Res);
			var spec3 = Specification.FromPredicate<int>(c => spec3Res);

			// Act
			var spec = spec1 & spec2 & spec3;

			// Assert
			var res = spec.IsSatisfiedBy(0);
			Assert.AreEqual(expectedResult, res);
		}

		[TestMethod]
		[DataRow(true, true, true, true)]
		[DataRow(true, false, true, false)]
		[DataRow(true, false, false, true)]
		[DataRow(false, false, false, false)]
		[DataRow(true, true, false, true)]
		public void ON_CodeOperatorOr_WHEN_SpecifiedSpecsResults_SHOULD_ReturnExpectedResult(bool expectedResult, bool spec1Res, bool spec2Res, bool spec3Res)
		{
			// Arrange
			var spec1 = Specification.FromPredicate<int>(c => spec1Res);
			var spec2 = Specification.FromPredicate<int>(c => spec2Res);
			var spec3 = Specification.FromPredicate<int>(c => spec3Res);

			// Act
			var spec = spec1 | spec2 | spec3;

			// Assert
			var res = spec.IsSatisfiedBy(0);
			Assert.AreEqual(expectedResult, res);
		}

		[TestMethod]
		[DataRow(true, true, true, true)]
		[DataRow(true, false, true, false)]
		[DataRow(true, false, false, true)]
		[DataRow(false, false, false, false)]
		[DataRow(false, true, false, true)]
		public void ON_CodeOperatorXOr_WHEN_SpecifiedSpecsResults_SHOULD_ReturnExpectedResult(bool expectedResult, bool spec1Res, bool spec2Res, bool spec3Res)
		{
			// Arrange
			var spec1 = Specification.FromPredicate<int>(c => spec1Res);
			var spec2 = Specification.FromPredicate<int>(c => spec2Res);
			var spec3 = Specification.FromPredicate<int>(c => spec3Res);

			// Act
			var spec = spec1 ^ spec2 ^ spec3;

			// Assert
			var res = spec.IsSatisfiedBy(0);
			Assert.AreEqual(expectedResult, res);
		}

		[TestMethod]
		[DataRow(true, false)]
		[DataRow(false, true)]
		public void ON_CodeOperatorNot_WHEN_SpecifiedSpecsResult_SHOULD_ReturnExpectedResult(bool expectedResult, bool spec1Res)
		{
			// Arrange
			var spec1 = Specification.FromPredicate<int>(c => spec1Res);

			// Act
			var spec = !spec1;

			// Assert
			var res = spec.IsSatisfiedBy(0);
			Assert.AreEqual(expectedResult, res);
		}
	}
}
