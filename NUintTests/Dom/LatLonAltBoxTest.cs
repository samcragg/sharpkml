using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests.Dom
{
    [TestFixture]
    public class LatLonAltBoxTest
    {
        [Test]
        public void TestParseAltitudeMode()
        {
            const string LatLonAltBoxAbsolute =
                "<LatLonAltBox>" +
                "<north>2.5</north>" +
                "<south>1.25</south>" +
                "<east>1.25</east>" +
                "<west>0</west>" +
                "<minAltitude>101.101</minAltitude>" +
                "<maxAltitude>202.202</maxAltitude>" +
                "<altitudeMode>absolute</altitudeMode>" +
                "</LatLonAltBox>";

            Parser parser = new Parser();
            parser.ParseString(LatLonAltBoxAbsolute, false);
            Assert.IsNotNull(parser.Root);

            LatLonAltBox box = parser.Root as LatLonAltBox;
            Assert.IsNotNull(box);

            // Verify the proper values in the object model:
            Assert.AreEqual(101.101, box.MinimumAltitude);
            Assert.AreEqual(202.202, box.MaximumAltitude);
            Assert.AreEqual(AltitudeMode.Absolute, box.AltitudeMode);
            Assert.AreEqual(2.5, box.North);
            Assert.AreEqual(1.25, box.South);
            Assert.AreEqual(1.25, box.East);
            Assert.AreEqual(0, box.West);

            const string LatLonAltBoxClampToGround =
                "<LatLonAltBox>" +
                "<altitudeMode>clampToGround</altitudeMode>" +
                "</LatLonAltBox>";
            parser.ParseString(LatLonAltBoxClampToGround, false);
            Assert.IsNotNull(parser.Root);

            box = parser.Root as LatLonAltBox;
            Assert.IsNotNull(box);
            Assert.IsNull(box.North);
            Assert.IsNull(box.South);
            Assert.IsNull(box.East);
            Assert.IsNull(box.West);
            Assert.IsNull(box.MinimumAltitude);
            Assert.IsNull(box.MaximumAltitude);
            Assert.IsNull(box.GXAltitudeMode);
            Assert.AreEqual(AltitudeMode.ClampToGround, box.AltitudeMode);

            const string LatLonAltBoxRelativeToGround =
                "<LatLonAltBox>" +
                "<altitudeMode>relativeToGround</altitudeMode>" +
                "</LatLonAltBox>";
            parser.ParseString(LatLonAltBoxRelativeToGround, false);
            box = (LatLonAltBox)parser.Root;
            Assert.AreEqual(AltitudeMode.RelativeToGround, box.AltitudeMode);

            const string LatLonAltBoxRelativeToSeaFloor =
                "<LatLonAltBox>" +
                "<gx:altitudeMode>relativeToSeaFloor</gx:altitudeMode>" +
                "</LatLonAltBox>";
            parser.ParseString(LatLonAltBoxRelativeToSeaFloor, false);
            box = (LatLonAltBox)parser.Root;
            Assert.AreEqual(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor, box.GXAltitudeMode);
        }

        [Test]
        public void TestSerialize()
        {
            // This needs to be in this order
            const string TestKml =
                "<LatLonAltBox xmlns=\"http://www.opengis.net/kml/2.2\">" +
                "<north>2.5</north>" +
                "<west>0</west>" +
                "<minAltitude>101.101</minAltitude>" +
                "<altitudeMode>absolute</altitudeMode>" +
                "</LatLonAltBox>";

            Parser parser = new Parser();
            parser.ParseString(TestKml, true);
            Assert.IsNotNull(parser.Root);

            LatLonAltBox box = parser.Root as LatLonAltBox;
            Assert.IsNotNull(box);

            // Check it was parsed ok
            Assert.AreEqual(2.5, box.North);
            Assert.IsNull(box.South);
            Assert.IsNull(box.East);
            Assert.AreEqual(0, box.West);
            Assert.AreEqual(101.101, box.MinimumAltitude);
            Assert.IsNull(box.MaximumAltitude);
            Assert.IsNull(box.GXAltitudeMode);
            Assert.AreEqual(AltitudeMode.Absolute, box.AltitudeMode);

            Serializer serializer = new Serializer();
            serializer.SerializeRaw(box);
            Assert.AreEqual(TestKml, serializer.Xml);
        }
    }
}
