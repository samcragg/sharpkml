// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using SharpKml.Base;

    /// <summary>
    /// Represents data found when parsing which is not recognized.
    /// </summary>
    public sealed class UnknownElement : Element, ICustomElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownElement"/> class.
        /// </summary>
        /// <param name="data">The unrecognized XML data to store.</param>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        public UnknownElement(XmlComponent data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this.UnknownData = data.Clone(); // Don't store the data from the user but store a copy instead.
        }

        /// <summary>
        /// Gets the attributes of the element.
        /// </summary>
        public new IEnumerable<XmlComponent> Attributes => base.Attributes;

        /// <summary>
        /// Gets the child elements.
        /// </summary>
        public IEnumerable<UnknownElement> Elements => this.Children.OfType<UnknownElement>();

        /// <summary>
        /// Gets all the XML content, including markup, in the current element.
        /// </summary>
        public string InnerXml => this.InnerText;

        /// <summary>
        /// Gets the name of the unknown element.
        /// </summary>
        public string Name => this.UnknownData.Name;

        /// <summary>
        /// Gets the information of the unrecognized element found during parsing.
        /// </summary>
        public XmlComponent UnknownData { get; }

        /// <summary>
        /// Gets a value indicating whether to process the children of the Element.
        /// </summary>
        bool ICustomElement.ProcessChildren => true;

        /// <summary>
        /// Writes the start of an XML element.
        /// </summary>
        /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
        void ICustomElement.CreateStartElement(XmlWriter writer)
        {
            writer.WriteStartElement(this.UnknownData.Prefix, this.UnknownData.Name, this.UnknownData.NamespaceUri);
        }
    }
}
