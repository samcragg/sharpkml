using NUnit.Framework;
using SharpKml.Base;

namespace NUintTests.Base
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
            Assert.AreEqual("DAttribute", property.Name);

            Assert.IsNull(browser.FindAttribute(new XmlComponent(null, "Derived.Public", null))); // this is an element
            Assert.IsNull(browser.FindAttribute(new XmlComponent(null, "Derived.Field", null))); // this isn't a property

            int counter = 0;
            foreach (var prop in browser.Attributes)
            {
                counter++;
            }
            Assert.AreEqual(1, counter);

            // Make sure the cache works
            Assert.AreSame(browser, TypeBrowser.Create(typeof(DerivedClass)));
            Assert.AreNotSame(browser, TypeBrowser.Create(typeof(BaseClass)));
        }

        [Test]
        public void TestElements()
        {
            TypeBrowser browser = TypeBrowser.Create(typeof(DerivedClass));

            // Check that the base was searched
            var property = browser.FindElement(new XmlComponent(null, "Base.Private", null));
            Assert.AreEqual("BPrivate", property.Name);
            property = browser.FindElement(new XmlComponent(null, "Base.Protected", null));
            Assert.AreEqual("BProtected", property.Name);

            // Check namespaces
            property = browser.FindElement(new XmlComponent(null, "Derived.Public", null));
            Assert.AreEqual("DPublic", property.Name);
            property = browser.FindElement(new XmlComponent(null, "Derived.Public", "ns"));
            Assert.AreEqual("DPublicNs", property.Name);

            // Check a readonly property
            property = browser.FindElement(new XmlComponent(null, "Derived.Private", null));
            Assert.AreEqual("DPrivate", property.Name);

            // Makes sure we don't have any attributes
            Assert.IsNull(browser.FindElement(new XmlComponent(null, "Derived.Attribute", null)));
            Assert.IsNull(browser.FindElement(new XmlComponent(null, "Derived.Field", null)));

            int counter = 0;
            foreach (var prop in browser.Elements)
            {
                counter++;
            }
            Assert.AreEqual(6, counter);
        }
    }
}
