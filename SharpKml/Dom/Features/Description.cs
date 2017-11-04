// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies a description of a <see cref="Feature"/>, which should be
    /// displayed in the description balloon.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 9.1.3.10.</para>
    /// <para>The text may include HTML content, with any HTML links needing
    /// special processing. See the standards for details.</para>
    /// </remarks>
    [KmlElement("description")]
    public sealed class Description : Element, IHtmlContent
    {
        /// <summary>
        /// Gets or sets the content of this instance.
        /// </summary>
        /// <remarks>The value may contain well formed HTML.</remarks>
        public string Text
        {
            get
            {
                return this.InnerText;
            }
            set
            {
                this.ClearInnerText();
                this.AddInnerText(value);
            }
        }
    }
}
