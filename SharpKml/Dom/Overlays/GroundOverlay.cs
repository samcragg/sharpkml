// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies how to display an image draped over the terrain.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 11.2</remarks>
    [KmlElement("GroundOverlay")]
    public sealed class GroundOverlay : Overlay
    {
        private LatLonBox box;
        private GX.LatLonQuad quad;

        /// <summary>
        /// Gets or sets the distance above the terrain in meters.
        /// </summary>
        /// <remarks>
        /// The value shall be interpreted according to <see cref="AltitudeMode"/>.
        /// Only <see cref="Dom.AltitudeMode.ClampToGround"/> or
        /// <see cref="Dom.AltitudeMode.Absolute"/> values are valid.
        /// </remarks>
        [KmlElement("altitude", 1)]
        public double? Altitude { get; set; }

        /// <summary>
        /// Gets or sets how <see cref="Altitude"/> should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode", 2)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets a bounding box for the overlay.
        /// </summary>
        [KmlElement(null, 3)]
        public LatLonBox Bounds
        {
            get => this.box;
            set => this.UpdatePropertyChild(value, ref this.box);
        }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// [Google Extension]
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace, 4)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets used the value used for nonrectangular quadrilateral
        /// ground overlays.
        /// [Google Extension]
        /// </summary>
        [KmlElement(null, KmlNamespaces.GX22Namespace, 5)]
        public GX.LatLonQuad GXLatLonQuad
        {
            get => this.quad;
            set => this.UpdatePropertyChild(value, ref this.quad);
        }
    }
}
