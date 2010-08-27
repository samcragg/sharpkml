using System;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine; // Needed for the Clone extension method

namespace Examples
{
    /// <summary>
    /// Creates a single Region and adds it to two placemarks by using the
    /// Clone extension method (normally there's an exception when you add
    /// an Element to another instance when it already belongs to an Element)
    /// </summary>
    public static class Clone
    {
        public static void Run()
        {
            // Create a Container and some Features.
            var placemark1 = new Placemark();
            var placemark2 = new Placemark();

            var folder = new Folder();
            folder.AddFeature(placemark1);
            folder.AddFeature(placemark2);

            // Add a copy of the region to each Placemark
            var region = CreateRegion();
            placemark1.Region = region.Clone();
            placemark2.Region = region.Clone();

            // Display the result
            var serializer = new Serializer();
            serializer.Serialize(folder);
            Console.WriteLine(serializer.Xml);
        }

        private static Region CreateRegion()
        {
            var box = new LatLonAltBox();
            box.North = 1;
            box.South = 2;
            box.East = 3;
            box.West = 4;

            var lod = new Lod();
            lod.MinimumPixels = 100;

            var region = new Region();
            region.LatLonAltBox = box;
            region.LevelOfDetail = lod;
            return region;
        }
    }
}
