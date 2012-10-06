using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the drawing style for a line geometry.
    /// </summary>
    /// <remarks>
    /// OGC KML 2.2 Section 12.11
    /// </remarks>
    [KmlElement("LineStyle")]
    public sealed class LineStyle : ColorStyle
    {
        /// <summary>
        /// The default value that should be used for <see cref="Width"/>.
        /// </summary>
        public const double DefaultWidth = 1.0;

        /// <summary>
        /// Gets or sets the width of the line, in pixels.
        /// </summary>
        [KmlElement("width", 1)]
        public double? Width { get; set; }

        /// <summary>
        /// Gets or sets the color of the portion of the line defined by
        /// <see cref="OuterWidth"/>. [Google Extension]
        /// </summary>
        [KmlElement("outerColor", KmlNamespaces.GX22Namespace, 2)]
        public Color32? OuterColor { get; set; }

        /// <summary>
        /// Gets or sets a value between 0.0 and 1.0 that specifies the proportion
        /// of the line that uses the <see cref="OuterColor"/>. [Google Extension]
        /// </summary>
        [KmlElement("outerWidth", KmlNamespaces.GX22Namespace, 3)]
        public double? OuterWidth { get; set; }

        /// <summary>
        /// Gets or sets the physical width of the line, in meters.
        /// [Google Extension]
        /// </summary>
        [KmlElement("physicalWidth", KmlNamespaces.GX22Namespace, 4)]
        public double? PhysicalWidth{ get; set; }

        /// <summary>
        /// Gets or sets whether or not to display a text label on a LineString.
        /// [Google Extension]
        /// </summary>
        [KmlElement("labelVisibility", KmlNamespaces.GX22Namespace, 5)]
        public bool? LabelVisibility { get; set; }
    }
}
