namespace UnitTests.IntegrationTests
{
    using System;
    using NUnit.Framework;
    using SharpKml.Dom;

    public sealed class ItemIconTests : DomSerialization
    {
        [Test]
        public void TestSerialization()
        {
            const string Kml =
@"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2"">
    <state>closed error</state>
    <href>http://www.example.com/</href>
  </ItemIcon>";

            ItemIcon parsed = this.Parse<ItemIcon>(Kml);

            Assert.That(parsed.State, Is.EqualTo(ItemIconStates.Closed | ItemIconStates.Error));
            Assert.That(parsed.Href, Is.EqualTo(new Uri("http://www.example.com")));
        }
    }
}
