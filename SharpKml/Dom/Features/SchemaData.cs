// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using SharpKml.Base;

    /// <summary>
    /// Encodes an instance of a user-defined data type defined by a referenced
    /// <see cref="Schema"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.4</remarks>
    [KmlElement("SchemaData")]
    public class SchemaData : KmlObject
    {
        private readonly List<GX.SimpleArrayData> arrayData = new List<GX.SimpleArrayData>();
        private readonly List<SimpleData> dataList = new List<SimpleData>();

        /// <summary>
        /// Gets a collection of value arrays.
        /// [Google Extension]
        /// </summary>
        [KmlElement(null, 2)]
        public IReadOnlyCollection<GX.SimpleArrayData> GXSimpleArray => this.arrayData;

        /// <summary>
        /// Gets or sets a reference to a <see cref="KmlObject.Id"/> belonging
        /// to a <see cref="Schema"/>.
        /// </summary>
        /// <remarks>
        /// Reference can either be a full URL or a reference to a <c>Schema</c>
        /// defined in a KML resource file (both external or local).
        /// </remarks>
        [KmlAttribute("schemaUrl")]
        public Uri SchemaUrl { get; set; }

        /// <summary>
        /// Gets a collection of user-defined fields.
        /// </summary>
        [KmlElement(null, 1)]
        public IReadOnlyCollection<SimpleData> SimpleData => this.dataList;

        /// <summary>
        /// Adds the specified <see cref="GX.SimpleArrayData"/> to this instance.
        /// [Google Extension]
        /// </summary>
        /// <param name="array">The <c>SimpleArrayData</c> to add to this instance.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// array belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddArray(GX.SimpleArrayData array)
        {
            this.AddAsChild(this.arrayData, array);
        }

        /// <summary>
        /// Adds the specified <see cref="SimpleData"/> to this instance.
        /// </summary>
        /// <param name="data">The <c>SimpleData</c> to add to this instance.</param>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// data belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddData(SimpleData data)
        {
            this.AddAsChild(this.dataList, data);
        }
    }
}
