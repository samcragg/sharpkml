// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SharpKml.Base;

    /// <summary>
    /// Represents the base class of all KML elements.
    /// </summary>
    public abstract class Element
    {
        // Will store the child type and it's order for each Element type.
        private static readonly Dictionary<Type, Dictionary<TypeInfo, int>> ChildTypes =
            new Dictionary<Type, Dictionary<TypeInfo, int>>();

        private readonly List<XmlComponent> attributes = new List<XmlComponent>();
        private readonly ElementCollection children;
        private readonly Dictionary<string, string> namespaces = new Dictionary<string, string>();
        private readonly List<Element> orphans = new List<Element>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        protected Element()
        {
            this.children = new ElementCollection(GetChildTypesFor(this.GetType()));
        }

        /// <summary>
        /// Gets the child elements of this instance.
        /// </summary>
        public IReadOnlyCollection<Element> Children => this.children;

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
        internal IEnumerable<XmlComponent> Attributes => this.attributes;

        /// <summary>
        /// Gets invalid child Elements found during parsing.
        /// </summary>
        internal IEnumerable<Element> Orphans => this.orphans;

        /// <summary>
        /// Gets the inner text of the XML element.
        /// </summary>
        protected internal string InnerText { get; private set; } = string.Empty;

        /// <summary>
        /// Adds the specified element to this instance.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(Element child)
        {
            if (!this.TryAddChild(child))
            {
                throw new ArgumentException("Element has not been registered as a valid child type.");
            }
        }

        /// <summary>
        /// Gets the known child types for the specified type.
        /// </summary>
        /// <param name="type">The type to get the registered children for.</param>
        /// <returns>
        /// A dictionary containing the registered children and their order.
        /// </returns>
        internal static Dictionary<TypeInfo, int> GetChildTypesFor(Type type)
        {
            lock (ChildTypes)
            {
                if (!ChildTypes.TryGetValue(type, out Dictionary<TypeInfo, int> childTypes))
                {
                    childTypes = new Dictionary<TypeInfo, int>();
                    AddChildTypesFromAttributes(type.GetTypeInfo(), childTypes);
                    ChildTypes.Add(type, childTypes);
                }

                return childTypes;
            }
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
        /// <param name="prefix">The namespace prefix</param>
        /// <param name="uri">The namespace URI</param>
        protected internal void AddNamespace(string prefix, string uri)
        {
            this.namespaces[prefix] = uri;
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
        protected internal virtual bool TryAddChild<T>(T child)
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

            Dictionary<TypeInfo, int> childTypes = GetChildTypesFor(this.GetType());
            TypeInfo childType = child.GetType().GetTypeInfo();
            if (!childTypes.ContainsKey(childType) &&
                !TryRegisterAsDerivedClass(childTypes, childType))
            {
                return false;
            }

            this.children.Add(child);
            child.Parent = this;
            return true;
        }

        /// <summary>
        /// Removes all characters from <see cref="InnerText"/>.
        /// </summary>
        protected void ClearInnerText()
        {
            this.InnerText = string.Empty;
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

        private static void AddChildTypesFromAttributes(TypeInfo type, Dictionary<TypeInfo, int> childTypes)
        {
            Type baseType = type.BaseType;
            if (baseType != null)
            {
                AddChildTypesFromAttributes(baseType.GetTypeInfo(), childTypes);
            }

            // Offset the order by the number of attributes we've added from
            // base classes (i.e. their children are ordered before ours)
            int offset = 0;
            if (childTypes.Count > 0)
            {
                offset = childTypes.Values.Max() + 1;
            }

            foreach (ChildTypeAttribute attribute in type.GetCustomAttributes(typeof(ChildTypeAttribute)))
            {
                childTypes.Add(
                    attribute.ChildType.GetTypeInfo(),
                    offset + attribute.Order);
            }
        }

        private static bool TryRegisterAsDerivedClass(Dictionary<TypeInfo, int> childTypes, TypeInfo childType)
        {
            foreach (KeyValuePair<TypeInfo, int> type in childTypes)
            {
                if (type.Key.IsAssignableFrom(childType))
                {
                    // This is a derived class so add it to the child type
                    // collection with the same index. Note it's OK to change
                    // the collection as we're breaking out of the iteration.
                    childTypes[childType] = type.Value;
                    return true;
                }
            }

            // We couldn't find a a type that childType inherits from
            return false;
        }
    }
}
