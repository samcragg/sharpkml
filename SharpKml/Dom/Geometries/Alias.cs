// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using SharpKml.Base;

    /// <summary>
    /// Contains a mapping from SourceHref to TargetHref.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.14</remarks>
    [KmlElement("Alias")]
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Matches the name in the KML standard")]
    public class Alias : KmlObject
    {
        /// <summary>
        /// Gets or sets the path for the texture file within the textured 3D object.
        /// </summary>
        [KmlElement("sourceHref", 2)]
        public Uri SourceHref { get; set; }

        /// <summary>
        /// Gets or sets the textured 3D object file to be fetched by an earth browser.
        /// </summary>
        /// <remarks>
        /// This reference can be a relative reference to an image file within
        /// a KMZ file, or it can be an absolute reference to the file (e.g. a URL).
        /// </remarks>
        [KmlElement("targetHref", 1)]
        public Uri TargetHref { get; set; }
    }
}
