using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Base
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

            Assert.AreEqual(Depth + 1, ElementWalker.Walk(root).Count()); // Depth + 1 to allow for root itself
        }

        [Test]
        public void TestWalkProperties()
        {
            Kml kml = new Kml();
            kml.Feature = new Folder(); // This will not be added to the Children collection

            Assert.AreEqual(2, ElementWalker.Walk(kml).Count());
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
            Assert.AreEqual(1, ElementWalker.Walk(coordinates).Count());

            // This class uses a private class deriving from ICustomElement as a child
            // Make sure it's not included.
            ItemIcon icon = new ItemIcon();
            icon.State = ItemIconStates.Open | ItemIconStates.Error;
            Assert.AreEqual(1, ElementWalker.Walk(icon).Count());
        }
    }
}
