using System;
using NUnit.Framework;
using SharpKml.Dom;

namespace UnitTests.Dom
{
    [TestFixture]
    public class ElementTest
    {
        [Test]
        public void TestAddChild()
        {
            var child = new Placemark();
            var parent1 = new Folder();
            var parent2 = new Folder();

            parent1.AddFeature(child);

            // child belongs to another element, should throw
            Assert.That(() => parent2.AddFeature(child),
                        Throws.TypeOf<InvalidOperationException>());

            // child has already been added once
            Assert.That(() => parent1.AddFeature(child),
                        Throws.TypeOf<InvalidOperationException>());
        }
    }
}
