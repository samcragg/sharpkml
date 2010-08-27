using System;
using SharpKml.Base;
using SharpKml.Engine;

namespace Examples
{
    /// <summary>
    /// Uses the StyleResolver.SplitStyles static member to move styles from
    /// the Features to the enclosing Document and sets the Feature.StyleUrl
    /// to reference the new style.
    /// </summary>
    public static class SplitStyles
    {
        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to split the styles:");
            if (file == null)
            {
                return;
            }

            var split = StyleResolver.SplitStyles(file.Root);
            var serializer = new Serializer();
            serializer.Serialize(split);
            Console.WriteLine(serializer.Xml);
        }
    }
}
