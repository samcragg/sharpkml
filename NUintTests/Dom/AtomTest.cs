using System.Collections.Generic;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom.Atom;

namespace NUintTests.Dom
{
    [TestFixture]
    public class AtomTest
    {
        // Taken from http://atompub.org/rfc4287.html Example 1.1
        private const string Simple =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<feed xmlns=\"http://www.w3.org/2005/Atom\">" +
            "  <title>Example Feed</title>" +
            "  <link href=\"http://example.org/\"/>" +
            "  <updated>2003-12-13T18:30:02Z</updated>" +
            "  <author>" +
            "    <name>John Doe</name>" +
            "  </author>" +
            "  <id>urn:uuid:60a76c80-d399-11d9-b93C-0003939e0af6</id>" +
            "  <entry>" +
            "    <title>Atom-Powered Robots Run Amok</title>" +
            "    <link href=\"http://example.org/2003/12/13/atom03\"/>" +
            "    <id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a</id>" +
            "    <updated>2003-12-13T18:30:02Z</updated>" +
            "    <summary>Some text.</summary>" +
            "  </entry>" +
            "</feed>";

        [Test]
        public void TestParse()
        {
            Parser parser = new Parser();
            parser.ParseString(Simple, true);

            var feed = parser.Root as Feed;
            Assert.IsNotNull(feed);

            Assert.AreEqual("Example Feed", feed.Title);
            this.AssertLink(feed.Links, "http://example.org/");
            Assert.AreEqual("2003-12-13T18:30:02Z", feed.Updated);
            Assert.AreEqual("urn:uuid:60a76c80-d399-11d9-b93C-0003939e0af6", feed.Id);

            Entry entry = null;
            foreach (var e in feed.Entries)
            {
                Assert.IsNull(entry); // Make sure we've only got one
                entry = e;
            }
            Assert.IsNotNull(entry); // Make sure we found one
            Assert.AreEqual("Atom-Powered Robots Run Amok", entry.Title);
            this.AssertLink(entry.Links, "http://example.org/2003/12/13/atom03");
            Assert.AreEqual("urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a", entry.Id);
            Assert.AreEqual("2003-12-13T18:30:02Z", entry.Updated);
            Assert.AreEqual("Some text.", entry.Summary);
        }

        private void AssertLink(IEnumerable<SharpKml.Dom.Atom.Link> links, string value)
        {
            SharpKml.Dom.Atom.Link link = null;
            foreach (var l in links)
            {
                Assert.IsNull(link); // Make sure we've only got one
                link = l;
            }
            Assert.IsNotNull(link.Href);
            Assert.IsNotNull(link); // Make sure we found one
            Assert.AreEqual(value, link.Href.AbsoluteUri);
        }
    }
}
