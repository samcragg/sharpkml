using System;
using System.Collections.Generic;
using SharpKml.Dom;
using SharpKml.Engine;

namespace Examples
{
    /// <summary>
    /// Uses a KmlFile to parse a kml file and iterates over the placemarks in the file,
    /// displaying their names in alphabetical order.
    /// </summary>
    public static class SortPlacemarks
    {
        private const string InputFile = "Sample.kml";

        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to show the placemarks of:");
            if (file == null)
            {
                return;
            }

            // It's good practice for the root element of the file to be a Kml element
            Kml kml = file.Root as Kml;
            if (kml != null)
            {
                List<Placemark> placemarks = new List<Placemark>();
                ExtractPlacemarks(kml.Feature, placemarks);

                // Sort using their names
                placemarks.Sort((a, b) => string.Compare(a.Name, b.Name));

                // Display the results
                foreach (var placemark in placemarks)
                {
                    Console.WriteLine(placemark.Name);
                }
            }
        }

        private static void ExtractPlacemarks(Feature feature, List<Placemark> placemarks)
        {
            // Is the passed in value a Placemark?
            Placemark placemark = feature as Placemark;
            if (placemark != null)
            {
                placemarks.Add(placemark);
            }
            else
            {
                // Is it a Container, as the Container might have a child Placemark?
                Container container = feature as Container;
                if (container != null)
                {
                    // Check each Feature to see if it's a Placemark or another Container
                    foreach (var f in container.Features)
                    {
                        ExtractPlacemarks(f, placemarks);
                    }
                }
            }
        }
    }
}
