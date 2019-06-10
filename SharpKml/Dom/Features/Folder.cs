// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Used to organize <see cref="Feature"/> elements hierarchically.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.10.</remarks>
    [KmlElement("Folder")]
    public class Folder : Container
    {
        /// <inheritdoc />
        [KmlElement(null, 1)]
        public override IReadOnlyCollection<Feature> Features => this.FeatureList;
    }
}
