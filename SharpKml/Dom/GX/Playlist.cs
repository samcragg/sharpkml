// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Contains any number of <see cref="TourPrimitive"/> elements.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Playlist", KmlNamespaces.GX22Namespace)]
    public class Playlist : KmlObject
    {
        private readonly List<TourPrimitive> tourPrimitives = new List<TourPrimitive>();

        /// <summary>
        /// Gets the <see cref="TourPrimitive"/>s stored by this instance.
        /// </summary>
        [KmlElement(null)]
        public IReadOnlyCollection<TourPrimitive> Values => this.tourPrimitives;

        /// <summary>
        /// Adds the specified <see cref="TourPrimitive"/> to this instance.
        /// </summary>
        /// <param name="tour">
        /// The <c>TourPrimitive</c> to add to this instance.
        /// </param>
        /// <exception cref="System.ArgumentNullException">tour is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// tour belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddTourPrimitive(TourPrimitive tour)
        {
            this.AddAsChild(this.tourPrimitives, tour);
        }
    }
}
