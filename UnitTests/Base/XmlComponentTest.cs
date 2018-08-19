using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [TestFixture]
    public class XmlComponentTest
    {
        [Test]
        public void CloneShouldReturnANewInstance()
        {
            var original = new XmlComponent("pre", "local", "uri");

            XmlComponent clone = original.Clone();

            Assert.That(clone, Is.Not.SameAs(original));
            Assert.That(clone, Is.EqualTo(original));
        }

        [Test]
        public void EqualsShouldReturnFalseIfTheNamespaceIsDifferent()
        {
            var xml1 = new XmlComponent("pre", "name", "uri");
            var xml2 = new XmlComponent("pre", "name", "other");

            Assert.That(xml1.Equals(xml2), Is.False);
        }

        [Test]
        public void EqualsShouldReturnFalseIfTheNameIsDifferent()
        {
            var xml1 = new XmlComponent("pre", "name", "uri");
            var xml2 = new XmlComponent("pre", "other", "uri");

            Assert.That(xml1.Equals(xml2), Is.False);
        }

        [Test]
        public void EqualsShouldReturnFalseIfTheNameDiffersByCase()
        {
            var xml1 = new XmlComponent("pre", "name", "uri");
            var xml2 = new XmlComponent("pre", "Name", "uri");

            Assert.That(xml1.Equals(xml2), Is.False);
        }

        [Test]
        public void EqualsShouldReturnTrueIfTheNameAndNamespaceAreEqual()
        {
            var xml1 = new XmlComponent("pre", "name", "uri");
            var xml2 = new XmlComponent(null, "name", "uri");

            Assert.That(xml1.Equals(xml2), Is.True);
        }

        [Test]
        public void EqualsShouldReturnTrueIfTheNamespaceIsTheIgnoreNamespace()
        {
            var xml1 = new XmlComponent("x", "name", "uri");
            var xml2 = new XmlComponent("y", "name", Parser.IgnoreNamespace);

            Assert.That(xml1.Equals(xml2), Is.True);
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
            Assert.That(xml.Name, Is.EqualTo("many:colons"));
            Assert.That(xml.Prefix, Is.EqualTo("too"));

            // Test a known namespace
            xml = new XmlComponent(null, "gx:known", "ignore me");
            Assert.That(xml.Name, Is.EqualTo("known"));
            Assert.That(xml.NamespaceUri, Is.EqualTo(KmlNamespaces.GX22Namespace));
            Assert.That(xml.Prefix, Is.EqualTo(KmlNamespaces.GX22Prefix));
        }
    }
}
