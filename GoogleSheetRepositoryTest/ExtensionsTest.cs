using GoogleSheetRepository.Extensions;

namespace GoogleSheetRepository.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(1,"A")]
        [TestCase(3, "C")]
        [TestCase(27, "AA")]
        [TestCase(54, "BB")]
        [TestCase(752, "ABX")]
        public void ExtensionTest_GetColumnAddress(int columnNumber, string expectedAddress)
        {
            Assert.AreEqual(expectedAddress, columnNumber.ConvertToColumnAddress());
        }
    }
}