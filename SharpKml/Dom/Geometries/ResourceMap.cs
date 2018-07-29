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
    /// Represents a collection of resource aliases.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 10.13</para>
    /// <para>This element allows texture files to be moved and renamed without
    /// having to update the original textured 3D object file that references
    /// those textures. One ResourceMap element can contain multiple mappings
    /// from different source textured object files into the same target
    /// resource.</para>
    /// </remarks>
    [KmlElement("ResourceMap")]
    [ChildType(typeof(Alias), 1)]
    public class ResourceMap : KmlObject
    {
        /// <summary>
        /// Gets a collection of untyped name/value pairs.
        /// </summary>
        public IEnumerable<Alias> Aliases => this.Children.OfType<Alias>();

        /// <summary>
        /// Adds the specified <see cref="Alias"/> to this instance.
        /// </summary>
        /// <param name="alias">The <c>Alias</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">alias is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// alias belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddAlias(Alias alias)
        {
            this.AddChild(alias);
        }
    }
}
