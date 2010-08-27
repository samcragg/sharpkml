using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace NUintTests.Engine
{
    [TestFixture]
    public class UpdateExtensionsTest
    {
        private static readonly TestCase[] TestCases =
        {
            new TestCase("California", "ChangeCalifornia-A", "ChangeCalifornia-ACheck"),
            new TestCase("Placemark", "ChangeGeometry", "ChangeGeometryCheck"),
            new TestCase("Placemark", "ChangeGeometry2", "ChangeGeometry2Check"),
            new TestCase("California", "ChangeLineString", "ChangeLineStringCheck"),
            new TestCase("Placemark", "ChangeStyle", "ChangeStyleCheck"),
            new TestCase("California", "DeleteAD", "DeleteADCheck")
        };

        [Test]
        public void TestSingleSimpleChange()
        {
            const string ChangeXml =
                "<Update>" +
                "<targetHref/>" +
                "<Change>" +
                "<Placemark targetId=\"p\">" +
                "<name>NEW NAME</name>" +
                "</Placemark>" +
                "</Change>" +
                "</Update>";

            var parser = new Parser();
            parser.ParseString("<Placemark id=\"p\"><name>hi</name></Placemark>", false);
            var file = KmlFile.Create(parser.Root, false);

            var target = file.FindObject("p") as Placemark;
            Assert.IsNotNull(target);
            Assert.AreEqual("hi", target.Name);

            parser.ParseString(ChangeXml, false);
            var update = parser.Root as Update;
            Assert.IsNotNull(update); // Verify the test XML

            update.Process(file);
            Assert.AreEqual("NEW NAME", target.Name);
            Assert.IsNull(target.TargetId);
        }

        [Test]
        public void TestSingleSimpleCreate()
        {
            const string CreateXml =
                "<Update>" +
                "<targetHref/>" +
                "<Create>" +
                "<Folder targetId=\"f\">" +
                "<Placemark id=\"px\">" +
                "<name>Update-Created Placemark</name>" +
                "<Point>" +
                "<coordinates>-11.11,22,22</coordinates>" +
                "</Point>" +
                "</Placemark>" +
                "</Folder>" +
                "</Create>" +
                "</Update>";

            var parser = new Parser();
            parser.ParseString("<Folder id=\"f\"/>", false);
            var file = KmlFile.Create(parser.Root, false);

            var target = parser.Root as Folder;
            Assert.IsNotNull(target);
            Assert.AreEqual(0, target.Features.Count());

            parser.ParseString(CreateXml, false);
            var update = parser.Root as Update;
            Assert.IsNotNull(update); // Verify the test XML

            update.Process(file);
            Assert.AreEqual("Update-Created Placemark", target.Features.ElementAt(0).Name);
            Assert.IsNull(target.TargetId);
            Assert.IsNotNull(file.FindObject("px")); // Make sure it was added to the KmlFile too.
        }

        [Test]
        public void TestSingleSimpleDelete()
        {
            const string DeleteXml =
                "<Update>" +
                "<targetHref/>" +
                "<Delete>" +
                "<Placemark targetId=\"p\"/>" +
                "</Delete>" +
                "</Update>";

            var parser = new Parser();
            parser.ParseString("<Folder><Placemark id=\"p\"/></Folder>", false);
            var file = KmlFile.Create(parser.Root, false);

            var target = parser.Root as Folder;
            Assert.IsNotNull(target);
            Assert.AreEqual(1, target.Features.Count());

            parser.ParseString(DeleteXml, false);
            var update = parser.Root as Update;
            Assert.IsNotNull(update);

            update.Process(file);
            Assert.AreEqual(0, target.Features.Count());
            Assert.IsNull(file.FindObject("p")); // Make sure it was deleted from the KmlFile too.
        }

        [Test]
        public void TestManyDeletes()
        {
            const int NumberOfFolders = 100;

            var folder = new Folder();
            for (int i = 0; i < NumberOfFolders; ++i)
            {
                folder.AddFeature(CreateFeature(i, true)); // Add the features with their Id set
            }
            Assert.AreEqual(NumberOfFolders, folder.Features.Count());

            KmlFile file = KmlFile.Create(folder, false);
            var update = new Update();
            for (int i = 0; i < NumberOfFolders; ++i)
            {
                var delete = new DeleteCollection();
                delete.Add(CreateFeature(i, false)); // This time set the TargetId
                update.AddUpdate(delete);
            }
            update.Process(file);
            Assert.AreEqual(0, folder.Features.Count());
        }
        
        // Update/Change on <coordinates> replaces the contents in the target from the source.
        [Test]
        public void TestChangeCoordinates()
        {
            // Create the target
            var point = new Point();
            point.Coordinate = new Vector(38.38, -122.122);

            var placemark = new Placemark();
            placemark.Id = "placemark123";
            placemark.Geometry = point;
            placemark.Name = "placemark name";

            var file = KmlFile.Create(placemark, false);

            // Now create the Update
            const double latitude = -38.38;
            const double longitude = 122.122;

            point = new Point();
            point.Coordinate = new Vector(latitude, longitude);

            placemark = new Placemark();
            placemark.Geometry = point;
            placemark.TargetId = "placemark123";

            var change = new ChangeCollection();
            change.Add(placemark);

            var update = new Update();
            update.AddUpdate(change);

            // Now test the update worked
            update.Process(file);

            placemark = file.Root as Placemark;
            Assert.IsNotNull(placemark);
            Assert.AreEqual("placemark123", placemark.Id);
            Assert.AreEqual("placemark name", placemark.Name);

            point = placemark.Geometry as Point;
            Assert.IsNotNull(point);
            Assert.AreEqual(latitude, point.Coordinate.Latitude);
            Assert.AreEqual(longitude, point.Coordinate.Longitude);
        }

        [Test]
        public void TestUpdateOperations()
        {
            using (var stream = SampleData.CreateStream("Engine.Data.Update.kml"))
            {
                var file = KmlFile.Load(stream);

                foreach (var test in TestCases)
                {
                    RunTestCase(test, file);
                }
            }
        }

        private static Feature CreateFeature(int index, bool id)
        {
            Feature output;
            int type = index % 7;
            switch (type)
            {
                case 0:
                    output = new Placemark();
                    break;
                case 1:
                    output = new Folder();
                    break;
                case 2:
                    output = new Document();
                    break;
                case 3:
                    output = new NetworkLink();
                    break;
                case 4:
                    output = new GroundOverlay();
                    break;
                case 5:
                    output = new ScreenOverlay();
                    break;
                default:
                    output = new PhotoOverlay();
                    break;
            }

            if (id)
            {
                output.Id = "i" + index;
            }
            else
            {
                output.TargetId = "i" + index;
            }
            return output;
        }

        private static void RunTestCase(TestCase test, KmlFile file)
        {
            // Need to create a file from the element for Process to use,
            // making sure we pass a copy not the real thing!
            var target = KmlFile.Create(file.FindObject(test.Target).Clone(), false);

            // Update is stored as an orphan of the parent folder
            var update = (Update)(file.FindObject(test.Input).Orphans.ElementAt(0));
            var expected = file.FindObject(test.Output).Children.ElementAt(0);
            update.Process(target);
            SampleData.CompareElements(expected, target.Root.Children.ElementAt(0));
        }

        private class TestCase
        {
            public string Input { get; private set; }
            public string Output { get; private set; }
            public string Target { get; private set; }

            public TestCase(string target, string input, string output)
            {
                this.Input = input;
                this.Output = output;
                this.Target = target;
            }
        }
    }
}
