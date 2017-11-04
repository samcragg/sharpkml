// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SharpKml.Dom;

    /// <summary>
    /// Helper class that looks for all elements with an "href" property.
    /// </summary>
    public sealed class LinkResolver
    {
        private readonly Uri[] links;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkResolver"/> class.
        /// </summary>
        /// <param name="kml">The input to parse.</param>
        public LinkResolver(KmlFile kml)
        {
            var set = new HashSet<Uri>();
            foreach (Element child in kml.Root.Flatten())
            {
                AddUriFromElement(set, child);
            }

            this.links = set.ToArray();
        }

        /// <summary>
        /// Gets the Uri's of the Links.
        /// </summary>
        public IReadOnlyList<Uri> Links
        {
            get { return this.links; }
        }

        /// <summary>
        /// Gets the normalized path from the Links that are relative.
        /// </summary>
        /// <returns>
        /// All the links that are relative.
        /// </returns>
        public IEnumerable<string> GetRelativePaths()
        {
            foreach (Uri uri in this.links)
            {
                Uri normal = uri.Normalize();
                if ((normal != null) && !normal.IsAbsoluteUri)
                {
                    // Only return the path part (i.e. exclude the fragment parts etc)
                    string uriPath = normal.GetPath();
                    if (!string.IsNullOrEmpty(uriPath))
                    {
                        yield return uriPath;
                    }
                }
            }
        }

        private static void AddUri(ISet<Uri> set, Uri uri)
        {
            if (uri != null)
            {
                set.Add(uri);
            }
        }

        // We should really have an internal interface instead of repeated casting
        // and checking for nulls, however, adding the interface to the base classes
        // (Feature, BasicLink) means they cannot be inherited outside the assembly.
        private static void AddUriFromElement(ISet<Uri> set, Element element)
        {
            var alias = element as Alias;
            if (alias != null)
            {
                AddUri(set, alias.TargetHref);
                return;
            }

            var feature = element as Feature;
            if (feature != null)
            {
                AddUri(set, feature.StyleUrl);
                return;
            }

            var icon = element as ItemIcon;
            if (icon != null)
            {
                AddUri(set, icon.Href);
                return;
            }

            var link = element as BasicLink;
            if (link != null)
            {
                AddUri(set, link.Href);
                return;
            }

            var schema = element as SchemaData;
            if (schema != null)
            {
                AddUri(set, schema.SchemaUrl);
                return;
            }
        }
    }
}
