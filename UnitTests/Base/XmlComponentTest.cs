using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [TestFixture]
    public class XmlComponentTest
    {
        [Test]
        public void TestClone()
        {
            XmlComponent original = new XmlComponent("pre", "local", "uri");
            XmlComponent clone = original.Clone();
            Assert.That(clone, Is.EqualTo(original));

            // Make sure changing a property on the clone does not change the original
            original.Value = "Hello";
            clone.Value = "World";
            Assert.That(clone.Value, Is.Not.EqualTo(original.Value));
        }

        [Test]
        public void TestEquals()
        {
            XmlComponent xml1 = new XmlComponent("pre", "local", "uri");
            XmlComponent xml2 = new XmlComponent(null, "local", "uri");
            XmlComponent xml3 = new XmlComponent("pre", "local", null);
            object boxed = xml2;

            Assert.That(xml1, Is.EqualTo(xml2));
            Assert.That(xml1, Is.EqualTo(boxed));
            Assert.That(xml1, Is.Not.EqualTo(xml3));
            Assert.That(xml2, Is.Not.EqualTo(xml3));
            Assert.That(xml2, Is.Not.EqualTo(new XmlComponent(null, "Local", "uri"))); // Check case sensitivity

            Assert.That(xml1.GetHashCode(), Is.EqualTo(xml2.GetHashCode()));
        }

        [Test]
        public void TestNamespaces()
        {
            XmlComponent xml = new XmlComponent("random", "pre:local", "uri");
            Assert.That(xml.Name, Is.EqualTo("local"));
            Assert.That(xml.NamespaceUri, Is.EqualTo("uri"));
            Assert.That(xml.Prefix, Is.EqualTo("pre")); // Make sure prefix was overloaded

            // Should ignore anything after the second colon
            xml = new XmlComponent(null, "too:many:colons", null);
            Assert.That(xml.Name, Is.EqualTo("many"));
            Assert.That(xml.Prefix, Is.EqualTo("too"));

            // Test a known namespace
            xml = new XmlComponent(null, "gx:known", "ignore me");
            Assert.That(xml.Name, Is.EqualTo("known"));
            Assert.That(xml.NamespaceUri, Is.EqualTo(KmlNamespaces.GX22Namespace));
            Assert.That(xml.Prefix, Is.EqualTo(KmlNamespaces.GX22Prefix));
        }
    }
}
