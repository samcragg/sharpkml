// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Specifies an addition, change, or deletion to a KML resource that has
    /// previously been retrieved via <see cref="NetworkLink"/>.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 13.3</para>
    /// <para>Update does not affect the KML resource itself; rather it updates
    /// its representation within the earth browser only.</para>
    /// <para>All KML objects within an update context, that is a grandchild of
    /// the Update element, shall have a <see cref="KmlObject.TargetId"/> that
    /// identifies the object to be updated, and shall not have an
    /// <see cref="KmlObject.Id"/>.</para>
    /// </remarks>
    [KmlElement("Update")]
    public class Update : Element
    {
        private readonly List<Element> updates = new List<Element>();

        /// <summary>
        /// Gets or sets the URL for the target KML resource that has been
        /// previously retrieved via <see cref="NetworkLink"/>.
        /// </summary>
        [KmlElement("targetHref", 1)]
        public Uri Target { get; set; }

        /// <summary>
        /// Gets the update elements (<see cref="ChangeCollection"/>, <see cref="CreateCollection"/>
        /// or <see cref="DeleteCollection"/>) associated with this instance.
        /// </summary>
        [KmlElement(null, 2)]
        public IReadOnlyCollection<Element> Updates => this.updates;

        /// <summary>
        /// Adds the specified <see cref="ChangeCollection"/> to <see cref="Updates"/>.
        /// </summary>
        /// <param name="update">The <c>Change</c> to add.</param>
        /// <exception cref="ArgumentNullException">update is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// update belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddUpdate(ChangeCollection update)
        {
            this.AddAsChild(this.updates, update);
        }

        /// <summary>
        /// Adds the specified <see cref="CreateCollection"/> to <see cref="Updates"/>.
        /// </summary>
        /// <param name="update">The <c>Create</c> to add.</param>
        /// <exception cref="ArgumentNullException">update is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// update belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddUpdate(CreateCollection update)
        {
            this.AddAsChild(this.updates, update);
        }

        /// <summary>
        /// Adds the specified <see cref="DeleteCollection"/> to <see cref="Updates"/>.
        /// </summary>
        /// <param name="update">The <c>Delete</c> to add.</param>
        /// <exception cref="ArgumentNullException">update is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// update belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddUpdate(DeleteCollection update)
        {
            this.AddAsChild(this.updates, update);
        }

        /// <inheritdoc />
        protected internal override void AddOrphan(Element orphan)
        {
            switch (orphan)
            {
                case ChangeCollection change:
                    this.AddUpdate(change);
                    break;

                case CreateCollection create:
                    this.AddUpdate(create);
                    break;

                case DeleteCollection delete:
                    this.AddUpdate(delete);
                    break;

                default:
                    base.AddOrphan(orphan);
                    break;
            }
        }
    }
}
