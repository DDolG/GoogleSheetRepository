using System.Reflection;

namespace GoogleSheetRepository.Extensions
{
    public static class PropertyExtensions
    {
        /// <summary>
        /// Converts properties to a string with name and type
        /// </summary>
        /// <returns></returns>
        public static object ConvertPropertyToHeaderCell(this PropertyInfo property)
        {
            var description = $"{property.Name}[{property.PropertyType}]";
            return (object)description;
        }
    }
}
