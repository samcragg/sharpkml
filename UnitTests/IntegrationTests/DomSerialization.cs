namespace UnitTests.IntegrationTests
{
    using System.Xml.Linq;
    using NUnit.Framework;
    using SharpKml.Base;
    using SharpKml.Dom;

    [Category("Integration")]
    public abstract class DomSerialization
    {
        protected T Parse<T>(string kml) where T : Element
        {
            var parser = new Parser();
            parser.ParseString(kml, namespaces: true);
            AssertElementSerializesCorrectly(kml, parser.Root);
            return (T)parser.Root;
        }

        private static void AssertElementSerializesCorrectly(string kml, Element element)
        {
            var serializer = new Serializer();
            serializer.Serialize(element);

            XDocument original = XmlTestHelper.Normalize(XDocument.Parse(kml));
            XDocument serialized = XmlTestHelper.Normalize(XDocument.Parse(serializer.Xml));

            Assert.That(
                XNode.DeepEquals(original, serialized),
                Is.True,
                () => "Expected:\r\n" + serialized.ToString() + "\r\nTo equal:\r\n" + original.ToString());
        }
    }
}
