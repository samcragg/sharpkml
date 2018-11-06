// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Helper class for extracting properties with a KmlAttribute/KmlElement
    /// assigned to them, searching the entire inheritance hierarchy of the type.
    /// </summary>
    internal sealed class TypeBrowser
    {
        // Used to stored child types in the right order
        private const int DepthMultiplier = 100;

        // Used for a cache. This is very important for performance as it reduces
        // the amount of work done in reflection, as the attributes associated
        // with a Type won't change during the lifetime of the program
        private static readonly Dictionary<Type, TypeBrowser> Types = new Dictionary<Type, TypeBrowser>();
        private static readonly object TypesLock = new object();

        private readonly Dictionary<XmlComponent, ElementInfo> attributes = new Dictionary<XmlComponent, ElementInfo>();
        private readonly Dictionary<XmlComponent, ElementInfo> elements = new Dictionary<XmlComponent, ElementInfo>();

        // We need to also store it in the correct order when iterating
        private readonly List<ElementInfo> orderedElements = new List<ElementInfo>();

        // Current depth of inheritance
        private int depth = 0;

        private TypeBrowser(Type type)
        {
            this.ExtractAttributes(type);

            // Go in reverse so we store the first element
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

            this.depth++;
        }

        private IEnumerable<ElementInfo> ExtractPropertyElements(TypeInfo typeInfo)
        {
            bool IsInstanceReadWriteProperty(PropertyInfo property)
            {
                return property.CanRead && property.CanWrite && !property.GetMethod.IsStatic;
            }

            foreach (PropertyInfo property in typeInfo.DeclaredProperties.Where(IsInstanceReadWriteProperty))
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
                        yield return new ElementInfo(property, null, kmlElement.ElementName, kmlElement.Namespace, kmlElement.Order + (this.depth * DepthMultiplier));
                    }
                }
            }

            Dictionary<TypeInfo, Dom.ChildTypeInfo> childTypes = Dom.Element.GetChildTypesFor(typeInfo.AsType());

            foreach (KeyValuePair<TypeInfo, Dom.ChildTypeInfo> kvp in childTypes.Where(kvp => kvp.Value.ParentType == typeInfo))
            {
                KmlElementAttribute kmlElement = GetElement(kvp.Key);

                if (kmlElement != null)
                {
                    yield return new ElementInfo(kvp.Key, null, null, kmlElement.Namespace, kvp.Value.Order + (this.depth * DepthMultiplier));
                }
                else
                {
                    yield return new ElementInfo(kvp.Key, null, null, null, kvp.Value.Order + (this.depth * DepthMultiplier));
                }
            }
        }

        /// <summary>
        /// Represents information about a property.
        /// </summary>
        internal sealed class ElementInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ElementInfo"/> class.
            /// </summary>
            /// <param name="property">The property information.</param>
            /// <param name="kmlAttribute">The KML attribute information.</param>
            public ElementInfo(PropertyInfo property, KmlAttributeAttribute kmlAttribute)
                : this(property)
            {
                this.Component = new XmlComponent(null, kmlAttribute.AttributeName, null);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ElementInfo"/> class.
            /// </summary>
            /// <param name="property">The property information.</param>
            /// <param name="prefix">The namespace prefix</param>
            /// <param name="local">Local name in the namespace</param>
            /// <param name="uri">Namespace Uri</param>
            /// <param name="order">Element order</param>
            public ElementInfo(PropertyInfo property, string prefix, string local, string uri, int order)
                : this(property)
            {
                this.Component = new XmlComponent(prefix, local, uri);
                this.Order = order;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ElementInfo"/> class.
            /// </summary>
            /// <param name="childType">The child type information.</param>
            /// <param name="prefix">The namespace prefix</param>
            /// <param name="local">Local name in the namespace</param>
            /// <param name="uri">Namespace Uri</param>
            /// <param name="order">Element order</param>
            public ElementInfo(TypeInfo childType, string prefix, string local, string uri, int order)
               : this(childType)
            {
                this.Component = new XmlComponent(prefix, local, uri);
                this.Order = order;
            }

            private ElementInfo(PropertyInfo property)
            {
                this.GetValue = CreateGetValueDelegate(property);
                this.SetValue = CreateSetValueDelegate(property);
                this.ValueType = property.PropertyType;
            }

            private ElementInfo(TypeInfo childType)
            {
                this.GetValue = CreateGetValueDelegate(childType);
                this.SetValue = new Action<object, object>((instance, value) => { throw new InvalidOperationException("Child type must be added through Element.TryAddChild()"); });
                this.ValueType = childType.AsType();
            }

            /// <summary>
            /// Gets the XML information for the property.
            /// </summary>
            public XmlComponent Component { get; }

            /// <summary>
            /// Gets a delegate that can read the property value for a given instance.
            /// </summary>
            public Func<object, object> GetValue { get; }

            /// <summary>
            /// Gets a delegate that can write the property value for a given instance.
            /// </summary>
            public Action<object, object> SetValue { get; }

            /// <summary>
            /// Gets the type the property represents.
            /// </summary>
            public Type ValueType { get; }

            /// <summary>
            /// Gets the order the property should be serialized in.
            /// </summary>
            internal int Order { get; }

            private static Func<object, object> CreateGetValueDelegate(PropertyInfo property)
            {
                ParameterExpression instance = Expression.Parameter(typeof(object));
                Expression getAndConvert = Expression.Convert(
                    Expression.Property(
                        Expression.Convert(instance, property.DeclaringType),
                        property),
                    typeof(object));

                return Expression.Lambda<Func<object, object>>(getAndConvert, instance).Compile();
            }

            private static Func<object, object> CreateGetValueDelegate(TypeInfo childType)
            {
                return new Func<object, object>((instance) =>
                {
                    return ((Dom.Element)instance).Children.Where(c => childType.IsAssignableFrom(c.GetType().GetTypeInfo()));
                });
            }

            private static Action<object, object> CreateSetValueDelegate(PropertyInfo property)
            {
                ParameterExpression instance = Expression.Parameter(typeof(object));
                ParameterExpression value = Expression.Parameter(typeof(object));
                Expression convertAndSet = Expression.Assign(
                    Expression.Property(
                        Expression.Convert(instance, property.DeclaringType),
                        property),
                    Expression.Convert(value, property.PropertyType));

                return Expression.Lambda<Action<object, object>>(convertAndSet, instance, value).Compile();
            }
        }
    }
}
