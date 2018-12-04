// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using SharpKml.Dom;

    /// <summary>
    /// Helper class for extracting properties with a KmlAttribute/KmlElement
    /// assigned to them, searching the entire inheritance hierarchy of the type.
    /// </summary>
    internal sealed partial class TypeBrowser
    {
        // Used for a cache. This is very important for performance as it reduces
        // the amount of work done in reflection, as the attributes associated
        // with a Type won't change during the lifetime of the program
        private static readonly Dictionary<Type, TypeBrowser> Types = new Dictionary<Type, TypeBrowser>();
        private static readonly object TypesLock = new object();

        private readonly Dictionary<XmlComponent, ElementInfo> attributes = new Dictionary<XmlComponent, ElementInfo>();
        private readonly Dictionary<XmlComponent, ElementInfo> elements = new Dictionary<XmlComponent, ElementInfo>();

        // We need to also store it in the correct order when iterating
        private readonly List<ElementInfo> orderedElements = new List<ElementInfo>();

        private TypeBrowser(Type type)
        {
            this.ExtractAttributes(type);

            // Go in reverse so we overwrite with the first element registered
            for (int i = this.orderedElements.Count - 1; i >= 0; i--)
            {
                ElementInfo info = this.orderedElements[i];
                this.elements[info.Component] = info;
            }
        }

        /// <summary>
        /// Gets the properties with a KmlAttribute attribute.
        /// </summary>
        public IEnumerable<ElementInfo> Attributes => this.attributes.Values;

        /// <summary>
        /// Gets the properties with a KmlElement attribute.
        /// </summary>
        public IEnumerable<ElementInfo> Elements => this.orderedElements;

        /// <summary>
        /// Creates TypeBrowser representing the specified type.
        /// </summary>
        /// <param name="type">The type to extract properties from.</param>
        /// <returns>
        /// A TypeBrowser containing information about the specified type.
        /// </returns>
        public static TypeBrowser Create(Type type)
        {
            TypeBrowser browser;
            lock (TypesLock)
            {
                if (!Types.TryGetValue(type, out browser))
                {
                    browser = new TypeBrowser(type);
                    Types.Add(type, browser);
                }
            }

            return browser;
        }

        /// <summary>
        /// Gets a KmlElement attribute associated with the specified
        /// MemberInfo.
        /// </summary>
        /// <param name="member">
        /// The MemberInfo to retrieve the attribute from.
        /// </param>
        /// <returns>
        /// A KmlElementAttribute associated with the specified value parameter
        /// if one was found; otherwise, null.
        /// </returns>
        public static KmlElementAttribute GetElement(MemberInfo member)
        {
            return GetAttribute<KmlElementAttribute>(member);
        }

        /// <summary>
        /// Gets a KmlElement attribute associated with the specified Enum.
        /// </summary>
        /// <param name="value">
        /// The Enum value to retrieve the attribute from.
        /// </param>
        /// <returns>
        /// A KmlElementAttribute associated with the specified value parameter
        /// if one was found; otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        public static KmlElementAttribute GetEnum(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                return GetAttribute<KmlElementAttribute>(type.GetTypeInfo().GetDeclaredField(name));
            }

            return null;
        }

        /// <summary>
        /// Finds a property with the specified XML attribute information.
        /// </summary>
        /// <param name="xml">The XML information to find.</param>
        /// <returns>
        /// The information for the first property found matching the specified
        /// information or null if no matches were found.
        /// </returns>
        public ElementInfo FindAttribute(XmlComponent xml)
        {
            this.attributes.TryGetValue(xml, out ElementInfo info);
            return info;
        }

        /// <summary>
        /// Finds a property with the specified XML element information.
        /// </summary>
        /// <param name="xml">The XML information to find.</param>
        /// <returns>
        /// A PropertyInfo for the first property found matching the specified
        /// information or null if no matches were found.
        /// </returns>
        public ElementInfo FindElement(XmlComponent xml)
        {
            this.elements.TryGetValue(xml, out ElementInfo info);
            return info;
        }

        private static T GetAttribute<T>(MemberInfo provider)
            where T : Attribute
        {
            if (provider == null)
            {
                return null;
            }

            return provider.GetCustomAttribute<T>(inherit: false);
        }

        private static bool IsEnumerable(PropertyInfo property)
        {
            if (property.PropertyType == typeof(string))
            {
                return false;
            }

            TypeInfo propertyType = property.PropertyType.GetTypeInfo();
            return typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(propertyType) &&
                   !typeof(ICustomElement).GetTypeInfo().IsAssignableFrom(propertyType);
        }

        private void ExtractAttributes(Type type)
        {
            if (type == null || type == typeof(object))
            {
                return; // We've reached the top, now we have to stop
            }

            // Look at the base type first as the KML schema specifies <sequence>
            // This will also find private fields in the base classes, which can't
            // be seen through a derived class.
            this.ExtractAttributes(type.GetTypeInfo().BaseType);

            // Add the elements in order
            IEnumerable<ElementInfo> elementInfos =
                this.ExtractPropertyElements(type.GetTypeInfo())
                    .OrderBy(e => e.Order);

            this.orderedElements.AddRange(elementInfos);
        }

        private IEnumerable<ElementInfo> ExtractPropertyElements(TypeInfo typeInfo)
        {
            bool IsSerializableProperty(PropertyInfo property)
            {
                return property.CanRead &&
                       !property.GetMethod.IsStatic &&
                       (property.CanWrite || IsEnumerable(property));
            }

            foreach (PropertyInfo property in typeInfo.DeclaredProperties.Where(IsSerializableProperty))
            {
                KmlAttributeAttribute attribute = GetAttribute<KmlAttributeAttribute>(property);
                if (attribute != null)
                {
                    var component = new XmlComponent(null, attribute.AttributeName, null);

                    // Check if a property has already been registered with the info.
                    // Ignore later properties - i.e. don't throw an exception.
                    if (!this.attributes.ContainsKey(component))
                    {
                        this.attributes.Add(component, new ElementInfo(property, attribute));
                    }
                }
                else
                {
                    KmlElementAttribute kmlElement = GetElement(property);
                    if (kmlElement != null)
                    {
                        yield return new ElementInfo(property, kmlElement);
                    }
                }
            }
        }
    }
}
