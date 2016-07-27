namespace SharpKml.Base
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Formats the value of an object to KML specification.
    /// </summary>
    internal class KmlFormatter : ICustomFormatter, IFormatProvider
    {
        private static readonly KmlFormatter SingleInstance = new KmlFormatter();

        private KmlFormatter()
        {
        }

        /// <summary>
        /// Gets the default instance of the KmlFormatter class.
        /// </summary>
        public static KmlFormatter Instance
        {
            get { return SingleInstance; }
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
                if (arg is bool)
                {
                    return GetBool((bool)arg);
                }
                else if (arg is DateTime)
                {
                    return GetDateTime((DateTime)arg);
                }
                else if (arg is double)
                {
                    return GetFloatingPoint((double)arg);
                }
                else if (arg is float)
                {
                    return GetFloatingPoint((float)arg);
                }
            }

            return GetDefaultFormat(format, arg, formatProvider);
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

        private static string GetDefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            // This is basically what StringBuilder does, but don't use
            // StringBuilder as it can go wrong in weird multi-threaded ways:
            // https://sharpkml.codeplex.com/workitem/2415
            string result = null;
            if ((formatProvider != null) && !(formatProvider is KmlFormatter))
            {
                var custom = (ICustomFormatter)formatProvider.GetFormat(typeof(ICustomFormatter));
                if (custom != null)
                {
                    result = custom.Format(format, arg, formatProvider);
                }
            }

            if (result == null)
            {
                var formattableArg = arg as IFormattable;
                if (formattableArg != null)
                {
                    result = formattableArg.ToString(format, formatProvider);
                }
                else if (arg != null)
                {
                    result = arg.ToString();
                }
            }

            // Protect against ToString etc returning null (also if arg is null)
            return result ?? string.Empty;
        }

        private static string GetFloatingPoint(double value)
        {
            // http://www.w3.org/TR/xmlschema-2/#double
            if (double.IsPositiveInfinity(value))
            {
                return "INF";
            }
            else if (double.IsNegativeInfinity(value))
            {
                return "-INF";
            }
            else
            {
                // MSDN says use G17 not R (round-trip)!?
                // https://msdn.microsoft.com/en-us/library/kfsatb94(v=vs.110).aspx
                return value.ToString("G17", CultureInfo.InvariantCulture);
            }
        }
    }
}
