using NUnit.Framework;
using SharpKml.Base;

namespace NUintTests.Base
{
    [TestFixture]
    public class VectorTest
    {
        [Test]
        public void TestConstruction()
        {
            Vector vector = new Vector();
            Assert.AreEqual(0.0, vector.Latitude);
            Assert.AreEqual(0.0, vector.Longitude);
            Assert.IsNull(vector.Altitude);

            vector = new Vector(1.0, 2.0);
            Assert.AreEqual(1.0, vector.Latitude);
            Assert.AreEqual(2.0, vector.Longitude);
            Assert.IsNull(vector.Altitude);

            vector = new Vector(1.0, 2.0, 3.0);
            Assert.AreEqual(1.0, vector.Latitude);
            Assert.AreEqual(2.0, vector.Longitude);
            Assert.AreEqual(3.0, vector.Altitude);
        }

        [Test]
        public void TestEquals()
        {
            Vector vector1 = new Vector(1.0, 2.0, 3.0);
            Vector vector2 = new Vector(1.0, 2.0, 3.0);

            Assert.AreNotSame(vector1, vector2);
            Assert.AreEqual(vector1, vector2);
            Assert.IsFalse(vector1.Equals(null));
            Assert.AreEqual(vector1.GetHashCode(), vector2.GetHashCode());

            vector2.Altitude = null;
            Assert.AreNotEqual(vector1, vector2);

            vector2.Altitude = 3.0;
            vector2.Latitude = 3.0;
            Assert.AreNotEqual(vector1, vector2);
        }
    }
}
