// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies a container of zero or more <see cref="ColorStyle"/> objects
    /// that can referenced from a <see cref="StyleMapCollection"/> or
    /// <see cref="Feature"/>.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 12.2</para>
    /// <para>Styles affect how a <see cref="Geometry"/> is presented in the
    /// geographic view and how a <c>Feature</c> appears in the list view.</para>
    /// </remarks>
    [KmlElement("Style")]
    public class Style : StyleSelector
    {
        private BalloonStyle balloon;
        private IconStyle icon;
        private LabelStyle label;
        private LineStyle line;
        private ListStyle list;
        private PolygonStyle polygon;

        /// <summary>
        /// Gets or sets the associated <see cref="BalloonStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 5)]
        public BalloonStyle Balloon
        {
            get => this.balloon;
            set => this.UpdatePropertyChild(value, ref this.balloon);
        }

        /// <summary>
        /// Gets or sets the associated <see cref="IconStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 1)]
        public IconStyle Icon
        {
            get => this.icon;
            set => this.UpdatePropertyChild(value, ref this.icon);
        }

        /// <summary>
        /// Gets or sets the associated <see cref="LabelStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 2)]
        public LabelStyle Label
        {
            get => this.label;
            set => this.UpdatePropertyChild(value, ref this.label);
        }

        /// <summary>
        /// Gets or sets the associated <see cref="LineStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 3)]
        public LineStyle Line
        {
            get => this.line;
            set => this.UpdatePropertyChild(value, ref this.line);
        }

        /// <summary>
        /// Gets or sets the associated <see cref="ListStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 6)]
        public ListStyle List
        {
            get => this.list;
            set => this.UpdatePropertyChild(value, ref this.list);
        }

        /// <summary>
        /// Gets or sets the associated <see cref="PolygonStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 4)]
        public PolygonStyle Polygon
        {
            get => this.polygon;
            set => this.UpdatePropertyChild(value, ref this.polygon);
        }
    }
}
