using System;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace NUintTests.Engine
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
            Assert.IsNotNull(file.FindObject("folder") as Folder);
            Assert.IsNotNull(file.FindObject("placemark") as Placemark);

            // Now test duplicates
            file = CreateFile("<Folder id='my_id'><Placemark id='my_id'/></Folder>", true);
            Assert.IsNotNull(file.FindObject("my_id") as Placemark);

            Assert.Throws<InvalidOperationException>(() => CreateFile("<Folder id='my_id'><Placemark id='my_id'/></Folder>", false));
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
            Assert.IsNotNull(file); // Make sure the parse worked

            // Verify both shared style selectors were found.
            var style = file.FindStyle("share-me");
            Assert.IsNotNull(style as Style); // Make sure it was foun and was the corrct object

            var styleMap = file.FindStyle("me-too");
            Assert.IsNotNull(styleMap as StyleMapCollection);

            // Verify that the local style is found as an Object...
            var obj = file.FindObject("not-me");
            Assert.IsNotNull(obj as Style);

            // ...but is not found as a shared style.
            Assert.IsNull(file.FindStyle("not-me"));
        }

        private static KmlFile CreateFile(string kml, bool duplicates)
        {
            var parser = new Parser();
            parser.ParseString(kml, false);
            return KmlFile.Create(parser.Root, duplicates);
        }
    }
}
