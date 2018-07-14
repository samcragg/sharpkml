// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;

    /// <summary>
    /// Specifies the allowed known child types of the object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ChildTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildTypeAttribute"/> class.
        /// </summary>
        /// <param name="childType">The type of the child.</param>
        /// <param name="order">The order for the child.</param>
        public ChildTypeAttribute(Type childType, int order)
        {
            this.ChildType = childType;
            this.Order = order;
        }

        /// <summary>
        /// Gets the type of the child.
        /// </summary>
        public Type ChildType { get; }

        /// <summary>
        /// Gets the order for the child.
        /// </summary>
        public int Order { get; }
    }
}
