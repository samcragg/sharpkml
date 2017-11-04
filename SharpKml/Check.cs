// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml
{
    using System;

    /// <summary>
    /// Used to validate method parameters.
    /// </summary>
    internal static class Check
    {
        /// <summary>
        /// Verifies a parameter is not null.
        /// </summary>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        public static void IsNotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
