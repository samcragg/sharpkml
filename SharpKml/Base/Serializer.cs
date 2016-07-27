﻿namespace SharpKml.Base
{
    using System;
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
        private string xml;

        /// <summary>
        /// Gets the XML content after the most recent call to
        /// <see cref="Serialize(Element)"/>.
        /// </summary>
        public string Xml
        {
            get { return this.xml; }
        }

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
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
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

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;
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
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            this.Serialize(root, settings);
        }

        private static bool IsCData(string input)
        {
            return input.StartsWith("<![CDATA[") && input.EndsWith("]]>");
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
                SerializeElement(writer, root);
                writer.Flush();
            }
        }

        private static void SerializeElement(XmlWriter writer, Element element)
        {
            // Write start tag
            XmlComponent component = KmlFactory.FindType(element.GetType());

            // Custom elements take priority over component
            ICustomElement customElement = element as ICustomElement;
            if (customElement != null)
            {
                customElement.CreateStartElement(writer);
                if (!customElement.ProcessChildren)
                {
                    return; // Don't need to to any more work.
                }
            }
            else if (component != null)
            {
                writer.WriteStartElement(component.Name, component.NamespaceUri);
            }
            else
            {
                // We can't handle it so ignore it
                System.Diagnostics.Debug.WriteLine("Unknown Element type - please register first." + element.GetType());
                return; // Skip
            }

            // Write the attributes - unknown, serialized then namespaces.
            foreach (var att in element.Attributes)
            {
                writer.WriteAttributeString(att.Prefix, att.Name, att.NamespaceUri, att.Value);
            }

            WriteAttributes(writer, element);

            foreach (var ns in element.Namespaces.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml))
            {
                writer.WriteAttributeString("xmlns", ns.Key, string.Empty, ns.Value);
            }

            // Now the text part
            WriteData(writer, element.InnerText);

            // Now write the elements - serialized, children then unknown children.
            WriteElements(writer, element);
            SerializeElements(writer, element.OrderedChildren);
            SerializeElements(writer, element.Orphans);

            // Finished...
            writer.WriteEndElement();
        }

        private static void SerializeElements(XmlWriter writer, IEnumerable<Element> elements)
        {
            foreach (var element in elements)
            {
                SerializeElement(writer, element);
            }
        }

        private static void WriteAttributes(XmlWriter writer, Element element)
        {
            TypeBrowser browser = TypeBrowser.Create(element.GetType());
            foreach (var property in browser.Attributes)
            {
                object value = property.Item1.GetValue(element, null);

                // Make sure it needs saving
                if (value != null)
                {
                    writer.WriteAttributeString(property.Item2.AttributeName, GetString(value));
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

        private static void WriteElements(XmlWriter writer, Element element)
        {
            TypeBrowser browser = TypeBrowser.Create(element.GetType());

            foreach (var property in browser.Elements)
            {
                object value = property.Item1.GetValue(element, null);

                // Make sure it needs saving
                if (value != null)
                {
                    // Is this an element?
                    if (property.Item2.ElementName == null)
                    {
                        SerializeElement(writer, (Element)value);
                    }
                    else
                    {
                        writer.WriteStartElement(property.Item2.ElementName, property.Item2.Namespace);
                        WriteData(writer, GetString(value));
                        writer.WriteEndElement();
                    }
                }
            }
        }

        private void Serialize(Element root, XmlWriterSettings settings)
        {
            using (var stream = new MemoryStream())
            {
                this.xml = null;
                Serialize(root, stream, settings);
                this.xml = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Length);
            }
        }
    }
}
