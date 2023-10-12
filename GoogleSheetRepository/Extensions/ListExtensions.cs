using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GoogleSheetRepository.Extensions;

public static class ListExtensions 
{
    /// <summary>
    /// Fills the properties of a class with a list of objects
    /// </summary>
    /// <returns></returns>
    public static T GetObjectFromProperty<T>(this List<object> objects) where T : class, new()
    {
        var result = new T();
        var properties = result.GetType().GetProperties().OrderBy(x => x.Name).ToList();
        var countProperties = 0;
        foreach ( var property in properties)
        {
            object? rowCell;
            try
            {
                rowCell = objects[countProperties++];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when get cell from row {ex.Message}");
                continue;
            }

            if (rowCell is string str && !string.IsNullOrEmpty(str))
            {
                try
                {
                    var conv = TypeDescriptor.GetConverter(property.PropertyType);
                    dynamic value = conv.ConvertFrom(rowCell);
                    property.SetValue(result, value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }
        return result;
    }
}
