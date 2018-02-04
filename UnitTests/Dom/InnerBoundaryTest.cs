using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Dom
{
    [TestFixture]
    public class InnerBoundaryTest
    {
        [Test]
        public void ShouldCreateANewInnerBoundaryInTheParentForMultipleLinearRings()
        {
            Vector GetCoordinate(InnerBoundary boundary)
            {
                return boundary.LinearRing.Coordinates.Single();
            }

            const string Kml = @"<Polygon>
  <innerBoundaryIs>
    <LinearRing>
      <coordinates>1,2</coordinates>
    </LinearRing>
    <LinearRing>
      <coordinates>3,4</coordinates>
    </LinearRing>
  </innerBoundaryIs>
</Polygon>";

            var parser = new Parser();
            parser.ParseString(Kml, namespaces: false);
            var polygon = (Polygon)parser.Root;

            Assert.That(polygon.InnerBoundary.Count(), Is.EqualTo(2));

            Assert.That(
                GetCoordinate(polygon.InnerBoundary.ElementAt(0)),
                Is.EqualTo(new Vector(2, 1)));

            Assert.That(
                GetCoordinate(polygon.InnerBoundary.ElementAt(1)),
                Is.EqualTo(new Vector(4, 3)));
        }
    }
}
