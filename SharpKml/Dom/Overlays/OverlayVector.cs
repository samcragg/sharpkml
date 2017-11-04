// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies a point on (or outside of) an image.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 11.7.3.1</remarks>
    [KmlElement("overlayXY")]
    public sealed class OverlayVector : VectorType
    {
        // Intentionally left blank - this is a simple concrete implementation of VectorType.
    }
}
