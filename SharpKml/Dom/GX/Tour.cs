﻿namespace SharpKml.Dom.GX
{
    using SharpKml.Base;

    /// <summary>
    /// Enables controlled flights through geospatial data.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Tour", KmlNamespaces.GX22Namespace)]
    public sealed class Tour : Feature
    {
        private Playlist playlist;

        /// <summary>
        /// Gets or sets the associated <see cref="Playlist"/> of this instance.
        /// </summary>
        [KmlElement(null)]
        public Playlist Playlist
        {
            get { return this.playlist; }
            set { this.UpdatePropertyChild(value, ref this.playlist); }
        }
    }
}
