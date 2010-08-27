using System.IO;
using System.Linq;
using NUnit.Framework;
using SharpKml.Engine;

namespace NUintTests.Engine
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
                Assert.AreEqual(1, resolver.Links.Count());
                Assert.AreEqual("foo.kml", resolver.Links.ElementAt(0).OriginalString);
            }

            // Test that everything is read
            using (var reader = new StringReader(Kml))
            {
                var resolver = new LinkResolver(reader, true);
                Assert.AreEqual(2, resolver.Links.Count());
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
                Assert.AreEqual(expected.Length, resolver.Links.Count());

                int index = 0;
                foreach (var uri in resolver.Links)
                {
                    Assert.AreEqual(expected[index++], uri.OriginalString);
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
                Assert.AreEqual(expected.Length, resolver.RelativePaths.Count());

                int index = 0;
                foreach (var path in resolver.RelativePaths)
                {
                    Assert.AreEqual(expected[index++], path);
                }
            }
        }
    }
}
