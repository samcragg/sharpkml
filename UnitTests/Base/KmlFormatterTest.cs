using System;
using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [TestFixture]
    public class KmlFormatterTest
    {
        [Test]
        public void TestLocalDateTime()
        {
            var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Local);
            TestDateTime(date);
        }

        [Test]
        public void TestUnspecifiedDateTime()
        {
            var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Unspecified);
            TestDateTime(date);
        }

        [Test]
        public void TestUtcDateTime()
        {
            var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Utc);
            TestDateTime(date);
        }

        [Test]
        [SetCulture("he-IL")]
        public void TestLocalDateTimeInOtherCulture()
        {
            var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Local);
            TestDateTime(date);
        }

        [TestCase(double.NegativeInfinity, ExpectedResult = "-INF")]
        [TestCase(double.NaN, ExpectedResult = "NaN")]
        [TestCase(double.PositiveInfinity, ExpectedResult = "INF")]
        public string ShouldOutputSpecialDoublesCorrectly(double value)
        {
            return string.Format(KmlFormatter.Instance, "{0}", value);
        }

        private void TestDateTime(DateTime date)
        {
            string formatted = null;
            Assert.That(() => formatted = string.Format(KmlFormatter.Instance, "{0}", date), Throws.Nothing);

            object parsed;
            ValueConverter.TryGetValue(typeof(DateTime), formatted, out parsed);
            Assert.That(parsed, Is.EqualTo(date));
        }
    }
}
