using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Base
{
    [TestFixture]
    public class SerializerTest
    {
        private class ChildElement : Element
        {
            [KmlElement("counter")]
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

        static SerializerTest()
        {
            KmlFactory.Register<ChildElement>(new XmlComponent(null, "ChildElementS", string.Empty));
            KmlFactory.Register<TestElement>(new XmlComponent(null, "TestElementS", string.Empty));
        }

        [Test]
        public void TestAttributes()
        {
            // Try the attributes first
            TestElement element = new TestElement();
            element.Attribute = "attribute";
            element.EnumAtt = ColorMode.Random;

            Serializer serializer = new Serializer();
            serializer.Serialize(element);
            Assert.IsTrue(FindNode(serializer.Xml, "TestElementS", r =>
                {
                    Assert.AreEqual("attribute", r.GetAttribute("Attribute"));
                    Assert.AreEqual("random", r.GetAttribute("EnumAtt"));
                }));


            // Try optional elements = make sure they're only serialized if they have a value
            element.Int = 42;
            serializer.Serialize(element);
            Assert.IsTrue(FindNode(serializer.Xml, "Int", r =>
                Assert.AreEqual(42, r.ReadElementContentAsInt())));
            Assert.IsFalse(FindNode(serializer.Xml, "OptionalInt", null));

            element.OptionalInt = 0;
            serializer.Serialize(element);
            Assert.IsTrue(FindNode(serializer.Xml, "OptionalInt", r =>
                Assert.AreEqual(0, r.ReadElementContentAsInt())));
        }

        [Test]
        public void TestChild()
        {
            TestElement element = new TestElement();
            element.Child = new ChildElement();
            element.Child.Counter = 1;

            Serializer serializer = new Serializer();
            serializer.Serialize(element);
            Assert.IsTrue(FindNode(serializer.Xml, "counter", r =>
                Assert.AreEqual(1, r.ReadElementContentAsInt())));
        }

        [Test]
        public void TestFindNode()
        {
            // Make sure the test methods are reporting correct results...
            const string Xml = "<?xml version='1.0'?><root><empty/><child attribute='text'>value</child></root>";
            
            string value = null;
            Assert.IsTrue(FindNode(Xml, "child", r => value = r.ReadElementContentAsString()));
            Assert.AreEqual("value", value);

            Assert.IsTrue(FindNode(Xml, "empty", null));

            Assert.IsFalse(FindNode(Xml, "invalid", null));
        }

        private static bool FindNode(string xml, string name, Action<XmlReader> callback)
        {
            using (var stringReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stringReader))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) &&
                        (reader.LocalName == name))
                    {
                        if (callback != null)
                        {
                            callback(reader);
                        }
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
