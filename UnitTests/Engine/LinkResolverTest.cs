using System.IO;
using System.Linq;
using NUnit.Framework;
using SharpKml.Engine;

namespace UnitTests.Engine
{
    [TestFixture]
    public class LinkResolverTest
    {
        [Test]
        public void TestDuplicates()
        {
            const string Kml =
                "<Folder xmlns='http://www.opengis.net/kml/2.2'>" +
                "<NetworkLink><Link><href>foo.kml</href></Link></NetworkLink>" +
                "<NetworkLink><Link><href>foo.kml</href></Link></NetworkLink>" +
                "</Folder>";

            // Test that duplicates are ignored
            using (var reader = new StringReader(Kml))
            {
                var resolver = new LinkResolver(reader, false);
                Assert.That(resolver.Links.Count(), Is.EqualTo(1));
                Assert.That(resolver.Links.ElementAt(0).OriginalString, Is.EqualTo("foo.kml"));
            }

            // Test that everything is read
            using (var reader = new StringReader(Kml))
            {
                var resolver = new LinkResolver(reader, true);
                Assert.That(resolver.Links.Count(), Is.EqualTo(2));
            }
        }

        [Test]
        public void TestAll()
        {
            // Verify that GetLinks finds all kinds of hrefs in a KML file.
            string[] expected =
            {
                "http://example.com/icon.jpg",
                "itemicon.png",
                "../more.kml",
                "go.jpeg",
                "so.jpeg",
                "po.jpeg",
                "#myschema",
                "model.dae",
                "style.kml#style"
            };

            using (var stream = SampleData.CreateStream("Engine.Data.Links.kml"))
            using (var reader = new StreamReader(stream))
            {
                var resolver = new LinkResolver(reader, true);
                Assert.That(resolver.Links.Count(), Is.EqualTo(expected.Length));

                int index = 0;
                foreach (var uri in resolver.Links)
                {
                    Assert.That(uri.OriginalString, Is.EqualTo(expected[index++]));
                }
            }
        }

        [Test]
        public void TestRelative()
        {
            string[] expected =
            {
                "itemicon.png",
                "../more.kml",
                "go.jpeg",
                "so.jpeg",
                "po.jpeg",
                "model.dae",
                "style.kml"
            };

            using (var stream = SampleData.CreateStream("Engine.Data.Links.kml"))
            using (var reader = new StreamReader(stream))
            {
                var resolver = new LinkResolver(reader, true);
                Assert.That(resolver.RelativePaths.Count(), Is.EqualTo(expected.Length));

                int index = 0;
                foreach (var path in resolver.RelativePaths)
                {
                    Assert.That(path, Is.EqualTo(expected[index++]));
                }
            }
        }
    }
}
