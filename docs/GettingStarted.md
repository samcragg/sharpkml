# Getting Started

A good resource for how to use KML in general is the
[KML Reference](https://developers.google.com/kml/documentation/kmlreference)
from Google; this library has been designed to follow the names of KML elements
as closely as possible.

For a quick example of how the library works, see the
[Basic Usage](BasicUsage.md) page or the
[Extracting information from a Kml file](ExtractingInformation.md) page.

For working with legacy files (for example, files created with an earlier
version Google Earth), please see the [Legacy Files](LegacyFiles.md) page. Or
for using some of the features offered in Google Earth, take a look at
[Google Earth compatibility](GoogleEarthCompatibility.md) page.

For a simple example of working with the elements inside a KML file, including
extracting a specific type, see the [Extracting Elements](ExtractingElements.md)
page.

If you need to do some advanced work with KML data, you may want to look at how
you can [extend the elements](Extending.md).

For more examples, please see the [Examples folder](../Examples), which includes:

| File               | Description                                                                                                                                                                                 |
|--------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| BalloonFeatures.cs | Displays an information balloon text for all the Features in a KML file.                                                                                                                    |
| Change.cs          | Shows how KML can be updated by an `Update` element, and how to use the `Process` extension method to apply the update in code.                                                         |
| Clone.cs           | Shows how to copy `Element`s (an `Element` can only have one parent and throws an exception if you try to add it to another `Element`.)                                               |
| CreateIconStyle.cs | Creates a simple `IconStyle`, based on the example given at the [IconStyle documentation](https://developers.google.com/kml/documentation/kmlreference#iconstyle)                         |
| CreateKml.cs       | A simple example of creating a KML file from code.                                                                                                                                          |
| CreateKmz.cs       | A simple example of how to create a KMZ file, which is a compressed archive containing a KML file and related resources for that file.                                                      |
| InlineStyles.cs    | Uses the `StyleResolver.InlineStyles` static method to create a new `Element` that has the styles inside the `Feature`, instead of using shared styles and specifying a `StyleUrl`. |
| ParseKml.cs        | Shows how to use the `Parser` class to parse a KML file, ignoring any namespaces declared in the XML.                                                                                     |
| ShowStyles.cs      | Iterates over all the styles in a KML file and displays their names in alphabetical order.                                                                                                  |
| SortPlacemarks.cs  | Iterates over the placemarks in a file and displays their names in alphabetical order.                                                                                                      |
| SplitStyles.cs     | Moves styles from a Feature to the enclosing Document and then sets the Feature.StyleUrl to reference the new style.                                                                        |
