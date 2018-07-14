// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies an image overlay to be displayed fixed to the screen.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 11.7</para>
    /// <para> The image position is determined by mapping a point relative to
    /// the image (<see cref="Origin"/>) to a point relative to the screen
    /// (<see cref="Screen"/>). The image may be rotated by <see cref="Rotation"/>
    /// degrees about a point relative to the screen (<see cref="RotationOrigin"/>).
    /// The image sizing is determined by <see cref="Size"/>.</para>
    /// </remarks>
    [KmlElement("ScreenOverlay")]
    public sealed class ScreenOverlay : Overlay
    {
        private OverlayVector overlay;
        private RotationVector rotation;
        private ScreenVector screen;
        private SizeVector size;

        /// <summary>
        /// Gets or sets a point on (or outside of) the image that is mapped
        /// to the screen coordinate <see cref="Screen"/>.
        /// </summary>
        /// <remarks>
        /// The origin of the coordinate system is the lower left corner of the icon.
        /// </remarks>
        [KmlElement(null, 1)]
        public OverlayVector Origin
        {
            get => this.overlay;
            set => this.UpdatePropertyChild(value, ref this.overlay);
        }

        /// <summary>
        /// Gets or sets the angle of rotation, in decimal degrees, of the
        /// parent object.
        /// </summary>
        /// <remarks>
        /// The value is an angle in decimal degrees counterclockwise starting
        /// from north. Use ±180 to indicate the rotation of the parent object.
        /// The center of the rotation, if not the objects center, is specified
        /// in <see cref="RotationOrigin"/>.
        /// </remarks>
        [KmlElement("rotation", 5)]
        public double? Rotation { get; set; }

        /// <summary>
        /// Gets or sets a point relative to the screen about which the screen
        /// overlay is rotated.
        /// </summary>
        /// <remarks>
        /// The origin of the coordinate system is the lower left corner of the screen.
        /// </remarks>
        [KmlElement(null, 3)]
        public RotationVector RotationOrigin
        {
            get => this.rotation;
            set => this.UpdatePropertyChild(value, ref this.rotation);
        }

        /// <summary>
        /// Gets or sets a point relative to the screen origin that the image
        /// is mapped to.
        /// </summary>
        /// <remarks>
        /// The origin of the coordinate system is the lower left corner of the screen.
        /// </remarks>
        [KmlElement(null, 2)]
        public ScreenVector Screen
        {
            get => this.screen;
            set => this.UpdatePropertyChild(value, ref this.screen);
        }

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <remarks>
        /// A value of −1 indicates to use the native dimension; a value of 0
        /// indicates to maintain the aspect ratio. Any other value sets the
        /// dimension to the specified value.
        /// </remarks>
        [KmlElement(null, 4)]
        public SizeVector Size
        {
            get => this.size;
            set => this.UpdatePropertyChild(value, ref this.size);
        }
    }
}
