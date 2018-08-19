// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents a collection of elements that are sorted by their type.
    /// </summary>
    internal sealed class ElementCollection : ICollection<Element>, IReadOnlyCollection<Element>
    {
        private const int InitialSize = 4;
        private static readonly Element[] EmptyElements = new Element[0];

        private readonly IDictionary<TypeInfo, int> order;
        private Element[] elements = EmptyElements;
        private long[] orderMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection"/> class.
        /// </summary>
        /// <param name="typeOrder">Represents the order of the elements.</param>
        public ElementCollection(IDictionary<TypeInfo, int> typeOrder)
        {
            this.order = typeOrder;
        }

        /// <inheritdoc/>
        public int Count { get; private set; }

        /// <inheritdoc/>
        bool ICollection<Element>.IsReadOnly => true;

        /// <inheritdoc/>
        public void Add(Element element)
        {
            this.EnsureFreeSpace();
            this.orderMap = null;
            this.elements[this.Count++] = element;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.elements = EmptyElements;
            this.orderMap = null;
            this.Count = 0;
        }

        /// <inheritdoc/>
        public bool Contains(Element item)
        {
            return Array.IndexOf(this.elements, item, 0, this.Count) >= 0;
        }

        /// <inheritdoc/>
        public void CopyTo(Element[] array, int arrayIndex)
        {
            Array.Copy(this.elements, 0, array, arrayIndex, this.Count);
        }

        /// <inheritdoc/>
        public IEnumerator<Element> GetEnumerator()
        {
            this.EnsureMapIsValid();
            for (int i = 0; i < this.Count; i++)
            {
                // We store the [order | index] in the 64 bit map, so truncate
                // it to just get the index
                yield return this.elements[(int)this.orderMap[i]];
            }
        }

        /// <inheritdoc/>
        public bool Remove(Element child)
        {
            int index = Array.IndexOf(this.elements, child, 0, this.Count);
            if (index < 0)
            {
                return false;
            }

            this.orderMap = null;
            this.Count--;

            // Quick check if we've removed the item at the end
            if (index < this.Count)
            {
                Array.Copy(this.elements, index + 1, this.elements, index, this.Count - index);
            }

            this.elements[this.Count] = null;
            return true;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Determines whether the specified type has serialization order information.
        /// </summary>
        /// <param name="elementType">The type of the element.</param>
        /// <returns><c>true</c> if the type is known; otherwise, <c>false</c>.</returns>
        internal bool HasOrderFor(TypeInfo elementType)
        {
            return this.order.ContainsKey(elementType);
        }

        /// <summary>
        /// Registers the specified type as a derived class of one that does have order.
        /// </summary>
        /// <param name="elementType">The type of the element.</param>
        /// <returns><c>true</c> if the type was registered; otherwise, <c>false</c>.</returns>
        internal bool RegisterAsDerivedClass(TypeInfo elementType)
        {
            foreach (KeyValuePair<TypeInfo, int> type in this.order)
            {
                if (type.Key.IsAssignableFrom(elementType))
                {
                    // This is a derived class so add it to the child type
                    // collection with the same index. Note it's OK to change
                    // the collection as we're breaking out of the iteration.
                    this.order[elementType] = type.Value;
                    return true;
                }
            }

            // We couldn't find a a type that childType inherits from
            return false;
        }

        private void EnsureFreeSpace()
        {
            if (this.elements.Length == 0)
            {
                this.elements = new Element[InitialSize];
            }
            else if (this.elements.Length == this.Count)
            {
                Array.Resize(ref this.elements, this.elements.Length * 2);
            }
        }

        private void EnsureMapIsValid()
        {
            if (this.orderMap == null)
            {
                this.orderMap = new long[this.Count];
                for (int i = 0; i < this.orderMap.Length; i++)
                {
                    int order = this.order[this.elements[i].GetType().GetTypeInfo()];
                    this.orderMap[i] = ((long)order << 32) | (uint)i;
                }

                Array.Sort(this.orderMap);
            }
        }
    }
}
