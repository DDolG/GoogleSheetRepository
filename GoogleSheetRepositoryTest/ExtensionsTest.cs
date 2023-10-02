using GoogleSheetRepository.Extensions;
using GoogleSheetRepository.Test.TestObjects;
using System;
using System.Reflection;

namespace GoogleSheetRepository.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(1, "B")]
        [TestCase(3, "D")]
        [TestCase(27, "AB")]
        [TestCase(54, "BC")]
        [TestCase(752, "ABY")]
        public void ExtensionTest_GetColumnAddress(int columnNumber, string expectedAddress)
        {
            Assert.AreEqual(expectedAddress, columnNumber.GetColumnAddressWithHeaderShift());
        }

        [TestCaseSource(nameof(TestConditionObjectList))]
        public void ExtensionTest_ConvertFromObjectListPropertyToClassInstance(List<object> properties, List<object> expected)
        {
            var actual = properties.GetObjectFromProperty<Moq>();
            Assert.AreEqual(expected[0], actual.Id);
            Assert.AreEqual(expected[1], actual.Name);
        }

        private static object[] TestConditionObjectList = {
           new object[] {new List<object> { "2","test" }, new List<object> { 2, "test" } },
           new object[] {new List<object> { "3","Test" }, new List<object> { 3, "Test" } },
           new object[] {new List<object> { "4","" }, new List<object> { 4, null } }
        };

        [TestCaseSource(nameof(TestConditionObjecToHeaderColumn))]
        public void ExtensionTest_ConvertFromObjectListPropertyToHeaderColumn(object headerCell, List<object> expected)
        {
            var actual = headerCell.GetColumnProperty();
            Assert.AreEqual(expected[0], actual.Name);
            Assert.AreEqual(expected[1], actual.PropertyType);
        }

        private static object[] TestConditionObjecToHeaderColumn = {
           new object[] {"Id[System.Int32]", new List<object> { "Id", "System.Int32" } },
           new object[] { "Name[System.String]", new List<object> { "Name", "System.String" } }
        };

        [TestCaseSource(nameof(TestConditionPropertyToHeaderCell))]
        public void ExtensionTest_ConvertPropertyInfoToHeaderCell(PropertyInfo propertyInfo, object expectedHeaderCell)
        {
            var actual = propertyInfo.ConvertPropertyToHeaderCell();
            Assert.AreEqual(expectedHeaderCell, actual);
        }

        private static object[] TestConditionPropertyToHeaderCell = {
           new object[] {typeof(Moq).GetProperty("Id"),"Id[System.Int32]"},
           new object[] {typeof(Moq).GetProperty("Name"),"Name[System.String]"}
        };
    }
}