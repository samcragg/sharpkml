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
        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to show the placemarks of:");
            if (file == null)
            {
                return;
            }

            // It's good practice for the root element of the file to be a Kml element
            if (file.Root is Kml kml)
            {
                var placemarks = new List<Placemark>();
                ExtractPlacemarks(kml.Feature, placemarks);

                // Sort using their names
                placemarks.Sort((a, b) => string.Compare(a.Name, b.Name));

                // Display the results
                foreach (Placemark placemark in placemarks)
                {
                    Console.WriteLine(placemark.Name);
                }
            }
        }

        private static void ExtractPlacemarks(Feature feature, List<Placemark> placemarks)
        {
            // Is the passed in value a Placemark?
            if (feature is Placemark placemark)
            {
                placemarks.Add(placemark);
            }
            else
            {
                // Is it a Container, as the Container might have a child Placemark?
                if (feature is Container container)
                {
                    // Check each Feature to see if it's a Placemark or another Container
                    foreach (Feature f in container.Features)
                    {
                        ExtractPlacemarks(f, placemarks);
                    }
                }
            }
        }
    }
}
