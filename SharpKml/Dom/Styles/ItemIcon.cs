// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using SharpKml.Base;

    /// <summary>
    /// Specifies the location for an icon used in the list view to reflect the
    /// state of the <see cref="Folder"/> or <see cref="NetworkLink"/> to which
    /// it is associated.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 12.14</remarks>
    [KmlElement("ItemIcon")]
    public class ItemIcon : KmlObject
    {
        private static readonly XmlComponent StateComponent = new XmlComponent(null, "state", KmlNamespaces.Kml22Namespace);

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        /// <remarks>
        /// The URL may contain a fragment component that allows indirect
        /// identification of some portion or subset of a resource.
        /// </remarks>
        [KmlElement("href", 2)]
        public Uri Href { get; set; }

        /// <summary>
        /// Gets or sets the current state of the <see cref="Folder"/> or
        /// <see cref="NetworkLink"/>.
        /// </summary>
        /// <remarks>
        /// Multiple values of <see cref="ItemIconStates"/> can be specified
        /// at the same time (using bitwise operations). See section 12.14.3
        /// for example combinations.
        /// </remarks>
        public ItemIconStates State
        {
            get => this.StateData?.State ?? default;
            set
            {
                if (this.StateData == null)
                {
                    this.StateData = new StateElement();
                }

                this.StateData.State = value;
            }
        }

        [KmlElement("state", 1)]
        private StateElement StateData { get; set; }

        /// <summary>
        /// Used to correctly serialize multiple ItemIconStates.
        /// </summary>
        internal class StateElement : Element, ICustomElement
        {
            private static readonly ItemIconStates[] States = GetStates();

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren => false;

            /// <summary>
            /// Gets or sets the ItemIconState.
            /// </summary>
            public ItemIconStates State { get; set; }

            /// <summary>
            /// Writes the start of an XML element.
            /// </summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            public void CreateStartElement(XmlWriter writer)
            {
                // Make sure there's something to save
                if (this.State != ItemIconStates.None)
                {
                    IEnumerable<string> attributes = States.Where(s => (s != ItemIconStates.None) && this.State.HasFlag(s))
                                                           .Select(s => TypeBrowser.GetEnum(s))
                                                           .Where(a => a != null)
                                                           .Select(a => a.ElementName);

                    string value = string.Join(" ", attributes);
                    writer.WriteElementString("state", KmlNamespaces.Kml22Namespace, value);
                }
            }

            /// <summary>
            /// Parses the specified value and converts it to an ItemIconState.
            /// </summary>
            /// <param name="value">The string to parse.</param>
            /// <remarks>Does not clear the existing State.</remarks>
            public void Parse(string value)
            {
                if (value != null)
                {
                    string[] tokens = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string token in tokens)
                    {
                        ValueConverter.TryGetValue(typeof(ItemIconStates), token, out object state);
                        if (state != null)
                        {
                            this.State |= (ItemIconStates)state;
                        }
                    }
                }
            }

            private static ItemIconStates[] GetStates()
            {
                return Enum.GetValues(typeof(ItemIconStates))
                           .Cast<ItemIconStates>()
                           .ToArray();
            }
        }
    }
}
