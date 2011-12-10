using System;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Base
{
    [TestFixture]
    public class ParserTest
    {
        private class ChildElement : Element
        {
            [KmlAttribute("counter")]
            public int Counter { get; set; }
        }

        private class TestElement : Element
        {
            private ChildElement _child;

            [KmlAttribute("Attribute")]
            public string Attribute { get; set; }

            [KmlAttribute("EnumAtt")]
            public ColorMode? EnumAtt { get; set; }

            // Also test that NamespaceUri doesn't matter if its string.Empty or null
            [KmlElement("Enum", "")]
            public ColorMode Enum { get; set; }

            [KmlElement("Int", null)]
            public int Int { get; set; }

            [KmlElement("OptionalInt", null)]
            public int? OptionalInt { get; set; }

            [KmlElement("WebAddress", null)]
            public Uri Uri { get; set; }

            [KmlElement(null)]
            public ChildElement Child
            {
                get { return _child; }
                set { this.UpdatePropertyChild(value, ref _child); }
            }
        }

        static ParserTest()
        {
            KmlFactory.Register<ChildElement>(new XmlComponent(null, "ChildElement", string.Empty));
            KmlFactory.Register<TestElement>(new XmlComponent(null, "TestElement", string.Empty));
        }

        [Test]
        public void TestAttributes()
        {
            const string Xml = @"
<TestElement Attribute=""string"" EnumAtt=""random"">
  <WebAddress>
    http://www.example.com
  </WebAddress>
  <Int>1</Int>
  <OptionalInt>
    2
  </OptionalInt>
  <Enum>random</Enum>
</TestElement>";

            Parser parser = new Parser();
            parser.ParseString(Xml, true);

            TestElement element = parser.Root as TestElement;
            Assert.IsNotNull(element);

            Assert.AreEqual("string", element.Attribute);
            Assert.AreEqual(ColorMode.Random, element.Enum);
            Assert.AreEqual(ColorMode.Random, element.EnumAtt);
            Assert.AreEqual(1, element.Int);
            Assert.AreEqual(2, element.OptionalInt);
            Assert.AreEqual(new Uri("http://www.example.com"), element.Uri);
        }

        [Test]
        public void TestInvalidValuesDefault()
        {
            const string Xml = @"
<TestElement>
  <Int>not a number</Int>
  <OptionalInt>not a number</OptionalInt>
  <Enum>not a value</Enum>
</TestElement>";

            Parser parser = new Parser();
            parser.ParseString(Xml, true);

            TestElement element = parser.Root as TestElement;
            Assert.IsNotNull(element);

            Assert.IsNull(element.Attribute);
            Assert.AreEqual(ColorMode.Normal, element.Enum);
            Assert.AreEqual(0, element.Int);
            Assert.IsNull(element.OptionalInt);
        }

        [Test]
        public void TestNamespaces()
        {
            const string withNs = @"
<LatLonAltBox xmlns=""http://www.opengis.net/kml/2.2"">
  <gx:altitudeMode xmlns:gx=""http://www.google.com/kml/ext/2.2"">
    relativeToSeaFloor
  </gx:altitudeMode>
</LatLonAltBox>";

            const string withoutNs = @"
<LatLonAltBox>
  <gx:altitudeMode>
    relativeToSeaFloor
  </gx:altitudeMode>
</LatLonAltBox>";

            // Test ParseString with a namespace but without namespace checking
            Parser parser = new Parser();
            parser.ParseString(withNs, false);

            LatLonAltBox box = parser.Root as LatLonAltBox;
            Assert.IsNotNull(box);
            Assert.AreEqual(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor, box.GXAltitudeMode);

            // Now without
            parser.ParseString(withoutNs, false);
            box = (LatLonAltBox)parser.Root;
            Assert.AreEqual(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor, box.GXAltitudeMode);

            // Test ParseString with a namespace and with namespace checking
            parser.ParseString(withNs, true);
            box = parser.Root as LatLonAltBox;

            Assert.IsNotNull(box);
            Assert.AreEqual(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor, box.GXAltitudeMode);

            // Now without, which shouldn't work as gx is unknown
            Assert.Catch<XmlException>(() => parser.ParseString(withoutNs, true));
        }

        [Test]
        public void TestChild()
        {
            const string ChildXml = "<ChildElement counter=\"1\" />";
            const string TestXml =
                "<TestElement>" +
                "<Enum>normal</Enum>" +
                "<Int>10</Int>" +
                ChildXml +
                "</TestElement>";

            Parser parser = new Parser();
            parser.ParseString(ChildXml, true);
            Assert.IsInstanceOf(typeof(ChildElement), parser.Root);

            parser.ParseString(TestXml, true);
            TestElement element = parser.Root as TestElement;
            Assert.IsNotNull(element);

            Assert.AreEqual(10, element.Int);
            Assert.AreEqual(1, element.Child.Counter);
        }

        [Test]
        public void TestAtom()
        {
            var parser = new Parser();
            parser.ParseString("<feed xmlns='http://www.w3.org/2005/Atom'/>", true);
            var feed = parser.Root as SharpKml.Dom.Atom.Feed;
            Assert.IsNotNull(feed);

            parser.ParseString(
                "<atom:content xmlns:atom='http://www.w3.org/2005/Atom'" +
                " xmlns='http://www.opengis.net/kml/2.2'>" +
                "<Placemark id='pm0'/>" +
                "</atom:content>",
                true);

            var content = parser.Root as SharpKml.Dom.Atom.Content;
            Assert.IsNotNull(content);
            Assert.AreEqual(1, content.Orphans.Count());
            var placemark = content.Orphans.ElementAt(0) as Placemark;
            Assert.IsNotNull(placemark);
            Assert.AreEqual("pm0", placemark.Id);
        }

        [Test]
        public void TestInvalidKml()
        {
            var parser = new Parser();
            parser.ParseString("<gml><this>is not<kml/></this>is also not</gml>", false);
            Assert.IsNull(parser.Root);

            parser.ParseString("<gml><Placemark><name>still not kml</name></Placemark></gml>", false);
            Assert.IsNull(parser.Root);
        }

        [Test]
        public void TestUnkownElement()
        {
            var parser = new Parser();
            parser.ParseString("<kml><a>b<c></c></a></kml>", false);

            var kml = parser.Root as Kml;
            Assert.IsNotNull(kml);
            Assert.AreEqual(1, kml.Orphans.Count());

            var unknown = kml.Orphans.ElementAt(0) as UnknownElement;
            Assert.IsNotNull(unknown);
            Assert.AreEqual("b", unknown.InnerText); // Other elements will be children
        }

        [Test]
        public void TestValidKml()
        {
            const string Xml =
                "<kml xmlns=\"http://www.opengis.net/kml/2.2\">" +
                "<Placemark>" +
                "<name>a good Placemark</name>" +
                "<Point>" +
                "<coordinates>1,2,3</coordinates>" +
                "</Point>" +
                "</Placemark>" +
                "</kml>";

            var parser = new Parser();
            parser.ParseString(Xml, true);
            Assert.IsNotNull(parser.Root);
            Assert.IsInstanceOf<Kml>(parser.Root);

            // Test empty but valid Kml
            parser.ParseString("<kml />", false);
            Assert.IsNotNull(parser.Root);
            Assert.IsInstanceOf<Kml>(parser.Root);
        }

        [Test]
        public void TestLegacyKml()
        {
            const string Xml =
                "<kml xmlns=\"http://earth.google.com/kml/2.2\">" +
                  "<Placemark>" +
                    "<name>My Placemark</name>" +
                  "</Placemark>" +
                "</kml>";

            var parser = new Parser();
            parser.ParseString(Xml, false);

            Kml root = parser.Root as Kml;
            Assert.IsNotNull(root);

            // Make sure it didn't add the old namespace
            Assert.IsFalse(root.Namespaces.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml).ContainsKey(string.Empty));

            // Make sure it serializes
            Serializer serializer = new Serializer();
            Assert.DoesNotThrow(() => serializer.Serialize(root));
            Assert.IsNotNullOrEmpty(serializer.Xml);
        }

        [Test]
        public void TestEmptyElement()
        {
            const string xml = @"<?xml version='1.0' encoding='UTF-8'?>
<kml xmlns='http://www.opengis.net/kml/2.2'>
  <Document>
    <Snippet/>
    <name>My Document</name>
    <Placemark>
        <name>My Placemark</name>
    </Placemark>
  </Document>
</kml>";

            var parser = new Parser();
            parser.ParseString(xml, true);

            Kml kml = parser.Root as Kml;
            Assert.IsTrue(kml != null);

            Document document = kml.Feature as Document;
            Assert.IsTrue(document != null);
            Assert.AreEqual("My Document", document.Name);

            Placemark placemark = document.Features.FirstOrDefault() as Placemark;
            Assert.IsTrue(placemark != null);
            Assert.AreEqual("My Placemark", placemark.Name);
        }
    }
}
