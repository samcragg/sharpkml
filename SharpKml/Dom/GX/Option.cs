using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Enables special viewing modes in Google Earth 6.0 and later.
    /// </summary>
    /// <remarks>
    /// This is not part of the OGC KML 2.2 standard.
    /// </remarks>
    [KmlElement("Option", KmlNamespaces.GX22Namespace)]
    public sealed class Option : Element
    {
        /// <summary>
        /// Gets or sets the name of the viewing mode.
        /// </summary>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether the viewing mode is on or off.
        /// </summary>
        [KmlAttribute("enabled")]
        public bool Enabled { get; set; }
    }
}
