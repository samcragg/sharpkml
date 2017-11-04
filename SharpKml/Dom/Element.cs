// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using SharpKml.Base;

    /// <summary>
    /// Represents the base class of all KML elements.
    /// </summary>
    public abstract class Element
    {
        private readonly List<XmlComponent> attributes = new List<XmlComponent>();
        private readonly List<Element> children = new List<Element>();
        private readonly Dictionary<TypeInfo, int> childTypes = new Dictionary<TypeInfo, int>(); // Will store the type and it's order
        private readonly List<Element> orphans = new List<Element>();
        private readonly StringBuilder text = new StringBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        protected Element()
        {
            this.Children = new ReadOnlyCollection<Element>(this.children);
            this.Namespaces = new XmlNamespaceManager(new NameTable());
        }

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
        internal IEnumerable<XmlComponent> Attributes
        {
            get { return this.attributes; }
        }

        /// <summary>
        /// Gets the child elements of this instance, in their serialization order.
        /// </summary>
        internal IEnumerable<Element> OrderedChildren
        {
            get
            {
                return this.children.OrderBy(e => e.GetType().GetTypeInfo(), new ChildTypeComparer(this));
            }
        }

        /// <summary>
        /// Gets the XML namespaces associated with this instance.
        /// </summary>
        internal XmlNamespaceManager Namespaces { get; private set; }

        /// <summary>
        /// Gets invalid child Elements found during parsing.
        /// </summary>
        internal IEnumerable<Element> Orphans
        {
            get { return this.orphans; }
        }

        /// <summary>
        /// Gets the child elements of this instance.
        /// </summary>
        protected internal ReadOnlyCollection<Element> Children { get; private set; }

        /// <summary>
        /// Gets the inner text of the XML element.
        /// </summary>
        protected internal string InnerText
        {
            get { return this.text.ToString(); }
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
        /// Adds a valid child Element to the end of <see cref="Children"/>.
        /// </summary>
        /// <typeparam name="T">A type deriving from Element.</typeparam>
        /// <param name="child">The Element to be added.</param>
        /// <returns>
        /// true if the value parameter is a valid child of this instance and,
        /// therefore, has been added to <c>Children</c>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">child is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// child belongs to another Element (i.e. the value of it's
        /// <see cref="Parent"/> is not null).
        /// </exception>
        protected internal bool AddChild<T>(T child)
            where T : Element // Use generics so we don't get a warning from FxCop telling us to use base class Element when a child calls this method
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if (child.Parent != null)
            {
                throw new InvalidOperationException("Cannot add child element to this instance because it belongs to another instance.");
            }

            // Check if this is a valid child. We use IsAssignableFrom to enable
            // derived classes to be added as well e.g. if Feature is registered
            // as a valid child type and the child is a Placemark then add it.
            TypeInfo childType = child.GetType().GetTypeInfo();
            foreach (var type in this.childTypes)
            {
                if (type.Key.IsAssignableFrom(childType))
                {
                    // If this is a derived class, add the type to the childtype
                    // collection with the same index.
                    if (type.Key != childType)
                    {
                        // It's ok to change the collection as we're breaking
                        // out of the iteration.
                        this.childTypes[childType] = type.Value;
                    }

                    this.children.Add(child);
                    child.Parent = this;
                    return true;
                }
            }

            return false;
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
            this.text.Append(text);
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
        /// Removes the specified Element from the <see cref="Children"/> collection.
        /// </summary>
        /// <param name="child">The Element to remove.</param>
        /// <returns>
        /// true if the value parameter is successfully removed; otherwise,
        /// false. This method also returns false if item was not found in
        /// <c>Children</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">child is null.</exception>
        protected internal bool RemoveChild(Element child)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }

            if ((child.Parent == this) && this.children.Remove(child))
            {
                child.Parent = null;
                return true;
            }

            return false; // Not ours
        }

        /// <summary>
        /// Removes all characters from <see cref="InnerText"/>.
        /// </summary>
        protected void ClearInnerText()
        {
            this.text.Clear();
        }

        /// <summary>
        /// Registers an element type as a valid child of this instance.
        /// </summary>
        /// <typeparam name="T">A type deriving from Element.</typeparam>
        protected void RegisterValidChild<T>()
            where T : Element
        {
            this.childTypes.Add(typeof(T).GetTypeInfo(), this.childTypes.Count); // Remember the order they were added.
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

        /// <summary>
        /// Private class used to sort the Children by the order the type
        /// was registered.
        /// </summary>
        private class ChildTypeComparer : IComparer<TypeInfo>
        {
            private Element owner;

            public ChildTypeComparer(Element owner)
            {
                this.owner = owner;
            }

            public int Compare(TypeInfo typeA, TypeInfo typeB)
            {
                int indexA = this.owner.childTypes[typeA];
                int indexB = this.owner.childTypes[typeB];
                return indexA.CompareTo(indexB);
            }
        }
    }
}
