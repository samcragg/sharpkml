using System;
using NUnit.Framework;
using SharpKml.Base;

namespace NUintTests.Base
{
    [TestFixture]
    public class MathHelpersTest
    {
        // The example from the Aviation Formulary.
        private const double LaxLatitude = 33.944066;
        private const double LaxLongitude = -118.408294;
        private const double JfkLatitude = 40.642480;
        private const double JfkLongitude = -73.788071;

        [Test]
        public void TestAzimuth()
        {
            // Sanity checks.
            Assert.AreEqual(0.0, MathHelpers.Azimuth(0, 0, 1, 0));
            Assert.AreEqual(90.0, MathHelpers.Azimuth(0, 0, 0, 1));
            Assert.AreEqual(180.0, MathHelpers.Azimuth(0, 0, -1, 0));
            Assert.AreEqual(-90.0, MathHelpers.Azimuth(0, 0, 0, -1));
            Assert.AreEqual(-180.0, MathHelpers.Azimuth(0, 0, -1, -0.0000001), 0.0001);
            
            // The known azimuth from LAX to JFK.
            double result = MathHelpers.Azimuth(LaxLatitude, LaxLongitude, JfkLatitude, JfkLongitude);
            Assert.AreEqual(65.8687, result, 0.0001);

            // The return flight.
            result = MathHelpers.Azimuth(JfkLatitude, JfkLongitude, LaxLatitude, LaxLongitude);
            Assert.AreEqual(-86.1617, result, 0.0001);
        }

        [Test]
        public void TestDistance2D()
        {
            // The known great circle distance in meters between LAX and JFK.
            double result = MathHelpers.Distance(LaxLatitude, LaxLongitude, JfkLatitude, JfkLongitude);
            Assert.AreEqual(3970683.0, result, 0.1);
        }

        [Test]
        public void TestDistance3D()
        {
            Assert.AreEqual(0.0, MathHelpers.Distance(0, 0, 0, 0, 0, 0));
            Assert.AreEqual(1.0, MathHelpers.Distance(0, 0, 0, 0, 0, 1));
            Assert.AreEqual(1000.0, MathHelpers.Distance(0, 0, 0, 0, 0, 1000));

            // Assert the 2d example works here.
            double result = MathHelpers.Distance(LaxLatitude, LaxLongitude, 0, JfkLatitude, JfkLongitude, 0);
            Assert.AreEqual(3970683.0, result, 0.1);

            // Put JFK 10,000 km in the sky:
            result = MathHelpers.Distance(LaxLatitude, LaxLongitude, 0, JfkLatitude, JfkLongitude, 1000000.0);
            Assert.AreEqual(4094670.171, result, 0.1);
        }

        [Test]
        public void TestElevation()
        {
            // This is basically a flat line.
            double result = MathHelpers.Elevation(0, 0, 0, 0.0000000000001, 0.0000000000001, 0.0);
            Assert.AreEqual(0.0, result, 0.001);

            // Near-vertical.
            result = MathHelpers.Elevation(0, 0, 0, 0.0000000000001, 0.0000000000001, 10000);
            Assert.AreEqual(90.0, result, 0.001);

            result = MathHelpers.Elevation(0, 0, 0, 0.145, 0.0000000000001, 609.6);
            Assert.AreEqual(2.1667, result, 0.001);

            result = MathHelpers.Elevation(37.0, -121.98, 600, 37.0, -122.0, 200);
            Assert.AreEqual(-12.7004, result, 0.001);
        }

        [Test]
        public void TestRadial()
        {
            // See http://williams.best.vwh.net/avform.htm#Example
            Vector result = MathHelpers.RadialPoint(LaxLatitude, LaxLongitude, 185200.0, 66.0);
            Assert.AreEqual(34.608154, result.Latitude, 0.000001);
            Assert.AreEqual(-116.558327, result.Longitude, 0.000001);
        }


        [Test]
        public void TestGroundDistance()
        {
            // Tests the GroundDistanceFromRangeAndElevation() function.
            double result = MathHelpers.GroundDistance(100.0, 0.0);
            Assert.AreEqual(100.0, result, 0.000001);

            result = MathHelpers.GroundDistance(100.0, 2.0);
            Assert.AreEqual(99.939083, result, 0.000001);

            result = MathHelpers.GroundDistance(100.0, 80.0);
            Assert.AreEqual(17.364818, result, 0.000001);

            result = MathHelpers.GroundDistance(100.0, 90.0);
            Assert.AreEqual(0.0, result, 0.000001);

            result = MathHelpers.GroundDistance(100.0, 100.0);
            Assert.AreEqual(17.364818, result, 0.000001);
        }

        [Test]
        public void TestHeight()
        {
            // Tests the HeightFromRangeAndElevation() function.
            double result = MathHelpers.Height(100.0, 0.0);
            Assert.AreEqual(0.0, result, 0.000001);

            result = MathHelpers.Height(100.0, 2.0);
            Assert.AreEqual(3.489950, result, 0.000001);

            result = MathHelpers.Height(100.0, 80.0);
            Assert.AreEqual(98.480775, result, 0.000001);

            result = MathHelpers.Height(100.0, 90.0);
            Assert.AreEqual(100.0, result, 0.000001);

            result = MathHelpers.Height(100.0, 100.0);
            Assert.AreEqual(98.480775, result, 0.000001);
        }

        // Tese test the conversion functions.
        [Test]
        public void TestDegreesToRadians()
        {
            Assert.AreEqual(0.0, MathHelpers.DegreesToRadians(0.0));
            Assert.AreEqual(Math.PI, MathHelpers.DegreesToRadians(180.0));
            Assert.AreEqual(Math.PI / 2, MathHelpers.DegreesToRadians(90.0));
            Assert.AreEqual(Math.PI / -2, MathHelpers.DegreesToRadians(-90.0));
        }

        [Test]
        public void TestRadiansToDegrees()
        {
            Assert.AreEqual(0.0, MathHelpers.RadiansToDegrees(0));
            Assert.AreEqual(360.0, MathHelpers.RadiansToDegrees(2 * Math.PI));
            Assert.AreEqual(90.0, MathHelpers.RadiansToDegrees(Math.PI / 2));
            Assert.AreEqual(-90.0, MathHelpers.RadiansToDegrees(Math.PI / -2));
        }

        [Test]
        public void TestMetersToRadians()
        {
            Assert.AreEqual(0, MathHelpers.MetersToRadians(0));
            Assert.AreEqual(1, MathHelpers.MetersToRadians(6366710));
        }

        [Test]
        public void TestRadiansToMeters()
        {
            Assert.AreEqual(0, MathHelpers.RadiansToMeters(0));
            Assert.AreEqual(6366710, MathHelpers.RadiansToMeters(1));
        }
    }
}
