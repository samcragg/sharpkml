using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Base
{
    [TestFixture]
    public class ElementWalkerTest
    {
        [Test]
        public void TestWalkChildren()
        {
            const int Depth = 50;

            Folder parent = new Folder();
            Folder root = parent;
            for (int i = 0; i < Depth; ++i)
            {
                Folder child = new Folder();
                parent.AddFeature(child); // Added to the Children collection
                parent = child;
            }

            Assert.That(ElementWalker.Walk(root).Count(), Is.EqualTo(Depth + 1)); // Depth + 1 to allow for root itself
        }

        [Test]
        public void TestWalkProperties()
        {
            Kml kml = new Kml();
            kml.Feature = new Folder(); // This will not be added to the Children collection

            Assert.That(ElementWalker.Walk(kml).Count(), Is.EqualTo(2));
        }

        [Test]
        public void TestWalkCustomElements()
        {
            const int Count = 10;
            CoordinateCollection coordinates = new CoordinateCollection();
            for (int i = 0; i < Count; ++i)
            {
                coordinates.Add(new Vector());
            }
            Assert.That(ElementWalker.Walk(coordinates).Count(), Is.EqualTo(1));

            // This class uses a private class deriving from ICustomElement as a child
            // Make sure it's not included.
            ItemIcon icon = new ItemIcon();
            icon.State = ItemIconStates.Open | ItemIconStates.Error;
            Assert.That(ElementWalker.Walk(icon).Count(), Is.EqualTo(1));
        }
    }
}
