// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using SharpKml.Dom;

    /// <summary>
    /// Contains the nested <see cref="ElementSerializer"/> helper struct.
    /// </summary>
    public partial class Serializer
    {
        // Note: We're avoiding LINQ as a micro-optimization so we don't put
        // pressure on the GC with objects for the lambdas (the same reason
        // we're a struct - to avoid allocation)
        private struct ElementSerializer
        {
            private readonly XmlNamespaceManager manager;
            private readonly HashSet<Element> serializedElements;
            private readonly Serializer serializer;
            private readonly TypeBrowser typeBrowser;
            private readonly XmlWriter writer;

            public ElementSerializer(Serializer serializer, XmlWriter writer, XmlNamespaceManager manager, Type elementType)
            {
                this.manager = manager;
                this.serializedElements = new HashSet<Element>();
                this.serializer = serializer;
                this.typeBrowser = TypeBrowser.Create(elementType);
                this.writer = writer;
            }

            public void SerializeElements(Element element, Type elementType)
            {
                if ((elementType == null) || (elementType == typeof(object)))
                {
                    return;
                }

                // Start at the top of the inheritance hierarchy
                this.SerializeElements(element, elementType.GetTypeInfo().BaseType);

                foreach (TypeBrowser.ElementInfo elementInfo in this.typeBrowser.Elements)
                {
                    if (elementInfo.Property.DeclaringType == elementType)
                    {
                        this.serializer.WriteElement(
                            this.writer,
                            this.manager,
                            element,
                            elementInfo);
                    }
                }

                this.SerializeExtensions(element, elementType);
            }

            public void SerializeOrphans(Element element)
            {
                foreach (Element orphan in element.Orphans)
                {
                    if (!this.serializedElements.Contains(orphan))
                    {
                        this.serializer.SerializeElement(this.writer, this.manager, orphan);
                    }
                }
            }

            private void SerializeExtensions(Element element, Type elementType)
            {
                foreach (Type type in KmlFactory.GetKnownExtensionTypes(elementType))
                {
                    foreach (Element orphan in element.Orphans)
                    {
                        // Make sure we don't serialize the same element twice
                        // by adding it to the set
                        if ((orphan.GetType() == type) && this.serializedElements.Add(orphan))
                        {
                            this.serializer.SerializeElement(this.writer, this.manager, orphan);
                        }
                    }
                }
            }
        }
    }
}
