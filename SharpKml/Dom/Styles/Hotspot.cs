// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies the position of the reference point on the icon.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 12.8.3.4</remarks>
    [KmlElement("hotSpot")]
    public sealed class Hotspot : VectorType
    {
        // Intentionally left blank - this is a simple concrete implementation of VectorType.
    }
}
