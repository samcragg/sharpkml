using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom.GX;

namespace UnitTests.Dom
{
    [TestFixture]
    public class SimpleArrayDataTest
    {
        [Test]
        public void ValuesShouldIgnoreTheNamespace()
        {
            const string Kml = @"
<gx:SimpleArrayData>
    <gx:value>value</gx:value>
</gx:SimpleArrayData>";

            var parser = new Parser();
            parser.ParseString(Kml, namespaces: false);
            var arrayData = (SimpleArrayData)parser.Root;

            Assert.That(arrayData.Values, Is.EquivalentTo(new[] { "value" }));
        }

        [Test]
        public void ValuesShouldReturnComplicatedInnerText()
        {
            const string Kml = @"
<gx:SimpleArrayData xmlns:gx='http://www.google.com/kml/ext/2.2'>
    <gx:value>one <![CDATA[two]]> three</gx:value>
</gx:SimpleArrayData>";

            var parser = new Parser();
            parser.ParseString(Kml, namespaces: true);
            var arrayData = (SimpleArrayData)parser.Root;

            Assert.That(arrayData.Values, Is.EquivalentTo(new[] { "one two three" }));
        }

        [Test]
        public void ValuesShouldReturnTheNestedValues()
        {
            const string Kml = @"
<gx:SimpleArrayData xmlns:gx='http://www.google.com/kml/ext/2.2'>
    <gx:value>1</gx:value>
    <gx:value>2</gx:value>
</gx:SimpleArrayData>";

            var parser = new Parser();
            parser.ParseString(Kml, namespaces: true);
            var arrayData = (SimpleArrayData)parser.Root;

            Assert.That(arrayData.Values, Is.EquivalentTo(new[] { "1", "2" }));
        }
    }
}
