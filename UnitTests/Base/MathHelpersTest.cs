using System;
using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
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
            Assert.That(MathHelpers.Azimuth(0, 0, 1, 0), Is.EqualTo(0.0));
            Assert.That(MathHelpers.Azimuth(0, 0, 0, 1), Is.EqualTo(90.0));
            Assert.That(MathHelpers.Azimuth(0, 0, -1, 0), Is.EqualTo(180.0));
            Assert.That(MathHelpers.Azimuth(0, 0, 0, -1), Is.EqualTo(-90.0));
            Assert.That(MathHelpers.Azimuth(0, 0, -1, -0.0000001), Is.EqualTo(-180.0).Within(0.0001));

            // The known azimuth from LAX to JFK.
            double result = MathHelpers.Azimuth(LaxLatitude, LaxLongitude, JfkLatitude, JfkLongitude);
            Assert.That(result, Is.EqualTo(65.8687).Within(0.0001));

            // The return flight.
            result = MathHelpers.Azimuth(JfkLatitude, JfkLongitude, LaxLatitude, LaxLongitude);
            Assert.That(result, Is.EqualTo(-86.1617).Within(0.0001));
        }

        [Test]
        public void TestDistance2D()
        {
            // The known great circle distance in meters between LAX and JFK.
            double result = MathHelpers.Distance(LaxLatitude, LaxLongitude, JfkLatitude, JfkLongitude);
            Assert.That(result, Is.EqualTo(3970683.0).Within(0.1));
        }

        [Test]
        public void TestDistance3D()
        {
            Assert.That(MathHelpers.Distance(0, 0, 0, 0, 0, 0), Is.EqualTo(0.0));
            Assert.That(MathHelpers.Distance(0, 0, 0, 0, 0, 1), Is.EqualTo(1.0));
            Assert.That(MathHelpers.Distance(0, 0, 0, 0, 0, 1000), Is.EqualTo(1000.0));

            // Assert the 2d example works here.
            double result = MathHelpers.Distance(LaxLatitude, LaxLongitude, 0, JfkLatitude, JfkLongitude, 0);
            Assert.That(result, Is.EqualTo(3970683.0).Within(0.1));

            // Put JFK 10,000 km in the sky:
            result = MathHelpers.Distance(LaxLatitude, LaxLongitude, 0, JfkLatitude, JfkLongitude, 1000000.0);
            Assert.That(result, Is.EqualTo(4094670.171).Within(0.1));
        }

        [Test]
        public void TestElevation()
        {
            // This is basically a flat line.
            double result = MathHelpers.Elevation(0, 0, 0, 0.0000000000001, 0.0000000000001, 0.0);
            Assert.That(result, Is.EqualTo(0.0).Within(0.001));

            // Near-vertical.
            result = MathHelpers.Elevation(0, 0, 0, 0.0000000000001, 0.0000000000001, 10000);
            Assert.That(result, Is.EqualTo(90.0).Within(0.001));

            result = MathHelpers.Elevation(0, 0, 0, 0.145, 0.0000000000001, 609.6);
            Assert.That(result, Is.EqualTo(2.1667).Within(0.001));

            result = MathHelpers.Elevation(37.0, -121.98, 600, 37.0, -122.0, 200);
            Assert.That(result, Is.EqualTo(-12.7004).Within(0.001));
        }

        [Test]
        public void TestRadial()
        {
            // See http://williams.best.vwh.net/avform.htm#Example
            Vector result = MathHelpers.RadialPoint(LaxLatitude, LaxLongitude, 185200.0, 66.0);
            Assert.That(result.Latitude, Is.EqualTo(34.608154).Within(0.000001));
            Assert.That(result.Longitude, Is.EqualTo(-116.558327).Within(0.000001));
        }

        [Test]
        public void TestGroundDistance()
        {
            // Tests the GroundDistanceFromRangeAndElevation() function.
            double result = MathHelpers.GroundDistance(100.0, 0.0);
            Assert.That(result, Is.EqualTo(100.0).Within(0.000001));

            result = MathHelpers.GroundDistance(100.0, 2.0);
            Assert.That(result, Is.EqualTo(99.939083).Within(0.000001));

            result = MathHelpers.GroundDistance(100.0, 80.0);
            Assert.That(result, Is.EqualTo(17.364818).Within(0.000001));

            result = MathHelpers.GroundDistance(100.0, 90.0);
            Assert.That(result, Is.EqualTo(0.0).Within(0.000001));

            result = MathHelpers.GroundDistance(100.0, 100.0);
            Assert.That(result, Is.EqualTo(17.364818).Within(0.000001));
        }

        [Test]
        public void TestHeight()
        {
            // Tests the HeightFromRangeAndElevation() function.
            double result = MathHelpers.Height(100.0, 0.0);
            Assert.That(result, Is.EqualTo(0.0).Within(0.000001));

            result = MathHelpers.Height(100.0, 2.0);
            Assert.That(result, Is.EqualTo(3.489950).Within(0.000001));

            result = MathHelpers.Height(100.0, 80.0);
            Assert.That(result, Is.EqualTo(98.480775).Within(0.000001));

            result = MathHelpers.Height(100.0, 90.0);
            Assert.That(result, Is.EqualTo(100.0).Within(0.000001));

            result = MathHelpers.Height(100.0, 100.0);
            Assert.That(result, Is.EqualTo(98.480775).Within(0.000001));
        }

        // Tese test the conversion functions.
        [Test]
        public void TestDegreesToRadians()
        {
            Assert.That(MathHelpers.DegreesToRadians(0.0), Is.EqualTo(0.0));
            Assert.That(MathHelpers.DegreesToRadians(180.0), Is.EqualTo(Math.PI));
            Assert.That(MathHelpers.DegreesToRadians(90.0), Is.EqualTo(Math.PI / 2));
            Assert.That(MathHelpers.DegreesToRadians(-90.0), Is.EqualTo(Math.PI / -2));
        }

        [Test]
        public void TestRadiansToDegrees()
        {
            Assert.That(MathHelpers.RadiansToDegrees(0), Is.EqualTo(0.0));
            Assert.That(MathHelpers.RadiansToDegrees(2 * Math.PI), Is.EqualTo(360.0));
            Assert.That(MathHelpers.RadiansToDegrees(Math.PI / 2), Is.EqualTo(90.0));
            Assert.That(MathHelpers.RadiansToDegrees(Math.PI / -2), Is.EqualTo(-90.0));
        }

        [Test]
        public void TestMetersToRadians()
        {
            Assert.That(MathHelpers.MetersToRadians(0), Is.EqualTo(0.0));
            Assert.That(MathHelpers.MetersToRadians(6366710), Is.EqualTo(1.0));
        }

        [Test]
        public void TestRadiansToMeters()
        {
            Assert.That(MathHelpers.RadiansToMeters(0), Is.EqualTo(0.0));
            Assert.That(MathHelpers.RadiansToMeters(1), Is.EqualTo(6366710.0));
        }
    }
}
