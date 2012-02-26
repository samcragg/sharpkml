using System;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Base
{
    [TestFixture]
    public class KmlFactoryTest
    {
        public class TestElementClass1 : Element
        {
        }

        public class TestElementClass2 : Element
        {
        }

        public class TestElementClass3 : Element
        {
        }

        static KmlFactoryTest()
        {
            // Register our own type only once
            KmlFactory.Register<TestElementClass1>(new XmlComponent(null, "test", string.Empty));
        }

        [Test]
        public void TestCreation()
        {
            Element element = KmlFactory.CreateElement(new XmlComponent(null, "test", string.Empty));
            Assert.That(element, Is.Not.Null);
            Assert.That(element, Is.InstanceOf<TestElementClass1>());
        }

        [Test]
        public void TestRegistration()
        {
            // Test that types in the KmlDom namespace are automatically registered.
            Assert.That(KmlFactory.FindType(typeof(Description)).Name, Is.EqualTo("description"));

            // Make sure it knows about our type registered in the static constructor
            Assert.That(KmlFactory.FindType(typeof(TestElementClass1)).Name, Is.EqualTo("test"));

            // This should be ok as the namespace is different
            KmlFactory.Register<TestElementClass2>(new XmlComponent(null, "test", "another namespace"));

            // But this should throw an exception
            Assert.That(
                () => KmlFactory.Register<TestElementClass3>(new XmlComponent(null, "test", string.Empty)),
                Throws.TypeOf<ArgumentException>());
        }
    }
}
