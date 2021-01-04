// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a KML AbstractContainerGroup.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.6.</remarks>
    public abstract class Container : Feature
    {
        /// <summary>
        /// Gets the <see cref="Feature"/>s contained by this instance.
        /// </summary>
        public abstract IReadOnlyCollection<Feature> Features { get; }

        /// <summary>
        /// Gets the mutable list of features.
        /// </summary>
        protected List<Feature> FeatureList { get; } = new List<Feature>();

        /// <summary>
        /// Adds the specified <see cref="Feature"/> to this instance.
        /// </summary>
        /// <param name="feature">The <c>Feature</c> to add to this instance.</param>
        /// <exception cref="ArgumentNullException">feature is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// feature belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddFeature(Feature feature)
        {
            this.AddAsChild(this.FeatureList, feature);
        }

        /// <summary>
        /// Returns the first <see cref="Feature"/> found with the specified id.
        /// </summary>
        /// <param name="id">The id of the <c>Feature</c> to search for.</param>
        /// <returns>
        /// The first <c>Feature</c> matching the specified id, if any;
        /// otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        public Feature FindFeature(string id)
        {
            Check.IsNotNull(id, nameof(id));

            return this.FeatureList.FirstOrDefault(f => string.Equals(f.Id, id, StringComparison.Ordinal));
        }

        /// <summary>
        /// Removes the specified <see cref="Feature"/> from this instance.
        /// </summary>
        /// <param name="id">The Id of the <c>Feature</c> to remove.</param>
        /// <returns>
        /// true if the value parameter is successfully removed; otherwise,
        /// false. This method also returns false if item was not found in
        /// <see cref="Features"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        public bool RemoveFeature(string id)
        {
            Check.IsNotNull(id, nameof(id));

            return this.RemoveFeature(this.FindFeature(id));
        }

        /// <summary>
        /// Removes the specified <see cref="Feature"/> from this instance.
        /// </summary>
        /// <param name="feature">The <c>Feature</c> to remove.</param>
        /// <returns>
        /// true if the value parameter is successfully removed; otherwise,
        /// false. This method also returns false if item was not found in
        /// <see cref="Features"/>.
        /// </returns>
        public bool RemoveFeature(Feature feature)
        {
            if ((feature == null) || !this.FeatureList.Remove(feature))
            {
                return false;
            }

            this.ResetParent(feature);
            return true;
        }
    }
}
