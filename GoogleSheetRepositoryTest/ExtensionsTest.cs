using GoogleSheetRepository.Extensions;
using GoogleSheetRepository.Test.TestObjects;

namespace GoogleSheetRepository.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(1,"B")]
        [TestCase(3, "D")]
        [TestCase(27, "AB")]
        [TestCase(54, "BC")]
        [TestCase(752, "ABY")]
        public void ExtensionTest_GetColumnAddress(int columnNumber, string expectedAddress)
        {
            Assert.AreEqual(expectedAddress, columnNumber.GetColumnAddressWithHeaderShift());
        }

        [TestCaseSource(nameof(TestCondition1))]
        public void ExtensionTest_ConvertFromObjectListPropertyToClassInstance(List<object> properties, List<object> expected)
        {
            var actual = properties.GetObjectFromProperty<Moq>();
            Assert.AreEqual(expected[0], actual.Id);
            Assert.AreEqual(expected[1], actual.Name);
        }

        private static object[] TestCondition1 = {
           new object[] {new List<object> { "2","test" }, new List<object> { 2, "test" } },
           new object[] {new List<object> { "3","Test" }, new List<object> { 3, "Test" } },
           new object[] {new List<object> { "4","" }, new List<object> { 4, null } }
        };

    }
}