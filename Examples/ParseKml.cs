using System;
using SharpKml.Base;
using SharpKml.Dom;

namespace Examples
{
    /// <summary>
    /// Parses a string (using the default Kml namespace) and converts it to Kml objects.
    /// </summary>
    public static class ParseKml
    {
        const string InputKml =
            "<Placemark>" +
                "<name>hi</name>" +
            "</Placemark>";

        public static void Run()
        {
            Console.WriteLine("Parsing '{0}'...", InputKml);

            Parser parser = new Parser();
            parser.ParseString(InputKml, false);

            Placemark placemark = (Placemark)parser.Root;
            Console.WriteLine("The placemark name is '{0}'.", placemark.Name);
        }
    }
}
