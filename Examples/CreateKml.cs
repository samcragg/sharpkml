using System;
using SharpKml.Base;
using SharpKml.Dom;

namespace Examples
{
    /// <summary>
    /// Creates a Point and Placemark and prints the resultant KML on to the console.
    /// </summary>
    public static class CreateKml
    {
        public static void Run()
        {
            Console.WriteLine("Creating a point at 37.42052549 latitude and -122.0816695 longitude.\n");

            // This will be used for the placemark
            Point point = new Point();
            point.Coordinate = new Vector(37.42052549, -122.0816695);

            Placemark placemark = new Placemark();
            placemark.Name = "Cool Statue";
            placemark.Geometry = point;

            // This is the root element of the file
            Kml kml = new Kml();
            kml.Feature = placemark;

            Serializer serializer = new Serializer();
            serializer.Serialize(kml);
            Console.WriteLine(serializer.Xml);

            Console.WriteLine("\nReading Xml...");

            Parser parser = new Parser();
            parser.ParseString(serializer.Xml, true);

            kml = (Kml)parser.Root;
            placemark = (Placemark)kml.Feature;
            point = (Point)placemark.Geometry;

            Console.WriteLine("Latitude:{0} Longitude:{1}", point.Coordinate.Latitude, point.Coordinate.Longitude);
        }
    }
}
