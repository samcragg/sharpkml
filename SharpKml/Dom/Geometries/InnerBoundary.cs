// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Specifies an inner boundary of a <see cref="Polygon"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.8.3.5.</remarks>
    [KmlElement("innerBoundaryIs")]
    public class InnerBoundary : Element
    {
        private LinearRing ring;

        /// <summary>
        /// Gets or sets the <see cref="LinearRing"/> acting as the boundary.
        /// </summary>
        public LinearRing LinearRing
        {
            get => this.ring;
            set => this.UpdatePropertyChild(value, ref this.ring);
        }

        // The standard states that there is [0..1] LinearRing elements in this
        // instance, however, some applications incorrectly generate multiple
        // LinearRings. We work around this by wrapping the additional
        // LinearRings in new instances of this class and adding them to the
        // parent, hence we pretend to support more than one for the sake of
        // parsing/serialization
#pragma warning disable IDE0051 // Remove unused private members
        [KmlElement(null, 1)]
        private IEnumerable<LinearRing> LinearRings
        {
            get
            {
                if (this.ring != null)
                {
                    yield return this.ring;
                }
            }
        }

        private void AddLinearRing(LinearRing linearRing)
        {
            if (this.ring == null)
            {
                this.LinearRing = linearRing;
            }
            else
            {
                // Since our LinearRing is set, we need to add it to the parent
                if (this.Parent is Polygon polygon)
                {
                    polygon.AddInnerBoundary(new InnerBoundary
                    {
                        LinearRing = linearRing,
                    });
                }
                else
                {
                    this.AddOrphan(linearRing);
                }
            }
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
