// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using System.Linq;
    using SharpKml.Base;

    /// <summary>
    /// Represents a container for zero or more <see cref="Geometry"/> elements
    /// associated with the same KML feature.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.2</remarks>
    [KmlElement("MultiGeometry")]
    [ChildType(typeof(Geometry), 1)]
    public sealed class MultipleGeometry : Geometry
    {
        /// <summary>
        /// Gets a collection of <see cref="Geometry"/> elements.
        /// </summary>
        public IEnumerable<Geometry> Geometry => this.Children.OfType<Geometry>();

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
            this.AddChild(geometry);
        }
    }
}
