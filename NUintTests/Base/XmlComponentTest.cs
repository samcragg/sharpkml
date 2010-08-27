using NUnit.Framework;
using SharpKml.Base;

namespace NUintTests.Base
{
    [TestFixture]
    public class XmlComponentTest
    {
        [Test]
        public void TestClone()
        {
            XmlComponent original = new XmlComponent("pre", "local", "uri");
            XmlComponent clone = original.Clone();
            Assert.AreEqual(original, clone);

            // Make sure changing a property on the clone does not change the original
            original.Value = "Hello";
            clone.Value = "World";
            Assert.AreNotEqual(original.Value, clone.Value);
        }

        [Test]
        public void TestEquals()
        {
            XmlComponent xml1 = new XmlComponent("pre", "local", "uri");
            XmlComponent xml2 = new XmlComponent(null, "local", "uri");
            XmlComponent xml3 = new XmlComponent("pre", "local", null);
            object boxed = xml2;

            Assert.AreEqual(xml1, xml2);
            Assert.AreEqual(xml1, boxed);
            Assert.AreNotEqual(xml1, xml3);
            Assert.AreNotEqual(xml2, xml3);
            Assert.AreNotEqual(xml2, new XmlComponent(null, "Local", "uri")); // Check case sensitivity

            Assert.AreEqual(xml1.GetHashCode(), xml2.GetHashCode());
        }

        [Test]
        public void TestNamespaces()
        {
            XmlComponent xml = new XmlComponent("random", "pre:local", "uri");
            Assert.AreEqual("local", xml.Name);
            Assert.AreEqual("uri", xml.NamespaceUri);
            Assert.AreEqual("pre", xml.Prefix); // Make sure prefix was overloaded

            // Should ignore anything after the second colon
            xml = new XmlComponent(null, "too:many:colons", null);
            Assert.AreEqual("many", xml.Name);
            Assert.AreEqual("too", xml.Prefix);

            // Test a known namespace
            xml = new XmlComponent(null, "gx:known", "ignore me");
            Assert.AreEqual("known", xml.Name);
            Assert.AreEqual(KmlNamespaces.GX22Namespace, xml.NamespaceUri);
            Assert.AreEqual(KmlNamespaces.GX22Prefix, xml.Prefix);
        }
    }
}
