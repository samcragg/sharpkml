// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Base
{
    using System;

    /// <summary>
    /// Specifies a class member is serialized as an XML attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class KmlAttributeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KmlAttributeAttribute"/> class.
        /// </summary>
        /// <param name="attributeName">The name of the XML attribute.</param>
        public KmlAttributeAttribute(string attributeName)
        {
            this.AttributeName = attributeName;
        }

        /// <summary>
        /// Gets the name of the XML attribute.
        /// </summary>
        public string AttributeName { get; }
    }
}
