namespace UnitTests.Dom
{
    using System.Linq;
    using NUnit.Framework;
    using SharpKml.Base;
    using SharpKml.Dom;

    [TestFixture]
    public sealed class ExtendedDataTest
    {
        [TestCase]
        public void ShouldAllowTheReadingOfCustomData()
        {
            const string Kml = @"<ExtendedData xmlns:prefix='camp'><camp:site number='14'>Name</camp:site></ExtendedData>";
            var parser = new Parser();
            parser.ParseString(Kml, namespaces: false);
            ExtendedData data = (ExtendedData)parser.Root;
            UnknownElement[] customData = data.OtherData.ToArray();

            Assert.That(customData, Has.Length.EqualTo(1));
            Assert.That(customData[0].UnknownData.Name, Is.EqualTo("site"));
            Assert.That(customData[0].UnknownData.Value, Is.EqualTo("Name"));
            Assert.That(customData[0].Attributes.Single().Name, Is.EqualTo("number"));
            Assert.That(customData[0].Attributes.Single().Value, Is.EqualTo("14"));
        }

        [TestCase]
        public void ShouldSaveExistingCustomData()
        {
            const string Kml = @"<ExtendedData xmlns:prefix='csm'><csm:data>Custom Data</csm:data></ExtendedData>";
            var parser = new Parser();
            parser.ParseString(Kml, namespaces: false);
            ExtendedData data = (ExtendedData)parser.Root;

            var serializer = new Serializer();
            serializer.SerializeRaw(data);

            Assert.That(serializer.Xml, Contains.Substring("Custom Data"));
        }
    }
}
