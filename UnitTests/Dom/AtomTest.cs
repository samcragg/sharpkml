using System.Collections.Generic;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom.Atom;

namespace UnitTests.Dom
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
            Assert.That(feed, Is.Not.Null);

            Assert.That(feed.Title, Is.EqualTo("Example Feed"));
            this.AssertLink(feed.Links, "http://example.org/");
            Assert.That(feed.Updated, Is.EqualTo("2003-12-13T18:30:02Z"));
            Assert.That(feed.Id, Is.EqualTo("urn:uuid:60a76c80-d399-11d9-b93C-0003939e0af6"));

            Entry entry = null;
            foreach (var e in feed.Entries)
            {
                Assert.That(entry, Is.Null); // Make sure we've only got one
                entry = e;
            }
            Assert.That(entry, Is.Not.Null); // Make sure we found one
            Assert.That(entry.Title, Is.EqualTo("Atom-Powered Robots Run Amok"));
            this.AssertLink(entry.Links, "http://example.org/2003/12/13/atom03");
            Assert.That(entry.Id, Is.EqualTo("urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a"));
            Assert.That(entry.Updated, Is.EqualTo("2003-12-13T18:30:02Z"));
            Assert.That(entry.Summary, Is.EqualTo("Some text."));
        }

        private void AssertLink(IEnumerable<SharpKml.Dom.Atom.Link> links, string value)
        {
            SharpKml.Dom.Atom.Link link;
            using (var e = links.GetEnumerator())
            {
                Assert.True(e.MoveNext());
                link = e.Current;
                Assert.False(e.MoveNext()); // Make sure we've only got one
            }

            Assert.That(link, Is.Not.Null); // Make sure we found one
            Assert.That(link.Href, Is.Not.Null);

            Assert.That(link.Href.AbsoluteUri, Is.EqualTo(value));
        }
    }
}
