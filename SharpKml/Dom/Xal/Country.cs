// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.Xal
{
    using SharpKml.Base;

    /// <summary>
    /// Represents a country.
    /// </summary>
    [KmlElement("Country", KmlNamespaces.XalNamespace)]
    public sealed class Country : Element
    {
        private AdministrativeArea area;

        /// <summary>
        /// Gets or sets the <see cref="AdministrativeArea"/> associated with
        /// this instance.
        /// </summary>
        [KmlElement(null)]
        public AdministrativeArea AdministrativeArea
        {
            get { return this.area; }
            set { this.UpdatePropertyChild(value, ref this.area); }
        }

        /// <summary>
        /// Gets or sets a country code as per ISO 3166-1.
        /// </summary>
        [KmlElement("CountryNameCode", KmlNamespaces.XalNamespace)]
        public string NameCode { get; set; }
    }
}
