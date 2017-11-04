// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies to display or hide the balloon.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 16.11</remarks>
    public enum DisplayMode
    {
        /// <summary>
        /// Specifies to display the balloon.
        /// </summary>
        [KmlElement("default")]
        Default = 0,

        /// <summary>
        /// Specifies to hide the balloon.
        /// </summary>
        [KmlElement("hide")]
        Hide
    }
}
