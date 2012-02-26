using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Dom
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
            Assert.That(parser.Root, Is.Not.Null);

            LatLonAltBox box = parser.Root as LatLonAltBox;
            Assert.That(box, Is.Not.Null);

            // Verify the proper values in the object model:
            Assert.That(box.MinimumAltitude, Is.EqualTo(101.101));
            Assert.That(box.MaximumAltitude, Is.EqualTo(202.202));
            Assert.That(box.AltitudeMode, Is.EqualTo(AltitudeMode.Absolute));
            Assert.That(box.North, Is.EqualTo(2.5));
            Assert.That(box.South, Is.EqualTo(1.25));
            Assert.That(box.East, Is.EqualTo(1.25));
            Assert.That(box.West, Is.EqualTo(0));

            const string LatLonAltBoxClampToGround =
                "<LatLonAltBox>" +
                "<altitudeMode>clampToGround</altitudeMode>" +
                "</LatLonAltBox>";
            parser.ParseString(LatLonAltBoxClampToGround, false);
            Assert.That(parser.Root, Is.Not.Null);

            box = parser.Root as LatLonAltBox;
            Assert.That(box, Is.Not.Null);
            Assert.That(box.North, Is.Null);
            Assert.That(box.South, Is.Null);
            Assert.That(box.East, Is.Null);
            Assert.That(box.West, Is.Null);
            Assert.That(box.MinimumAltitude, Is.Null);
            Assert.That(box.MaximumAltitude, Is.Null);
            Assert.That(box.GXAltitudeMode, Is.Null);
            Assert.That(box.AltitudeMode, Is.EqualTo(AltitudeMode.ClampToGround));

            const string LatLonAltBoxRelativeToGround =
                "<LatLonAltBox>" +
                "<altitudeMode>relativeToGround</altitudeMode>" +
                "</LatLonAltBox>";
            parser.ParseString(LatLonAltBoxRelativeToGround, false);
            box = (LatLonAltBox)parser.Root;
            Assert.That(box.AltitudeMode, Is.EqualTo(AltitudeMode.RelativeToGround));

            const string LatLonAltBoxRelativeToSeaFloor =
                "<LatLonAltBox>" +
                "<gx:altitudeMode>relativeToSeaFloor</gx:altitudeMode>" +
                "</LatLonAltBox>";
            parser.ParseString(LatLonAltBoxRelativeToSeaFloor, false);
            box = (LatLonAltBox)parser.Root;
            Assert.That(box.GXAltitudeMode, Is.EqualTo(SharpKml.Dom.GX.AltitudeMode.RelativeToSeafloor));
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
            Assert.That(parser.Root, Is.Not.Null);

            LatLonAltBox box = parser.Root as LatLonAltBox;
            Assert.That(box, Is.Not.Null);

            // Check it was parsed ok
            Assert.That(box.North, Is.EqualTo(2.5));
            Assert.That(box.South, Is.Null);
            Assert.That(box.East, Is.Null);
            Assert.That(box.West, Is.EqualTo(0));
            Assert.That(box.MinimumAltitude, Is.EqualTo(101.101));
            Assert.That(box.MaximumAltitude, Is.Null);
            Assert.That(box.GXAltitudeMode, Is.Null);
            Assert.That(box.AltitudeMode, Is.EqualTo(AltitudeMode.Absolute));

            Serializer serializer = new Serializer();
            serializer.SerializeRaw(box);
            Assert.That(serializer.Xml, Is.EqualTo(TestKml));
        }
    }
}