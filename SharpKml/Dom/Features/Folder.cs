﻿// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
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
        /// Initializes static members of the <see cref="Folder"/> class.
        /// </summary>
        static Folder()
        {
            RegisterValidChild<Folder, Feature>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        public Folder()
        {
        }
    }
}
