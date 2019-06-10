// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Represents a container for zero or more <see cref="Geometry"/> elements
    /// associated with the same KML feature.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.2.</remarks>
    [KmlElement("MultiGeometry")]
    public class MultipleGeometry : Geometry
    {
        private readonly List<Geometry> geometries = new List<Geometry>();

        /// <summary>
        /// Gets a collection of <see cref="Geometry"/> elements.
        /// </summary>
        [KmlElement(null, 1)]
        public IReadOnlyCollection<Geometry> Geometry => this.geometries;

        /// <summary>
        /// Adds the specified <see cref="Geometry"/> to this instance.
        /// </summary>
        /// <param name="geometry">The <c>Geometry</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">geometry is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// geometry belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddGeometry(Geometry geometry)
        {
            this.AddAsChild(this.geometries, geometry);
        }
    }
}
