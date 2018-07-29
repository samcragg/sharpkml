// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies a point relative to the screen origin that an image is mapped to.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 11.7.3.2</remarks>
    [KmlElement("screenXY")]
    public class ScreenVector : VectorType
    {
        // Intentionally left blank - this is a simple concrete implementation of VectorType.
    }
}
