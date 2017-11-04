// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using System;
    using SharpKml.Base;

    /// <summary>
    /// Copy of <see cref="Dom.Timestamp"/> in the extension namespace.
    /// </summary>
    /// <remarks>
    /// <para>This is not part of the OGC KML 2.2 standard.</para>
    /// <para>This class allows for the inclusion of time values in
    /// <see cref="AbstractView"/>.</para>
    /// </remarks>
    [KmlElement("TimeStamp", KmlNamespaces.GX22Namespace)]
    public sealed class Timestamp : TimePrimitive
    {
        /// <summary>
        /// Gets or sets the moment in time.
        /// </summary>
        [KmlElement("when", 1)]
        public DateTime? When { get; set; }
    }
}
