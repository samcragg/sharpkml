# Basic Usage

There's an excellent introduction and guide to the Kml language available from
Google [here](http://code.google.com/apis/kml/documentation/kml_tut.html) so be
sure to check that out to understand exactly what Kml can do. As mentioned in
the tutorial, one of the most common things to do is to create `Placemark`s, so
that's what we'll do here.

First create a new C# console application and add the
[NuGet package](https://www.nuget.org/packages/SharpKml.Core/) to the project.
This can be done with Visual Studio or can be done with the `dotnet` core
tooling from the command line:

```text
dotnet new console
dotnet add package SharpKml.Core
```

For a simple `Placemark` we need to know where it should be, so we'll use a
`Point` as follows:

```csharp
// This will be the location of the Placemark.
var point = new Point
{
    Coordinate = new Vector(-13.163959, -72.545992),
};
```

This should hopefully be straight forward; the location is stored as a latitude,
longitude in the Coordinate property of the Point element. Now we know where the
`Placemark` will be, let's create it and give it a name:

```csharp
// This is the Element we are going to save to the Kml file.
var placemark = new Placemark
{
    Geometry = point,
    Name = "Machu Picchu",
};
```

The only thing left to do now is to save it to a `Kml` file and then we can open
it up in a Kml viewer (such as Google Earth). The following will save the
`Placemark` in the application directory in a file called `my placemark.kml`:

```csharp
// This allows us to save an Element easily.
KmlFile kml = KmlFile.Create(placemark, false);
using (FileStream stream = File.OpenWrite("my placemark.kml"))
{
    kml.Save(stream);
}
```

That's all there is to it, open the file up in your Kml viewer and you'll see a
push pin over Machu Picchu, labelled with the text we set in the `Placemark.Name`
property.

Here's the complete code for reference:

```csharp
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

class Program
{
    static void Main(string[]() args)
    {
        // This will be the location of the Placemark.
        var point = new Point
        {
            point.Coordinate = new Vector(-13.163959, -72.545992),
        }

        // This is the Element we are going to save to the Kml file.
        var placemark = new Placemark
        {
            Geometry = point,
            Name = "Machu Picchu",
        }

        // This allows us to save and Element easily.
        KmlFile kml = KmlFile.Create(placemark, false);
        using (var stream = System.IO.File.OpenWrite("my placemark.kml"))
        {
            kml.Save(stream);
        }
    }
}
```
