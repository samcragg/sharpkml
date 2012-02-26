using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Dom
{
    [TestFixture]
    public class IconStyleTest
    {
        [Test]
        public void TestParse()
        {
            // Checks that <Icon> is parsed as an IconStyle.IconLink
            Parser parser = new Parser();
            parser.ParseString(@"<IconStyle><Icon><href>image.jpg</href></Icon></IconStyle>", false);

            var icon = parser.Root as IconStyle;
            Assert.That(icon, Is.Not.Null);
            Assert.That(icon.Icon, Is.Not.Null);
            Assert.That(icon.Icon.Href.OriginalString, Is.EqualTo("image.jpg"));
        }
    }
}
