// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.Xal
{
    using SharpKml.Base;

    /// <summary>
    /// Defines the details of an address.
    /// </summary>
    /// <remarks>
    /// Can define multiple addresses, including tracking address history.
    /// </remarks>
    [KmlElement("AddressDetails", KmlNamespaces.XalNamespace)]
    public class AddressDetails : Element
    {
        private Country country;

        /// <summary>
        /// Gets or sets the <see cref="Country"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public Country Country
        {
            get => this.country;
            set => this.UpdatePropertyChild(value, ref this.country);
        }
    }
}
