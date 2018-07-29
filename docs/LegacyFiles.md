# Legacy Files

The OGC KML specification specifies that Kml elements should be inside the
`http://www.opengis.com/kml/2.2` namespace. However, before the specification
was finalized, programs may have used different namespaces (a common example
being `http://earth.google.com/kml/2.2`) and you may need to work with those
files. One way of doing this is use a `Parser` as follows:

```csharp
using System;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

class Program
{
    const string Xml =
        "<kml xmlns='http://earth.google.com/kml/2.2'>" +
            "<Placemark>" +
                "<name>SamplePlacemark</name>" +
            "</Placemark>" +
        "</kml>";

    static KmlFile ParseLegacyFile()
    {
        // Manually parse the Xml
        Parser parser = new Parser();
        parser.ParseString(Xml, false); // Ignore the namespaces

        // The element will be stored in parser.Root - wrap it inside
        // a KmlFile
        return KmlFile.Create(parser.Root, true);
    }

    static void Main(string[]() args)
    {
        KmlFile file = ParseLegacyFile();

        // Prove it worked ok
        Kml kml = (Kml)file.Root;
        Placemark placemark = (Placemark)kml.Feature;
        Console.WriteLine(placemark.Name); // Outputs "SamplePlacemark"
    }
}
```

Please note when the file is saved, the OGC namespace will be used, overwriting
the original namespace (this is consistent with `libkml`).
