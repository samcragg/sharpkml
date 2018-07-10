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
    /// Represents a container for KML features, shared styles and user-defined schemas.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.7</remarks>
    [KmlElement("Document")]
    public sealed class Document : Container
    {
        /// <summary>
        /// Initializes static members of the <see cref="Document"/> class.
        /// </summary>
        static Document()
        {
            RegisterValidChild<Document, Schema>();
            RegisterValidChild<Document, Feature>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        public Document()
        {
        }

        /// <summary>
        /// Gets a collection of <see cref="Schema"/> contained by this instance.
        /// </summary>
        public IEnumerable<Schema> Schemas
        {
            get { return this.Children.OfType<Schema>(); }
        }

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
            this.AddChild(schema);
        }
    }
}
