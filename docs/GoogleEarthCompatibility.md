# Google Earth Compatibility

To use some of the advanced features offered by Google Earth in your Kml files
(such as a tour) there exists the several classes in the `SharpKml.Dom.GX`
namespace to help you.

However, for the results to display correctly in Google Earth, the Google
extension namespace must be added to the root `kml` node. Below is a simple
example that creates a tour.

```csharp
using System;
using System.Reflection;
using System.Xml;
using SharpKml.Base;
using SharpKml.Dom;

class Program
{
    static void Main(string[]() args)
    {
        var flyTo = new SharpKml.Dom.GX.FlyTo
        {
            Mode = SharpKml.Dom.GX.FlyToMode.Bounce,
        };

        var tour = new SharpKml.Dom.GX.Tour
        {
            Playlist = new SharpKml.Dom.GX.Playlist(),
            Playlist.AddTourPrimitive(flyTo),
        };

        var kml = new Kml();
        kml.AddNamespacePrefix(KmlNamespaces.GX22Prefix, KmlNamespaces.GX22Namespace);
        kml.Feature = tour;

        Serializer serializer = new Serializer();
        serializer.Serialize(kml);
        Console.WriteLine(serializer.Xml);
    }
}
```
