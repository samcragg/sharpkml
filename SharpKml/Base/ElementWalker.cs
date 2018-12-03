// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections.Generic;
    using SharpKml.Dom;

    /// <summary>
    /// Navigates all the children of an element.
    /// </summary>
    internal static class ElementWalker
    {
        /// <summary>
        /// Navigates the specified <see cref="Element"/> and it's children.
        /// </summary>
        /// <param name="root">The <c>Element</c> to navigate.</param>
        /// <exception cref="ArgumentNullException">root is null.</exception>
        /// <returns>An IEnumerable collection of the Elements.</returns>
        public static IEnumerable<Element> Walk(Element root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            return WalkElement(root);
        }

        private static IEnumerable<Element> GetPropertyElements(Element element, TypeBrowser.ElementInfo info)
        {
            // All properties with their ElementName set to null will be Elements
            // Check here to avoid the GetValue the property is not an Element.
            if (string.IsNullOrEmpty(info.Component.Name))
            {
                object value = info.GetValue(element);
                if (value is IEnumerable<Element> collection)
                {
                    foreach (Element e in collection)
                    {
                        yield return e;
                    }
                }
                else if (value is Element propertyElement)
                {
                    yield return propertyElement;
                }
            }
        }

        // Because the Element's won't be nested too deep (max 100), a Stack based
        // iterative approach is not necessary so recursion is used for clarity.
        private static IEnumerable<Element> WalkElement(Element element)
        {
            var customElement = element as ICustomElement;
            if ((customElement == null) || customElement.ProcessChildren)
            {
                yield return element; // Is a valid Element

                // Explore the properties
                var browser = TypeBrowser.Create(element.GetType());
                foreach (TypeBrowser.ElementInfo info in browser.Elements)
                {
                    foreach (Element property in GetPropertyElements(element, info))
                    {
                        foreach (Element e in WalkElement(property))
                        {
                            yield return e;
                        }
                    }
                }
            }
        }
    }
}
