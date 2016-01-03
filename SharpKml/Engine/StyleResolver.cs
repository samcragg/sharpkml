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

        private IDictionary<string, StyleSelector> styleMap;
        private Style style = new Style();
        private StyleState state;

        private IFileResolver fileResolver;
        private int nestedDepth;
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
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var instance = new StyleResolver(file.StyleMap);
            instance.fileResolver = resolver;
            instance.state = state;
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
            foreach (var e in clone.Flatten())
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
                var tuple = instance.Split(e);
                if (tuple != null)
                {
                    sharedStyles.Add(tuple);
                }
            }

            // Finished iterating the flattened hierarchy (which incudes
            // Children) so we can now add the shared styles
            foreach (var style in sharedStyles)
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

            var pair = new Pair();
            pair.State = state;
            pair.Selector = this.style;
            return pair;
        }

        private StyleSelector CreateStyleMap(Uri url)
        {
            // This is the same order as the C++ version - Normal then Highlight
            var map = new StyleMapCollection();
            map.Add(this.CreatePair(StyleState.Normal, url));
            map.Add(this.CreatePair(StyleState.Highlight, url));
            return map;
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

            var feature = element as Feature;
            if (feature != null)
            {
                // Check if it's a Document, which inherits from Feature, as
                // Documents contain shared styles
                var document = element as Document;
                if (document != null)
                {
                    // Create a copy of the styles so we can iterate the copy
                    // and remove them from the Document.
                    var styles = document.Styles.ToList();
                    foreach (var style in styles)
                    {
                        if (style.Id != null)
                        {
                            this.styleMap[style.Id] = style;
                            style.Id = null; // The C++ version clears the id, so we will too...
                            document.RemoveChild(style);
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
            var style = selector as Style;
            if (style != null)
            {
                this.style.Merge(style);
            }
            else
            {
                var styleMap = selector as StyleMapCollection;
                if (styleMap != null)
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
                    StyleSelector style;
                    if (this.styleMap.TryGetValue(id, out style))
                    {
                        this.Merge(style);
                    }
                }
                else if (this.fileResolver != null)
                {
                    KmlFile file = this.LoadFile(path);
                    if (file != null)
                    {
                        StyleSelector style;
                        if (file.StyleMap.TryGetValue(id, out style))
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

            var style = element as StyleSelector;
            if (style != null)
            {
                // Add the style to the map so we generate an unique id
                if (style.Id != null)
                {
                    this.styleMap[style.Id] = style;
                }

                // Find the Document to put the Style in, making sure it doesn't
                // already have one as a Parent.
                var document = element.GetParent<Document>();
                if ((document != null) && (element.Parent != document))
                {
                    // Is the style in a Feature that doesn't have a StyleUrl?
                    var feature = element.Parent as Feature;
                    if ((feature != null) && (feature.StyleUrl == null))
                    {
                        // Create a copy of the style, using a new id
                        StyleSelector shared = (StyleSelector)Activator.CreateInstance(style.GetType());
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
