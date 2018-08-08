using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SharpKml.Dom;

namespace UnitTests.Dom
{
    [TestFixture]
    public class ElementCollectionTests
    {
        private ElementCollection elements;

        [SetUp]
        public void SetUp()
        {
            this.elements = new ElementCollection(new Dictionary<TypeInfo, int>
            {
                { typeof(Type1).GetTypeInfo(), 0 },
                { typeof(Type2).GetTypeInfo(), 1 },
                { typeof(Type3).GetTypeInfo(), 2 },
            });
        }

        [Test]
        public void ShouldAddElementsInTheCorrectOrder()
        {
            this.elements.Add(new Type2());
            this.elements.Add(new Type1());
            this.elements.Add(new Type3());

            Assert.That(
                this.elements.Select(e => e.GetType()),
                Is.EqualTo(new[] { typeof(Type1), typeof(Type2), typeof(Type3) }));
        }

        [Test]
        public void ShouldAddSameTypeElementsAfterExistingElements()
        {
            this.elements.Add(new Type2 { Value = 1 });
            this.elements.Add(new Type2 { Value = 2 });
            this.elements.Add(new Type2 { Value = 3 });

            Assert.That(
                this.elements.Cast<Type2>().Select(e => e.Value),
                Is.EqualTo(new[] { 1, 2, 3 }));
        }

        private class Type1 : Element
        {
        }

        private class Type2 : Element
        {
            public int Value { get; set; }
        }

        private class Type3 : Element
        {
        }
    }
}
