# Extracting information from a Kml file

The library can be used to load a Kml file in memory and then iterate over its
elements. To do this we need to load the file from disk:

```csharp
// This will read a Kml file into memory.
KmlFile file = KmlFile.Load("YourKmlFile.kml");

// Kmz (compressed Kml files) can also be loaded:
KmzFile kmz = KmzFile.Open("YourKmzFile.kmz");
KmlFile file = KmlFile.LoadFromKmz(kmz);
```

Once the file is loaded you can iterate over the elements inside. Here's a quick
example that uses `System.Linq` extensions:

```csharp
// Make sure these are placed at the top of your file:
// using System.Linq;
// using SharpKml.Engine;

// It's good practice for the root element of the file to be a Kml element,
// though not compulsory
Kml kml = file.Root as Kml;
if (kml != null)
{
    foreach (var placemark in kml.Flatten().OfType<Placemark>())
    {
        Console.WriteLine(placemark.Name);
    }
}
```
