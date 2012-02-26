using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Base
{
    [TestFixture]
    public class ParserTest
    {
        public class ChildElement : Element
        {
            [KmlAttribute("counter")]
            public int Counter { get; set; }
        }

        public class TestElement : Element
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

        public class DoubleElement : Element
        {
            [KmlElement("Double", null)]
            public double Double { get; set; }
        }

        static ParserTest()
        {
            KmlFactory.Register<ChildElement>(new XmlComponent(null, "ChildElement", string.Empty));
            KmlFactory.Register<TestElement>(new XmlComponent(null, "TestElement", string.Empty));
            KmlFactory.Register<DoubleElement>(new XmlComponent(null, "DoubleElement", string.Empty));
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
            Assert.That(element, Is.Not.Null);

            Assert.That(element.Attribute, Is.EqualTo("string"));
            Assert.That(element.Enum, Is.EqualTo(ColorMode.Random));
            Assert.That(element.EnumAtt, Is.EqualTo(ColorMode.Random));
            Assert.That(element.Int, Is.EqualTo(1));
            Assert.That(element.OptionalInt, Is.EqualTo(2));
            Assert.That(element.Uri, Is.EqualTo(new Uri("http://www.example.com")));
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
            Assert.That(element, Is.Not.Null);

            Assert.That(element.Attribute, Is.Null);
            Assert.That(element.Enum, Is.EqualTo(ColorMode.Normal));
            Assert.That(element.Int, Is.EqualTo(0));
            Assert.That(element.OptionalInt, Is.Null);
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
            Assert.That(box, Is.Not.Null);
            Assert.That(box.GXAltitudeMode, Is.EqualTo(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor));

            // Now without
            parser.ParseString(withoutNs, false);
            box = (LatLonAltBox)parser.Root;
            Assert.That(box.GXAltitudeMode, Is.EqualTo(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor));

            // Test ParseString with a namespace and with namespace checking
            parser.ParseString(withNs, true);
            box = parser.Root as LatLonAltBox;

            Assert.That(box, Is.Not.Null);
            Assert.That(box.GXAltitudeMode, Is.EqualTo(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor));

            // Now without, which shouldn't work as gx is unknown
            Assert.That(() => parser.ParseString(withoutNs, true),
                        Throws.TypeOf<XmlException>());
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
            Assert.That(parser.Root, Is.InstanceOf<ChildElement>());

            parser.ParseString(TestXml, true);
            TestElement element = parser.Root as TestElement;
            Assert.That(element, Is.Not.Null);

            Assert.That(element.Int, Is.EqualTo(10));
            Assert.That(element.Child.Counter, Is.EqualTo(1));
        }

        [Test]
        public void TestAtom()
        {
            var parser = new Parser();
            parser.ParseString("<feed xmlns='http://www.w3.org/2005/Atom'/>", true);
            var feed = parser.Root as SharpKml.Dom.Atom.Feed;
            Assert.That(feed, Is.Not.Null);

            parser.ParseString(
                "<atom:content xmlns:atom='http://www.w3.org/2005/Atom'" +
                " xmlns='http://www.opengis.net/kml/2.2'>" +
                "<Placemark id='pm0'/>" +
                "</atom:content>",
                true);

            var content = parser.Root as SharpKml.Dom.Atom.Content;
            Assert.That(content, Is.Not.Null);
            Assert.That(content.Orphans.Count(), Is.EqualTo(1));
            var placemark = content.Orphans.ElementAt(0) as Placemark;
            Assert.That(placemark, Is.Not.Null);
            Assert.That(placemark.Id, Is.EqualTo("pm0"));
        }

        [Test]
        public void TestInvalidKml()
        {
            var parser = new Parser();
            parser.ParseString("<gml><this>is not<kml/></this>is also not</gml>", false);
            Assert.That(parser.Root, Is.Null);

            parser.ParseString("<gml><Placemark><name>still not kml</name></Placemark></gml>", false);
            Assert.That(parser.Root, Is.Null);
        }

        [Test]
        public void TestUnkownElement()
        {
            var parser = new Parser();
            parser.ParseString("<kml><a>b<c></c></a></kml>", false);

            var kml = parser.Root as Kml;
            Assert.That(kml, Is.Not.Null);
            Assert.That(kml.Orphans.Count(), Is.EqualTo(1));

            var unknown = kml.Orphans.ElementAt(0) as UnknownElement;
            Assert.That(unknown, Is.Not.Null);
            Assert.That(unknown.InnerText, Is.EqualTo("b")); // Other elements will be children
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
            Assert.That(parser.Root, Is.Not.Null);
            Assert.That(parser.Root, Is.InstanceOf<Kml>());

            // Test empty but valid Kml
            parser.ParseString("<kml />", false);
            Assert.That(parser.Root, Is.Not.Null);
            Assert.That(parser.Root, Is.InstanceOf<Kml>());
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
            Assert.That(root, Is.Not.Null);

            // Make sure it didn't add the old namespace
            Assert.That(root.Namespaces.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml),
                        Is.Not.Contains(string.Empty));

            // Make sure it serializes
            Serializer serializer = new Serializer();
            Assert.That(() => serializer.Serialize(root), Throws.Nothing);
            Assert.That(serializer.Xml, Is.Not.Null.Or.Empty);
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
            Assert.That(kml, Is.Not.Null);

            Document document = kml.Feature as Document;
            Assert.That(document, Is.Not.Null);
            Assert.That(document.Name, Is.EqualTo("My Document"));

            Placemark placemark = document.Features.FirstOrDefault() as Placemark;
            Assert.That(placemark, Is.Not.Null);
            Assert.That(placemark.Name, Is.EqualTo("My Placemark"));
        }

        [Test]
        public void TestCulturalSettings()
        {
            const string xml = "<DoubleElement><Double>12.34</Double></DoubleElement>";

            var parser = new Parser();
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                parser.ParseString(xml, true);
                Assert.That(((DoubleElement)parser.Root).Double, Is.EqualTo(12.34));

                Thread.CurrentThread.CurrentCulture = new CultureInfo("de");
                parser.ParseString(xml, true);
                Assert.That(((DoubleElement)parser.Root).Double, Is.EqualTo(12.34));
            }
            catch (ArgumentException) // Culture doesn't exist
            {
                throw new InconclusiveException("German culture not available.");
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }
    }
}
