// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SharpKml.Dom;

    /// <summary>
    /// Merges and resolves <see cref="Style"/> links.
    /// </summary>
    public sealed class StyleResolver
    {
        // This is the maximum number of styleUrls followed in resolving a style
        // selector.  Note: the KML standard specifies no such limit.  This is used
        // primarily to inhibit infinite loops on styleUrls that are self referencing.
        private const int MaximumNestingDepth = 5;

        private readonly IDictionary<string, StyleSelector> styleMap;
        private IFileResolver fileResolver;
        private int nestedDepth;
        private StyleState state;
        private Style style = new Style();
        private int styleId;

        private StyleResolver(IDictionary<string, StyleSelector> map)
        {
            this.styleMap = map;
        }

        /// <summary>
        /// Resolves all the styles in the specified <see cref="Feature"/>.
        /// </summary>
        /// <param name="feature">
        /// The <c>Feature</c> to search for styles.
        /// </param>
        /// <param name="file">
        /// The <see cref="KmlFile"/> the feature belongs to.
        /// </param>
        /// <param name="state">
        /// The <see cref="StyleState"/> of the styles to look for.
        /// </param>
        /// <returns>
        /// A new <see cref="Style"/> that has been resolved.
        /// </returns>
        /// <exception cref="ArgumentNullException">feature/file is null.</exception>
        public static Style CreateResolvedStyle(Feature feature, KmlFile file, StyleState state)
        {
            return CreateResolvedStyle(feature, file, state, null);
        }

        /// <summary>
        /// Resolves all the styles in the specified <see cref="Feature"/>.
        /// </summary>
        /// <param name="feature">
        /// The <c>Feature</c> to search for styles.
        /// </param>
        /// <param name="file">
        /// The <see cref="KmlFile"/> the feature belongs to.
        /// </param>
        /// <param name="state">
        /// The <see cref="StyleState"/> of the styles to look for.
        /// </param>
        /// <param name="resolver">
        /// Used to resolve external files referenced by the styles.
        /// </param>
        /// <returns>
        /// A new <see cref="Style"/> that has been resolved.
        /// </returns>
        /// <exception cref="ArgumentNullException">feature/file is null.</exception>
        public static Style CreateResolvedStyle(Feature feature, KmlFile file, StyleState state, IFileResolver resolver)
        {
            Check.IsNotNull(feature, nameof(feature));
            Check.IsNotNull(file, nameof(file));

            var instance = new StyleResolver(file.StyleMap)
            {
                fileResolver = resolver,
                state = state,
            };
            instance.Merge(feature.StyleUrl);

            foreach (StyleSelector selector in feature.Styles)
            {
                instance.Merge(selector);
            }

            return instance.style;
        }

        /// <summary>
        /// Inlines the shared Style of the features in the specified element.
        /// </summary>
        /// <typeparam name="T">
        /// A class deriving from <see cref="Element"/>.
        /// </typeparam>
        /// <param name="element">The element instance.</param>
        /// <returns>A new element with the shared styles inlined.</returns>
        public static T InlineStyles<T>(T element)
            where T : Element
        {
            var instance = new StyleResolver(new Dictionary<string, StyleSelector>());

            // Don't modify the original but create a copy instead
            T clone = element.Clone();
            foreach (Element e in clone.Flatten())
            {
                instance.InlineElement(e);
            }

            return clone;
        }

        /// <summary>
        /// Changes inlined styles to shared styles in the closest Document parent.
        /// </summary>
        /// <typeparam name="T">
        /// A class deriving from <see cref="Element"/>.
        /// </typeparam>
        /// <param name="element">The element instance.</param>
        /// <returns>
        /// A new element with the inlined styles changed to shared styles.
        /// </returns>
        public static T SplitStyles<T>(T element)
            where T : Element
        {
            var instance = new StyleResolver(new Dictionary<string, StyleSelector>());

            // Can't modify the Children collection while we're iterating so
            // create a temporary list of styles to add
            var sharedStyles = new List<Tuple<Document, StyleSelector>>();

            T clone = element.Clone();
            var children = clone.Flatten().ToList(); // Adding a style will add to the children.
            foreach (Element e in children)
            {
                Tuple<Document, StyleSelector> tuple = instance.Split(e);
                if (tuple != null)
                {
                    sharedStyles.Add(tuple);
                }
            }

            // Finished iterating the flattened hierarchy (which incudes
            // Children) so we can now add the shared styles
            foreach (Tuple<Document, StyleSelector> style in sharedStyles)
            {
                style.Item1.AddStyle(style.Item2);
            }

            return clone;
        }

        private Pair CreatePair(StyleState state, Uri url)
        {
            this.Reset();

            this.state = state;
            this.Merge(url);

            return new Pair
            {
                State = state,
                Selector = this.style,
            };
        }

        private StyleSelector CreateStyleMap(Uri url)
        {
            // This is the same order as the C++ version - Normal then Highlight
            return new StyleMapCollection
            {
                this.CreatePair(StyleState.Normal, url),
                this.CreatePair(StyleState.Highlight, url),
            };
        }

        private string CreateUniqueId()
        {
            // Keep trying until we find a unique identifier
            while (true)
            {
                string id = "_" + this.styleId++;
                if (!this.styleMap.ContainsKey(id))
                {
                    return id;
                }
            }
        }

        private void InlineElement(Element element)
        {
            if (element.IsChildOf<Update>())
            {
                return;
            }

            if (element is Feature feature)
            {
                // Check if it's a Document, which inherits from Feature, as
                // Documents contain shared styles
                if (element is Document document)
                {
                    // Create a copy of the styles so we can iterate the copy
                    // and remove them from the Document.
                    var styles = document.Styles.ToList();
                    foreach (StyleSelector style in styles)
                    {
                        if (style.Id != null)
                        {
                            this.styleMap[style.Id] = style;
                            style.Id = null; // The C++ version clears the id, so we will too...
                            document.RemoveStyle(style);
                        }
                    }
                }
                else
                {
                    // If it's a local reference and we've captured the shared style
                    // give a copy of that and clear the StyleUrl
                    if ((feature.StyleUrl != null) && !feature.StyleUrl.IsAbsoluteUri)
                    {
                        string id = feature.StyleUrl.GetFragment();
                        if (this.styleMap.ContainsKey(id))
                        {
                            feature.AddStyle(this.CreateStyleMap(feature.StyleUrl));
                            feature.StyleUrl = null;
                        }
                    }
                }
            }
        }

        private KmlFile LoadFile(string path)
        {
            try
            {
                byte[] data = this.fileResolver.ReadFile(path);
                using (var stream = new MemoryStream(data, false))
                {
                    if (path.EndsWith(".kml", StringComparison.OrdinalIgnoreCase))
                    {
                        return KmlFile.Load(stream);
                    }

                    if (path.EndsWith(".kmz", StringComparison.OrdinalIgnoreCase))
                    {
                        if (this.fileResolver.SupportsKmz)
                        {
                            return this.fileResolver.ExtractDefaultKmlFileFromKmzArchive(stream);
                        }
                    }
                }
            }
            catch (IOException)
            {
                // Silently fail
            }

            return null;
        }

        private void Merge(StyleSelector selector)
        {
            if (selector is Style style)
            {
                this.style.Merge(style);
            }
            else if (selector is StyleMapCollection styleMap)
            {
                foreach (Pair pair in styleMap)
                {
                    if (pair.State == this.state)
                    {
                        this.Merge(pair.StyleUrl);
                        this.Merge(pair.Selector);
                    }
                }
            }
        }

        private void Merge(Uri url)
        {
            if ((this.nestedDepth++ >= MaximumNestingDepth) || (url == null))
            {
                return; // Silently fail
            }

            string id = url.GetFragment();
            if (!string.IsNullOrEmpty(id))
            {
                // If there's no path this is a StyleSelector within this file.
                string path = url.GetPath();
                if (string.IsNullOrEmpty(path))
                {
                    if (this.styleMap.TryGetValue(id, out StyleSelector style))
                    {
                        this.Merge(style);
                    }
                }
                else if (this.fileResolver != null)
                {
                    KmlFile file = this.LoadFile(path);
                    if (file != null)
                    {
                        if (file.StyleMap.TryGetValue(id, out StyleSelector style))
                        {
                            this.Merge(style);
                        }
                    }
                }
            }
        }

        private void Reset()
        {
            this.nestedDepth = 0;
            this.style = new Style();
        }

        private Tuple<Document, StyleSelector> Split(Element element)
        {
            if (element.IsChildOf<Update>())
            {
                return null;
            }

            if (element is StyleSelector style)
            {
                // Add the style to the map so we generate an unique id
                if (style.Id != null)
                {
                    this.styleMap[style.Id] = style;
                }

                // Find the Document to put the Style in, making sure it doesn't
                // already have one as a Parent.
                Document document = element.GetParent<Document>();
                if ((document != null) && (element.Parent != document))
                {
                    // Is the style in a Feature that doesn't have a StyleUrl?
                    if (element.Parent is Feature feature && (feature.StyleUrl == null))
                    {
                        // Create a copy of the style, using a new id
                        var shared = (StyleSelector)Activator.CreateInstance(style.GetType());
                        shared.Merge(style);
                        shared.Id = this.CreateUniqueId();

                        // Tell the feature to use the new shared style and
                        // remove the old style.
                        this.styleMap.Add(shared.Id, shared);
                        feature.StyleUrl = new Uri("#" + shared.Id, UriKind.Relative);
                        feature.ClearStyles();

                        // This will be added to the Document later when we've
                        // finished iterating through the Children
                        return Tuple.Create(document, shared);
                    }
                }
            }

            return null; // Nothing to add
        }
    }
}
