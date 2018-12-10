namespace UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    internal static class XmlTestHelper
    {
        // https://blogs.msdn.microsoft.com/ericwhite/2009/01/27/equality-semantics-of-linq-to-xml-trees/
        public static XDocument Normalize(XDocument source)
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
