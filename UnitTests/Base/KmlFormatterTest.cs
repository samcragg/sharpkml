using System;
using System.Globalization;
using System.Threading;
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
        public void TestLocalDateTimeInOtherCulture()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("he-IL");
                var date = new DateTime(2012, 11, 10, 9, 8, 7, DateTimeKind.Local);
                TestDateTime(date);
            }
            catch (ArgumentException)
            {
                throw new InconclusiveException("Culture not available.");
            }
            catch (NotSupportedException)
            {
                throw new InconclusiveException("Culture not available.");
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
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
