// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Specifies the location and orientation of a textured 3D object resource.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.9</remarks>
    [KmlElement("Model")]
    public sealed class Model : Geometry, IBoundsInformation
    {
        private Link link;
        private Location location;
        private Orientation orientation;
        private ResourceMap resources;
        private Scale scale;

        /// <summary>
        /// Gets or sets how the altitude value should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode", 1)]
        public AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets the location of a textured 3D object resource.
        /// </summary>
        [KmlElement(null, 5)]
        public Link Link
        {
            get { return this.link; }
            set { this.UpdatePropertyChild(value, ref this.link); }
        }

        /// <summary>
        /// Gets or sets the coordinates of the Model's origin.
        /// </summary>
        [KmlElement(null, 2)]
        public Location Location
        {
            get { return this.location; }
            set { this.UpdatePropertyChild(value, ref this.location); }
        }

        /// <summary>
        /// Gets or sets the orientation of the Model's coordinate axes relative
        /// to a local earth-fixed reference frame.
        /// </summary>
        [KmlElement(null, 3)]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set { this.UpdatePropertyChild(value, ref this.orientation); }
        }

        /// <summary>
        /// Gets or sets a collection of texture file mappings.
        /// </summary>
        [KmlElement(null, 6)]
        public ResourceMap Resources
        {
            get { return this.resources; }
            set { this.UpdatePropertyChild(value, ref this.resources); }
        }

        /// <summary>
        /// Gets or sets the Model's scales along the x, y, and z axes in the
        /// Model's coordinate space.
        /// </summary>
        [KmlElement(null, 4)]
        public Scale Scale
        {
            get { return this.scale; }
            set { this.UpdatePropertyChild(value, ref this.scale); }
        }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// [Google Extension]
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace, 7)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }

        /// <summary>
        /// Gets the coordinates of the bounds of this instance.
        /// </summary>
        IEnumerable<Vector> IBoundsInformation.Coordinates
        {
            get
            {
                if (this.Location != null)
                {
                    yield return new Vector(
                        this.Location.Latitude.GetValueOrDefault(),
                        this.Location.Longitude.GetValueOrDefault());
                }
            }
        }
    }
}
