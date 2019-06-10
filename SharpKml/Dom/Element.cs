// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SharpKml.Base;
    using static System.Diagnostics.Debug;

    /// <summary>
    /// Represents the base class of all KML elements.
    /// </summary>
    public abstract class Element
    {
        private readonly List<XmlComponent> attributes = new List<XmlComponent>();
        private readonly Dictionary<string, string> namespaces = new Dictionary<string, string>();
        private readonly List<Element> orphans = new List<Element>();

        /// <summary>
        /// Gets the collection of extension elements added to this instance.
        /// </summary>
        public IEnumerable<Element> Children =>
            this.orphans.Where(x => KmlFactory.IsKnownExtensionType(this.GetType(), x.GetType()));

        /// <summary>
        /// Gets the parent Element of this instance.
        /// </summary>
        /// <remarks>
        /// Will return null if this instance has not been attached to an Element.
        /// </remarks>
        public Element Parent { get; private set; }

        /// <summary>
        /// Gets unknown attributes found during parsing.
        /// </summary>
        internal IReadOnlyCollection<XmlComponent> Attributes => this.attributes;

        /// <summary>
        /// Gets invalid child Elements found during parsing.
        /// </summary>
        internal IReadOnlyCollection<Element> Orphans => this.orphans;

        /// <summary>
        /// Gets the inner text of the XML element.
        /// </summary>
        protected internal string InnerText { get; private set; } = string.Empty;

        /// <summary>
        /// Adds the specified element to this instance.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <exception cref="ArgumentNullException">child is null.</exception>
        public void AddChild(Element child)
        {
            Check.IsNotNull(child, nameof(child));

            if (!KmlFactory.IsKnownExtensionType(this.GetType(), child.GetType()))
            {
                throw new ArgumentException("Element has not been registered as a valid child type.");
            }

            this.AddAsChild(this.orphans, child);
        }

        /// <summary>
        /// Removes the specified Element from the <see cref="Children"/> collection.
        /// </summary>
        /// <param name="child">The Element to remove.</param>
        /// <returns>
        /// true if the value parameter is successfully removed; otherwise,
        /// false. This method also returns false if item was not found in
        /// <c>Children</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">child is null.</exception>
        public bool RemoveChild(Element child)
        {
            Check.IsNotNull(child, nameof(child));

            if ((child.Parent == this) && this.orphans.Remove(child))
            {
                child.Parent = null;
                return true;
            }

            return false; // Not ours
        }

        /// <summary>
        /// Stores unknown attributes found during parsing for later serialization.
        /// </summary>
        /// <param name="attribute">Contains serialization information.</param>
        protected internal virtual void AddAttribute(XmlComponent attribute)
        {
            if (attribute != null)
            {
                this.attributes.Add(attribute);
            }
        }

        /// <summary>
        /// Stores the inner text of the XML element.
        /// </summary>
        /// <param name="text">The text content of the XML element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Adding to the buffer would exceed StringBuilder.MaxCapacity.
        /// </exception>
        protected internal virtual void AddInnerText(string text)
        {
            this.InnerText += text;
        }

        /// <summary>
        /// Adds an XML namespace to the element.
        /// </summary>
        /// <param name="prefix">The namespace prefix.</param>
        /// <param name="ns">The namespace URI.</param>
        protected internal void AddNamespace(string prefix, string ns)
        {
            this.namespaces[prefix] = ns;
        }

        /// <summary>
        /// Stores an invalid child Element for later serialization.
        /// </summary>
        /// <param name="orphan">The Element to store for serialization.</param>
        protected internal virtual void AddOrphan(Element orphan)
        {
            if (orphan != null)
            {
                orphan.Parent = this;
                this.orphans.Add(orphan);
            }
        }

        /// <summary>
        /// Returns namespaces declared for the current Element (excluding the
        /// default XML namespace).
        /// </summary>
        /// <returns>
        /// A IDictionary containing the declared namespaces where Key is
        /// namespace prefix and Value namespace URI.
        /// </returns>
        protected internal IDictionary<string, string> GetNamespaces()
        {
            return this.namespaces.Where(kvp => kvp.Key != "xml")
                .ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// Validates and adds a child Element to the collection.
        /// </summary>
        /// <typeparam name="T">A type deriving from Element.</typeparam>
        /// <param name="collection">The collection to add to.</param>
        /// <param name="child">The Element to be added.</param>
        /// <exception cref="ArgumentNullException">child is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// child belongs to another Element (i.e. the value of it's
        /// <see cref="Parent"/> is not null).
        /// </exception>
        protected void AddAsChild<T>(ICollection<T> collection, T child)
            where T : Element
        {
            Check.IsNotNull(collection, nameof(collection));
            Check.IsNotNull(child, nameof(child));

            if (child.Parent != null)
            {
                throw new InvalidOperationException("Cannot add child element to this instance because it belongs to another instance.");
            }

            collection.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Removes all characters from <see cref="InnerText"/>.
        /// </summary>
        protected void ClearInnerText()
        {
            this.InnerText = string.Empty;
        }

        /// <summary>
        /// removes a child Element from the collection.
        /// </summary>
        /// <typeparam name="T">A type deriving from Element.</typeparam>
        /// <param name="collection">The collection to remove from.</param>
        /// <param name="child">The Element to be removed.</param>
        /// <returns>
        /// <c>true</c> if the child was removed; otherwise, <c>false</c>.
        /// </returns>
        protected bool RemoveChild<T>(ICollection<T> collection, T child)
            where T : Element
        {
            Check.IsNotNull(collection, nameof(collection));
            Check.IsNotNull(child, nameof(child));

            Assert(child.Parent == this, "Should only be called on children attached to this instance");
            if (collection.Remove(child))
            {
                child.Parent = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Resets the parent of the specified element.
        /// </summary>
        /// <param name="element">The element to reset.</param>
        protected void ResetParent(Element element)
        {
            Check.IsNotNull(element, nameof(element));
            if (element.Parent != this)
            {
                throw new InvalidOperationException("Can only reset the parent of elements owned by this instance.");
            }

            element.Parent = null;
        }

        /// <summary>
        /// Resets the parent of the specified elements.
        /// </summary>
        /// <param name="elements">The elements to reset.</param>
        protected void ResetParents(IEnumerable<Element> elements)
        {
            Check.IsNotNull(elements, nameof(elements));
            foreach (Element element in elements)
            {
                this.ResetParent(element);
            }
        }

        /// <summary>
        /// Sets the specified element to the specified value, updating
        /// <see cref="Parent"/> information as necessary.
        /// </summary>
        /// <typeparam name="T">A type deriving from Element.</typeparam>
        /// <param name="value">The new value to set to the element.</param>
        /// <param name="element">The element to set the value to.</param>
        protected void UpdatePropertyChild<T>(T value, ref T element)
            where T : Element
        {
            if (element != null)
            {
                element.Parent = null;
            }

            if (value != null)
            {
                value.Parent = this;
            }

            element = value;
        }
    }
}
