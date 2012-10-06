using System;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Base
{
    [TestFixture]
    public class ValueConverterTest
    {
        [Test]
        public void TestBool()
        {
            // The Xml specification states:
            // ·boolean· can have the following legal literals {true, false, 1, 0}.
            object value;
            ValueConverter.TryGetValue(typeof(bool), "true", out value);
            Assert.That(value, Is.True);
            ValueConverter.TryGetValue(typeof(bool), "1", out value);
            Assert.That(value, Is.True);

            ValueConverter.TryGetValue(typeof(bool), "false", out value);
            Assert.That(value, Is.False);
            ValueConverter.TryGetValue(typeof(bool), "0", out value);
            Assert.That(value, Is.False);

            // Invalid data
            ValueConverter.TryGetValue(typeof(bool), "10", out value);
            Assert.That(value, Is.Null);

            ValueConverter.TryGetValue(typeof(bool), string.Empty, out value);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void TestDateTime()
        {
            Tuple<string, DateTime>[] ValidDateTimes =
            {
                Tuple.Create("1997", new DateTime(1997, 1, 1)),
                Tuple.Create("1997-07",  new DateTime(1997, 7, 1)),
                Tuple.Create("1997-07-16", new DateTime(1997, 7, 16)),
                Tuple.Create("1997-07-16T07:30:15Z", new DateTime(1997, 7, 16, 7, 30, 15)),
                Tuple.Create("1997-07-16T10:30:15+03:00", new DateTime(1997, 7, 16, 7, 30, 15)),
                Tuple.Create("1997-07-16T14:30:15Z",  new DateTime(1997, 7, 16, 14, 30, 15)), // Check 24hour value
                Tuple.Create("1997-07-16T07:30:01",  new DateTime(1997, 7, 16, 7, 30, 1)),   // Time without timezone
                Tuple.Create("1997-07-16T07:30:01.01", new DateTime(1997, 7, 16, 7, 30, 1, 10))
            };

            foreach (var date in ValidDateTimes)
            {
                object value;
                ValueConverter.TryGetValue(typeof(DateTime), date.Item1, out value);
                Console.WriteLine(date.Item1);
                DateTime dateTime = (DateTime)value;
                Assert.That(dateTime, Is.EqualTo(date.Item2));
            }
        }

        [Test]
        public void TestTypes()
        {
            // The type converter must be able to parse these types, even if the
            // passed in string is not valid.
            object value;
            Assert.True(ValueConverter.TryGetValue(typeof(AltitudeMode), string.Empty, out value));
            Assert.True(ValueConverter.TryGetValue(typeof(bool), string.Empty, out value));
            Assert.True(ValueConverter.TryGetValue(typeof(Color32), string.Empty, out value));
            Assert.True(ValueConverter.TryGetValue(typeof(DateTime), string.Empty, out value));
            Assert.True(ValueConverter.TryGetValue(typeof(double), string.Empty, out value));
            Assert.True(ValueConverter.TryGetValue(typeof(int), string.Empty, out value));
            Assert.True(ValueConverter.TryGetValue(typeof(string), string.Empty, out value));
            Assert.True(ValueConverter.TryGetValue(typeof(Uri), string.Empty, out value));

            // Make sure it's not always returning true and also that is doesn't
            // throw an exception for unknown types.
            Assert.False(ValueConverter.TryGetValue(this.GetType(), string.Empty, out value));
        }
    }
}
