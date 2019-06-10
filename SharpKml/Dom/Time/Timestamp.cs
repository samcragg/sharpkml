// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using SharpKml.Base;

    /// <summary>
    /// Specifies a single moment in time.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 15.3.</remarks>
    [KmlElement("TimeStamp")]
    public class Timestamp : TimePrimitive
    {
        /// <summary>
        /// Gets or sets the moment in time.
        /// </summary>
        [KmlElement("when", 1)]
        public DateTime? When { get; set; }
    }
}
