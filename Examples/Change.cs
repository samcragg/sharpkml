using System;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace Examples
{
    /// <summary>
    /// Uses the Update.Process extension method to process an update.
    /// </summary>
    public static class Change
    {
        public static void Run()
        {
            // Create our Kml
            var folder = new Folder();
            folder.Id = "f0";
            folder.Name = "Folder 0";

            var placemark = new Placemark();
            placemark.Id = "pm0";
            placemark.Name = "Placemark 0";
            folder.AddFeature(placemark);

            placemark = new Placemark();
            placemark.Id = "pm1";
            placemark.Name = "Placemark 1";
            folder.AddFeature(placemark);

            var kml = new Kml();
            kml.Feature = folder;

            // Display to the user
            var serializer = new Serializer();
            serializer.Serialize(kml);
            Console.WriteLine("Original Kml:\n" + serializer.Xml);

            // This is what we're going to change to
            placemark = new Placemark();
            placemark.Geometry = new Point { Coordinate = new Vector(38, -120) };
            placemark.Name = "new name";
            placemark.TargetId = "pm0";

            var update = new Update();
            update.AddUpdate(new ChangeCollection() { placemark });

            serializer.Serialize(update);
            Console.WriteLine("\nUpdate:\n" + serializer.Xml);

            // Run the update
            var file = KmlFile.Create(kml, false);
            update.Process(file);

            serializer.Serialize(kml);
            Console.WriteLine("\nUpdated Kml:\n" + serializer.Xml);
        }
    }
}
