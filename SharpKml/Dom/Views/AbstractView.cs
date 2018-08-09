// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections.Generic;
    using System.Linq;
    using SharpKml.Base;

    /// <summary>
    /// Represents an AbstractViewGroup.
    /// </summary>
    /// <remarks>
    /// OGC KML 2.2 Section 14.1
    /// </remarks>
    [ChildType(typeof(GX.Option), 1)]
    public abstract class AbstractView : KmlObject
    {
        private TimePrimitive primitive;

        /// <summary>
        /// Gets or sets the horizontal field of view of the AbstractView
        /// during a tour. [Google Extension]
        /// </summary>
        [KmlElement("horizFov", KmlNamespaces.GX22Namespace, 2)]
        public double? GXHorizontalFOV { get; set; }

        /// <summary>
        /// Gets or sets the associated time primitive.
        /// [Google Extension]
        /// </summary>
        /// <remarks>
        /// The time primitive must be in the Google Extension namespace.
        /// </remarks>
        [KmlElement(null, 1)]
        public TimePrimitive GXTimePrimitive
        {
            get => this.primitive;
            set => this.UpdatePropertyChild(value, ref this.primitive);
        }

        /// <summary>
        /// Gets the <see cref="GX.Option"/>s stored by this instance.
        /// [Google Extension]
        /// </summary>
        public IEnumerable<GX.Option> ViewerOptions => this.Children.OfType<GX.Option>();

        /// <summary>
        /// Adds the specified <see cref="GX.Option"/> to this instance.
        /// [Google Extension]
        /// </summary>
        /// <param name="option">
        /// The <c>Option</c> to add to this instance.
        /// </param>
        /// <exception cref="System.ArgumentNullException">option is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// option belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddGXOption(GX.Option option)
        {
            this.TryAddChild(option);
        }
    }
}
