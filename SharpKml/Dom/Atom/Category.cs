// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.Atom
{
    using System;
    using SharpKml.Base;

    /// <summary>
    /// Represents information about a category associated with an entry or feed.
    /// </summary>
    /// <remarks>
    /// RFC 4287 Section 4.2.2 (see http://atompub.org/rfc4287.html)
    /// This is not part of the OGC KML 2.2 standard.
    /// </remarks>
    [KmlElement("category", KmlNamespaces.AtomNamespace)]
    public sealed class Category : Element
    {
        /// <summary>
        /// Gets or sets the human-readable label for display in end-user applications.
        /// </summary>
        /// <remarks>The content is Language-Sensitive.</remarks>
        [KmlAttribute("label")]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the URI that identifies a categorization scheme.
        /// </summary>
        [KmlAttribute("scheme")]
        public Uri Scheme { get; set; }

        /// <summary>
        /// Gets or sets the category to which the entry or feed belongs.
        /// </summary>
        [KmlAttribute("term")]
        public string Term { get; set; }
    }
}
