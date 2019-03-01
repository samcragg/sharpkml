// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using System.Diagnostics.CodeAnalysis;
    using SharpKml.Base;

    /// <summary>
    /// Enables special viewing modes in Google Earth 6.0 and later.
    /// </summary>
    /// <remarks>
    /// This is not part of the OGC KML 2.2 standard.
    /// </remarks>
    [KmlElement("Option", KmlNamespaces.GX22Namespace)]
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Matches the name in the KML standard")]
    public class Option : Element
    {
        /// <summary>
        /// Gets or sets the name of the viewing mode.
        /// </summary>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the viewing mode is on or off.
        /// </summary>
        [KmlAttribute("enabled")]
        public bool Enabled { get; set; }
    }
}
