// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using SharpKml.Base;

    /// <summary>
    /// Controls changes during a tour to KML features.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("AnimatedUpdate", KmlNamespaces.GX22Namespace)]
    public sealed class AnimatedUpdate : TourPrimitive
    {
        private Update update;

        /// <summary>
        /// Gets or sets the amount of time, in seconds.
        /// </summary>
        [KmlElement("duration", KmlNamespaces.GX22Namespace)]
        public double? Duration { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Update"/> of this instance.
        /// </summary>
        [KmlElement(null)]
        public Update Update
        {
            get { return this.update; }
            set { this.UpdatePropertyChild(value, ref this.update); }
        }

        /// <summary>
        /// Gets or sets the number of seconds to wait before starting the update.
        /// </summary>
        [KmlElement("delayedStart", KmlNamespaces.GX22Namespace)]
        public double? DelayedStart { get; set; }
    }
}
