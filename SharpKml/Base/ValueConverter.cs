using System;
using System.Globalization;
using System.Reflection;

namespace SharpKml.Base
{
    /// <summary>
    /// Converts a string to an object according to the Kml specification.
    /// </summary>
    internal static class ValueConverter
    {
        // These are the only valid DateTime formats
        private static readonly string[] dateTimeFormats =
        {
            "yyyy", // xsd:gYear
            "yyyy-MM", // xsd:gYearMonth
            "yyyy-MM-dd", // xsd:date
            "yyyy-MM-ddTHH:mm:ss.FFFFFFF", // xsd:dateTime
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ", // xsd:dateTime
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzzzzz" // xsd:dateTime
        };

        /// <summary>Tries to convert the specified string to an object.</summary>
        /// <param name="type">The type the value should be converted to.</param>
        /// <param name="text">The string to convert.</param>
        /// <param name="value">The output, if successful; otherwise, null.</param>
        /// <returns>
        /// true if the specified string was converted to the specified type;
        /// otherwise, false.
        /// </returns>
        public static bool TryGetValue(Type type, string text, out object value)
        {
            if (type.IsEnum)
            {
                value = GetEnum(type, text);
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(type);
                if (typeCode == TypeCode.String)
                {
                    value = text;
                }
                else if (typeCode == TypeCode.Object)
                {
                    if (type == typeof(Color32))
                    {
                        value = Color32.Parse(text);
                    }
                    else if (type == typeof(Uri))
                    {
                        Uri uri;
                        Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out uri);
                        value = uri; // Will be null if TryCreate failed
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Unknown type:" + type);
                        value = null;
                        return false;
                    }
                }
                else
                {
                    value = GetPrimitive(typeCode, text);
                }
            }
            return true;
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

            DateTime date;
            if (DateTime.TryParseExact(value, dateTimeFormats, CultureInfo.InvariantCulture, Style, out date))
            {
                return date;
            }
            return null;
        }

        private static object GetEnum(Type type, string value)
        {
            if (value != null)
            {
                value = value.Trim();
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
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

        // Only called on Primitive types, as these all have a TryParse method
        private static object GetPrimitive(TypeCode typeCode, string value)
        {
            NumberStyles numberStyle = NumberStyles.Any;
            IFormatProvider provider = CultureInfo.InvariantCulture; 
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return GetBool(value);
                case TypeCode.Byte:
                    byte b;
                    return byte.TryParse(value, numberStyle, provider, out b) ? (object)b : null;
                case TypeCode.Char:
                    char c;
                    return char.TryParse(value, out c) ? (object)c : null;
                case TypeCode.DateTime:
                    return GetDateTime(value);
                case TypeCode.Decimal:
                    decimal de;
                    return decimal.TryParse(value, numberStyle, provider, out de) ? (object)de : null;
                case TypeCode.Double:
                    double d;
                    return double.TryParse(value, numberStyle, provider, out d) ? (object)d : null;
                case TypeCode.Int16:
                    short s;
                    return short.TryParse(value, numberStyle, provider, out s) ? (object)s : null;
                case TypeCode.Int32:
                    int i;
                    return int.TryParse(value, numberStyle, provider, out i) ? (object)i : null;
                case TypeCode.Int64:
                    long l;
                    return long.TryParse(value, numberStyle, provider, out l) ? (object)l : null;
                case TypeCode.SByte:
                    sbyte sb;
                    return sbyte.TryParse(value, numberStyle, provider, out sb) ? (object)sb : null;
                case TypeCode.Single:
                    float f;
                    return float.TryParse(value, numberStyle, provider, out f) ? (object)f : null;
                case TypeCode.UInt16:
                    ushort us;
                    return ushort.TryParse(value, numberStyle, provider, out us) ? (object)us : null;
                case TypeCode.UInt32:
                    uint ui;
                    return uint.TryParse(value, numberStyle, provider, out ui) ? (object)ui : null;
                case TypeCode.UInt64:
                    ulong ul;
                    return ulong.TryParse(value, numberStyle, provider, out ul) ? (object)ul : null;
            }
            System.Diagnostics.Debug.WriteLine("Unknown TypeCode:" + typeCode);
            return null; // Failed to convert
        }
    }
}
