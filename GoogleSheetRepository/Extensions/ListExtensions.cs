﻿using System.Reflection;
using System.Runtime.CompilerServices;

namespace GoogleSheetRepository.Extensions;

public static class ListExtensions 
{
    public static T GetObjectFromProperty<T>(this List<object> objects) where T : class, new()
    {
        var result = new T();
        var properties = result.GetType().GetProperties().OrderBy(x => x.Name).ToList();
        var countProperties = 0;
        foreach ( var property in properties)
        {
            var rowCell = objects[countProperties++];
            if (rowCell is string str && !string.IsNullOrEmpty(str))
            {
                try
                {
                    dynamic value = Convert.ChangeType(rowCell, property.PropertyType);
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