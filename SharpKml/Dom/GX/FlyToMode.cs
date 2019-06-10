// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using SharpKml.Base;

    /// <summary>
    /// Specifies the type of flight mode.
    /// </summary>
    public enum FlyToMode
    {
        /// <summary>
        /// FlyTos each begin and end at zero velocity.
        /// </summary>
        [KmlElement("bounce")]
        Bounce = 0,

        /// <summary>
        /// FlyTos allow for an unbroken flight from point to point to point (and on).
        /// </summary>
        [KmlElement("smooth")]
        Smooth,
    }
}
