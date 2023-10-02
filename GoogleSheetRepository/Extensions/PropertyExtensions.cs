using System.Reflection;

namespace GoogleSheetRepository.Extensions
{
    public static class PropertyExtensions
    {
        public static object ConvertPropertyToHeaderCell(this PropertyInfo property)
        {
            var description = $"{property.Name}[{property.PropertyType}]";
            return (object)description;
        }
    }
}
