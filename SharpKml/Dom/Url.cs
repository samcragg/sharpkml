// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using SharpKml.Base;

    /// <summary>
    /// Specifies the location and handling of a resource.
    /// </summary>
    /// <remarks>Legacy API - not part of OGC KML 2.2</remarks>
    [Obsolete("Url deprecated in 2.2")]
    [KmlElement("Url")]
    public sealed class Url : LinkType
    {
        /// <summary>
        /// Converts the specified <see cref="Url"/> into a <see cref="Link"/>.
        /// </summary>
        /// <param name="url">The value to convert.</param>
        /// <returns>
        /// A copy of the specified value parameter as a <c>Link</c>.
        /// </returns>
        public static explicit operator Link(Url url)
        {
            return new Link
            {
                Href = url.Href,
                HttpQuery = url.HttpQuery,
                Id = url.Id,
                RefreshInterval = url.RefreshInterval,
                RefreshMode = url.RefreshMode,
                TargetId = url.TargetId,
                ViewBoundScale = url.ViewBoundScale,
                ViewFormat = url.ViewFormat,
                ViewRefreshMode = url.ViewRefreshMode,
                ViewRefreshTime = url.ViewRefreshTime
            };
        }

        /// <summary>
        /// Converts the specified <see cref="Url"/> into a <see cref="Link"/>.
        /// </summary>
        /// <param name="url">The value to convert.</param>
        /// <returns>
        /// A copy of the specified value parameter as a <c>Link</c>.
        /// </returns>
        public static Link FromUrl(Url url)
        {
            return (Link)url;
        }
    }
}
