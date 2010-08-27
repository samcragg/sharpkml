using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Dom
{
    [TestFixture]
    public class CoordinateCollectionTest
    {
        [Test]
        public void TestCollection()
        {
            CoordinateCollection coords = new CoordinateCollection();
            coords.Add(new Vector(1, 2, 3));
            coords.Add(new Vector(2, 3, 4));
            Assert.Throws<ArgumentNullException>(() => coords.Add(null));

            Assert.AreEqual(2, coords.Count);

            Vector vector = null;
            int counter = 0;
            foreach (var point in coords)
            {
                ++counter;
                vector = point;
            }
            Assert.AreEqual(2, counter);
            Assert.AreEqual(4, vector.Altitude);

            Assert.IsTrue(coords.Contains(vector));
            Assert.IsFalse(coords.Contains(new Vector()));
            Assert.IsFalse(coords.Contains(null));

            Assert.IsTrue(coords.Remove(vector));
            Assert.IsFalse(coords.Remove(new Vector()));
            Assert.IsFalse(coords.Remove(null));

            Assert.AreEqual(1, coords.Count);

            coords.Add(vector);
            Vector[] vectors = new Vector[3];
            coords.CopyTo(vectors, 1);
            Assert.IsNull(vectors[0]);
            Assert.IsNotNull(vectors[1]);
            Assert.IsNotNull(vectors[2]);

            coords.Clear();
            Assert.AreEqual(0, coords.Count);
        }

        [Test]
        public void Test3dParse()
        {
            CoordinateCollection coords = new CoordinateCollection();
            coords.AddInnerText("1.123,-2.789,3000.5919");

            Assert.AreEqual(1, coords.Count);
            // ElementAt is a Linq extension and not part of Coordinates
            AssertVector(coords.ElementAt(0), -2.789, 1.123, 3000.5919);

            // This should NOT destroy the first value
            coords.AddInnerText("\n-122.123,38.789,1050.0987 -122.123,39.789,1050.098");
            Assert.AreEqual(3, coords.Count);
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
            Assert.AreEqual(2, vectors.Length);
            AssertVector(vectors.ElementAt(0), -38.789, 122.123);
            AssertVector(vectors.ElementAt(1), -39.789, 122.123);

            vectors = ParseVector("\n" +
                                   "  -160.073803556017,22.0041773078075\n" +
                                   "  -160.121962433575,21.9639787234984\n" +
                                   "  -160.22633646805,21.8915919620539\n" +
                                   "  ");
            Assert.AreEqual(3, vectors.Length);

            vectors = ParseVector("1E-02, 2E-02");
            AssertVector(vectors.First(), 0.02, 0.01);
        }

        [Test]
        public void TestBadInput()
        {
            Vector[] vectors = ParseVector("bad,input");
            Assert.AreEqual(0, vectors.Length);

            // Not really bad but the KML specification states tuples should be seperated by space
            vectors = ParseVector("1,2,3,4,5,6,7,8,9");
            Assert.AreEqual(3, vectors.Length);
            AssertVector(vectors.ElementAt(0), 2.0, 1.0, 3.0);
            AssertVector(vectors.ElementAt(1), 5.0, 4.0, 6.0);
            AssertVector(vectors.ElementAt(2), 8.0, 7.0, 9.0);
        }

        [Test]
        public void TestSerialize()
        {
            const string XmlFormat = "<coordinates xmlns=\"http://www.opengis.net/kml/2.2\">{0}</coordinates>";
            Serializer serializer = new Serializer();
            CoordinateCollection coords = new CoordinateCollection();

            // First test empty
            serializer.SerializeRaw(coords);
            Assert.AreEqual("<coordinates xmlns=\"http://www.opengis.net/kml/2.2\" />", serializer.Xml);

            coords.Add(new Vector(1, 2, 3));
            serializer.SerializeRaw(coords);
            string output = serializer.Xml.Replace("\r", "");
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, XmlFormat,
                "2,1,3\n"), output);

            // Make sure altitude is only saved if it's specified
            coords.Clear();
            coords.Add(new Vector(1, 2));
            serializer.SerializeRaw(coords);
            output = serializer.Xml.Replace("\r", "");
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, XmlFormat,
                "2,1\n"), output);

            coords.Clear();
            coords.Add(new Vector(1, 2, 3));
            coords.Add(new Vector(2, 1, 3));
            serializer.SerializeRaw(coords);
            output = serializer.Xml.Replace("\r", "");
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, XmlFormat,
                "2,1,3\n1,2,3\n"), output);
        }

        private static void AssertVector(Vector vector, double lat, double lon)
        {
            Assert.AreEqual(lat, vector.Latitude);
            Assert.AreEqual(lon, vector.Longitude);
            Assert.IsNull(vector.Altitude);
        }

        private static void AssertVector(Vector vector, double lat, double lon, double alt)
        {
            Assert.AreEqual(lat, vector.Latitude);
            Assert.AreEqual(lon, vector.Longitude);
            Assert.AreEqual(alt, vector.Altitude);
        }

        private static Vector[] ParseVector(string input)
        {
            CoordinateCollection coords = new CoordinateCollection();
            coords.AddInnerText(input);
            return coords.ToArray();
        }
    }
}
