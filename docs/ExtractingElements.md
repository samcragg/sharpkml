# Extracting Elements

A common requirement for processing Kml files is to extract a particular element
from the file and then do some processing with them. For example, we might want
to extract the `Polygon`s from the following Kml:

```xml
<?xml version='1.0' encoding='UTF-8'?>
<kml xmlns='http://www.opengis.net/kml/2.2'>
  <Document>
    <name>Polygon.kml</name>
    <open>0</open>
    <Placemark>
      <name>hollow box</name>
      <Polygon id='TestPolygon'>
        <extrude>1</extrude>
        <altitudeMode>relativeToGround</altitudeMode>
        <outerBoundaryIs>
          <LinearRing>
            <coordinates>
              -122.366278,37.818844,30
              -122.365248,37.819267,30
              -122.365640,37.819861,30
              -122.366669,37.819429,30
              -122.366278,37.818844,30
            </coordinates>
          </LinearRing>
        </outerBoundaryIs>
        <innerBoundaryIs>
          <LinearRing>
            <coordinates>
              -122.366212,37.818977,30
              -122.365424,37.819294,30
              -122.365704,37.819731,30
              -122.366488,37.819402,30
              -122.366212,37.818977,30
            </coordinates>
          </LinearRing>
        </innerBoundaryIs>
      </Polygon>
    </Placemark>
  </Document>
</kml>
```

To do that we can use the `Flatten` extension method, which enables us to
iterate all the elements and can then filter them out using LINQ, like so:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Text;
using SharpKml.Dom;
using SharpKml.Engine;

class Program
{
   const string Xml =
@"<?xml version='1.0' encoding='UTF-8'?>
<kml>
... as above ...
</kml>";

    static void Main(string[]() args)
    {
        // First get the Kml into a KmlFile object
        KmlFile file;
        using (var stream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(Xml)))
        {
            file = KmlFile.Load(stream);
        }

        // Use the Flatten extension method to iterate over all the elements
        // then use the Linq extension OfType to select only Polygons
        foreach (var poly in file.Root.Flatten().OfType<Polygon>())
        {
            Console.WriteLine(poly.Id);
        }

        Console.ReadKey();
    }
}
```
