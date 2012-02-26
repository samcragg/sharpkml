using System;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace UnitTests.Engine
{
    [TestFixture]
    public class KmlFileTest
    {
        [Test]
        public void TestObjectIds()
        {
            // Test basic parse first
            var file = CreateFile("<Folder id='folder'><Placemark id='placemark'/></Folder>", false);

            // Make sure we can find the folder, making sure it's also the correct type
            Assert.That(file.FindObject("folder"), Is.InstanceOf<Folder>());
            Assert.That(file.FindObject("placemark"), Is.InstanceOf<Placemark>());

            // Now test duplicates
            file = CreateFile("<Folder id='my_id'><Placemark id='my_id'/></Folder>", true);
            Assert.That(file.FindObject("my_id"), Is.InstanceOf<Placemark>());

            Assert.That(() => CreateFile("<Folder id='my_id'><Placemark id='my_id'/></Folder>", false),
                        Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void TestStyles()
        {
            // Verify a basic shared style is found and a local style is not found.
            const string Kml =
                "<Document>" +
                "<Style id='share-me'/>" +
                "<StyleMap id='me-too'/>" +
                "<Folder><Style id='not-me'/></Folder>" +
                "</Document>";

            var file = CreateFile(Kml, false);
            Assert.That(file, Is.Not.Null); // Make sure the parse worked

            // Verify both shared style selectors were found.
            var style = file.FindStyle("share-me");
            Assert.That(style, Is.InstanceOf<Style>() ); // Make sure it was foun and was the corrct object

            var styleMap = file.FindStyle("me-too");
            Assert.That(styleMap , Is.InstanceOf<StyleMapCollection>() );

            // Verify that the local style is found as an Object...
            var obj = file.FindObject("not-me");
            Assert.That(obj , Is.InstanceOf<Style>() );

            // ...but is not found as a shared style.
            Assert.That(file.FindStyle("not-me"),Is.Null);
        }

        private static KmlFile CreateFile(string kml, bool duplicates)
        {
            var parser = new Parser();
            parser.ParseString(kml, false);
            return KmlFile.Create(parser.Root, duplicates);
        }
    }
}
