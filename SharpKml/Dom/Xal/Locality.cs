namespace SharpKml.Dom.Xal
{
    using SharpKml.Base;

    /// <summary>
    /// Represents an area one level lower than administrative.
    /// </summary>
    /// <remarks>
    /// Typically this includes: cities, reservations and any other built-up areas.
    /// </remarks>
    [KmlElement("Locality", KmlNamespaces.XalNamespace)]
    public sealed class Locality : Element
    {
        private PostalCode code;
        private Thoroughfare thoroughfare;

        /// <summary>
        /// Gets or sets the name of the Locality.
        /// </summary>
        [KmlElement("LocalityName", KmlNamespaces.XalNamespace)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PostalCode"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public PostalCode PostalCode
        {
            get { return this.code; }
            set { this.UpdatePropertyChild(value, ref this.code); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Thoroughfare"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public Thoroughfare Thoroughfare
        {
            get { return this.thoroughfare; }
            set { this.UpdatePropertyChild(value, ref this.thoroughfare); }
        }
    }
}
