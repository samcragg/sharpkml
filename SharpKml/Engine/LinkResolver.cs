namespace SharpKml.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using SharpKml.Base;
    using SharpKml.Dom;

    /// <summary>
    /// Helper class that looks for all elements with an "href" property.
    /// </summary>
    internal sealed class LinkResolver
    {
        private bool duplicates;
        private List<Uri> links = new List<Uri>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkResolver"/> class.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="duplicates">Allow duplicates or not.</param>
        public LinkResolver(TextReader input, bool duplicates)
        {
            this.duplicates = duplicates;
            Parser parser = new Parser();
            parser.ElementAdded += this.OnElementAdded;
            parser.Parse(input);
        }

        /// <summary>
        /// Gets the Uri's of the Links.
        /// </summary>
        public IEnumerable<Uri> Links
        {
            get { return this.links; }
        }

        /// <summary>
        /// Gets the normalized path from the Links that are relative.
        /// </summary>
        public IEnumerable<string> RelativePaths
        {
            get
            {
                foreach (var uri in this.links)
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
        }

        private void AddUri(Uri uri)
        {
            if (uri != null)
            {
                if (!this.duplicates && this.links.Contains(uri))
                {
                    return;
                }

                this.links.Add(uri);
            }
        }

        // We should really have an internal interface instead of repeated casting
        // and checking for nulls, however, adding the interface to the base classes
        // (Feature, BasicLink) means they cannot be inherited outside the assembly.
        private void OnElementAdded(object sender, ElementEventArgs e)
        {
            var alias = e.Element as Alias;
            if (alias != null)
            {
                this.AddUri(alias.TargetHref);
                return;
            }

            var feature = e.Element as Feature;
            if (feature != null)
            {
                this.AddUri(feature.StyleUrl);
                return;
            }

            var icon = e.Element as ItemIcon;
            if (icon != null)
            {
                this.AddUri(icon.Href);
                return;
            }

            var link = e.Element as BasicLink;
            if (link != null)
            {
                this.AddUri(link.Href);
                return;
            }

            var schema = e.Element as SchemaData;
            if (schema != null)
            {
                this.AddUri(schema.SchemaUrl);
                return;
            }
        }
    }
}
