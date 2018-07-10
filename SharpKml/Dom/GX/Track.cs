// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using SharpKml.Base;

    /// <summary>
    /// Describes how an object moves through the world over a given time period.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Track", KmlNamespaces.GX22Namespace)]
    public sealed class Track : Geometry
    {
        private static readonly XmlComponent AnglesComponent = new XmlComponent(null, "angles", KmlNamespaces.GX22Namespace);
        private static readonly XmlComponent CoordComponent = new XmlComponent(null, "coord", KmlNamespaces.GX22Namespace);
        private static readonly XmlComponent WhenComponent = new XmlComponent(null, "when", KmlNamespaces.Kml22Namespace);

        private ExtendedData data;
        private Model model;

        /// <summary>
        /// Initializes static members of the <see cref="Track"/> class.
        /// </summary>
        static Track()
        {
            RegisterValidChild<Track, WhenElement>();
            RegisterValidChild<Track, CoordElement>();
            RegisterValidChild<Track, AnglesElement>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        public Track()
        {
        }

        /// <summary>
        /// Gets or sets how the altitude value should be interpreted.
        /// </summary>
        [KmlElement("altitudeMode")]
        public Dom.AltitudeMode? AltitudeMode { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="Angle"/> containing the heading,
        /// tilt and roll for the icons and models.
        /// </summary>
        public IEnumerable<Angle> Angles
        {
            get
            {
                return from e in this.Children.OfType<AnglesElement>()
                       select e.Value;
            }
        }

        /// <summary>Gets a collection of coordinates for the Track.</summary>
        public IEnumerable<Vector> Coordinates
        {
            get
            {
                return from e in this.Children.OfType<CoordElement>()
                       select e.Value;
            }
        }

        /// <summary>
        /// Gets or sets custom data elements defined in a <see cref="Schema"/>
        /// earlier in the KML file.
        /// </summary>
        [KmlElement(null)]
        public ExtendedData ExtendedData
        {
            get { return this.data; }
            set { this.UpdatePropertyChild(value, ref this.data); }
        }

        /// <summary>
        /// Gets or sets extended altitude mode information.
        /// </summary>
        [KmlElement("altitudeMode", KmlNamespaces.GX22Namespace)]
        public GX.AltitudeMode? GXAltitudeMode { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Model"/> used to indicate the current
        /// position on the track.
        /// </summary>
        [KmlElement(null)]
        public Model Model
        {
            get { return this.model; }
            set { this.UpdatePropertyChild(value, ref this.model); }
        }

        /// <summary>
        /// Gets a collection of time values that corresponds to a position.
        /// </summary>
        public IEnumerable<DateTime> When
        {
            get
            {
                return from e in this.Children.OfType<WhenElement>()
                       select e.Value;
            }
        }

        /// <summary>
        /// Adds the specified value to <see cref="Angles"/>.</summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public void AddAngle(Angle value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.AddChild(new AnglesElement(value));
        }

        /// <summary>
        /// Adds the specified value to <see cref="Coordinates"/>.</summary>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public void AddCoordinate(Vector value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.AddChild(new CoordElement(value));
        }

        /// <summary>
        /// Adds the specified value to <see cref="When"/>.</summary>
        /// <param name="value">The value to add.</param>
        public void AddWhen(DateTime value)
        {
            this.AddChild(new WhenElement(value));
        }

        /// <summary>
        /// Processes the &lt;gx:angles&gt;, &lt;gx:coord&gt; and &lt;when&gt;
        /// elements.
        /// </summary>
        /// <param name="orphan">The <see cref="Element"/> to add.</param>
        protected internal override void AddOrphan(Element orphan)
        {
            UnknownElement unknown = orphan as UnknownElement;
            if (unknown != null)
            {
                Element child = ConvertUnknown(unknown);
                if (child != null)
                {
                    this.AddChild(child);
                    return;
                }
            }

            base.AddOrphan(orphan);
        }

        private static Element ConvertUnknown(UnknownElement unknown)
        {
            XmlComponent data = unknown.UnknownData;
            if (AnglesComponent.Equals(data))
            {
                return new AnglesElement(unknown.InnerText);
            }
            else if (CoordComponent.Equals(data))
            {
                return new CoordElement(unknown.InnerText);
            }
            else if (WhenComponent.Equals(data))
            {
                DateTime value;
                if (DateTime.TryParse(unknown.InnerText, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                {
                    return new WhenElement(value);
                }
            }

            return null;
        }

        /// <summary>
        /// Used to correctly serialize a 3D vector.
        /// </summary>
        internal abstract class VectorElement : Element, ICustomElement
        {
            private readonly double x;
            private readonly double y;
            private readonly double z;

            /// <summary>
            /// Initializes a new instance of the <see cref="VectorElement"/> class.
            /// </summary>
            /// <param name="value">The value to serialize.</param>
            public VectorElement(string value)
            {
                // The vector is stored with ' ' as the separator.
                string[] values = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                this.x = GetValue(values, 0);
                this.y = GetValue(values, 1);
                this.z = GetValue(values, 2);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VectorElement"/> class.
            /// </summary>
            /// <param name="x">The first value.</param>
            /// <param name="y">The second value.</param>
            /// <param name="z">The third value.</param>
            protected VectorElement(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren
            {
                get { return false; }
            }

            /// <summary>
            /// Gets the name of the XML element.
            /// </summary>
            protected abstract string Name { get; }

            /// <summary>
            /// Gets the first value of the vector.
            /// </summary>
            protected double X
            {
                get { return this.x; }
            }

            /// <summary>
            /// Gets the second value of the vector.
            /// </summary>
            protected double Y
            {
                get { return this.y; }
            }

            /// <summary>
            /// Gets the third value of the vector.
            /// </summary>
            protected double Z
            {
                get { return this.z; }
            }

            /// <summary>
            /// Writes the start of an XML element.
            /// </summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            public void CreateStartElement(XmlWriter writer)
            {
                string value = string.Format(KmlFormatter.Instance, "{0} {1} {2}", this.x, this.y, this.z);
                writer.WriteElementString(KmlNamespaces.GX22Prefix, this.Name, KmlNamespaces.GX22Namespace, value);
            }

            private static double GetValue(string[] array, int index)
            {
                if (index < array.Length)
                {
                    double value;
                    if (double.TryParse(array[index], NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                    {
                        return value;
                    }
                }

                return default(double);
            }
        }

        /// <summary>
        /// Used to correctly serialize an Angle in Angles.
        /// </summary>
        internal class AnglesElement : VectorElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AnglesElement"/> class.
            /// </summary>
            /// <param name="value">The value to serialize, must not be null.</param>
            public AnglesElement(Angle value)
                : base(value.Heading, value.Pitch, value.Roll) // Must be stored in this order.
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AnglesElement"/> class.
            /// </summary>
            /// <param name="value">The value to serialize.</param>
            public AnglesElement(string value)
                : base(value)
            {
            }

            /// <summary>
            /// Gets an Angle that represents the value of this instance.
            /// </summary>
            public Angle Value
            {
                get { return new Angle(this.Y, this.X, this.Z); }
            }

            /// <summary>
            /// Gets the name of the XML element.
            /// </summary>
            protected override string Name
            {
                get { return "angles"; }
            }
        }

        /// <summary>
        /// Used to correctly serialize a Vector in Coordinates.
        /// </summary>
        internal class CoordElement : VectorElement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CoordElement"/> class.
            /// </summary>
            /// <param name="value">The value to serialize, must not be null.</param>
            public CoordElement(Vector value)
                : base(value.Longitude, value.Latitude, value.Altitude.GetValueOrDefault()) // Must be stored in this order.
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CoordElement"/> class.
            /// </summary>
            /// <param name="value">The value to serialize.</param>
            public CoordElement(string value)
                : base(value)
            {
            }

            /// <summary>
            /// Gets a Vector that represents the value of this instance.
            /// </summary>
            public Vector Value
            {
                get { return new Vector(this.Y, this.X, this.Z); }
            }

            /// <summary>
            /// Gets the name of the XML element.
            /// </summary>
            protected override string Name
            {
                get { return "coord"; }
            }
        }

        /// <summary>
        /// Used to correctly serialize the strings in When.
        /// </summary>
        internal class WhenElement : Element, ICustomElement
        {
            private readonly DateTime value;

            /// <summary>
            /// Initializes a new instance of the <see cref="WhenElement"/> class.
            /// </summary>
            /// <param name="value">
            /// The value to set the <see cref="Element.InnerText"/> to.
            /// </param>
            public WhenElement(DateTime value)
            {
                this.value = value;
            }

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren
            {
                get { return false; }
            }

            /// <summary>
            /// Gets the value passed into the constructor.
            /// </summary>
            public DateTime Value
            {
                get { return this.value; }
            }

            /// <summary>
            /// Writes the start of an XML element.
            /// </summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            public void CreateStartElement(XmlWriter writer)
            {
                string elementValue = KmlFormatter.Instance.Format(null, this.value, null);
                writer.WriteElementString("when", KmlNamespaces.Kml22Namespace, elementValue);
            }
        }
    }
}
