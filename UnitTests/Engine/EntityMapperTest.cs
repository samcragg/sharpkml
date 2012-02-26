using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpKml.Dom;
using SharpKml.Engine;

namespace UnitTests.Engine
{
    [TestFixture]
    public class EntityMapperTest
    {
        private static readonly List<Tuple<string, string>> AltMarkup = new List<Tuple<string, string>>()
        {
            Tuple.Create("1st display name", "1st"),
            Tuple.Create("2nd display name", "2nd"),
            Tuple.Create("name", "data name"),
            Tuple.Create("description", "data description")
        };

        private static readonly Dictionary<string, string> ExpectedEntities = new Dictionary<string, string>()
        {
            { "id", "foo" },
            { "targetId", "bar" },
            { "name", "__NAME__" },
            { "address", "__ADDRESS__" },
            { "Snippet", "__SNIPPET__" },
            { "description", "__DESCRIPTION__" },
            { "TrailHeadType/ElevationGain", "10000" },
            { "TrailHeadType/ElevationGain/displayName", "<i>change in altitude</i>" },
            { "TrailHeadType/TrailHeadName", "Mount Everest" },
            { "TrailHeadType/TrailHeadName/displayName", "<b>Trail Head Name</b>" },
            { "TrailHeadType/TrailLength", "347.45" },
            { "TrailHeadType/TrailLength/displayName", "<i>The length in miles</i>" },
            { "holeNumber", "1" },
            { "holeYardage", "234" }
        };

        private static readonly List<Tuple<string, string>> Replacements = new List<Tuple<string, string>>()
        {
            Tuple.Create("abcdef", "abcdef"),
            Tuple.Create("abc$[]def", "abc$[]def"),
            Tuple.Create("abc$[noSuchEntity]def", "abc$[noSuchEntity]def"),
            Tuple.Create("$[name]$[description]", "__NAME____DESCRIPTION__"),
            Tuple.Create("xxx$[name]xxx$[description]xxx", "xxx__NAME__xxx__DESCRIPTION__xxx"),
            Tuple.Create("  $[name]  $[description]  ", "  __NAME__  __DESCRIPTION__  "),
            Tuple.Create("  $[name]$[name]  $[description] $[description] $[name]", "  __NAME____NAME__  __DESCRIPTION__ __DESCRIPTION__ __NAME__")
        };

        private static readonly List<Tuple<string, string>> SchemaMappings = new List<Tuple<string, string>>()
        {
            Tuple.Create("s_name:simple field display name 1", "one"),
            Tuple.Create("s_name:sfield2", "2")
        };

        [Test]
        public void TestAltMarkupData()
        {
            using (var stream = SampleData.CreateStream("Engine.Data.Entity Data.kml"))
            {
                KmlFile file = KmlFile.Load(stream);
                Placemark placemark = file.Root as Placemark;
                Assert.That(placemark, Is.Not.Null);

                EntityMapper mapper = new EntityMapper(file);
                mapper.ParseEntityFields(placemark);

                Assert.That(mapper.Entities.Count, Is.EqualTo(6));
                for (int i = 0; i < 4; ++i)
                {
                    Assert.That(mapper.Markup[i], Is.EqualTo(AltMarkup[i]));
                }

                // Assert that a second parse produces the same result (this
                // is different to the C++ version, which clears the Entities
                // but adds to the Markup - we reset both.
                mapper.ParseEntityFields(placemark);
                Assert.That(mapper.Entities.Count, Is.EqualTo(6));
                Assert.That(mapper.Markup.Count, Is.EqualTo(4));
            }
        }

        [Test]
        public void TestAltMarkupSchemaData()
        {
            using (var stream = SampleData.CreateStream("Engine.Data.Schema Data.kml"))
            {
                KmlFile file = KmlFile.Load(stream);
                Kml root = file.Root as Kml;
                Assert.That(root, Is.Not.Null);

                Document document = root.Feature as Document;
                Placemark placemark = document.Features.ElementAt(0) as Placemark;

                EntityMapper mapper = new EntityMapper(file);
                mapper.ParseEntityFields(placemark);

                for (int i = 0; i < 2; ++i)
                {
                    Assert.That(mapper.Markup[i], Is.EqualTo(SchemaMappings[i]));
                }
            }
        }

        [Test]
        public void TestExpandEntities()
        {
            using (var stream = SampleData.CreateStream("Engine.Data.Entities.kml"))
            {
                KmlFile file = KmlFile.Load(stream);
                Document document = file.Root as Document;
                Assert.That(document, Is.Not.Null);

                Placemark placemark = document.Features.ElementAt(0) as Placemark;
                EntityMapper mapper = new EntityMapper(file);
                mapper.ParseEntityFields(placemark);

                // Verify that CreateExpandedEntities handles various kinds of
                // entity references, spacing, multiple references.
                foreach (var replacement in Replacements)
                {
                    Assert.That(mapper.ExpandEntities(replacement.Item1), Is.EqualTo(replacement.Item2));
                }
            }
        }

        [Test]
        public void TestGetEntityFields()
        {
            using (var stream = SampleData.CreateStream("Engine.Data.Entities.kml"))
            {
                KmlFile file = KmlFile.Load(stream);
                Document document = file.Root as Document;
                Assert.That(document, Is.Not.Null);

                Placemark placemark = document.Features.ElementAt(0) as Placemark;
                EntityMapper mapper = new EntityMapper(file);
                mapper.ParseEntityFields(placemark);

                // Make sure it found everything
                Assert.That(mapper.Entities.Count, Is.EqualTo(ExpectedEntities.Count));
                foreach (var entry in mapper.Entities)
                {
                    Assert.That(entry.Value, Is.EqualTo(ExpectedEntities[entry.Key]));
                }
            }
        }
    }
}
