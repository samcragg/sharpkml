// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using SharpKml.Base;

    /// <summary>
    /// The camera remains still for the specified <see cref="Duration"/> before
    /// playing the next <see cref="TourPrimitive"/>.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Wait", KmlNamespaces.GX22Namespace)]
    public sealed class Wait : TourPrimitive
    {
        /// <summary>
        /// Gets or sets the amount of time, in seconds.
        /// </summary>
        [KmlElement("duration", KmlNamespaces.GX22Namespace)]
        public double? Duration { get; set; }
    }
}
