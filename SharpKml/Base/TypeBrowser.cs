// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Helper class for extracting properties with a KmlAttribute/KmlElement
    /// assigned to them, searching the entire inheritance hierarchy of the type.
    /// </summary>
    internal class TypeBrowser
    {
        // Used for a cache. This is very important for performance as it reduces
        // the amount of work done in reflection, as the attributes associated
        // with a Type won't change during the lifetime of the program (ignoring
        // funky Emit etc. vodoo which this library doesn't use)
        private static readonly Dictionary<Type, TypeBrowser> Types = new Dictionary<Type, TypeBrowser>();
        private static readonly object TypesLock = new object();

        private readonly Dictionary<XmlComponent, Tuple<PropertyInfo, KmlAttributeAttribute>> attributes =
            new Dictionary<XmlComponent, Tuple<PropertyInfo, KmlAttributeAttribute>>();

        // Needs to be ordered
        private readonly List<Tuple<XmlComponent, PropertyInfo, KmlElementAttribute>> elements =
            new List<Tuple<XmlComponent, PropertyInfo, KmlElementAttribute>>();

        private TypeBrowser(Type type)
        {
            this.ExtractAttributes(type);
        }

        /// <summary>
        /// Gets the properties with a KmlAttribute attribute.
        /// </summary>
        public IEnumerable<Tuple<PropertyInfo, KmlAttributeAttribute>> Attributes =>
            this.attributes.Values;

        /// <summary>
        /// Gets the properties with a KmlElement attribute.
        /// </summary>
        public IEnumerable<Tuple<PropertyInfo, KmlElementAttribute>> Elements =>
            this.elements.Select(e => Tuple.Create(e.Item2, e.Item3));

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
        /// Gets a KmlAttribute attribute associated with the specified
        /// MemberInfo.
        /// </summary>
        /// <param name="member">
        /// The MemberInfo to retrieve the attribute from.
        /// </param>
        /// <returns>
        /// A KmlAttributeAttribute associated with the specified value parameter
        /// if one was found; otherwise, null.
        /// </returns>
        public static KmlAttributeAttribute GetAttribute(MemberInfo member)
        {
            return GetAttribute<KmlAttributeAttribute>(member);
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
        /// A PropertyInfo for the first property found matching the specified
        /// information or null if no matches were found.
        /// </returns>
        public PropertyInfo FindAttribute(XmlComponent xml)
        {
            if (this.attributes.TryGetValue(xml, out Tuple<PropertyInfo, KmlAttributeAttribute> property))
            {
                return property.Item1;
            }

            return null;
        }

        /// <summary>
        /// Finds a property with the specified XML element information.
        /// </summary>
        /// <param name="xml">The XML information to find.</param>
        /// <returns>
        /// A PropertyInfo for the first property found matching the specified
        /// information or null if no matches were found.
        /// </returns>
        public PropertyInfo FindElement(XmlComponent xml)
        {
            return this.elements
                .Where(e => e.Item1.Equals(xml))
                .Select(e => e.Item2)
                .FirstOrDefault();
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

        private void ExtractAttributes(Type type)
        {
            if (type == null || type == typeof(object))
            {
                return; // We've reached the top, now we have to stop
            }

            // Look at the base type first as the KML schema specifies <sequence>
            // This will also find private fields in the base classes, which can't
            // be seen through a derived class.
            TypeInfo typeInfo = type.GetTypeInfo();
            this.ExtractAttributes(typeInfo.BaseType);

            // Store the found elements here so we can add them in order later
            var elements = new List<Tuple<XmlComponent, PropertyInfo, KmlElementAttribute>>();
            foreach (PropertyInfo property in typeInfo.DeclaredProperties.Where(p => !p.GetMethod.IsStatic))
            {
                KmlAttributeAttribute attribute = GetAttribute(property);
                if (attribute != null)
                {
                    var component = new XmlComponent(null, attribute.AttributeName, null);

                    // Check if a property has already been registered with the info.
                    // Ignore later properties - i.e. don't throw an exception.
                    if (!this.attributes.ContainsKey(component))
                    {
                        this.attributes.Add(component, Tuple.Create(property, attribute));
                    }
                }
                else
                {
                    KmlElementAttribute element = GetElement(property);
                    if (element != null)
                    {
                        var component = new XmlComponent(null, element.ElementName, element.Namespace);
                        elements.Add(Tuple.Create(component, property, element));
                    }
                }
            }

            // Now add the elements in order
            this.elements.AddRange(elements.OrderBy((Tuple<XmlComponent, PropertyInfo, KmlElementAttribute> e) => e.Item3.Order));
        }
    }
}
