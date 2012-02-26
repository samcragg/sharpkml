using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests.Dom
{
    [TestFixture]
    public class ItemIconTest
    {
        [Test]
        public void TestParse()
        {
            Parser parser = new Parser();

            // Try with empty state
            parser.ParseString(@"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2"" />", true);
            ItemIcon item = parser.Root as ItemIcon;
            Assert.That(item, Is.Not.Null);
            Assert.That(item.State, Is.EqualTo(ItemIconStates.None));

            // Try with more than one value
            parser.ParseString(@"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2""><state>open error</state></ItemIcon>", true);
            Assert.That(((ItemIcon)parser.Root).State, Is.EqualTo(ItemIconStates.Open | ItemIconStates.Error));
            parser.ParseString(@"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2""><state>open open error</state></ItemIcon>", true);
            Assert.That(((ItemIcon)parser.Root).State, Is.EqualTo(ItemIconStates.Open | ItemIconStates.Error));

            // Try with an empty/invalid value
            parser.ParseString(@"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2""><state /></ItemIcon>", true);
            Assert.That(((ItemIcon)parser.Root).State, Is.EqualTo(ItemIconStates.None));

            parser.ParseString(@"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2""><state>unknown</state></ItemIcon>", true);
            Assert.That(((ItemIcon)parser.Root).State, Is.EqualTo(ItemIconStates.None));
        }

        [Test]
        public void TestSerialize()
        {
            ItemIcon item = new ItemIcon();
            Serializer serializer = new Serializer();

            // This shouldn't not produce a "state" element
            item.State = ItemIconStates.None;
            serializer.SerializeRaw(item);

            string xml = @"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2"" />";
            Assert.That(serializer.Xml, Is.EqualTo(xml));

            // Try with more than one value
            item.State = ItemIconStates.Open | ItemIconStates.Error;
            serializer.SerializeRaw(item);

            xml = @"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2""><state>open error</state></ItemIcon>";
            Assert.That(serializer.Xml, Is.EqualTo(xml));

            // Try with an invalid value
            item.State = (ItemIconStates)0x80;
            serializer.SerializeRaw(item);

            xml = @"<ItemIcon xmlns=""http://www.opengis.net/kml/2.2""><state /></ItemIcon>";
            Assert.That(serializer.Xml, Is.EqualTo(xml));
        }
    }
}
