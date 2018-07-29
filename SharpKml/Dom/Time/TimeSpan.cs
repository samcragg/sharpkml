// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using SharpKml.Base;

    /// <summary>
    /// Specifies an extent in time bounded by begin and end temporal values.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 15.2</remarks>
    [KmlElement("TimeSpan")]
    public class TimeSpan : TimePrimitive
    {
        /// <summary>
        /// Gets or sets the beginning instant of a time period.
        /// </summary>
        [KmlElement("begin", 1)]
        public DateTime? Begin { get; set; }

        /// <summary>
        /// Gets or sets the ending instant of a time period.
        /// </summary>
        [KmlElement("end", 2)]
        public DateTime? End { get; set; }
    }
}
