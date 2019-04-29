using System;
using System.Globalization;
using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [SetCulture("DE-de")]
    [TestFixture]
    public class KmlFormatterTest
    {
        [Test]
        public void TestLocalDateTime()
        {
            var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Local);
            this.TestDateTime(date);
        }

        [Test]
        public void TestUnspecifiedDateTime()
        {
            var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Unspecified);
            this.TestDateTime(date);
        }

        [Test]
        public void TestUtcDateTime()
        {
            var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Utc);
            this.TestDateTime(date);
        }

        [TestCase(double.NegativeInfinity, ExpectedResult = "-INF")]
        [TestCase(double.NaN, ExpectedResult = "NaN")]
        [TestCase(double.PositiveInfinity, ExpectedResult = "INF")]
        [TestCase(1.17, ExpectedResult = "1.1699999999999999")]
        public string ShouldOutputSpecialDoublesCorrectly(double value)
        {
            return string.Format(KmlFormatter.Instance, "{0}", value);
        }

        [TestCase(1.17, ExpectedResult = "1.17")]
        public string OutputShouldMatchInput(double value)
        {
            return string.Format(KmlFormatter.Instance, "{0:G}", value);
        }

        private void TestDateTime(DateTime date)
        {
            string formatted = string.Format(KmlFormatter.Instance, "{0}", date);

            var parsed = DateTime.Parse(
                formatted,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal);

            Assert.That(parsed, Is.EqualTo(date.ToUniversalTime()));
        }
    }
}
