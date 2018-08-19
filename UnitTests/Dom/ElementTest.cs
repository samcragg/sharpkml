namespace UnitTests.Dom
{
    using System;
    using NUnit.Framework;
    using SharpKml.Base;
    using SharpKml.Dom;

    [TestFixture]
    public class ElementTest
    {
        public sealed class AddChildTests : ElementTest
        {
            [Test]
            public void ShouldBeAbleToAddAnExtensionToADerivedClass()
            {
                KmlFactory.Register<BaseElement>(new XmlComponent(null, nameof(BaseElement), nameof(ElementTest)));
                KmlFactory.Register<DerivedElement>(new XmlComponent(null, nameof(DerivedElement), nameof(ElementTest)));
                KmlFactory.RegisterExtension<BaseElement, ExtensionElement>();

                var parent = new DerivedElement();
                var child = new ExtensionElement();
                parent.AddChild(child);

                Assert.That(parent.Children, Has.Member(child));
            }

            [Test]
            public void ShouldBeAbleToAddAValidChild()
            {
                var child = new Placemark();
                var parent = new Folder();

                parent.AddFeature(child);

                Assert.That(parent.Children, Has.Member(child));
            }

            [Test]
            public void ShouldNotAllowChildrenOfOtherNodesToBeAdded()
            {
                var child = new Placemark();
                var parent1 = new Folder();
                var parent2 = new Folder();

                parent1.AddFeature(child);

                Assert.That(() => parent2.AddFeature(child),
                            Throws.TypeOf<InvalidOperationException>());
            }
        }

        private class BaseElement : Element
        {
        }

        private class DerivedElement : BaseElement
        {
        }

        private class ExtensionElement : Element
        {
        }
    }
}
