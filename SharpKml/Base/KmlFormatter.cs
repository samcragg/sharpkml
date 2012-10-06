using System;
using System.Globalization;
using System.Text;

namespace SharpKml.Base
{
    /// <summary>Formats the value of an object to KML specification.</summary>
    internal class KmlFormatter : ICustomFormatter, IFormatProvider
    {
        private static KmlFormatter _instance = new KmlFormatter();

        private KmlFormatter()
        {
        }

        /// <summary>Gets the default instance of the KmlFormatter class.</summary>
        public static KmlFormatter Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Converts the value of a specified object to an equivalent string
        /// representation using specified format and the invariant-culture
        /// formatting information.
        /// </summary>
        /// <param name="format">
        /// A format string containing formatting specifications.
        /// </param>
        /// <param name="arg">An object to format.</param>
        /// <param name="formatProvider">
        /// An IFormatProvider object that supplies format information about
        /// the current instance.
        /// </param>
        /// <returns>
        /// The string representation of the value of arg, formatted as specified
        /// by format and the invariant-culture.
        /// </returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                var convertible = arg as IConvertible;
                if (convertible != null)
                {
                    switch (convertible.GetTypeCode())
                    {
                        case TypeCode.Boolean:
                            return GetBool((bool)arg);

                        case TypeCode.DateTime:
                            return GetDateTime((DateTime)arg);

                        case TypeCode.Decimal:
                            return GetDecimal((decimal)arg);

                        case TypeCode.Double:
                            return GetFloatingPoint((double)arg);

                        case TypeCode.Single:
                            return GetFloatingPoint((float)arg);
                    }
                }
            }

            var formatter = new StringBuilder();
            formatter.AppendFormat(CultureInfo.InvariantCulture, "{0:" + format + "}", arg);
            return formatter.ToString();
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="formatType">
        /// An object that specifies the type of format object to return.
        /// </param>
        /// <returns>
        /// This instance if the value parameter is ICustomFormatter; otherwise, null.
        /// </returns>
        /// <remarks>Part of the IFormatProvider interface.</remarks>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return null;
        }

        private static string GetBool(bool value)
        {
            return value ? "true" : "false"; // bool.ToString returns True or False, we need all lower case
        }

        private static string GetDateTime(DateTime date)
        {
            if (date.Kind == DateTimeKind.Utc)
            {
                return date.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }
            else
            {
                return date.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
            }
        }

        private static string GetFloatingPoint(double value)
        {
            // Return a maximum 15 meaningful digits
            return value.ToString("#0.##############", CultureInfo.InvariantCulture);
        }

        private static string GetDecimal(decimal value)
        {
            // Return a maximum 15 meaningful digits
            return value.ToString("#0.##############", CultureInfo.InvariantCulture);
        }
    }
}
