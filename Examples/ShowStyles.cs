using System;
using System.Linq;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace Examples
{
    /// <summary>
    /// Uses a KmlFile to parse a kml file and iterates over the styles in the file,
    /// displaying their names in alphabetical order.
    /// </summary>
    public class ShowStyles
    {
        private const string InputFile = "Sample.kml";

        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to show the styles of:");
            if (file == null)
            {
                return;
            }

            Console.WriteLine("Style names:");
            // We're going to extend the first style later
            StyleSelector firstStyle = null;
            foreach (var style in file.Styles.OrderBy(s => s.Id)) // Use the Linq extension to order them by Id
            {
                if (firstStyle == null)
                {
                    firstStyle = style;
                }
                Console.WriteLine(style.Id);
            }

            // If there was a style display it's Xml
            if (firstStyle != null)
            {
                Console.WriteLine("\nExpanding '{0}':", firstStyle.Id);
                Serializer serializer = new Serializer();
                serializer.Serialize(firstStyle);
                Console.WriteLine(serializer.Xml);
            }
        }
    }
}
