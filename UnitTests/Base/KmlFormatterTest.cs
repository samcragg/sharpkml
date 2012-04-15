using System;
using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [TestFixture]
    public class KmlFormatterTest
    {
        [Test]
        public void TestDateTimeKind()
        {
            DateTime baseDate = new DateTime(2012, 11, 10, 9, 8, 7);
            TestDateTime(DateTime.SpecifyKind(baseDate, DateTimeKind.Local));
            TestDateTime(DateTime.SpecifyKind(baseDate, DateTimeKind.Unspecified));
            TestDateTime(DateTime.SpecifyKind(baseDate, DateTimeKind.Utc));
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
