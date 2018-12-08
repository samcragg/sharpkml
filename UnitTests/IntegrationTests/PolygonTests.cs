namespace UnitTests.IntegrationTests
{
    using System.Linq;
    using NUnit.Framework;
    using SharpKml.Dom;

    public sealed class PolygonTests : DomSerialization
    {
        [Test]
        public void TestSerialization()
        {
            const string Kml =
@"<Polygon xmlns=""http://www.opengis.net/kml/2.2"">
  <extrude>true</extrude>
  <altitudeMode>relativeToGround</altitudeMode>
  <outerBoundaryIs>
    <LinearRing>
      <coordinates>-122.5,37.5,30
-122.5,37.5,30
-122.5,37.5,30
-122.5,37.5,30
-122.5,37.5,30</coordinates>
    </LinearRing>
  </outerBoundaryIs>
  <innerBoundaryIs>
    <LinearRing>
      <coordinates>-122.5,37.5,30
-122.5,37.5,30
-122.5,37.5,30
-122.5,37.5,30
-122.5,37.5,30</coordinates>
    </LinearRing>
  </innerBoundaryIs>
</Polygon>";

            Polygon parsed = this.Parse<Polygon>(Kml);

            Assert.That(parsed.Extrude, Is.True);
            Assert.That(parsed.AltitudeMode, Is.EqualTo(AltitudeMode.RelativeToGround));
            Assert.That(parsed.OuterBoundary.LinearRing.Coordinates, Has.Count.EqualTo(5));
            Assert.That(parsed.InnerBoundary.Single().LinearRing.Coordinates, Has.Count.EqualTo(5));
        }
    }
}
