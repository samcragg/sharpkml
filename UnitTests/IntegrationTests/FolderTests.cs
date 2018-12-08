namespace UnitTests.IntegrationTests
{
    using NUnit.Framework;
    using SharpKml.Dom;

    public sealed class FolderTests : DomSerialization
    {
        [Test]
        public void TestSerialization()
        {
            const string Kml =
@"<Folder xmlns=""http://www.opengis.net/kml/2.2"">
  <name>Folder.kml</name>
  <open>true</open>
  <description>
    A folder is a container that can hold multiple other objects
  </description>
  <Placemark>
    <name>Folder object 1 (Placemark)</name>
    <Point>
      <coordinates>-122.5,37.5,0</coordinates>
    </Point>
  </Placemark>
  <Placemark>
    <name>Folder object 2 (Polygon)</name>
    <Polygon>
      <outerBoundaryIs>
        <LinearRing>
          <coordinates>-122.5,37.5,0
-122.5,37.5,0
-122.5,37.5,0
-122.5,37.5,0</coordinates>
        </LinearRing>
      </outerBoundaryIs>
    </Polygon>
  </Placemark>
  <Placemark>
    <name>Folder object 3 (Path)</name>
    <LineString>
      <tessellate>true</tessellate>
      <coordinates>-122.5,37.5,0
-122.5,37.5,0</coordinates>
    </LineString>
  </Placemark>
</Folder>";

            Folder parsed = this.Parse<Folder>(Kml);

            Assert.That(parsed.Name, Is.EqualTo("Folder.kml"));
            Assert.That(parsed.Open, Is.True);
            Assert.That(parsed.Description.Text.Trim(), Is.EqualTo("A folder is a container that can hold multiple other objects"));
            Assert.That(parsed.Features, Has.Count.EqualTo(3));
        }
    }
}
