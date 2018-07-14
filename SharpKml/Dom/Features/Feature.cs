// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SharpKml.Base;

    /// <summary>
    /// Represents a KML AbstractFeatureGroup
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.1</remarks>
    [ChildType(typeof(StyleSelector), 1)]
    public abstract class Feature : KmlObject
    {
        /// <summary>
        /// The default value that should be used for <see cref="Visibility"/>.
        /// </summary>
        public const bool DefaultVisibility = true;

        private Xal.AddressDetails address;
        private Description description;
        private ExtendedData extended;
        private Region region;
        private Snippet snippet;
        private TimePrimitive time;
        private AbstractView view;

        /// <summary>
        /// Gets or sets an unstructured address for the Feature, such as street,
        /// city, state address, and/or a postal code.
        /// </summary>
        /// <remarks>
        /// This may be used to geocode the location of a Feature if it does not
        /// contain a Geometry element.
        /// </remarks>
        [KmlElement("address", 6)]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets a structured address for the Feature formatted according to xAL 2.0.
        /// </summary>
        /// <remarks>
        /// This may be used to geocode the location of a Feature if it does not
        /// contain a <see cref="Geometry"/> element.
        /// </remarks>
        [KmlElement(null, 7)]
        public Xal.AddressDetails AddressDetails
        {
            get => this.address;
            set => this.UpdatePropertyChild(value, ref this.address);
        }

        /// <summary>
        /// Gets or sets the author of the Feature.
        /// </summary>
        /// <remarks>This value is inheritable - see section 9.6.2</remarks>
        [KmlElement(null, 4)]
        public Atom.Author AtomAuthor { get; set; }

        /// <summary>
        /// Gets or sets the URL of the source resource that contains the Feature.
        /// </summary>
        /// <remarks>
        /// This value is inheritable - see section 9.6.2
        /// The URL is encoded as the value of <see cref="Atom.Link.Href"/>. The
        /// <see cref="Atom.Link.Relation"/> property shall be present and its
        /// value shall be "related".
        /// </remarks>
        [KmlElement(null, 5)]
        public Atom.Link AtomLink { get; set; }

        /// <summary>
        /// Gets or sets a description of the Feature.
        /// </summary>
        /// <remarks>
        /// This should be displayed in the description balloon. The text may
        /// include HTML content, which requires special processing for
        /// embedded HTML links - see Section 9.1.3.10 for details.
        /// </remarks>
        [KmlElement(null, 10)]
        public Description Description
        {
            get => this.description;
            set => this.UpdatePropertyChild(value, ref this.description);
        }

        /// <summary>
        /// Gets or sets additional user-defined data.
        /// </summary>
        [KmlElement(null, 16)]
        public ExtendedData ExtendedData
        {
            get => this.extended;
            set => this.UpdatePropertyChild(value, ref this.extended);
        }

        /// <summary>
        /// Gets or sets the visibility of a description balloon.
        /// [Google Extension]
        /// </summary>
        [KmlElement("balloonVisibility", KmlNamespaces.GX22Namespace, 17)]
        public bool? GXBalloonVisibility { get; set; }

        /// <summary>
        /// Gets or sets a label for the Feature.
        /// </summary>
        [KmlElement("name", 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether a <see cref="Container"/> appears expanded (true)
        /// or collapsed (false) when first loaded into the list view.
        /// </summary>
        [KmlElement("open", 3)]
        public bool? Open { get; set; }

        /// <summary>
        /// Gets or sets a value representing a telephone number.
        /// </summary>
        /// <remarks>The number should be formatted according to [RFC 3966].</remarks>
        [KmlElement("phoneNumber", 8)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Region"/> of the Feature.
        /// </summary>
        /// <remarks>This value is inheritable - see section 9.6.2</remarks>
        [KmlElement(null, 15)]
        public Region Region
        {
            get => this.region;
            set => this.UpdatePropertyChild(value, ref this.region);
        }

        /// <summary>
        /// Gets or sets a short description of the of the Feature.
        /// </summary>
        /// <remarks>
        /// This is used instead of Description in the list view if it exists.
        /// </remarks>
        [KmlElement(null, 9)]
        public Snippet Snippet
        {
            get => this.snippet;
            set => this.UpdatePropertyChild(value, ref this.snippet);
        }

        /// <summary>
        /// Gets the <see cref="StyleSelector"/>s contained by this instance.
        /// </summary>
        public IEnumerable<StyleSelector> Styles => this.Children.OfType<StyleSelector>();

        /// <summary>
        /// Gets or sets a reference to a <see cref="Style"/> or
        /// <see cref="StyleMapCollection"/>.
        /// </summary>
        /// <remarks>
        /// The value of the fragment shall be the id of a <c>Style</c> or
        /// <c>StyleMap</c> defined in a <see cref="Document"/>.
        /// </remarks>
        [KmlElement("styleUrl", 13)]
        public Uri StyleUrl { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="TimePrimitive"/> that affects the visibility
        /// of the Feature.
        /// </summary>
        /// <remarks>This value is inheritable - see section 9.6.2</remarks>
        [KmlElement(null, 12)]
        public TimePrimitive Time
        {
            get => this.time;
            set => this.UpdatePropertyChild(value, ref this.time);
        }

        /// <summary>
        /// Gets or sets a viewpoint for the Feature.
        /// </summary>
        [KmlElement(null, 11)]
        public AbstractView Viewpoint
        {
            get => this.view;
            set => this.UpdatePropertyChild(value, ref this.view);
        }

        /// <summary>
        /// Gets or sets whether the Feature shall be drawn in the geographic
        /// view when it is initially loaded (true), or not (false).
        /// </summary>
        /// <remarks>
        /// In order for a Feature to be visible, the Visibility of all its
        /// ancestors shall also be set to true.
        /// </remarks>
        [KmlElement("visibility", 2)]
        public bool? Visibility { get; set; }

        /// <summary>
        /// Adds the specified <see cref="StyleSelector"/> to this instance.
        /// </summary>
        /// <param name="selector">
        /// The <c>StyleSelector</c> to add to this instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <c>selector</c> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <c>selector></c> belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddStyle(StyleSelector selector)
        {
            this.AddChild(selector);
        }

        /// <summary>
        /// Removes all the <see cref="StyleSelector"/>s from this instance.
        /// </summary>
        public void ClearStyles()
        {
            var selectors = this.Styles.ToList();
            foreach (StyleSelector selector in selectors)
            {
                this.RemoveChild(selector);
            }
        }

        /// <summary>
        /// Removes the specified <see cref="StyleSelector"/> from this instance.
        /// </summary>
        /// <param name="selector">
        /// The <c>StyleSelector</c> to add to this instance.
        /// </param>
        /// <returns>
        /// true if <c>selector</c> is successfully removed; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <c>selector</c> is null.
        /// </exception>
        public bool RemoveStyle(StyleSelector selector)
        {
            return this.RemoveChild(selector);
        }
    }
}
