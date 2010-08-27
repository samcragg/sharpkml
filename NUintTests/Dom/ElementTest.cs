using System;
using NUnit.Framework;
using SharpKml.Dom;

namespace NUintTests.Dom
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
            Assert.Throws<InvalidOperationException>(() => parent2.AddFeature(child));

            // child has already been added once
            Assert.Throws<InvalidOperationException>(() => parent1.AddFeature(child));
        }
    }
}
