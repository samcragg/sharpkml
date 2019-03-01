// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Engine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using SharpKml.Base;
    using SharpKml.Dom;

    /// <summary>
    /// Provides extension methods for <see cref="Element"/> objects.
    /// </summary>
    public static class ElementExtensions
    {
        /// <summary>
        /// Creates a deep copy of the <see cref="Element"/>.
        /// </summary>
        /// <typeparam name="T">A class deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <returns>A new <c>Element</c> representing the same data.</returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static T Clone<T>(this T element)
            where T : Element
        {
            Check.IsNotNull(element, nameof(element));

            // Special case as IconStyle has the same Kml name as Icon
            if (element is IconStyle.IconLink iconLink)
            {
                return (T)CloneIconLink(iconLink);
            }
            else
            {
                return (T)CloneElement(element);
            }
        }

        /// <summary>
        /// Provides a way to iterate over all children <see cref="Element"/>s
        /// contained by this instance.
        /// </summary>
        /// <param name="element">The class instance.</param>
        /// <returns>An IEnumerable&lt;Element&gt; for specified element.</returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static IEnumerable<Element> Flatten(this Element element)
        {
            Check.IsNotNull(element, nameof(element));

            return ElementWalker.Walk(element);
        }

        /// <summary>
        /// Finds the <see cref="Element.Parent"/> of the element which is
        /// of the specified type.
        /// </summary>
        /// <typeparam name="T">A type deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <returns>
        /// The closest element in the hierarchy of the specified type or null
        /// if no elements in the hierarchy are of the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static T GetParent<T>(this Element element)
            where T : Element
        {
            Check.IsNotNull(element, nameof(element));

            Element parent = element.Parent;
            while (parent != null)
            {
                if (parent is T typed)
                {
                    return typed;
                }

                parent = parent.Parent;
            }

            return null;
        }

        /// <summary>
        /// Determines if the <see cref="Element"/> has a parent of the
        /// specified type.
        /// </summary>
        /// <typeparam name="T">A type deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <returns>
        /// True if an element further up the hierarchy is of the specified type;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">element is null.</exception>
        public static bool IsChildOf<T>(this Element element)
            where T : Element
        {
            return GetParent<T>(element) != null;
        }

        /// <summary>
        /// Copies the <see cref="Element"/>s properties from the specified source.
        /// </summary>
        /// <typeparam name="T">A class deriving from <c>Element</c>.</typeparam>
        /// <param name="element">The class instance.</param>
        /// <param name="source">The element to copy from.</param>
        /// <exception cref="ArgumentException">
        /// element and source do not have the same type.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// element or source is null.
        /// </exception>
        public static void Merge<T>(this T element, T source)
            where T : Element
        {
            Check.IsNotNull(element, nameof(element));
            Check.IsNotNull(source, nameof(source));

            // Check that one isn't a more derived class
            if (element.GetType() != source.GetType())
            {
                throw new ArgumentException("source type must match that of the target instance.");
            }

            // Make sure we're not playing with ourselves... so to speak
            if (!object.ReferenceEquals(element, source))
            {
                Merge(source, element, source.GetType());
            }
        }

        private static Element CloneElement(Element element)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new Serializer();
                serializer.Serialize(element, stream);

                stream.Position = 0;
                var parser = new Parser();
                parser.Parse(stream);
                return parser.Root;
            }
        }

        private static Element CloneIconLink(IconStyle.IconLink iconLink)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new Serializer();
                if (iconLink.Parent == null)
                {
                    var parent = new IconStyle
                    {
                        Icon = iconLink
                    };
                    serializer.Serialize(parent, stream);
                    parent.Icon = null; // Sets the Icon's Parent property back to null
                }
                else
                {
                    serializer.Serialize(iconLink.Parent, stream);
                }

                stream.Position = 0;
                var parser = new Parser();
                parser.Parse(stream);

                var root = (IconStyle)parser.Root;
                IconStyle.IconLink output = root.Icon;
                root.Icon = null; // Clear the output's parent
                return output;
            }
        }

        private static object CreateClone(object value)
        {
            Type type = value.GetType();

            // First check if we can simple return the passed in value as value
            // types and strings are immutable. This also includes the Color32.
            if (type == typeof(string) || type.GetTypeInfo().IsValueType)
            {
                return value;
            }
            else if (type == typeof(Uri))
            {
                return new Uri(((Uri)value).OriginalString, UriKind.RelativeOrAbsolute);
            }

            return null;
        }

        private static void Merge(object source, object target, Type type)
        {
            var browser = TypeBrowser.Create(type);
            foreach (TypeBrowser.ElementInfo element in browser.Attributes.Concat(browser.Elements))
            {
                object value = element.GetValue(source);
                if (element.IsCollection)
                {
                    foreach (object item in (IEnumerable)value)
                    {
                        MergeValue(element, target, item);
                    }
                }
                else
                {
                    MergeValue(element, target, value);
                }
            }

            if (source is IHtmlContent htmlContent && (htmlContent.Text != null))
            {
                ((IHtmlContent)target).Text = htmlContent.Text;
            }
        }

        private static Element MergeElements(Element source, Element target)
        {
            // If the target is null there's nothing to merge with, so create a copy.
            // Also create a copy if the source is an ICustomElement and overwrite
            // the value in target.
            if (target == null || (source is ICustomElement))
            {
                return Clone(source);
            }

            // Else merge and return the updated target.
            Merge(target, source);
            return target;
        }

        private static void MergeValue(TypeBrowser.ElementInfo element, object target, object value)
        {
            // First check if it's an element and merge any existing info.
            if (value is Element sourceElement)
            {
                value = MergeElements(sourceElement, element.GetValue(target) as Element);
            }
            else if (value != null)
            {
                value = CreateClone(value);
            }

            if (value != null)
            {
                element.SetValue(target, value);
            }
        }
    }
}
