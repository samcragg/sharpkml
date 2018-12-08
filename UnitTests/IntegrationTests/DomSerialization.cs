namespace UnitTests.IntegrationTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using NUnit.Framework;
    using SharpKml.Base;
    using SharpKml.Dom;

    [Category("Integration")]
    public abstract class DomSerialization
    {
        protected T Parse<T>(string kml) where T : Element
        {
            var parser = new Parser();
            parser.ParseString(kml, namespaces: true);
            AssertElementSerializesCorrectly(kml, parser.Root);
            return (T)parser.Root;
        }

        private static void AssertElementSerializesCorrectly(string kml, Element element)
        {
            var serializer = new Serializer();
            serializer.Serialize(element);

            XDocument original = Normalize(XDocument.Parse(kml));
            XDocument serialized = Normalize(XDocument.Parse(serializer.Xml));

            Assert.That(
                XNode.DeepEquals(original, serialized),
                Is.True,
                () => "Expected:\r\n" + serialized.ToString() + "\r\nTo equal:\r\n" + original.ToString());
        }

        // https://blogs.msdn.microsoft.com/ericwhite/2009/01/27/equality-semantics-of-linq-to-xml-trees/
        private static XDocument Normalize(XDocument source)
        {
            return new XDocument(
                source.Declaration,
                source.Nodes().Select(n =>
                {
                    // Remove comments, processing instructions, and text nodes that are
                    // children of XDocument.  Only white space text nodes are allowed as
                    // children of a document, so we can remove all text nodes.
                    if (n is XComment || n is XProcessingInstruction || n is XText)
                    {
                        return null;
                    }
                    else
                    {
                        return n is XElement e ? NormalizeElement(e) : n;
                    }
                }));
        }

        private static IEnumerable<XAttribute> NormalizeAttributes(XElement element)
        {
            return element.Attributes()
                    .Where(a => !a.IsNamespaceDeclaration)
                    .OrderBy(a => a.Name.NamespaceName)
                    .ThenBy(a => a.Name.LocalName)
                    .Select(a => a);
        }

        private static XElement NormalizeElement(XElement element)
        {
            return new XElement(
                element.Name,
                NormalizeAttributes(element),
                element.Nodes().Select(NormalizeNode)
            );
        }

        private static XNode NormalizeNode(XNode node)
        {
            // Trim comments and processing instructions from normalized tree
            if (node is XComment || node is XProcessingInstruction)
            {
                return null;
            }
            else if (node is XElement e)
            {
                return NormalizeElement(e);
            }
            else
            {
                // Only thing left is XCData and XText, so clone them
                return node;
            }
        }
    }
}
