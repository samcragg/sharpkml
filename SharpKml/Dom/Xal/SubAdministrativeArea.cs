// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.Xal
{
    using SharpKml.Base;

    /// <summary>
    /// Represents information about a sub-administrative area.
    /// </summary>
    /// <remarks>
    /// An example of a sub-administrative areas is a county. There are two
    /// places where the name of an administrative area can be specified and in
    /// this case, one becomes sub-administrative area.
    /// </remarks>
    [KmlElement("SubAdministrativeArea", KmlNamespaces.XalNamespace)]
    public class SubAdministrativeArea : Element
    {
        private Locality locality;

        /// <summary>
        /// Gets or sets the <see cref="Locality"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public Locality Locality
        {
            get => this.locality;
            set => this.UpdatePropertyChild(value, ref this.locality);
        }

        /// <summary>
        /// Gets or sets the name of the sub-administrative area.
        /// </summary>
        [KmlElement("SubAdministrativeAreaName", KmlNamespaces.XalNamespace)]
        public string Name { get; set; }
    }
}
