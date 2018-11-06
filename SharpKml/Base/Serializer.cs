// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using SharpKml.Dom;

    /// <summary>
    /// Serializes a derived class of <see cref="Element"/> into XML data.
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Gets the XML content after the most recent call to
        /// <see cref="Serialize(Element)"/>.
        /// </summary>
        public string Xml { get; private set; }

        /// <summary>
        /// Serializes the specified <see cref="Element"/> to XML.
        /// </summary>
        /// <param name="root">
        /// The <c>Element</c> to serialize, including all its children.
        /// </param>
        /// <remarks>
        /// The generated XML will be indented and have a full XML
        /// declaration header.
        /// </remarks>
        /// <exception cref="ArgumentNullException">root is null.</exception>
        public void Serialize(Element root)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates
            };

            this.Serialize(root, settings);
        }

        /// <summary>
        /// Serializes the specified <see cref="Element"/> as XML, writing the
        /// result to the specified stream.
        /// </summary>
        /// <param name="root">
        /// The <c>Element</c> to serialize, including all its children.
        /// </param>
        /// <param name="stream">Where to write the output.</param>
        /// <remarks>
        /// The generated XML will be indented and have a full XML
        /// declaration header.
        /// </remarks>
        /// <exception cref="ArgumentNullException">root or stream is null.</exception>
        public void Serialize(Element root, Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var settings = new XmlWriterSettings
            {
                Indent = true,
                NamespaceHandling = NamespaceHandling.OmitDuplicates
            };

            Serialize(root, stream, settings);
        }

        /// <summary>
        /// Serializes the specified <see cref="Element"/> to XML without formatting.
        /// </summary>
        /// <param name="root">
        /// The <c>Element</c> to serialize, including all its children.
        /// </param>
        /// <remarks>
        /// The generated XML will not contain white space between elements and
        /// the XML declaration will also be omitted.
        /// </remarks>
        /// <exception cref="ArgumentNullException">root is null.</exception>
        public void SerializeRaw(Element root)
        {
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            this.Serialize(root, settings);
        }

        private static string FindPrefix(XmlNamespaceManager manager, string ns)
        {
            // Give preference to the default namespace over using a prefix
            if (manager.DefaultNamespace == ns)
            {
                return string.Empty;
            }

            string prefix = manager.LookupPrefix(ns);
            if (prefix != null)
            {
                return prefix;
            }

            // Set the default namespace
            manager.AddNamespace(string.Empty, ns);
            return string.Empty;
        }

        private static string GetString(object value)
        {
            TypeInfo typeInfo = value.GetType().GetTypeInfo();
            if (typeInfo.IsEnum)
            {
                KmlElementAttribute att = TypeBrowser.GetEnum((Enum)value);
                if (att != null)
                {
                    return att.ElementName;
                }
            }

            return string.Format(KmlFormatter.Instance, "{0}", value);
        }

        private static bool IsCData(string input)
        {
            return input.StartsWith("<![CDATA[") && input.EndsWith("]]>");
        }

        private static void Serialize(Element root, Stream stream, XmlWriterSettings settings)
        {
            // We check here so the public functions don't need to
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            settings.Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var manager = new XmlNamespaceManager(new NameTable());
                SerializeElement(writer, manager, root);
                writer.Flush();
            }
        }

        private static void SerializeElement(XmlWriter writer, XmlNamespaceManager manager, Element element)
        {
            if (WriteStartTag(writer, manager, element, out string ns))
            {
                manager.PushScope();
                WriteAttributes(writer, manager, element);

                // Add the default namespace (if there is one) here so that it
                // takes precedence over other prefixes with the same namespace
                if (ns != null)
                {
                    manager.AddNamespace(string.Empty, ns);
                }

                WriteData(writer, element.InnerText);

                // Write the child elements: serialized, children then unknown children.
                WriteElements(writer, manager, element);
                SerializeElements(writer, manager, element.Orphans);

                writer.WriteEndElement();
                manager.PopScope();
            }
        }

        private static void SerializeElements(XmlWriter writer, XmlNamespaceManager manager, IEnumerable<Element> elements)
        {
            foreach (Element element in elements)
            {
                SerializeElement(writer, manager, element);
            }
        }

        private static void WriteAttributes(XmlWriter writer, XmlNamespaceManager manager, Element element)
        {
            // Write the attributes in this order: unknown, serialized and then namespaces.
            foreach (XmlComponent att in element.Attributes)
            {
                writer.WriteAttributeString(att.Prefix, att.Name, att.NamespaceUri, att.Value);
            }

            WriteAttributesForElement(writer, element);

            foreach (KeyValuePair<string, string> ns in element.GetNamespaces())
            {
                writer.WriteAttributeString("xmlns", ns.Key, string.Empty, ns.Value);
                manager.AddNamespace(ns.Key, ns.Value);
            }
        }

        private static void WriteAttributesForElement(XmlWriter writer, Element element)
        {
            var browser = TypeBrowser.Create(element.GetType());
            foreach (TypeBrowser.ElementInfo attribute in browser.Attributes)
            {
                object value = attribute.GetValue(element);

                // Make sure it needs saving
                if (value != null)
                {
                    writer.WriteAttributeString(attribute.Component.Name, GetString(value));
                }
            }
        }

        private static void WriteData(XmlWriter writer, string data)
        {
            // The XmlWriter will escape any illegal XML characters, but the original
            // C++ code would CDATA it instead, making sure it's not already a CDATA.

            // First make sure there is some data, as if we write string.Empty
            // then WriteEndElement will always write a full end element.
            if (!string.IsNullOrWhiteSpace(data))
            {
                if (IsCData(data))
                {
                    writer.WriteRaw(data); // Data is already escaped.
                }
                else if (data.IndexOfAny(new char[] { '&', '\'', '<', '>', '\"' }) != -1)
                {
                    // Illegal character found and the string isn't CDATA
                    writer.WriteCData(data);
                }
                else
                {
                    // Just write normal.
                    writer.WriteString(data);
                }
            }
        }

        private static void WriteElements(XmlWriter writer, XmlNamespaceManager manager, Element element)
        {
            var browser = TypeBrowser.Create(element.GetType());

            foreach (TypeBrowser.ElementInfo elementInfo in browser.Elements)
            {
                object value = elementInfo.GetValue(element);

                // Make sure it needs saving
                if (value != null)
                {
                    // Is this an element?
                    if (string.IsNullOrEmpty(elementInfo.Component.Name))
                    {
                        if (value is IEnumerable<Element>)
                        {
                            SerializeElements(writer, manager, (IEnumerable<Element>)value);
                        }
                        else
                        {
                            SerializeElement(writer, manager, (Element)value);
                        }
                    }
                    else
                    {
                        writer.WriteStartElement(elementInfo.Component.Name, elementInfo.Component.NamespaceUri);
                        WriteData(writer, GetString(value));
                        writer.WriteEndElement();
                    }
                }
            }
        }

        private static bool WriteStartTag(XmlWriter writer, XmlNamespaceManager manager, Element element, out string ns)
        {
            ns = null;

            // Custom elements take priority over component
            if (element is ICustomElement customElement)
            {
                customElement.CreateStartElement(writer);
                if (!customElement.ProcessChildren)
                {
                    return false; // Don't need to do any more work.
                }
            }
            else
            {
                XmlComponent component = KmlFactory.FindType(element.GetType());
                if (component == null)
                {
                    // We can't handle it so ignore it
                    System.Diagnostics.Debug.WriteLine("Unknown Element type - please register first." + element.GetType());
                    return false;
                }

                ns = component.NamespaceUri;
                string prefix = FindPrefix(manager, ns);
                writer.WriteStartElement(prefix, component.Name, component.NamespaceUri);
            }

            return true;
        }

        private void Serialize(Element root, XmlWriterSettings settings)
        {
            using (var stream = new MemoryStream())
            {
                this.Xml = null;
                Serialize(root, stream, settings);
                this.Xml = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            }
        }
    }
}
