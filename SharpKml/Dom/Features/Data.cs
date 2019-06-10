// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Diagnostics.CodeAnalysis;
    using SharpKml.Base;

    /// <summary>
    /// Represents an untyped name/value pair.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.3.</remarks>
    [KmlElement("Data")]
    [SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "Matches the name in the KML standard")]
    public class Data : KmlObject
    {
        /// <summary>
        /// Gets or sets an alternate display name.
        /// </summary>
        [KmlElement("displayName", 1)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the name of the data pair.
        /// </summary>
        /// <remarks>
        /// The value shall be unique within the context of its <see cref="Element.Parent"/>.
        /// </remarks>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the data pair.
        /// </summary>
        [KmlElement("value", 2)]
        public string Value { get; set; }
    }
}
