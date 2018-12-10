namespace UnitTests.Dom
{
    using System;
    using NUnit.Framework;
    using SharpKml.Base;
    using SharpKml.Dom;

    [TestFixture]
    public class ElementTest
    {
        static ElementTest()
        {
            KmlFactory.Register<BaseElement>(new XmlComponent(null, nameof(BaseElement), nameof(ElementTest)));
            KmlFactory.Register<DerivedElement>(new XmlComponent(null, nameof(DerivedElement), nameof(ElementTest)));
            KmlFactory.RegisterExtension<BaseElement, BaseElementExtension>();
        }

        public sealed class AddChildTests : ElementTest
        {
            [Test]
            public void ShouldBeAbleToAddAnExtensionToADerivedClass()
            {
                var parent = new DerivedElement();
                var child = new BaseElementExtension();

                parent.AddChild(child);

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

        private class BaseElementExtension : Element
        {
        }

        private class DerivedElement : BaseElement
        {
        }
    }
}
