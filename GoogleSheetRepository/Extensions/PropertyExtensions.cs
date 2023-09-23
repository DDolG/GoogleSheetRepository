using System.Reflection;

namespace GoogleSheetRepository.Extensions
{
    internal static class PropertyExtensions
    {
        internal static object GetPropertyDescription(this PropertyInfo property)
        {
            var description = $"{property.Name}[{property.PropertyType}]";
            return (object)description;
        }
    }
}
