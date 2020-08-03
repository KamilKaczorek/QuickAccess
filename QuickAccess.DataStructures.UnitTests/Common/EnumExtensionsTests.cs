using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickAccess.DataStructures.Common;
using UUT = QuickAccess.DataStructures.Common.EnumExtensions;

namespace QuickAccess.DataStructures.UnitTests.Common
{
    
    [TestClass]
    public class EnumExtensionsTests
    {
        [TestMethod]
        [DataRow(FlagCheckResult.All, IntEnum.All, IntEnum.All)]
        [DataRow(FlagCheckResult.All, IntEnum.All, IntEnum.Item1)]
        [DataRow(FlagCheckResult.All, IntEnum.Item1 | IntEnum.Item2, IntEnum.Item1 | IntEnum.Item2)]
        [DataRow(FlagCheckResult.All, IntEnum.Item1 | IntEnum.Item4, IntEnum.Item1 | IntEnum.Item4)]
        [DataRow(FlagCheckResult.All, IntEnum.Item1 | IntEnum.Item4 | IntEnum.Item3, IntEnum.Item1 | IntEnum.Item4)]
        [DataRow(FlagCheckResult.NotAll, IntEnum.Item1 | IntEnum.Item3, IntEnum.Item1 | IntEnum.Item4)]
        [DataRow(FlagCheckResult.None, IntEnum.Item1 | IntEnum.Item3, IntEnum.Item4)]
        [DataRow(FlagCheckResult.None, IntEnum.Item1 | IntEnum.Item3, IntEnum.Item2 | IntEnum.Item4)]
        [DataRow(FlagCheckResult.None, IntEnum.Item1 | IntEnum.Item3, IntEnum.None)]
        public void ON_EvaluateContainsFlags_SHOULD_Return(FlagCheckResult expected,  IntEnum source, IntEnum flags)
        {
            var actual = UUT.EvaluateContainsFlags(source, flags);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow(FlagCheckResult.All, ByteEnum.All, ByteEnum.All)]
        [DataRow(FlagCheckResult.All, ByteEnum.All, ByteEnum.Item1)]
        [DataRow(FlagCheckResult.All, ByteEnum.Item1 | ByteEnum.Item2, ByteEnum.Item1 | ByteEnum.Item2)]
        [DataRow(FlagCheckResult.All, ByteEnum.Item1 | ByteEnum.Item4, ByteEnum.Item1 | ByteEnum.Item4)]
        [DataRow(FlagCheckResult.All, ByteEnum.Item1 | ByteEnum.Item4 | ByteEnum.Item3, ByteEnum.Item1 | ByteEnum.Item4)]
        [DataRow(FlagCheckResult.NotAll, ByteEnum.Item1 | ByteEnum.Item3, ByteEnum.Item1 | ByteEnum.Item4)]
        [DataRow(FlagCheckResult.None, ByteEnum.Item1 | ByteEnum.Item3, ByteEnum.Item4)]
        [DataRow(FlagCheckResult.None, ByteEnum.Item1 | ByteEnum.Item3, ByteEnum.Item2 | ByteEnum.Item4)]
        [DataRow(FlagCheckResult.None, ByteEnum.Item1 | ByteEnum.Item3, ByteEnum.None)]
        public void ON_EvaluateContainsFlags_WHEN_Underlying_Enum_Type_Byte_SHOULD_Return(FlagCheckResult expected, ByteEnum source, ByteEnum flags)
        {
            var actual = UUT.EvaluateContainsFlags(source, flags);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        [DataRow(FlagCheckResult.All, UIntEnum.All, UIntEnum.All)]
        [DataRow(FlagCheckResult.All, UIntEnum.All, UIntEnum.Item1)]
        [DataRow(FlagCheckResult.All, UIntEnum.Item1 | UIntEnum.Item2, UIntEnum.Item1 | UIntEnum.Item2)]
        [DataRow(FlagCheckResult.All, UIntEnum.Item1 | UIntEnum.Item4, UIntEnum.Item1 | UIntEnum.Item4)]
        [DataRow(FlagCheckResult.All, UIntEnum.Item1 | UIntEnum.Item4 | UIntEnum.Item3, UIntEnum.Item1 | UIntEnum.Item4)]
        [DataRow(FlagCheckResult.NotAll, UIntEnum.Item1 | UIntEnum.Item3, UIntEnum.Item1 | UIntEnum.Item4)]
        [DataRow(FlagCheckResult.None, UIntEnum.Item1 | UIntEnum.Item3, UIntEnum.Item4)]
        [DataRow(FlagCheckResult.None, UIntEnum.Item1 | UIntEnum.Item3, UIntEnum.Item2 | UIntEnum.Item4)]
        [DataRow(FlagCheckResult.None, UIntEnum.Item1 | UIntEnum.Item3, UIntEnum.None)]
        public void ON_EvaluateContainsFlags_WHEN_Underlying_Enum_Type_UInt_SHOULD_Return(FlagCheckResult expected, UIntEnum source, UIntEnum flags)
        {
            var actual = UUT.EvaluateContainsFlags(source, flags);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow(FlagCheckResult.All, LongEnum.All, LongEnum.All)]
        [DataRow(FlagCheckResult.All, LongEnum.All, LongEnum.Item1)]
        [DataRow(FlagCheckResult.All, LongEnum.Item1 | LongEnum.Item2, LongEnum.Item1 | LongEnum.Item2)]
        [DataRow(FlagCheckResult.All, LongEnum.Item1 | LongEnum.Item4, LongEnum.Item1 | LongEnum.Item4)]
        [DataRow(FlagCheckResult.All, LongEnum.Item1 | LongEnum.Item4 | LongEnum.Item3, LongEnum.Item1 | LongEnum.Item4)]
        [DataRow(FlagCheckResult.NotAll, LongEnum.Item1 | LongEnum.Item3, LongEnum.Item1 | LongEnum.Item4)]
        [DataRow(FlagCheckResult.None, LongEnum.Item1 | LongEnum.Item3, LongEnum.Item4)]
        [DataRow(FlagCheckResult.None, LongEnum.Item1 | LongEnum.Item3, LongEnum.Item2 | LongEnum.Item4)]
        [DataRow(FlagCheckResult.None, LongEnum.Item1 | LongEnum.Item3, LongEnum.None)]
        public void ON_EvaluateContainsFlags_WHEN_Underlying_Enum_Type_Long_SHOULD_Return(FlagCheckResult expected, LongEnum source, LongEnum flags)
        {
            var actual = UUT.EvaluateContainsFlags(source, flags);
            Assert.AreEqual(expected, actual);
        }

        [Flags]
        public enum ByteEnum : byte
        {
            None = 0,
            Item1 = 0x01,
            Item2 = 0x02,
            Item3 = 0x04,
            Item4 = 0x08,


            All = Item1 | Item2 | Item3 | Item4,
        }

        [Flags]
        public enum IntEnum
        {
            None = 0,
            Item1 = 0x01,
            Item2 = 0x02,
            Item3 = 0x04,
            Item4 = 0x08_00_00_00,


            All = Item1 | Item2 | Item3 | Item4,
        }

        [Flags]
        public enum UIntEnum : uint
        {
            None = 0,
            Item1 = 0x01,
            Item2 = 0x02,
            Item3 = 0x04,
            Item4 = 0x80_00_00_00,


            All = Item1 | Item2 | Item3 | Item4,
        }

        [Flags]
        public enum LongEnum : long
        {
            None = 0,
            Item1 = 0x01,
            Item2 = 0x02,
            Item3 = 0x04,
            Item4 = 0x08_00_00_00_00_00,


            All = Item1 | Item2 | Item3 | Item4,
        }
    }
}
