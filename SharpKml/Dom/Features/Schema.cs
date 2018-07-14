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
    /// Specifies a user-defined schema that is used to add user-defined data
    /// encoded within a child <see cref="ExtendedData"/> element of a <see cref="Feature"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.8</remarks>
    // This should inherit from Element, but the C++ version inherits from Object
    [KmlElement("Schema")]
    public sealed class Schema : KmlObject
    {
        /// <summary>
        /// Initializes static members of the <see cref="Schema"/> class.
        /// </summary>
        static Schema()
        {
            RegisterValidChild<Schema, SimpleField>();
            RegisterValidChild<Schema, GX.SimpleArrayField>();
        }

        /// <summary>
        /// Gets a collection of <see cref="GX.SimpleArrayField"/> contained by this instance.
        /// [Google extension]
        /// </summary>
        public IEnumerable<GX.SimpleArrayField> Arrays => this.Children.OfType<GX.SimpleArrayField>();

        /// <summary>
        /// Gets a collection of <see cref="SimpleField"/> contained by this instance.
        /// </summary>
        public IEnumerable<SimpleField> Fields => this.Children.OfType<SimpleField>();

        /// <summary>
        /// Gets or sets a value acting as an identifier.
        /// </summary>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Adds the specified <see cref="GX.SimpleArrayField"/> to this instance.
        /// [Google extension]
        /// </summary>
        /// <param name="array">The <c>SimpleArrayField</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">array is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// array belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddArray(GX.SimpleArrayField array)
        {
            this.AddChild(array);
        }

        /// <summary>
        /// Adds the specified <see cref="SimpleField"/> to this instance.
        /// </summary>
        /// <param name="field">The <c>SimpleField</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">field is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// field belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddField(SimpleField field)
        {
            this.AddChild(field);
        }
    }
}
