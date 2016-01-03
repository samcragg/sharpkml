﻿namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Used to organize <see cref="Feature"/> elements hierarchically.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.10</remarks>
    [KmlElement("Folder")]
    public sealed class Folder : Container
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        public Folder()
        {
            this.RegisterValidChild<Feature>();
        }
    }
}
