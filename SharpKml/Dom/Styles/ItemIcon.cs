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
    public sealed class ItemIcon : KmlObject
    {
        private static readonly XmlComponent StateComponent = new XmlComponent(null, "state", KmlNamespaces.Kml22Namespace);
        private readonly StateElement state = new StateElement();

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemIcon"/> class.
        /// </summary>
        public ItemIcon()
        {
            this.RegisterValidChild<StateElement>();
            this.AddChild(this.state);
        }

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
            get { return this.state.State; }
            set { this.state.State = value; }
        }

        /// <summary>
        /// Parses the &lt;state&gt; element.
        /// </summary>
        /// <param name="orphan">The <see cref="Element"/> to add.</param>
        protected internal override void AddOrphan(Element orphan)
        {
            UnknownElement unknown = orphan as UnknownElement;
            if (unknown != null)
            {
                if (StateComponent.Equals(unknown.UnknownData))
                {
                    this.state.Parse(unknown.InnerText);
                    return;
                }
            }

            base.AddOrphan(orphan);
        }

        /// <summary>
        /// Used to correctly serialize multiple ItemIconStates.
        /// </summary>
        internal class StateElement : Element, ICustomElement
        {
            private static readonly ItemIconStates[] States = GetStates();

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            public bool ProcessChildren
            {
                get { return false; }
            }

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
                    var tokens = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var token in tokens)
                    {
                        object state;
                        ValueConverter.TryGetValue(typeof(ItemIconStates), token, out state);
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
