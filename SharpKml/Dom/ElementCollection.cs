// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Dom
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents a collection of elements that are sorted by their type.
    /// </summary>
    internal sealed class ElementCollection : ICollection<Element>, IReadOnlyCollection<Element>
    {
        private readonly IComparer<Element> elementComparer;
        private readonly List<Element> elements = new List<Element>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCollection"/> class.
        /// </summary>
        /// <param name="typeOrder">Represents the order of the elements.</param>
        public ElementCollection(IReadOnlyDictionary<TypeInfo, int> typeOrder)
        {
            this.elementComparer = new ChildTypeComparer(typeOrder);
        }

        /// <inheritdoc/>
        public int Count => this.elements.Count;

        /// <inheritdoc/>
        bool ICollection<Element>.IsReadOnly => true;

        /// <inheritdoc/>
        public void Add(Element element)
        {
            int index = this.elements.BinarySearch(element, this.elementComparer);
            if (index < 0)
            {
                // If binary search doesn't find the element then it returns "a
                // negative number that is the bitwise complement of the index
                // of the next element that is larger than item"
                index = ~index;
            }
            else
            {
                // We've found an item of the same order - insert this element
                // after all the elements that compare equal
                for (index++; index < this.elements.Count; index++)
                {
                    if (this.elementComparer.Compare(this.elements[index], element) != 0)
                    {
                        break;
                    }
                }
            }

            this.elements.Insert(index, element);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.elements.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(Element item)
        {
            return this.elements.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(Element[] array, int arrayIndex)
        {
            this.elements.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<Element> GetEnumerator()
        {
            return this.elements.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(Element child)
        {
            return this.elements.Remove(child);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class ChildTypeComparer : IComparer<Element>
        {
            private readonly IReadOnlyDictionary<TypeInfo, int> typeOrder;

            public ChildTypeComparer(IReadOnlyDictionary<TypeInfo, int> typeOrder)
            {
                this.typeOrder = typeOrder;
            }

            public int Compare(Element elementA, Element elementB)
            {
                int indexA = this.typeOrder[elementA.GetType().GetTypeInfo()];
                int indexB = this.typeOrder[elementB.GetType().GetTypeInfo()];
                return indexA.CompareTo(indexB);
            }
        }
    }
}
