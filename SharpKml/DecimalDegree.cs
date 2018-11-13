// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml
{
    using System;

    /// <summary>
    /// Utility methods to parse decimal degrees.
    /// </summary>
    internal static class DecimalDegree
    {
        private const int Emin = 308;
        private const int MaxSignificandDigits = 17;

        // We use 18 as that's the maximum amount of significant digits for a
        // double - if it's greater we'll fall back to Math.Pow
        private static readonly double[] PowersOf10 =
        {
            1,
            1e+1,
            1e+2,
            1e+3,
            1e+4,
            1e+5,
            1e+6,
            1e+7,
            1e+8,
            1e+9,
            1e+10,
            1e+11,
            1e+12,
            1e+13,
            1e+14,
            1e+15,
            1e+16,
            1e+17,
            1e+18,
        };

        /// <summary>
        /// Parses the specified string for a decimal number.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="index">The index to start parsing.</param>
        /// <param name="value">The parsed value, if successful.</param>
        /// <returns>
        /// <c>true</c> if a number was parsed; otherwise, <c>false</c>.
        /// </returns>
        public static bool Parse(string text, ref int index, out double value)
        {
            int originalIndex = index;
            int sign = ParseSign(text, ref index);

            NumberInfo number = default;
            if (ParseSignificand(text, ref index, ref number))
            {
                number.Scale += (short)ParseExponent(text, ref index);
                value = MakeDouble(number, sign);
                return true;
            }
            else
            {
                value = default;
                index = originalIndex;
                return false;
            }
        }

        private static double MakeDouble(in NumberInfo number, int sign)
        {
            long significand = sign * (long)number.Significand;

            // Improve the accuracy of the result for negative exponents
            if (number.Scale < 0)
            {
                // Allow for denormalized numbers
                if (number.Scale < -Emin)
                {
                    return (significand / Pow10(-Emin - number.Scale)) / 1e308;
                }
                else
                {
                    return significand / Pow10(-number.Scale);
                }
            }
            else
            {
                return significand * Pow10(number.Scale);
            }
        }

        private static void ParseDigits(string text, ref int index, ref NumberInfo number)
        {
            for (; index < text.Length; index++)
            {
                uint digit = (uint)(text[index] - '0');
                if (digit > 9)
                {
                    break;
                }

                // Ignore leading zeros. In this case Digits will be zero and
                // digit will be zero - adding together gives zero
                if ((number.Digits + digit) == 0)
                {
                    continue;
                }

                number.Digits++;
                if (number.Digits > MaxSignificandDigits)
                {
                    continue;
                }

                number.Significand = (number.Significand * 10) + digit;
            }
        }

        private static int ParseExponent(string text, ref int index)
        {
            int originalIndex = index;
            if (index < text.Length)
            {
                char c = text[index];
                if ((c == 'e') || (c == 'E'))
                {
                    index++;
                    int sign = ParseSign(text, ref index);
                    int startIndex = index;
                    NumberInfo exponent = default;
                    ParseDigits(text, ref index, ref exponent);
                    if (index > startIndex)
                    {
                        return (int)exponent.Significand * sign;
                    }
                }
            }

            // We didn't parse a valid exponent, go back to the start
            index = originalIndex;
            return 0;
        }

        private static int ParseSign(string text, ref int index)
        {
            if (index < text.Length)
            {
                char c = text[index];
                if (c == '-')
                {
                    index++;
                    return -1;
                }
                else if (c == '+')
                {
                    index++;
                }
            }

            return 1;
        }

        private static bool ParseSignificand(string text, ref int index, ref NumberInfo number)
        {
            int originalIndex = index;
            ParseDigits(text, ref index, ref number);
            int integerDigits = number.Digits;
            int significantZeros = 0;

            // Is there a decimal part as well?
            if ((index < text.Length) && (text[index] == '.'))
            {
                index++; // Skip the separator
                int fractionalStart = index;
                ParseDigits(text, ref index, ref number);

                if (integerDigits == 0)
                {
                    significantZeros = index - fractionalStart - number.Digits;
                }

                // Check it's not just a decimal point
                if ((index - originalIndex) == 1)
                {
                    index = originalIndex;
                }
            }

            int totalDigits = Math.Min((int)number.Digits, MaxSignificandDigits);
            if (significantZeros > 0)
            {
                number.Scale = (short)(-totalDigits - significantZeros);
            }
            else
            {
                number.Scale = (short)(integerDigits - totalDigits);
            }

            return index != originalIndex;
        }

        private static double Pow10(int power)
        {
            if (power <= 18)
            {
                return PowersOf10[power];
            }
            else
            {
                return Math.Pow(10, power);
            }
        }

        /// <summary>
        /// Contains the information when parsing a number.
        /// </summary>
        internal struct NumberInfo
        {
            /// <summary>
            /// Gets the number of digits parsed.
            /// </summary>
            public ushort Digits;

            /// <summary>
            /// Gets the scale to raise the significand by.
            /// </summary>
            public short Scale;

            /// <summary>
            /// Represents the total number as if the decimal was not there.
            /// </summary>
            public ulong Significand;
        }
    }
}
