// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using SharpKml.Base;

    /// <summary>
    /// Represents a series of points.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 16.9.</remarks>
    [KmlElement("coordinates")]
    public sealed class CoordinateCollection : Element, ICollection<Vector>, ICustomElement, IReadOnlyCollection<Vector>
    {
        private readonly List<Vector> points;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateCollection"/> class.
        /// </summary>
        public CoordinateCollection()
        {
            this.points = new List<Vector>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateCollection"/> class.
        /// </summary>
        /// <param name="points">The points to populate the instance with.</param>
        /// <exception cref="ArgumentNullException">points is null.</exception>
        public CoordinateCollection(IEnumerable<Vector> points)
        {
            Check.IsNotNull(points, nameof(points));

            this.points = new List<Vector>(points);
        }

        /// <summary>
        /// Gets or sets the delimiter to use between each point.
        /// </summary>
        public static string Delimiter { get; set; } = "\n";

        /// <summary>
        /// Gets the number of points contained in this instance.
        /// </summary>
        public int Count => this.points.Count;

        /// <summary>
        /// Gets a value indicating whether this instance is read-only.
        /// </summary>
        bool ICollection<Vector>.IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether to process the children of the Element.
        /// </summary>
        bool ICustomElement.ProcessChildren => true;

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get.</param>
        /// <returns>The value at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// index is less than 0 or index is equal to or greater than <see cref="Count"/>.
        /// </exception>
        internal Vector this[int index] => this.points[index];

        /// <summary>
        /// Adds a point to this instance.
        /// </summary>
        /// <param name="item">The point to be added.</param>
        /// <exception cref="ArgumentNullException">item is null.</exception>
        public void Add(Vector item)
        {
            Check.IsNotNull(item, nameof(item));

            this.points.Add(item);
        }

        /// <summary>
        /// Removes all points from this instance.
        /// </summary>
        public void Clear()
        {
            this.points.Clear();
        }

        /// <summary>
        /// Determines whether a point is contained in this instance.
        /// </summary>
        /// <param name="item">The point to locate.</param>
        /// <returns>
        /// true if the point is found in this instance; otherwise, false. This
        /// method also returns false if the specified value parameter is null.
        /// </returns>
        public bool Contains(Vector item)
        {
            return this.points.Contains(item);
        }

        /// <summary>
        /// Copies this instance to a compatible one-dimensional array, starting
        /// at the specified index of the target array.
        /// </summary>
        /// <param name="array">The destination one-dimensional array.</param>
        /// <param name="arrayIndex">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The number of points contained in this instance is greater than the
        /// available space from arrayIndex to the end of the destination array.
        /// </exception>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// arrayIndex is less than 0.
        /// </exception>
        public void CopyTo(Vector[] array, int arrayIndex)
        {
            this.points.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>An enumerator for this instance.</returns>
        public IEnumerator<Vector> GetEnumerator()
        {
            return this.points.GetEnumerator();
        }

        /// <summary>
        /// Removes the first occurrence of a specific point from this instance.
        /// </summary>
        /// <param name="item">The point to remove.</param>
        /// <returns>
        /// true if the specified value parameter is successfully removed;
        /// otherwise, false. This method also returns false if the specified
        /// value parameter was not found or is null.
        /// </returns>
        public bool Remove(Vector item)
        {
            return this.points.Remove(item);
        }

        /// <summary>
        /// Writes the start of an XML element.
        /// </summary>
        /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
        void ICustomElement.CreateStartElement(XmlWriter writer)
        {
            // This element is being serialized so we need to update the InnerText
            var sb = new StringBuilder();
            bool first = true;

            foreach (Vector point in this.points)
            {
                if (!first)
                {
                    sb.Append(Delimiter);
                }
                else
                {
                    first = false;
                }

                if (point.Altitude != null)
                {
                    sb.AppendFormat(
                        KmlFormatter.Instance,
                        "{0},{1},{2}",
                        point.Longitude,
                        point.Latitude,
                        point.Altitude.Value);
                }
                else
                {
                    sb.AppendFormat(
                        KmlFormatter.Instance,
                        "{0},{1}",
                        point.Longitude,
                        point.Latitude);
                }
            }

            this.ClearInnerText();
            base.AddInnerText(sb.ToString()); // Don't use our overloaded version

            writer.WriteStartElement("coordinates", KmlNamespaces.Kml22Namespace);
        }

        /// <summary>
        /// Returns an enumerator that iterates through this instance.
        /// </summary>
        /// <returns>An enumerator for this instance.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Parses the inner text of the XML element.
        /// </summary>
        /// <param name="text">The text content of the XML element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Adding to the buffer would exceed StringBuilder.MaxCapacity.
        /// </exception>
        protected internal override void AddInnerText(string text)
        {
            base.AddInnerText(text);
            this.Parse(this.InnerText);
        }

        private static bool SkipSeparator(string value, ref int index)
        {
            SkipWhitespace(value, ref index);
            if ((index < value.Length) && (value[index] == ','))
            {
                index++;
                SkipWhitespace(value, ref index);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void SkipWhitespace(string value, ref int index)
        {
            while (index < value.Length)
            {
                if (!char.IsWhiteSpace(value[index]))
                {
                    break;
                }

                index++;
            }
        }

        private void Parse(string input)
        {
            this.points.Clear();
            for (int i = 0; i < input.Length;)
            {
                SkipWhitespace(input, ref i);
                if (!DecimalDegree.Parse(input, ref i, out double longitude))
                {
                    break;
                }

                if (!SkipSeparator(input, ref i) ||
                    !DecimalDegree.Parse(input, ref i, out double latitude))
                {
                    break;
                }

                if (SkipSeparator(input, ref i) &&
                    !DecimalDegree.ParseNan(input, ref i) &&
                    DecimalDegree.Parse(input, ref i, out double altitude))
                {
                    this.points.Add(new Vector(latitude, longitude, altitude));
                }
                else
                {
                    this.points.Add(new Vector(latitude, longitude));
                }
            }
        }
    }
}
