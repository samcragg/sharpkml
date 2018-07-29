// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Encodes an instance of a user-defined field defined by a referenced
    /// <see cref="SimpleField"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.5</remarks>
    [KmlElement("SimpleData")]
    public class SimpleData : Element, IHtmlContent
    {
        /// <summary>
        /// Gets or sets a value acting as an identifier.
        /// </summary>
        /// <remarks>
        /// This shall be used to identify the <see cref="SimpleField"/> by name.
        /// The identified <c>SimpleField</c> shall be declared within the
        /// <see cref="Schema"/> element that is referenced from the
        /// <see cref="SchemaData.SchemaUrl"/>.
        /// </remarks>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content of this instance.
        /// </summary>
        /// <remarks>The value may contain well formed HTML.</remarks>
        public string Text
        {
            get => this.InnerText;
            set
            {
                this.ClearInnerText();
                this.AddInnerText(value);
            }
        }
    }
}
