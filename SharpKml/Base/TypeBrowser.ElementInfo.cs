// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Contains the nested <see cref="ElementInfo"/> helper class.
    /// </summary>
    internal sealed partial class TypeBrowser
    {
        /// <summary>
        /// Represents information about a property.
        /// </summary>
        [DebuggerDisplay("{Property.Name,nq}")]
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
            /// <param name="kmlElement">The KML element information.</param>
            public ElementInfo(PropertyInfo property, KmlElementAttribute kmlElement)
                : this(property)
            {
                this.Component = new XmlComponent(null, kmlElement.ElementName, kmlElement.Namespace);
                this.Order = kmlElement.Order;
            }

            private ElementInfo(PropertyInfo property)
            {
                this.GetValue = CreateGetValueDelegate(property);
                this.IsCollection = IsEnumerable(property);
                this.Property = property;
                this.SetValue = CreateSetValueDelegate(property, this.IsCollection, out Type valueType);
                this.ValueType = valueType;
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
            /// Gets a value indicating whether this instance represents a
            /// collection or not.
            /// </summary>
            public bool IsCollection { get; }

            /// <summary>
            /// Gets the property represented by this instance.
            /// </summary>
            public PropertyInfo Property { get; }

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

            private static Expression CallAddMethod(PropertyInfo property, Expression instance, Expression value, out Type valueType)
            {
                Type collectionType = FindCollectionType(property.PropertyType);
                if (collectionType == null)
                {
                    throw new InvalidOperationException("Collection type must implement IEnumerable<T>");
                }

                MethodInfo addMethod =
                    (from method in property.DeclaringType.GetRuntimeMethods()
                     where method.Name.StartsWith("Add", StringComparison.Ordinal)
                     let parameters = method.GetParameters()
                     where (parameters.Length == 1) && (parameters[0].ParameterType == collectionType)
                     select method)
                    .FirstOrDefault();

                if (addMethod == null)
                {
                    throw new InvalidOperationException("Unable to find add method for " + property.Name);
                }

                valueType = collectionType;
                return Expression.Call(
                    Expression.Convert(instance, property.DeclaringType),
                    addMethod,
                    Expression.Convert(value, valueType));
            }

            private static Expression CallSetProperty(PropertyInfo property, Expression instance, Expression value, out Type valueType)
            {
                valueType = property.PropertyType;
                return Expression.Assign(
                    Expression.Property(
                        Expression.Convert(instance, property.DeclaringType),
                        property),
                    Expression.Convert(value, property.PropertyType));
            }

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

            private static Action<object, object> CreateSetValueDelegate(
                PropertyInfo property,
                bool isCollection,
                out Type valueType)
            {
                ParameterExpression instance = Expression.Parameter(typeof(object));
                ParameterExpression value = Expression.Parameter(typeof(object));
                Expression setValue = isCollection ?
                    CallAddMethod(property, instance, value, out valueType) :
                    CallSetProperty(property, instance, value, out valueType);

                return Expression.Lambda<Action<object, object>>(setValue, instance, value).Compile();
            }

            private static Type FindCollectionType(Type type)
            {
                bool IsGenericIEnumerable(Type interfaceType)
                {
                    TypeInfo typeInfo = interfaceType.GetTypeInfo();
                    return typeInfo.IsGenericType &&
                           (typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                }

                // In case we're passed an interface, add it to the items to search
                return type.GetTypeInfo().ImplementedInterfaces
                    .Concat(new[] { type })
                    .Where(IsGenericIEnumerable)
                    .Select(i => i.GetTypeInfo().GenericTypeArguments[0])
                    .FirstOrDefault();
            }
        }
    }
}
