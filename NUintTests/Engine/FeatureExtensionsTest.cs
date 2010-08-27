using System;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace NUintTests.Engine
{
    [TestFixture]
    public class FeatureExtensionsTest
    {
        private static readonly TestCase[] TestCases =
        {
            new TestCase("simple-placemark", new BoundingBox(37.4222899014025, 37.4222899014025, -122.082203542568, -122.082203542568)),
            new TestCase("floating-placemark", new BoundingBox(37.4220033612141, 37.4220033612141, -122.084075, -122.084075)),
            new TestCase("placemarks-folder", new BoundingBox(37.4222899014025, 37.4215692786755, -122.082203542568, -122.085766700618)),
            new TestCase("tessellated-linestring-placemark", new BoundingBox(36.1067787047714, 36.0905099328766, -112.081423783034, -112.087026775269)),
            new TestCase("purple-line-placemark", new BoundingBox(36.0944767260255, 36.086463123013, -112.265654928602, -112.269526855561)),
            new TestCase("b41", new BoundingBox(37.4228181532365, 37.4220817196725, -122.08509907149, -122.086016227378)),
            new TestCase("pentagon", new BoundingBox(38.872910162817, 38.868757801256, -77.0531553685479, -77.0584405629039)),
            new TestCase("model", new BoundingBox(40.009993372683, 40.009993372683, -105.272774533734, -105.272774533734)),
            new TestCase("photooverlay", new BoundingBox(45.968226693, 45.968226693, 7.71792711000002, 7.71792711000002))
        };

        [Test]
        public void TestCalculateLookAt()
        {
            Placemark placemark = null;
            Assert.Throws<ArgumentNullException>(() => placemark.CalculateLookAt());

            placemark = new Placemark();
            Assert.IsNull(placemark.CalculateLookAt()); // Nothing to look at

            Point point = new Point();
            point.Coordinate = new Vector(37, -122);
            placemark.Geometry = point;

            var lookat = placemark.CalculateLookAt();
            Assert.IsNotNull(lookat);
            Assert.AreEqual(37.0, lookat.Latitude);
            Assert.AreEqual(-122.0, lookat.Longitude);
            Assert.AreEqual(1000.0, lookat.Range);
            Assert.AreEqual(AltitudeMode.RelativeToGround, lookat.AltitudeMode);
            Assert.IsNull(lookat.Altitude);
            Assert.IsNull(lookat.Heading);
            Assert.IsNull(lookat.Tilt);

            LineString line = new LineString();
            line.Coordinates = new CoordinateCollection();
            line.Coordinates.Add(new Vector(37, -122));
            line.Coordinates.Add(new Vector(38, -121));

            placemark = new Placemark();
            placemark.Geometry = line;

            lookat = placemark.CalculateLookAt();
            Assert.AreEqual(37.5, lookat.Latitude);
            Assert.AreEqual(-121.5, lookat.Longitude);
            Assert.AreEqual(135123.4361, lookat.Range, 0.0001);
        }

        [Test]
        public void TestCalculateLookAtFolder()
        {
            Location location = new Location();
            location.Latitude = 0;
            location.Longitude = 0;

            Model model = new Model();
            model.Location = location;

            Placemark placemark = new Placemark();
            placemark.Geometry = model;

            Folder folder = new Folder();
            folder.AddFeature(placemark);

            var lookat = folder.CalculateLookAt();
            Assert.AreEqual(0.0, lookat.Latitude);
            Assert.AreEqual(0.0, lookat.Longitude);
            Assert.AreEqual(1000.0, lookat.Range);

            Point point = new Point();
            point.Coordinate = new Vector(10, 10);

            PhotoOverlay overlay = new PhotoOverlay();
            overlay.Location = point;

            folder.AddFeature(overlay);
            lookat = folder.CalculateLookAt();
            Assert.AreEqual(5.0, lookat.Latitude);
            Assert.AreEqual(5.0, lookat.Longitude);
            Assert.AreEqual(1494183.4444, lookat.Range, 0.0001);
        }

        [Test]
        public void TestCalculateBounds()
        {
            Placemark placemark = null;
            Assert.Throws<ArgumentNullException>(() => placemark.CalculateBounds());

            placemark = new Placemark();
            Assert.IsNull(placemark.CalculateBounds());

            using (var stream = SampleData.CreateStream("Engine.Data.Bounds.kml"))
            {
                KmlFile file = KmlFile.Load(stream);
                foreach (var test in TestCases)
                {
                    RunTestCase(file, test.Id, test.Box);
                }
            }
        }

        private static void RunTestCase(KmlFile file, string id, BoundingBox box)
        {
            Feature feature = file.FindObject(id) as Feature;
            Assert.IsNotNull(feature); // Verify the test data

            var bounds = feature.CalculateBounds();
            Assert.IsNotNull(bounds);
            Assert.AreEqual(box.East, bounds.East);
            Assert.AreEqual(box.North, bounds.North);
            Assert.AreEqual(box.South, bounds.South);
            Assert.AreEqual(box.West, bounds.West);
        }

        private class TestCase
        {
            public BoundingBox Box { get; private set; }

            public string Id { get; private set; }

            public TestCase(string id, BoundingBox box)
            {
                this.Box = box;
                this.Id = id;
            }
        }
    }
}
