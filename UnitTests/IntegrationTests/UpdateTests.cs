namespace UnitTests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using SharpKml.Dom;

    public sealed class UpdateTests : DomSerialization
    {
        [Test]
        public void TestSerialization()
        {
            const string Kml =
@"<Update xmlns=""http://www.opengis.net/kml/2.2"">
  <targetHref>http://myserver.com/Point.kml</targetHref>
  <Create>
    <Document targetId=""region24"">
      <Placemark id=""placemark891"">
        <Point>
          <coordinates>-95.5,40.43,0</coordinates>
        </Point>
      </Placemark>
    </Document>
  </Create>
  <Delete>
    <Placemark targetId=""pa3556"" />
  </Delete>
  <Change>
    <Point targetId=""point123"">
      <coordinates>-95.5,40.43,0</coordinates>
    </Point>
  </Change>
</Update>";
            Update parsed = this.Parse<Update>(Kml);

            Assert.That(parsed.Target, Is.EqualTo(new Uri("http://myserver.com/Point.kml")));
            Assert.That(parsed.Updates, Has.Count.EqualTo(3));

            AssertCollectionOfType<Document>("region24", parsed.Updates.OfType<CreateCollection>().Single());
            AssertCollectionOfType<Placemark>("pa3556", parsed.Updates.OfType<DeleteCollection>().Single());
            AssertCollectionOfType<Point>("point123", parsed.Updates.OfType<ChangeCollection>().Single());
        }

        private static void AssertCollectionOfType<T>(string targetId, IReadOnlyCollection<KmlObject> collection)
        {
            KmlObject updateTarget = collection.Single();
            Assert.That(updateTarget, Is.TypeOf<T>());
            Assert.That(updateTarget.TargetId, Is.EqualTo(targetId));
        }
    }
}
