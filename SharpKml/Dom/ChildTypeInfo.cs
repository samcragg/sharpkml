// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Stores informations about Child types of an Element.
    /// </summary>
    internal struct ChildTypeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildTypeInfo"/> struct.
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="parentType">Parent type</param>
        public ChildTypeInfo(int order, TypeInfo parentType)
        {
            this.Order = order;
            this.ParentType = parentType;
        }

        /// <summary>
        /// Gets Child Type info.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Gets the Type owning this child type
        /// </summary>
        public TypeInfo ParentType { get; private set; }
    }
}
