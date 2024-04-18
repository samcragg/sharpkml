using System.Numerics;
using NUnit.Framework;
using Vector = SharpKml.Base.Vector;

namespace UnitTests.Base
{
    [TestFixture]
    public class VectorTest
    {
        [Test]
        public void TestConstruction()
        {
            Vector vector = new Vector();
            Assert.That(vector.Latitude, Is.EqualTo(0.0));
            Assert.That(vector.Longitude, Is.EqualTo(0.0));
            Assert.That(vector.Altitude, Is.Null);

            vector = new Vector(1.0, 2.0);
            Assert.That(vector.Latitude, Is.EqualTo(1.0));
            Assert.That(vector.Longitude, Is.EqualTo(2.0));
            Assert.That(vector.Altitude, Is.Null);

            vector = new Vector(1.0, 2.0, 3.0);
            Assert.That(vector.Latitude, Is.EqualTo(1.0));
            Assert.That(vector.Longitude, Is.EqualTo(2.0));
            Assert.That(vector.Altitude, Is.EqualTo(3.0));
        }

        [Test]
        public void TestEquals()
        {
            Vector vector1 = new Vector(1.0, 2.0, 3.0);
            Vector vector2 = new Vector(1.0, 2.0, 3.0);

            Assert.That(vector1, Is.Not.SameAs(vector2));
            Assert.That(vector1, Is.EqualTo(vector2));
            Assert.That(vector1.Equals(null), Is.False);
            Assert.That(vector1.GetHashCode(), Is.EqualTo(vector2.GetHashCode()));

            vector2.Altitude = null;
            Assert.That(vector1, Is.Not.EqualTo(vector2));

            vector2.Altitude = 3.0;
            vector2.Latitude = 3.0;
            Assert.That(vector1, Is.Not.EqualTo(vector2));
        }

        [Test]
        public void TestSimpleMultiplication()
        {
            Vector vector1 = new Vector();
            vector1 *= 1;
            Assert.That(vector1.Longitude, Is.EqualTo(0));
            Assert.That(vector1.Latitude, Is.EqualTo(0));
            Assert.That(vector1.Altitude, Is.Null);
        }

        [Test]
        public void TestMoreMultiplication()
        {
            Vector vector1 = new Vector(10, 10, 10);
            vector1 *= 10;
            Assert.That(vector1.Longitude, Is.EqualTo(100));
            Assert.That(vector1.Latitude, Is.EqualTo(100));
            Assert.That(vector1.Altitude, Is.EqualTo(100));
        }

        [Test]
        public void TestVectorAddition()
        {
            Vector vector1 = new Vector(10, 10);
            Vector vector2 = new Vector(-10, -10, -10);
            Vector endVector = vector1 + vector2;

            Assert.That(endVector.Longitude, Is.EqualTo(0));
            Assert.That(endVector.Latitude, Is.EqualTo(0));
            Assert.That(endVector.Altitude, Is.Null);
        }

        [Test]
        public void TestVectorInversion()
        {
            Vector vector1 = new Vector(10, 5);
            vector1 = -vector1;

            Assert.That(vector1.Latitude, Is.EqualTo(-10));
            Assert.That(vector1.Longitude, Is.EqualTo(-5));
            Assert.That(vector1.Altitude, Is.Null);
        }

        [Test]
        public void TestVectorSubtrack()
        {
            Vector vector1 = new Vector(10, 5);
            Vector vector2 = new Vector(20, 10, 5);

            Vector endVector = vector1 - vector2;

            Assert.That(endVector.Latitude, Is.EqualTo(-10));
            Assert.That(endVector.Longitude, Is.EqualTo(-5));
            Assert.That(endVector.Altitude, Is.Null);
        }
    }
}
