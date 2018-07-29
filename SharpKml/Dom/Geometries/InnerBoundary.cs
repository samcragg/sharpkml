// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies an inner boundary of a <see cref="Polygon"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.8.3.5</remarks>
    [KmlElement("innerBoundaryIs")]
    public class InnerBoundary : Element
    {
        private LinearRing ring;

        /// <summary>
        /// Gets or sets the <see cref="LinearRing"/> acting as the boundary.
        /// </summary>
        [KmlElement(null, 1)]
        public LinearRing LinearRing
        {
            get => this.ring;
            set => this.UpdatePropertyChild(value, ref this.ring);
        }

        /// <inheritdoc />
        protected internal override bool AddChild<T>(T child)
        {
            if (child is LinearRing linearRing &&
                (this.ring != null) &&
                (this.Parent != null))
            {
                return this.Parent.AddChild(new InnerBoundary
                {
                    LinearRing = linearRing
                });
            }

            return base.AddChild(child);
        }
    }
}
