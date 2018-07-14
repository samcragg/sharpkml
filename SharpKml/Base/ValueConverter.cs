// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Converts a string to an object according to the Kml specification.
    /// </summary>
    internal static class ValueConverter
    {
        private static readonly Dictionary<Type, Func<string, object>> Converters = new Dictionary<Type, Func<string, object>>();

        // These are the only valid DateTime formats
        private static readonly string[] DateTimeFormats =
        {
            "yyyy", // xsd:gYear
            "yyyy-MM", // xsd:gYearMonth
            "yyyy-MM-dd", // xsd:date
            "yyyy-MM-ddTHH:mm:ss.FFFFFFF", // xsd:dateTime
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ", // xsd:dateTime
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzzzzz" // xsd:dateTime
        };

        static ValueConverter()
        {
            Converters.Add(typeof(bool), GetBool);
            Converters.Add(typeof(DateTime), GetDateTime);
            Converters.Add(typeof(string), str => str);
            Converters.Add(typeof(Color32), str => Color32.Parse(str));

            AddConverter<byte>(byte.TryParse);
            AddConverter<decimal>(decimal.TryParse);
            AddConverter<double>(double.TryParse);
            AddConverter<float>(float.TryParse);
            AddConverter<int>(int.TryParse);
            AddConverter<long>(long.TryParse);
            AddConverter<sbyte>(sbyte.TryParse);
            AddConverter<short>(short.TryParse);
            AddConverter<uint>(uint.TryParse);
            AddConverter<ulong>(ulong.TryParse);
            AddConverter<ushort>(ushort.TryParse);

            Converters.Add(typeof(char), str =>
            {
                return char.TryParse(str, out char value) ? (object)value : null;
            });

            Converters.Add(typeof(Uri), str =>
            {
                Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out Uri value);
                return value;
            });
        }

        private delegate bool TryParse<T>(string s, NumberStyles style, IFormatProvider provider, out T result);

        /// <summary>
        /// Tries to convert the specified string to an object.
        /// </summary>
        /// <param name="type">The type the value should be converted to.</param>
        /// <param name="text">The string to convert.</param>
        /// <param name="value">The output, if successful; otherwise, null.</param>
        /// <returns>
        /// true if the specified string was converted to the specified type;
        /// otherwise, false.
        /// </returns>
        public static bool TryGetValue(Type type, string text, out object value)
        {
            if (Converters.TryGetValue(type, out Func<string, object> converter))
            {
                value = converter(text);
            }
            else
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.IsEnum)
                {
                    value = GetEnum(typeInfo, text);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown type:" + type);
                    value = null;
                    return false;
                }
            }

            return true;
        }

        private static void AddConverter<T>(TryParse<T> tryParse)
        {
            Converters.Add(typeof(T), str =>
            {
                return tryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out T value) ? (object)value : null;
            });
        }

        // A bool can be either true/1 or false/0
        private static object GetBool(string value)
        {
            if (value != null)
            {
                value = value.Trim();
                if (value.Equals("true", StringComparison.Ordinal) ||
                    value.Equals("1", StringComparison.Ordinal))
                {
                    return true;
                }
                else if (value.Equals("false", StringComparison.Ordinal) ||
                         value.Equals("0", StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return null;
        }

        private static object GetDateTime(string value)
        {
            const DateTimeStyles Style =
                DateTimeStyles.AdjustToUniversal |
                DateTimeStyles.AllowWhiteSpaces |
                DateTimeStyles.AssumeUniversal;

            if (DateTime.TryParseExact(value, DateTimeFormats, CultureInfo.InvariantCulture, Style, out DateTime date))
            {
                return date;
            }

            return null;
        }

        private static object GetEnum(TypeInfo typeInfo, string value)
        {
            if (value != null)
            {
                value = value.Trim();
                foreach (FieldInfo field in typeInfo.DeclaredFields.Where(f => f.IsStatic))
                {
                    KmlElementAttribute element = TypeBrowser.GetElement(field);
                    if (element != null && string.Equals(element.ElementName, value, StringComparison.Ordinal))
                    {
                        return field.GetValue(null);
                    }
                }
            }

            return null;
        }
    }
}
