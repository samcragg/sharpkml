using System;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Base
{
    [TestFixture]
    public class KmlFactoryTest
    {
        private class TestElementClass1 : Element
        {
        }

        private class TestElementClass2 : Element
        {
        }

        private class TestElementClass3 : Element
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
            Assert.IsNotNull(element);
            Assert.IsInstanceOf<TestElementClass1>(element);
        }

        [Test]
        public void TestRegistration()
        {
            // Test that types in the KmlDom namespace are automatically registered.
            Assert.AreEqual("description", KmlFactory.FindType(typeof(Description)).Name);

            // Make sure it knows about our type registered in the static constructor
            Assert.AreEqual("test", KmlFactory.FindType(typeof(TestElementClass1)).Name);

            // This should be ok as the namespace is different
            KmlFactory.Register<TestElementClass2>(new XmlComponent(null, "test", "another namespace"));

            // But this should throw an exception
            Assert.Catch<ArgumentException>(() =>
                KmlFactory.Register<TestElementClass3>(new XmlComponent(null, "test", string.Empty)));
        }
    }
}
