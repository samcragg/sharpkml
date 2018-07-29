// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Represents a geographic location defined by a single geodetic longitude,
    /// geodetic latitude, and (optional) altitude coordinate tuple.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.3</remarks>
    [KmlElement("Point")]
    public class Point : Geometry, IBoundsInformation
    {
        private CoordinateCollection coords;

        /// <summary>
        /// Gets or sets how the altitude value should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode", 2)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets a single coordinate tuple.
        /// </summary>
        public Vector Coordinate
        {
            get
            {
                // Because the parser could have created Coordinates, check it contains a value.
                if ((this.Coordinates != null) && (this.Coordinates.Count > 0))
                {
                    return this.Coordinates[0];
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    this.Coordinates = null;
                }
                else
                {
                    if (this.Coordinates == null)
                    {
                        this.Coordinates = new CoordinateCollection();
                    }

                    this.Coordinates.Clear();
                    this.Coordinates.Add(value);
                }
            }
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
        /// [Google Extension]
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace, 4)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }

        /// <summary>
        /// Gets the coordinates of the bounds of this instance.
        /// </summary>
        IEnumerable<Vector> IBoundsInformation.Coordinates
        {
            get
            {
                if (this.Coordinate != null)
                {
                    yield return this.Coordinate;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parsed/serialized coordinates.
        /// </summary>
        /// <remarks>
        /// Use a non-public (cannot be private as that can't be accessed under
        /// partial trust) property to enable automatic parsing/serialization
        /// </remarks>
        [KmlElement(null, 3)]
        internal CoordinateCollection Coordinates
        {
            get => this.coords;
            set => this.UpdatePropertyChild(value, ref this.coords);
        }
    }
}
