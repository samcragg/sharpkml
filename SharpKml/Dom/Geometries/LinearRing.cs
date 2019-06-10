// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using SharpKml.Base;

    /// <summary>
    /// Defines a closed line string that should not cross itself.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 10.5.</para>
    /// <para>If LinearRing is used to define a boundary for a <see cref="Polygon"/>
    /// then <see cref="Extrude"/>, <see cref="Tessellate"/> and
    /// <see cref="LinearRing.AltitudeMode"/> should not be specified.</para>
    /// </remarks>
    [KmlElement("LinearRing")]
    public class LinearRing : Geometry, IBoundsInformation
    {
        private static readonly IEnumerable<Vector> EmptyCoordinates = Enumerable.Empty<Vector>();
        private CoordinateCollection coords;

        /// <summary>
        /// Gets or sets how the altitude value should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode", 3)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets a the coordinate tuples.
        /// </summary>
        /// <remarks>
        /// Should contain four or more coordinates, where the first and last
        /// coordinates must be the same.
        /// </remarks>
        [KmlElement(null, 4)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "This object is a DTO")]
        public CoordinateCollection Coordinates
        {
            get => this.coords;
            set => this.UpdatePropertyChild(value, ref this.coords);
        }

        /// <summary>
        /// Gets or sets whether to connect a geometry to the ground.
        /// </summary>
        /// <remarks>
        /// The geometry is extruded toward the Earth's center of mass. To
        /// extrude a geometry, <see cref="AltitudeMode"/> shall be either
        /// <see cref="Dom.AltitudeMode.RelativeToGround"/> or
        /// <see cref="Dom.AltitudeMode.Absolute"/>, and the altitude component
        /// should be greater than 0 (that is, in the air).
        /// </remarks>
        [KmlElement("extrude", 1)]
        public bool? Extrude { get; set; }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// [Google Extension].
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace, 5)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets an offset (in meters) to apply to all the points
        /// without modifying them. [Google Extension].
        /// </summary>
        [KmlElement("altitudeOffset", KmlNamespaces.GX22Namespace, 6)]
        public double? GXAltitudeOffset { get; set; }

        /// <summary>
        /// Gets or sets whether to drape a geometry over the terrain.
        /// </summary>
        /// <remarks>
        /// To enable tessellation, the value should be set to true and
        /// <see cref="AltitudeMode"/> shall be <see cref="Dom.AltitudeMode.ClampToGround"/>.
        /// </remarks>
        [KmlElement("tessellate", 2)]
        public bool? Tessellate { get; set; }

        /// <summary>
        /// Gets the coordinates of the bounds of this instance.
        /// </summary>
        IEnumerable<Vector> IBoundsInformation.Coordinates =>
            this.Coordinates ?? EmptyCoordinates;
    }
}
