using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Dom
{
    [TestFixture]
    public class CoordinateCollectionTest
    {
        private const string XmlFormat = "<coordinates xmlns=\"http://www.opengis.net/kml/2.2\">{0}</coordinates>";

        [Test]
        public void TestCollection()
        {
            CoordinateCollection coords = new CoordinateCollection();
            coords.Add(new Vector(1, 2, 3));
            coords.Add(new Vector(2, 3, 4));
            Assert.That(() => coords.Add(null),
                        Throws.TypeOf<ArgumentNullException>());

            Assert.That(coords.Count, Is.EqualTo(2));

            Vector vector = null;
            int counter = 0;
            foreach (var point in coords)
            {
                ++counter;
                vector = point;
            }
            Assert.That(counter, Is.EqualTo(2));
            Assert.That(vector.Altitude, Is.EqualTo(4));

            Assert.True(coords.Contains(vector));
            Assert.False(coords.Contains(new Vector()));
            Assert.False(coords.Contains(null));

            Assert.True(coords.Remove(vector));
            Assert.False(coords.Remove(new Vector()));
            Assert.False(coords.Remove(null));

            Assert.That(coords.Count, Is.EqualTo(1));

            coords.Add(vector);
            Vector[] vectors = new Vector[3];
            coords.CopyTo(vectors, 1);
            Assert.That(vectors[0], Is.Null);
            Assert.That(vectors[1], Is.Not.Null);
            Assert.That(vectors[2], Is.Not.Null);

            coords.Clear();
            Assert.That(coords.Count, Is.EqualTo(0));
        }

        [Test]
        public void Test3dParse()
        {
            CoordinateCollection coords = new CoordinateCollection();
            coords.AddInnerText("1.123,-2.789,3000.5919");

            Assert.That(coords.Count, Is.EqualTo(1));
            AssertVector(coords.ElementAt(0), -2.789, 1.123, 3000.5919);

            // This should NOT destroy the first value
            coords.AddInnerText("\n-122.123,38.789,1050.0987 -122.123,39.789,1050.098");
            Assert.That(coords.Count, Is.EqualTo(3));
            AssertVector(coords.ElementAt(1), 38.789, -122.123, 1050.0987);
            AssertVector(coords.ElementAt(2), 39.789, -122.123, 1050.098);
        }

        [Test]
        public void Test2dParse()
        {
            Vector[] vectors = ParseVector("10.10,-20.20");
            AssertVector(vectors.First(), -20.20, 10.10);

            vectors = ParseVector("15.10, -24.20");
            AssertVector(vectors.First(), -24.20, 15.10);

            vectors = ParseVector("15.11 , -24.25");
            AssertVector(vectors.First(), -24.25, 15.11);

            vectors = ParseVector("122.123,-38.789 122.123,-39.789");
            Assert.That(vectors.Length, Is.EqualTo(2));
            AssertVector(vectors.ElementAt(0), -38.789, 122.123);
            AssertVector(vectors.ElementAt(1), -39.789, 122.123);

            vectors = ParseVector("\n" +
                                   "  -160.073803556017,22.0041773078075\n" +
                                   "  -160.121962433575,21.9639787234984\n" +
                                   "  -160.22633646805,21.8915919620539\n" +
                                   "  ");
            Assert.That(vectors.Length, Is.EqualTo(3));

            vectors = ParseVector("1E-02, 2E-02");
            AssertVector(vectors.First(), 0.02, 0.01);
        }

        [Test]
        public void TestBadInput()
        {
            Vector[] vectors = ParseVector("bad,input");
            Assert.That(vectors.Length, Is.EqualTo(0));

            // Not really bad but the KML specification states tuples should be seperated by space
            vectors = ParseVector("1,2,3,4,5,6,7,8,9");
            Assert.That(vectors.Length, Is.EqualTo(3));
            AssertVector(vectors.ElementAt(0), 2.0, 1.0, 3.0);
            AssertVector(vectors.ElementAt(1), 5.0, 4.0, 6.0);
            AssertVector(vectors.ElementAt(2), 8.0, 7.0, 9.0);
        }

        [Test]
        public void TestSerialize()
        {
            var serializer = new Serializer();
            var coords = new CoordinateCollection();

            // First test empty.
            serializer.SerializeRaw(coords);
            Assert.That(serializer.Xml, Is.EqualTo("<coordinates xmlns=\"http://www.opengis.net/kml/2.2\" />"));

            // Now with a value.
            coords.Add(new Vector(1, 2, 3));
            serializer.SerializeRaw(coords);

            string expected = string.Format(CultureInfo.InvariantCulture, XmlFormat, "2,1,3");
            Assert.That(serializer.Xml, Is.EqualTo(expected));
        }

        [Test]
        public void SerializeShouldNotOutputAltitudeIfItIsNotSpecified()
        {
            var coords = new CoordinateCollection(new[] { new Vector(1, 2) });

            var serializer = new Serializer();
            serializer.SerializeRaw(coords);

            var expected = string.Format(CultureInfo.InvariantCulture, XmlFormat, "2,1");
            Assert.That(serializer.Xml, Is.EqualTo(expected));
        }

        [Test]
        public void SerializeShouldUseTheDelimiterToSeparatePoints()
        {
            var coords = new CoordinateCollection(new []
                {
                    new Vector(1, 2, 3),
                    new Vector(2, 1, 3)
                });

            var serializer = new Serializer();

            CoordinateCollection.Delimiter = " ";
            serializer.SerializeRaw(coords);
            CoordinateCollection.Delimiter = "\n"; // Reset to prove it worked during the call to serialize

            var expected = string.Format(CultureInfo.InvariantCulture, XmlFormat, "2,1,3 1,2,3");
            Assert.That(serializer.Xml, Is.EqualTo(expected));
        }

        private static void AssertVector(Vector vector, double lat, double lon)
        {
            Assert.That(vector.Latitude, Is.EqualTo(lat));
            Assert.That(vector.Longitude, Is.EqualTo(lon));
            Assert.That(vector.Altitude, Is.Null);
        }

        private static void AssertVector(Vector vector, double lat, double lon, double alt)
        {
            Assert.That(vector.Latitude, Is.EqualTo(lat));
            Assert.That(vector.Longitude, Is.EqualTo(lon));
            Assert.That(vector.Altitude, Is.EqualTo(alt));
        }

        private static Vector[] ParseVector(string input)
        {
            CoordinateCollection coords = new CoordinateCollection();
            coords.AddInnerText(input);
            return coords.ToArray();
        }
    }
}
