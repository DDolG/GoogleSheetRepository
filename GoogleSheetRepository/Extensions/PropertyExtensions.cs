using System.Reflection;

namespace GoogleSheetRepository.Extensions
{
    public static class PropertyExtensions
    {
        public static object GetPropertyDescription(this PropertyInfo property)
        {
            var description = $"{property.Name}[{property.PropertyType}]";
            return (object)description;
        }
    }
}
