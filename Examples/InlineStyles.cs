using System;
using SharpKml.Base;
using SharpKml.Engine;

namespace Examples
{
    /// <summary>
    /// Uses the StyleResolver.InlineStyles static method to create a new
    /// Element that has the styles inside the Feature instead of using
    /// shared styles and specifying a StyleUrl.
    /// </summary>
    public static class InlineStyles
    {
        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to inline the styles:");
            if (file == null)
            {
                return;
            }

            var inlined = StyleResolver.InlineStyles(file.Root);
            var serializer = new Serializer();
            serializer.Serialize(inlined);
            Console.WriteLine(serializer.Xml);
        }
    }
}
