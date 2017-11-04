// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.Xal
{
    using SharpKml.Base;

    /// <summary>
    /// Represents a container for either simple or complex (extended) postal codes.
    /// </summary>
    [KmlElement("PostalCode", KmlNamespaces.XalNamespace)]
    public sealed class PostalCode : Element
    {
        /// <summary>
        /// Gets or sets the postcode.
        /// </summary>
        /// <remarks>
        /// The postcode is formatted according to country-specific rules e.g.
        /// SW3 0A8-1A, 600074, 2067 etc.
        /// </remarks>
        [KmlElement("PostalCodeNumber", KmlNamespaces.XalNamespace)]
        public string Number { get; set; }
    }
}
