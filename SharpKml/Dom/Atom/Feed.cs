// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.Atom
{
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Acts as a container for metadata and data associated with an Atom feed.
    /// </summary>
    /// <remarks>
    /// RFC 4287 Section 4.1.1 (see http://atompub.org/rfc4287.html)
    /// This is not part of the OGC KML 2.2 standard.
    /// </remarks>
    [KmlElement("feed", KmlNamespaces.AtomNamespace)]
    public class Feed : Element
    {
        private readonly List<Category> categories = new List<Category>();
        private readonly List<Entry> entries = new List<Entry>();
        private readonly List<Link> links = new List<Link>();

        /// <summary>
        /// Gets the categories associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public IReadOnlyCollection<Category> Categories => this.categories;

        /// <summary>
        /// Gets the entries associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public IReadOnlyCollection<Entry> Entries => this.entries;

        /// <summary>
        /// Gets or sets a permanent, universally unique identifier for this instance.
        /// </summary>
        [KmlElement("id", KmlNamespaces.AtomNamespace)]
        public string Id { get; set; }

        /// <summary>
        /// Gets the links associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public IReadOnlyCollection<Link> Links => this.links;

        /// <summary>
        /// Gets or sets a human-readable title for this instance.
        /// </summary>
        [KmlElement("title", KmlNamespaces.AtomNamespace)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a Date construct indicating the most recent instant in time
        /// when this instance was modified in a way the publisher considers significant.
        /// </summary>
        /// <remarks>
        /// Not all modifications necessarily result in a changed Updated value.
        /// </remarks>
        [KmlElement("updated", KmlNamespaces.AtomNamespace)]
        public string Updated { get; set; }

        /// <summary>
        /// Adds the specified <see cref="Category"/> to this instance.
        /// </summary>
        /// <param name="category">The <c>Category</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">category is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// category belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddCategory(Category category)
        {
            this.AddAsChild(this.categories, category);
        }

        /// <summary>
        /// Adds the specified <see cref="Entry"/> to this instance.
        /// </summary>
        /// <param name="entry">The <c>Entry</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">entry is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// entry belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddEntry(Entry entry)
        {
            this.AddAsChild(this.entries, entry);
        }

        /// <summary>
        /// Adds the specified <see cref="Link"/> to this instance.
        /// </summary>
        /// <param name="link">The <c>Link</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">link is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// link belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddLink(Link link)
        {
            this.AddAsChild(this.links, link);
        }
    }
}
