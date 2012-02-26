using System.Linq;
using NUnit.Framework;
using SharpKml.Base;

namespace UnitTests.Base
{
    [TestFixture]
    public class TypeBrowserTest
    {
        private class BaseClass
        {
            [KmlElement("Base.Public", null)]
            public int BPublic { get; set; }

            [KmlElement("Base.Protected", null)]
            protected int BProtected { get; set; }

            [KmlElement("Base.Private", null)]
            private int BPrivate { get; set; }
        }

        private class DerivedClass : BaseClass
        {
            [KmlElement("Derived.Public", null)]
            public int DPublic { get; set; }

            [KmlElement("Derived.Public", "ns")]
            public int DPublicNs { get; set; }

            [KmlElement("Derived.Private", null)]
            private int DPrivate { get { return 0; } }

            [KmlAttribute("Derived.Attribute")]
            public int DAttribute { get; set; }

            [KmlElement("Derived.Field", null)]
            public int DField = 0; // Shouldn't see this
        }

        [Test]
        public void TestAttributes()
        {
            TypeBrowser browser = TypeBrowser.Create(typeof(DerivedClass));

            // Make sure the namespaces work
            var property = browser.FindAttribute(new XmlComponent(null, "Derived.Attribute", null));
            Assert.That(property.Name, Is.EqualTo("DAttribute"));

            Assert.That(browser.FindAttribute(new XmlComponent(null, "Derived.Public", null)),
                        Is.Null); // This is an element
            Assert.That(browser.FindAttribute(new XmlComponent(null, "Derived.Field", null)),
                        Is.Null); // This isn't a property

            Assert.That(browser.Attributes.Count(), Is.EqualTo(1));

            // Make sure the cache works
            Assert.That(TypeBrowser.Create(typeof(DerivedClass)), Is.SameAs(browser));
            Assert.That(TypeBrowser.Create(typeof(BaseClass)), Is.Not.SameAs(browser));
        }

        [Test]
        public void TestElements()
        {
            TypeBrowser browser = TypeBrowser.Create(typeof(DerivedClass));

            // Check that the base was searched
            var property = browser.FindElement(new XmlComponent(null, "Base.Private", null));
            Assert.That(property.Name, Is.EqualTo("BPrivate"));
            property = browser.FindElement(new XmlComponent(null, "Base.Protected", null));
            Assert.That(property.Name, Is.EqualTo("BProtected"));

            // Check namespaces
            property = browser.FindElement(new XmlComponent(null, "Derived.Public", null));
            Assert.That(property.Name, Is.EqualTo("DPublic"));
            property = browser.FindElement(new XmlComponent(null, "Derived.Public", "ns"));
            Assert.That(property.Name, Is.EqualTo("DPublicNs"));

            // Check a readonly property
            property = browser.FindElement(new XmlComponent(null, "Derived.Private", null));
            Assert.That(property.Name, Is.EqualTo("DPrivate"));

            // Makes sure we don't have any attributes
            Assert.That(browser.FindElement(new XmlComponent(null, "Derived.Attribute", null)),
                        Is.Null);
            Assert.That(browser.FindElement(new XmlComponent(null, "Derived.Field", null)),
                        Is.Null);

            Assert.That(browser.Elements.Count(), Is.EqualTo(6));
        }
    }
}
