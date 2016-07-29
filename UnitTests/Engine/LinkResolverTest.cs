using System.Collections.Generic;
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
        public void DuplicateLinksShouldBeIgnored()
        {
            const string Kml =
                "<Folder xmlns='http://www.opengis.net/kml/2.2'>" +
                "<NetworkLink><Link><href>foo.kml</href></Link></NetworkLink>" +
                "<NetworkLink><Link><href>foo.kml</href></Link></NetworkLink>" +
                "</Folder>";

            using (var reader = new StringReader(Kml))
            {
                var resolver = new LinkResolver(KmlFile.Load(reader));

                Assert.That(resolver.Links.Count, Is.EqualTo(1));
                Assert.That(resolver.Links[0].OriginalString, Is.EqualTo("foo.kml"));
            }
        }

        [Test]
        public void ShouldFindAllTheLinkTypesInTheKmlFile()
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
                var resolver = new LinkResolver(KmlFile.Load(reader));

                IEnumerable<string> links = resolver.Links.Select(u => u.OriginalString);

                Assert.That(links, Is.EquivalentTo(expected));
            }
        }

        [Test]
        public void GetRelativePathsShouldReturnTheNormalizedPath()
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
                var resolver = new LinkResolver(KmlFile.Load(reader));

                IEnumerable<string> relatives = resolver.GetRelativePaths();

                Assert.That(relatives, Is.EquivalentTo(expected));
            }
        }
    }
}
