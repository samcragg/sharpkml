namespace SharpKml.Dom
{
    using System;
    using System.Xml;
    using SharpKml.Base;

    /// <summary>
    /// Represents data found when parsing which is not recognized.
    /// </summary>
    public sealed class UnknownElement : Element, ICustomElement
    {
        private XmlComponent data;

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

            this.data = data.Clone(); // Don't store the data from the user but store a copy instead.
        }

        /// <summary>
        /// Gets the unrecognized data found during parsing.
        /// </summary>
        public XmlComponent UnknownData
        {
            get { return this.data.Clone(); } // Don't give them our data to play with
        }

        /// <summary>
        /// Gets a value indicating whether to process the children of the Element.
        /// </summary>
        bool ICustomElement.ProcessChildren
        {
            get { return true; }
        }

        /// <summary>
        /// Writes the start of an XML element.
        /// </summary>
        /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
        void ICustomElement.CreateStartElement(XmlWriter writer)
        {
            writer.WriteStartElement(this.data.Prefix, this.data.Name, this.data.NamespaceUri);
        }
    }
}
