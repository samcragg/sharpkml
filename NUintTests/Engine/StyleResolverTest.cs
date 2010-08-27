using System;
using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace NUintTests.Engine
{
    [TestFixture]
    public class StyleResolverTest
    {
        private static readonly TestCase[] TestCases =
        {
            new TestCase("AllStylesTest", "f0", "AllStylesTest", StyleState.Normal),
            new TestCase("AllStylesTest", "f1", null, StyleState.Highlight),
            new TestCase("AllStylesTest", "f1", null, StyleState.Normal),
            new TestCase("DuplicateIdTest", "line", "DuplicateIdTest", StyleState.Normal),
            new TestCase("DuplicateIdTest", "point", "DuplicateIdTest", StyleState.Normal),
            new TestCase("InlineTest", "f0", "InlineTest", StyleState.Normal),
            new TestCase("InlineStyleMapTest", "pm0", "InlineStyleMapTest-Highlight", StyleState.Highlight),
            new TestCase("InlineStyleMapTest", "pm0", "InlineStyleMapTest-Normal", StyleState.Normal),
            new TestCase("NestingTest", "p0", null, StyleState.Highlight),
            new TestCase("NestingTest", "p0", null, StyleState.Normal),
            new TestCase("NestingTest", "p1", null, StyleState.Highlight),
            new TestCase("NestingTest", "p1", null, StyleState.Normal),
            new TestCase("SharedTest", "pm0", "SharedTest-Highlight", StyleState.Highlight),
            new TestCase("SharedTest", "pm0", "SharedTest-Normal", StyleState.Normal),
            new TestCase("SimpleTest", "pm0", "SimpleTest", StyleState.Normal),
            new TestCase("UnknownTest", "jb", "UnknownTest", StyleState.Normal)
        };

        [Test]
        public void TestInlineBasic()
        {
            // Build our Kml with a shared style
            const string InputKml =
                "<Document>" +
                "<Style id='_0' />" +
                "<Placemark>" +
                "<styleUrl>#_0</styleUrl>" +
                "</Placemark>" +
                "</Document>";

            const string ExpectedKml =
                "<Document>" +
                "<Placemark>" +
                "<StyleMap>" +
                "<Pair><key>normal</key><Style/></Pair>" +
                "<Pair><key>highlight</key><Style/></Pair>" +
                "</StyleMap>" +
                "</Placemark>" +
                "</Document>";

            CompareText(InputKml, ExpectedKml, e => StyleResolver.InlineStyles(e));
        }

        [Test]
        public void TestInlineStyleUrl()
        {
            var placemark = new Placemark();
            placemark.StyleUrl = new Uri("http://example.com/style.kml#cool-style");

            var output = StyleResolver.InlineStyles(placemark);
            Assert.AreEqual(placemark.StyleUrl, ((Placemark)output).StyleUrl);

            placemark.StyleUrl = new Uri("#non-existent-local-reference", UriKind.Relative);
            output = StyleResolver.InlineStyles(placemark);
            Assert.AreEqual(placemark.StyleUrl, ((Placemark)output).StyleUrl);

            var document = new Document();
            document.AddStyle(new Style { Id = "non-existent-local-reference" });
            document.AddFeature(placemark);
            document = StyleResolver.InlineStyles(document);

            placemark = (Placemark)document.Features.ElementAt(0);
            Assert.IsNull(placemark.StyleUrl);
        }

        [Test]
        public void TestInlineUpdate()
        {
            var document = new Document();
            document.AddStyle(new Style { Id = "style0" });

            var create = new CreateCollection();
            create.Add(document);

            var update = new Update();
            update.AddUpdate(create);

            var output = StyleResolver.InlineStyles(update);

            // Make sure it didn't change anything
            SampleData.CompareElements(update, output);
        }

        [Test]
        public void TestInlineComplex()
        {
            using (var data = SampleData.CreateStream("Engine.Data.Style Data.kml"))
            using (var output = SampleData.CreateStream("Engine.Data.Style Output.kml"))
            {
                Parser parser = new Parser();
                parser.Parse(data);
                Document actual = GetDocument((Document)(((Kml)parser.Root).Feature), "InlineStylesTest");

                parser.Parse(output);
                Document expected = GetDocument((Document)(((Kml)parser.Root).Feature), "InlineStylesTest");

                actual = StyleResolver.InlineStyles(actual);
                SampleData.CompareElements(expected, actual);
            }
        }

        [Test]
        public void TestResolver()
        {
            using (var data = SampleData.CreateStream("Engine.Data.Style Data.kml"))
            using (var output = SampleData.CreateStream("Engine.Data.Style Output.kml"))
            {
                Parser parser = new Parser();
                parser.Parse(data);
                Document dataDoc = (Document)(((Kml)parser.Root).Feature);

                parser.Parse(output);
                Document outputDoc = (Document)(((Kml)parser.Root).Feature);

                foreach (var test in TestCases)
                {
                    Document doc = GetDocument(dataDoc, test.Input);
                    Style expected = null;
                    if (test.Output != null)
                    {
                        expected = (Style)(GetDocument(outputDoc, test.Output).Styles.First());
                    }
                    RunTestCase(doc, test.FeatureId, test.State, expected);
                }
            }
        }

        [Test]
        public void TestSplitBasic()
        {
            const string InputKml =
                "<Document>" +
                "<Placemark>" +
                "<Style/>" +
                "</Placemark>" +
                "</Document>";

            const string ExpectedKml =
                "<Document>" +
                "<Style id='_0'/>" +
                "<Placemark>" +
                "<styleUrl>#_0</styleUrl>" +
                "</Placemark>" +
                "</Document>";

            CompareText(InputKml, ExpectedKml, e => StyleResolver.SplitStyles(e));
        }

        [Test]
        public void TestSplitComplex()
        {
            // Test with a Document that already has a shared style to make
            // sure it's not disturbed. Also check with a Placemark that
            // doesn't have a style.
            const string InputKml =
                "<Document>" +
                "<StyleMap id='stylemap0'/>" +
                "<Placemark><name>no style</name></Placemark>" +
                "<Placemark>" +
                "<Style><LineStyle/></Style>" +
                "</Placemark>" +
                "<Placemark>" +
                "<name>has shared stylemap</name>" +
                "<styleUrl>#stylemap0</styleUrl>" +
                "</Placemark>" +
                "</Document>";

            const string ExpectedKml =
                "<Document>" +
                "<StyleMap id='stylemap0'/>" +
                "<Style id='_0'><LineStyle/></Style>" +
                "<Placemark><name>no style</name></Placemark>" +
                "<Placemark><styleUrl>#_0</styleUrl></Placemark>" +
                "<Placemark>" +
                "<name>has shared stylemap</name>" +
                "<styleUrl>#stylemap0</styleUrl>" +
                "</Placemark>" +
                "</Document>";

            CompareText(InputKml, ExpectedKml, e => StyleResolver.SplitStyles(e));
        }

        [Test]
        public void TestSplitNoDocument()
        {
            // Make sure if there's no document then it's unaltered.
            const string Kml =
                "<Folder>" +
                "<name>f</name>" +
                "<Placemark>" +
                "<Style><IconStyle/></Style>" +
                "</Placemark>" +
                "</Folder>";

            CompareText(Kml, Kml, e => StyleResolver.SplitStyles(e)); // Input and the output should be the same
        }

        [Test]
        public void TestSplitUniqueId()
        {
            // Force a collision and make sure it overwrites the id
            const string InputKml =
                "<Document>" +
                "<Style id='_0'/>" +
                "<Placemark><Style id='my_id'/></Placemark>" +
                "</Document>";

            const string ExpectedKml =
                "<Document>" +
                "<Style id='_0'/>" +
                "<Style id='_1'/>" +
                "<Placemark>" +
                "<styleUrl>#_1</styleUrl>" +
                "</Placemark>" +
                "</Document>";

            CompareText(InputKml, ExpectedKml, e => StyleResolver.SplitStyles(e));
        }

        private static void CompareText(string actual, string expected, Func<Element, Element> function)
        {
            var parser = new Parser();
            parser.ParseString(expected, false);
            var expectedElement = parser.Root;
            Assert.IsNotNull(expectedElement);

            parser.ParseString(actual, false);
            var actualElement = parser.Root;
            Assert.IsNotNull(actualElement);

            SampleData.CompareElements(expectedElement, function(actualElement));
        }

        private static Document GetDocument(Document parent, string id)
        {
            foreach (var feature in parent.Features)
            {
                if (string.Equals(feature.Id, id, StringComparison.Ordinal))
                {
                    return feature as Document;
                }
            }
            return null;
        }

        private static void RunTestCase(Document data, string id, StyleState state, Style expected)
        {
            KmlFile file = KmlFile.Create(data, true);
            Feature feature = file.FindObject(id) as Feature;
            Assert.IsNotNull(feature); // Make sure the test data is ok

            var style = StyleResolver.CreateResolvedStyle(feature, file, state, true);
            if (expected == null)
            {
                // Make sure everything is null
                Assert.IsNull(style.Balloon);
                Assert.IsNull(style.Icon);
                Assert.IsNull(style.Label);
                Assert.IsNull(style.Line);
                Assert.IsNull(style.List);
                Assert.IsNull(style.Polygon);
            }
            else
            {
                SampleData.CompareElements(expected, style);
            }
        }

        private class TestCase
        {
            public string FeatureId { get; private set; }
            public string Input { get; private set; }
            public string Output { get; private set; }
            public StyleState State { get; private set; }

            public TestCase(string input, string feature, string output, StyleState state)
            {
                this.FeatureId = feature;
                this.Input = input;
                this.Output = output;
                this.State = state;
            }
        }
    }
}
