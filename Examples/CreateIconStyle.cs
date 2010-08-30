using System;
using SharpKml.Base;
using SharpKml.Dom;

namespace Examples
{
    /// <summary>
    /// Creates a simple IconStyle, based on the example given at
    /// http://code.google.com/apis/kml/documentation/kmlreference.html#iconstyle
    /// </summary>
    public static class CreateIconStyle
    {
        public static void Run()
        {
            // Create the style first
            var style = new Style();
            style.Id = "randomColorIcon";
            style.Icon = new IconStyle();
            style.Icon.Color = new Color32(255, 0, 255, 0);
            style.Icon.ColorMode = ColorMode.Random;
            style.Icon.Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/pal3/icon21.png"));
            style.Icon.Scale = 1.1;

            // Now create the object to apply the style to
            var placemark = new Placemark();
            placemark.Name = "IconStyle.kml";
            placemark.StyleUrl = new Uri("#randomColorIcon", UriKind.Relative);
            placemark.Geometry = new Point
            {
                Coordinate = new Vector(37.831145, -122.36868)
            };

            // Package it all together...
            var document = new Document();
            document.AddFeature(placemark);
            document.AddStyle(style);
            
            // And display the result
            var serializer = new Serializer();
            serializer.Serialize(document);
            Console.WriteLine(serializer.Xml);
        }
    }
}
