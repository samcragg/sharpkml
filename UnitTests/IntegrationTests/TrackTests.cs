namespace UnitTests.IntegrationTests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using NUnit.Framework;
    using SharpKml.Base;
    using SharpKml.Dom.GX;

    public sealed class TrackTests : DomSerialization
    {
        [Test]
        public void TestSerialization()
        {
            const string Kml =
@"<gx:Track xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"">
    <when>2010-05-28T02:02:09Z</when>
    <when>2010-05-28T02:02:35Z</when>
    <when>2010-05-28T02:02:44Z</when>
    <when>2010-05-28T02:02:53Z</when>
    <when>2010-05-28T02:02:54Z</when>
    <when>2010-05-28T02:02:55Z</when>
    <when>2010-05-28T02:02:56Z</when>
    <gx:coord>-122.5 37.5 156.5</gx:coord>
    <gx:coord>-122.5 37.5 152.5</gx:coord>
    <gx:coord>-122.5 37.5 147.5</gx:coord>
    <gx:coord>-122.5 37.5 142.5</gx:coord>
    <gx:coord>-122.5 37.5 141.5</gx:coord>
    <gx:coord>-122.5 37.5 141.5</gx:coord>
    <gx:coord>-122.5 37.5 140.5</gx:coord>
</gx:Track>";

            Track parsed = this.Parse<Track>(Kml);

            Assert.That(parsed.When.Count(), Is.EqualTo(7));
            Assert.That(
                parsed.When.First(),
                Is.EqualTo(DateTime.Parse("2010-05-28T02:02:09Z", null, DateTimeStyles.AdjustToUniversal)));

            Assert.That(parsed.Coordinates.Count(), Is.EqualTo(7));
            Vector coordinate = parsed.Coordinates.First();
            Assert.That(coordinate.Longitude, Is.EqualTo(-122.5).Within(0.01));
            Assert.That(coordinate.Latitude, Is.EqualTo(37.5).Within(0.01));
            Assert.That(coordinate.Altitude, Is.EqualTo(156.5).Within(0.01));
        }
    }
}
