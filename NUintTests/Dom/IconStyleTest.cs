using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Dom
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
            Assert.IsNotNull(icon);
            Assert.IsNotNull(icon.Icon);
            Assert.AreEqual("image.jpg", icon.Icon.Href.OriginalString);
        }
    }
}
