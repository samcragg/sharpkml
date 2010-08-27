﻿using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Dom
{
    [TestFixture]
    public class IHtmlContentTest
    {
        private class TestClass : Element, IHtmlContent
        {
            [KmlAttribute("att")]
            public string Att { get; set; }

            public string Text
            {
                get
                {
                    return this.InnerText;
                }
                set
                {
                    this.ClearInnerText();
                    this.AddInnerText(value);
                }
            }
        }

        static IHtmlContentTest()
        {
            KmlFactory.Register<TestClass>(new XmlComponent(null, "file", "http://example.com"));
        }

        [Test]
        public void TestParse()
        {
            const string xml =
                "<file xmlns=\"http://example.com\" xmlns:x=\"http://example.com\" att=\"value\">" +
                "<root>" +
                "<![CDATA[<>]]><x:child/>" +
                "</root>" +
                "</file>";

            Parser parser = new Parser();
            parser.ParseString(xml, true);

            TestClass root = parser.Root as TestClass;
            Assert.IsNotNull(root);
            Assert.AreEqual("<root><><x:child /></root>", root.Text);
            Assert.AreEqual("value", root.Att);
        }

        [Test]
        public void TestSerialize()
        {
            TestClass element = new TestClass();
            element.Att = "value";
            element.Text = "<root><><x:child /></root>";

            Serializer serializer = new Serializer();
            serializer.SerializeRaw(element);

            const string xml = "<file att=\"value\" xmlns=\"http://example.com\"><![CDATA[<root><><x:child /></root>]]></file>";
            Assert.AreEqual(xml, serializer.Xml);
        }
    }
}
