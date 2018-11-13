using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Base
{
    [TestFixture]
    public class TypeBrowserTest
    {
        private TypeBrowser browser;

        [SetUp]
        public void SetUp()
        {
            this.browser = TypeBrowser.Create(typeof(DerivedClass));
        }

        public sealed class FindAttributeTests : TypeBrowserTest
        {
            [Test]
            public void ShouldFindPropertiesWithAttributesByName()
            {
                TypeBrowser.ElementInfo result = this.browser.FindAttribute(
                    new XmlComponent(null, "Derived.Attribute", null));

                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ShouldGetThePropertyValue()
            {
                var instance = new DerivedClass { DAttribute = 123 };
                TypeBrowser.ElementInfo info = this.browser.FindAttribute(
                    new XmlComponent(null, "Derived.Attribute", null));

                object result = info.GetValue(instance);

                Assert.That(result, Is.EqualTo(123));
            }

            [Test]
            public void ShouldIgnorePropertiesWithElements()
            {
                TypeBrowser.ElementInfo result = this.browser.FindAttribute(
                    new XmlComponent(null, "Derived.Public", null));

                Assert.That(result, Is.Null);
            }

            [Test]
            public void ShouldSetThePropertyValue()
            {
                var instance = new DerivedClass();
                TypeBrowser.ElementInfo info = this.browser.FindAttribute(
                    new XmlComponent(null, "Derived.Attribute", null));

                info.SetValue(instance, 123);

                Assert.That(instance.DAttribute, Is.EqualTo(123));
            }
        }

        public sealed class FindElementTests : TypeBrowserTest
        {
            [Test]
            public void ShouldFindPropertiesWithElementsByName()
            {
                TypeBrowser.ElementInfo result = this.browser.FindElement(
                    new XmlComponent(null, "Derived.Public", null));

                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ShouldFindPropertiesWithElementsInBase()
            {
                TypeBrowser.ElementInfo result = this.browser.FindElement(
                    new XmlComponent(null, "Base.Public", null));

                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ShouldFindPropertiesWithElementsThatArePrivate()
            {
                TypeBrowser.ElementInfo result = this.browser.FindElement(
                    new XmlComponent(null, "Base.Private", null));

                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public void ShouldFindPropertiesWithNamespaces()
            {
                TypeBrowser.ElementInfo noNamespace = this.browser.FindElement(
                    new XmlComponent(null, "Derived.Public", null));

                TypeBrowser.ElementInfo withNamespace = this.browser.FindElement(
                    new XmlComponent(null, "Derived.Public", "ns"));

                Assert.That(noNamespace.Component.NamespaceUri, Is.Empty);
                Assert.That(withNamespace.Component.NamespaceUri, Is.EqualTo("ns"));
            }

            [Test]
            public void ShouldGetThePropertyValue()
            {
                var instance = new DerivedClass { PrivateProperty = 123 };
                TypeBrowser.ElementInfo info = this.browser.FindElement(
                    new XmlComponent(null, "Base.Private", null));

                object result = info.GetValue(instance);

                Assert.That(result, Is.EqualTo(123));
            }

            [Test]
            public void ShouldIgnorePropertiesWithAttributes()
            {
                TypeBrowser.ElementInfo result = this.browser.FindElement(
                    new XmlComponent(null, "Derived.Attribute", null));

                Assert.That(result, Is.Null);
            }

            [Test]
            public void ShouldIgnoreReadOnlyProperties()
            {
                TypeBrowser.ElementInfo result = this.browser.FindElement(
                    new XmlComponent(null, "Derived.ReadOnly", null));

                Assert.That(result, Is.Null);
            }

            [Test]
            public void ShouldSetThePropertyValue()
            {
                var instance = new DerivedClass();
                TypeBrowser.ElementInfo info = this.browser.FindElement(
                    new XmlComponent(null, "Base.Private", null));

                info.SetValue(instance, 123);

                Assert.That(instance.PrivateProperty, Is.EqualTo(123));
            }
        }

        private class BaseClass : Element
        {
            [KmlElement("Base.Public", null)]
            public int BPublic { get; set; }

            internal int PrivateProperty
            {
                get => this.BPrivate;
                set => this.BPrivate = value;
            }

            [KmlElement("Base.Private", null)]
            private int BPrivate { get; set; }
        }

        private class DerivedClass : BaseClass
        {
            [KmlAttribute("Derived.Attribute")]
            public int DAttribute { get; set; }

            [KmlElement("Derived.Public", null)]
            public int DPublic { get; set; }

            [KmlElement("Derived.Public", "ns")]
            public int DPublicNs { get; set; }

            [KmlElement("Derived.ReadOnly", null)]
            public int DReadOnly => 0;
        }
    }
}
