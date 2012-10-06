using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Represents an AbstractViewGroup.
    /// </summary>
    /// <remarks>
    /// OGC KML 2.2 Section 14.1
    /// </remarks>
    public abstract class AbstractView : KmlObject
    {
        private TimePrimitive _primitive;

        /// <summary>
        /// Initializes a new instance of the AbstractView class.
        /// </summary>
        protected AbstractView()
        {
            this.RegisterValidChild<GX.Option>();
        }

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
            get { return _primitive; }
            set { this.UpdatePropertyChild(value, ref _primitive); }
        }

        /// <summary>
        /// Gets or sets the horizontal field of view of the AbstractView
        /// during a tour. [Google Extension]
        /// </summary>
        [KmlElement("horizFov", KmlNamespaces.GX22Namespace, 2)]
        public double? GXHorizontalFOV { get; set; }

        /// <summary>
        /// Gets the <see cref="GX.Option"/>s stored by this instance.
        /// [Google Extension]
        /// </summary>
        public IEnumerable<GX.Option> ViewerOptions
        {
            get { return this.Children.OfType<GX.Option>(); }
        }

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
            this.AddChild(option);
        }
    }
}
