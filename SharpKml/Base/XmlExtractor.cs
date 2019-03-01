// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System.Globalization;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Used to get the inner text of a XML element.
    /// </summary>
    /// <remarks>
    /// XmlReader.ReadInnerXml() is namespace aware, meaning it will add namespace
    /// declarations to any child element which inherits a namespace (e.g. if
    /// ReadInnerXml() is called on the following &lt;root&gt; node:
    /// <![CDATA[<root xmlns="http://example.com"><child>Some text</child></root>]]>
    /// the returned value would be
    /// <![CDATA[<child xmlns="http://example.com">Some text</child>]]>)
    /// The output produced by this class matches the C++ version.
    /// </remarks>
    internal class XmlExtractor
    {
        private readonly XmlReader reader;
        private StringBuilder xml = new StringBuilder();

        private XmlExtractor(XmlReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Extracts the inner XML of the XmlReader, without adding additional
        /// namespaces.
        /// </summary>
        /// <param name="reader">The XmlReader to extract data from.</param>
        /// <returns>
        /// A string representing the inner XML of the current XML node.
        /// </returns>
        public static string FlattenXml(XmlReader reader)
        {
            var instance = new XmlExtractor(reader);
            instance.ProcessChild();
            return instance.xml.ToString();
        }

        private string GetAttributes()
        {
            var sb = new StringBuilder();
            while (this.reader.MoveToNextAttribute())
            {
                sb.AppendFormat(
                    CultureInfo.InvariantCulture,
                    " {0}=\"{1}\"",
                    this.reader.Name,
                    this.reader.Value);
            }

            return sb.ToString();
        }

        private void ProcessChild()
        {
            while (this.reader.Read())
            {
                switch (this.reader.NodeType)
                {
                    case XmlNodeType.Element:
                        // Check here before we access attributes.
                        if (this.reader.IsEmptyElement)
                        {
                            this.xml.AppendFormat(
                                CultureInfo.InvariantCulture,
                                "<{0}{1} />",
                                this.reader.Name,
                                this.GetAttributes());
                        }
                        else
                        {
                            this.xml.AppendFormat(
                                CultureInfo.InvariantCulture,
                                "<{0}{1}>",
                                this.reader.Name,
                                this.GetAttributes());

                            this.ProcessChild();

                            this.xml.AppendFormat(
                                CultureInfo.InvariantCulture,
                                "</{0}>",
                                this.reader.Name);
                        }

                        break;

                    case XmlNodeType.EndElement:
                        return;

                    case XmlNodeType.CDATA: // Fall through
                    case XmlNodeType.Text:
                        this.xml.Append(this.reader.Value);
                        break;
                }
            }
        }
    }
}
