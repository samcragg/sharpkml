// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom.GX
{
    using SharpKml.Base;

    /// <summary>
    /// Allows the tour to be paused until a user takes action to continue the tour.
    /// </summary>
    public enum PlayMode
    {
        /// <summary>
        /// Waits for user action to continue the tour.
        /// </summary>
        [KmlElement("pause")]
        Pause = 0
    }
}
