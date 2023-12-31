﻿using GoogleSheetRepository.Models;
using System.Text.RegularExpressions;

namespace GoogleSheetRepository.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Fills the table header with the property name and type
        /// </summary>
        /// <returns></returns>
        public static ColumnPropertyHeader GetColumnProperty(this object headerCell)
        {
            var header = headerCell as string;
            var match = Regex.Match(header, @"^(.*?)\[");
            var name = string.Empty;
            if (match.Success) {
                name = match.Groups[1].Value;
            }
            else
            {
                throw new ArgumentException($"Can not get property name from {header}");
            }
            match = Regex.Match(header, @"\[(.*?)\]$");
            var typeOfProperty = string.Empty;
            if (match.Success)
            {
                typeOfProperty = match.Groups[1].Value;
            }
            else
            {
                throw new ArgumentException($"Can not get property type from {header}");
            }

            return new ColumnPropertyHeader
            {
                Name = name,
                PropertyType = typeOfProperty
            };
        }
    }
}
