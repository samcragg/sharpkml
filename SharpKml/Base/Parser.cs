// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using SharpKml.Dom;

    /// <summary>
    /// Creates a structure of <see cref="Element"/>s from XML data.
    /// </summary>
    public sealed class Parser
    {
        /// <summary>
        /// Represents the namespace used when ignoring unknown prefixes.
        /// </summary>
        internal const string IgnoreNamespace = "uri:ignore";

        // The maximum nesting depth we permit. Depths beyond this are treated as errors.
        private const int MaxNestingDepth = 100;

        private bool currentElementIsEmpty;
        private string defaultNamespace;
        private XmlReader reader;

        /// <summary>
        /// Raised when a new <see cref="Element"/> is parsed and added to the
        /// hierarchy.
        /// </summary>
        public event EventHandler<ElementEventArgs> ElementAdded;

        /// <summary>
        /// Gets the root <see cref="Element"/> found during parsing.
        /// </summary>
        public Element Root { get; private set; }

        /// <summary>
        /// Parses the specified stream for classes deriving from
        /// <see cref="Element"/>.
        /// </summary>
        /// <param name="input">The stream containing the XML data.</param>
        /// <exception cref="ArgumentNullException">input is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The XML is nested too deeply.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// The caller does not have sufficient permissions to access the
        /// location of the XML data.
        /// </exception>
        /// <exception cref="XmlException">
        /// An error occurred while parsing the XML.
        /// </exception>
        public void Parse(Stream input)
        {
            this.defaultNamespace = null; // This method is strict about namespaces
            var reader = XmlReader.Create(input);
            this.Parse(reader);
        }

        /// <summary>
        /// Parses the specified stream for classes deriving from
        /// <see cref="Element"/>.
        /// </summary>
        /// <param name="input">
        /// The <see cref="TextReader"/> containing the XML data.
        /// </param>
        /// <exception cref="ArgumentNullException">input is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// The XML is nested too deeply.
        /// </exception>
        /// <exception cref="XmlException">
        /// An error occurred while parsing the XML.
        /// </exception>
        public void Parse(TextReader input)
        {
            this.defaultNamespace = null; // This method is strict about namespaces
            var reader = XmlReader.Create(input);
            this.Parse(reader);
        }

        /// <summary>
        /// Parses the specified string for classes deriving from
        /// <see cref="Element"/>.
        /// </summary>
        /// <param name="xml">The XML data to parse.</param>
        /// <param name="namespaces">
        /// true to allow namespace support; false to ignore namespaces.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// The XML is nested too deeply.
        /// </exception>
        /// <exception cref="XmlException">
        /// An error occurred while parsing the XML.
        /// </exception>
        public void ParseString(string xml, bool namespaces)
        {
            using (var stream = new StringReader(xml))
            {
                if (namespaces)
                {
                    this.Parse(stream);
                }
                else
                {
                    this.defaultNamespace = KmlNamespaces.Kml22Namespace;
                    this.Parse(
                        XmlReader.Create(
                        stream,
                        new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment },
                        new XmlParserContext(null, new IgnoreNamespaceManager(), null, XmlSpace.Default)));
                }
            }
        }

        private static void AssignValue(object instance, PropertyInfo property, string text)
        {
            if (!property.CanWrite)
            {
                return; // Can't do anything
            }

            // Get the type, checking if it's nullable, as TryParse doesn't exist on "int?"
            Type type = property.PropertyType;
            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                type = nullableType;
            }

            if (ValueConverter.TryGetValue(type, text, out object value))
            {
                if (value != null)
                {
                    property.SetValue(instance, value, null);
                }
                else if ((nullableType != null) || !type.GetTypeInfo().IsValueType)
                {
                    property.SetValue(instance, null, null);
                }
            }
        }

        private void AddChild(Element parent)
        {
            Element child = this.CreateElement();
            bool isOrphan;
            if (child is UnknownElement)
            {
                var browser = TypeBrowser.Create(parent.GetType());
                PropertyInfo property = browser.FindElement(this.GetXmlComponent());
                if (property != null)
                {
                    // We're not going to add it to the parent, which has the potential to
                    // lose any attributes/child elements assigned to the unknown, but this
                    // is the behaviour of the C++ version.
                    this.PopulateElement(child);
                    AssignValue(parent, property, child.InnerText);
                    return;
                }

                isOrphan = true;
            }
            else
            {
                isOrphan = !this.AddChildToParent(parent, child);
            }

            this.PopulateElement(child);
            this.OnElementAdded(child);

            if (isOrphan)
            {
                parent.AddOrphan(child); // Save for later serialization
            }
        }

        private bool AddChildToParent(Element parent, Element child)
        {
            if (parent.AddChild(child))
            {
                return true;
            }
            else
            {
                // Search for a property that we can assign to
                TypeInfo typeInfo = child.GetType().GetTypeInfo();
                var browser = TypeBrowser.Create(parent.GetType());
                foreach (PropertyInfo property in browser.Elements.Select(x => x.Item1))
                {
                    if (property.PropertyType.GetTypeInfo().IsAssignableFrom(typeInfo))
                    {
                        property.SetValue(parent, child, null);
                        return true;
                    }
                }
            }

            return false;
        }

        private Element CreateElement()
        {
            if (this.reader.Depth > MaxNestingDepth)
            {
                throw new InvalidOperationException("Maximum nesting depth has been reached.");
            }

            if (this.reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            // Need to check this here before we move to the attributes,
            // as reader.NodeType will never be EndElement for empty elements
            // and when we move to an attribute, IsEmptyElement doesn't work
            this.currentElementIsEmpty = this.reader.IsEmptyElement;

            Element element = KmlFactory.CreateElement(this.GetXmlComponent());
            return element ?? new UnknownElement(new XmlComponent(this.reader));
        }

        private XmlComponent GetXmlComponent()
        {
            if (this.defaultNamespace == null)
            {
                return new XmlComponent(this.reader);
            }

            return new XmlComponent(null, this.reader.Name, this.defaultNamespace);
        }

        private void OnElementAdded(Element element)
        {
            this.ElementAdded?.Invoke(this, new ElementEventArgs(element));
        }

        private void Parse(XmlReader reader)
        {
            try
            {
                this.Root = null; // If anything bad happens, makes sure Root is empty.
                this.reader = reader;

                // Try to find the first element
                if (this.reader.MoveToContent() == XmlNodeType.Element)
                {
                    Element element = this.CreateElement();
                    this.PopulateElement(element);

                    // Do not allow unknown root elements
                    if (!(element is UnknownElement))
                    {
                        if (element != null)
                        {
                            this.OnElementAdded(element);
                        }

                        this.Root = element;
                    }
                }
            }
            finally
            {
                this.reader = null;
                if (reader != null)
                {
                    ((IDisposable)reader).Dispose();
                }
            }
        }

        private void PopulateElement(Element element)
        {
            if (element == null)
            {
                return;
            }

            this.ProcessAttributes(element);
            if (element is IHtmlContent htmlContent)
            {
                if (this.currentElementIsEmpty)
                {
                    htmlContent.Text = string.Empty;
                }
                else
                {
                    htmlContent.Text = XmlExtractor.FlattenXml(this.reader);
                }
            }
            else if (!this.currentElementIsEmpty)
            {
                while (this.reader.Read())
                {
                    switch (this.reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            this.AddChild(element);
                            break;

                        case XmlNodeType.EndElement:
                            return;

                        case XmlNodeType.CDATA: // Treat like normal text
                        case XmlNodeType.Text:
                            element.AddInnerText(this.reader.Value);
                            break;
                    }
                }
            }
        }

        private void ProcessAttributes(Element element)
        {
            var browser = TypeBrowser.Create(element.GetType());
            while (this.reader.MoveToNextAttribute())
            {
                // Check for namespaces first
                if (string.Equals("xmlns", this.reader.Name, StringComparison.Ordinal))
                {
                    // Set default namespace only on unknown elements
                    if (element is UnknownElement)
                    {
                        element.AddNamespace(string.Empty, this.reader.Value);
                    }
                }
                else if (string.Equals("xmlns", this.reader.Prefix, StringComparison.Ordinal))
                {
                    element.AddNamespace(this.reader.LocalName, this.reader.Value);
                }
                else
                {
                    // just a normal attribute
                    PropertyInfo property = browser.FindAttribute(new XmlComponent(null, this.reader.LocalName, null)); // Attributes never have namespace info.
                    if (property != null)
                    {
                        AssignValue(element, property, this.reader.Value);
                    }
                    else
                    {
                        // Unknown, save for later serialization
                        element.AddAttribute(new XmlComponent(this.reader));
                    }
                }
            }
        }

        /// <summary>
        /// Allows unknown namespaces to be resolved without raising an
        /// XmlException.
        /// </summary>
        private class IgnoreNamespaceManager : XmlNamespaceManager
        {
            public IgnoreNamespaceManager()
                : base(new NameTable())
            {
            }

            public override string LookupNamespace(string prefix)
            {
                return base.LookupNamespace(prefix) ?? IgnoreNamespace;
            }
        }
    }
}
