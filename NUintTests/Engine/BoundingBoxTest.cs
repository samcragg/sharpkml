using NUnit.Framework;
using SharpKml.Engine;

namespace NUintTests.Engine
{
    [TestFixture]
    public class BoundingBoxTest
    {
        [Test]
        public void BasicTest()
        {
            const double North = 45.45;
            const double South = -12.12;
            const double East = 123.123;
            const double West = -89.89;

            BoundingBox box = new BoundingBox();
            Assert.IsTrue(box.IsEmpty);

            box.Expand(North, East);
            Assert.IsFalse(box.IsEmpty);
            box.Expand(South, West);

            VerifyBounds(box, North, South, East, West);
            Assert.AreEqual(16.665, box.Center.Latitude, 0.000001);
            Assert.AreEqual(16.6165, box.Center.Longitude, 0.000001);
        }

        [Test]
        public void TestAlign()
        {
            BoundingBox box = new BoundingBox();
            box.North = 37.786807;  // Lincoln Park 3
            box.South = 37.781563;  // Lincoln Park 7
            box.East = -122.494135;  // Lincoln Park 18
            box.West = -122.504031;  // Lincoln Park 5

            BoundingBox qt = new BoundingBox(180, -180, 180, -180);
            box.Align(qt, 24);

            Assert.AreEqual(37.79296875, qt.North, 0.000001);
            Assert.AreEqual(37.7490234375, qt.South, 0.000001);
            Assert.AreEqual(-122.4755859375, qt.East, 0.000001);
            Assert.AreEqual(-122.51953125, qt.West, 0.000001);
        }

        [Test]
        public void TestContainedBy()
        {
            BoundingBox first = new BoundingBox(180, -180, 180, -180);
            BoundingBox second = new BoundingBox(1, -1, 1, -1);
            Assert.IsTrue(second.ContainedBy(first));

            second = new BoundingBox(1000, -1, 1, -1);
            Assert.IsFalse(second.ContainedBy(first));
        }

        [Test]
        public void TestContains()
        {
            double[][] points =
            {
                new double[] { 46.3941, 10.1168 },
                new double[] { 46.6356, 8.84678 },
                new double[] { 46.69, 8.95711 },
                new double[] { 46.158, 8.97531 },
                new double[] { 46.1719, 8.79744 },
                new double[] { 46.1217, 8.35152 },
                new double[] { 46.62, 8.5706 },
                new double[] { 46.7067, 8.953 },
                new double[] { 46.6087, 8.82036 },
                new double[] { 46.1546, 8.9633 },
                new double[] { 46.2368, 10.1363 },
                new double[] { 46.7079, 9.19907 },
                new double[] { 45.9296, 8.92094 },
                new double[] { 46.1738, 8.84359 },
                new double[] { 46.5616, 8.34504 },
                new double[] { 46.7389, 8.97314 },
                new double[] { 46.7493, 8.23686 },
                new double[] { 46.7233, 8.92272 },
                new double[] { 45.9528, 8.95471 }
            };

            BoundingBox box = new BoundingBox();

            foreach (var point in points)
            {
                box.Expand(point[0], point[1]);
            }

            VerifyBounds(box, 46.7493, 45.9296, 10.1363, 8.23686);
            Assert.AreEqual(46.33945, box.Center.Latitude, 0.000001);
            Assert.AreEqual(9.18658, box.Center.Longitude, 0.000001);

            foreach (var point in points)
            {
                Assert.IsTrue(box.Contains(point[0], point[1]));
                box.Expand(point[0], point[1]);
            }
        }

        [Test]
        public void TestExpand()
        {
            const double North = 89.123;
            const double South = -2.222;
            const double East = -88.888;
            const double West = -154.6789;

            BoundingBox box = new BoundingBox(North, South, East, West);
            BoundingBox other = new BoundingBox();

            other.Expand(box);
            VerifyBounds(box, North, South, East, West);
        }

        private static void VerifyBounds(BoundingBox box, double north, double south, double east, double west)
        {
            Assert.AreEqual(east, box.East);
            Assert.AreEqual(north, box.North);
            Assert.AreEqual(south, box.South);
            Assert.AreEqual(west, box.West);
        }
    }
}
