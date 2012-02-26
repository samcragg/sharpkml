using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [TestFixture]
    public class XmlExtractorTest
    {
        [Test]
        public void TestCData()
        {
            const string child = "<![CDATA[<>]]><x:child/>";
            const string xml =
                "<file xmlns=\"http://example.com\" xmlns:x=\"http://example.com\">" +
                "<root>" +
                child +
                "</root>" +
                "</file>";

            using (StringReader input = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(input))
            {
                Assert.True(FindRootNode(reader));
                Assert.That(XmlExtractor.FlattenXml(reader), Is.EqualTo("<><x:child />"));
            }
        }

        [Test]
        public void TestNamespaces()
        {
            const string child = "Root <x:child att=\"hi\">Child Text <empty /></x:child> Text";
            const string xml =
                "<file xmlns=\"http://example.com\" xmlns:x=\"http://example.com\">" +
                "<root>" +
                child +
                "</root>" +
                "</file>";

            using (StringReader input = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(input))
            {
                Assert.True(FindRootNode(reader));
                Assert.That(XmlExtractor.FlattenXml(reader), Is.EqualTo(child));

                // Make sure it advanced past the last child part
                Assert.That(reader.NodeType, Is.EqualTo(XmlNodeType.EndElement));
                Assert.That(reader.Name, Is.EqualTo("root"));
            }
        }

        private static bool FindRootNode(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&
                    string.Equals(reader.Name, "root", StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
