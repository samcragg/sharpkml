namespace UnitTests.Base
{
    using System;
    using NUnit.Framework;
    using SharpKml.Base;
    using SharpKml.Dom;

    [TestFixture]
    public class KmlFactoryTest
    {
        static KmlFactoryTest()
        {
            // Register our own type only once
            KmlFactory.Register<ManuallyRegisteredElement>(new XmlComponent(null, "test", ""));
        }

        public sealed class CreateElementTests : KmlFactoryTest
        {
            [Test]
            public void ShouldCreateAnInstanceOfTheSpecifiedType()
            {
                Element element = KmlFactory.CreateElement(new XmlComponent(null, "test", ""));

                Assert.That(element, Is.Not.Null);
                Assert.That(element, Is.InstanceOf<ManuallyRegisteredElement>());
            }
        }

        public sealed class FindTypeTests : KmlFactoryTest
        {
            [Test]
            public void ShouldFindAutomaticallyRegisteredTypes()
            {
                XmlComponent result = KmlFactory.FindType(typeof(Description));

                Assert.That(result.Name, Is.EqualTo("description"));
            }

            [Test]
            public void ShouldFindManuallyRegisteredTypes()
            {
                XmlComponent result = KmlFactory.FindType(typeof(ManuallyRegisteredElement));

                Assert.That(result.Name, Is.EqualTo("test"));
            }

            [Test]
            public void ShouldThrowForNullArguments()
            {
                Assert.That(
                    () => KmlFactory.FindType(null),
                    Throws.TypeOf<ArgumentNullException>());
            }
        }

        public sealed class RegisterTests : KmlFactoryTest
        {
            [Test]
            public void ShouldBeAbleToRegisterTypesInDifferentNamespaces()
            {
                var component = new XmlComponent(null, "test", "another namespace");

                Assert.That(
                    () => KmlFactory.Register<SeparateNamespaceElement>(component),
                    Throws.Nothing);
            }

            [Test]
            public void ShouldNotAllowATypeToBeRegisteredTwice()
            {
                var component = new XmlComponent(null, "alias", "");

                Assert.That(
                    () => KmlFactory.Register<ManuallyRegisteredElement>(component),
                    Throws.TypeOf<ArgumentException>());
            }

            [Test]
            public void ShouldNotAllowDuplicateElementNames()
            {
                // This was registered to TestElementClass1 in the static
                // constructor
                var component = new XmlComponent(null, "test", "");

                Assert.That(
                    () => KmlFactory.Register<NotRegisteredElement>(component),
                    Throws.TypeOf<ArgumentException>());
            }

            private class SeparateNamespaceElement : Element
            {
            }
        }

        public sealed class ReplaceTests : KmlFactoryTest
        {
            [Test]
            public void ShouldCheckTheExistingTypeExists()
            {
                Assert.That(
                    () => KmlFactory.Replace<NotRegisteredElement, ReplacedElement>(),
                    Throws.TypeOf<ArgumentException>());
            }

            [Test]
            public void ShouldReplaceTheSpecifiedType()
            {
                var component = new XmlComponent(null, "existing", "");
                KmlFactory.Register<ExistingElement>(component);

                KmlFactory.Replace<ExistingElement, ReplacedElement>();
                Element result = KmlFactory.CreateElement(component);

                Assert.That(result, Is.InstanceOf<ReplacedElement>());
            }

            private class ExistingElement : Element
            {
            }

            private class ReplacedElement : Element
            {
            }
        }

        private class ManuallyRegisteredElement : Element
        {
        }

        private class NotRegisteredElement : Element
        {
        }
    }
}
