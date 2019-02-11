// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;

    /// <summary>
    /// Specifies the options for how to serialize the KML data.
    /// </summary>
    [Flags]
    public enum SerializerOptions
    {
        /// <summary>
        /// Represent no options.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Allows the serializer to relax the precision when serializing
        /// floating-point numbers to enable the output to contain more human
        /// readable values.
        /// </summary>
        /// <remarks>
        /// This option may result in a loss of precision, where the read value
        /// will not exactly equal the original value
        /// </remarks>
        ReadableFloatingPoints = 0x01,

        /// <summary>
        /// Represents the default options.
        /// </summary>
        Default = 0x00,
    }
}
