// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Represents a container for KML features, shared styles and user-defined schemas.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.7.</remarks>
    [KmlElement("Document")]
    public class Document : Container
    {
        private readonly List<Schema> schemas = new List<Schema>();

        /// <inheritdoc />
        [KmlElement(null, 2)]
        public override IReadOnlyCollection<Feature> Features => this.FeatureList;

        /// <summary>
        /// Gets a collection of <see cref="Schema"/> contained by this instance.
        /// </summary>
        [KmlElement(null, 1)]
        public IReadOnlyCollection<Schema> Schemas => this.schemas;

        /// <summary>
        /// Adds the specified <see cref="Schema"/> to this instance.
        /// </summary>
        /// <param name="schema">The <c>Schema</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">schema is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// schema belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddSchema(Schema schema)
        {
            this.AddAsChild(this.schemas, schema);
        }
    }
}
