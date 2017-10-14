## Project Description

SharpKML is an implementation of the [Open Geospatial Consortium (OGC) KML 2.2 standard](http://www.opengeospatial.org/standards/kml/) developed in C#, able to read/write both KML files and KMZ files.

The library has been based on Google's C{"++"} implementation of the standard ([libkml](http://code.google.com/p/libkml/)), however, instead of using the SWIG bindings the code has been written from scratch to give a more C# feel, by using properties, extension methods and the built in Xml handling of the .NET framework.

## Getting Started

The easiest way to use the library is to install the [SharpKml.Core NuGet package](https://www.nuget.org/packages/SharpKml.Core/), however, you can also download the binaries/source code from this project page.

## Compatibility

The main library for handling KML data targets the Portable Class Libraries, so is compatible with .NET Framework 4 and higher, Silverlight 5, Windows Phone 8, Windows Store apps (Windows 8) and higher.

If you need to support KMZ compressed archives then there is a separate library which targets the .NET 4 Framework. This library has a dependency on [DotNetZip](http://dotnetzip.codeplex.com/) for the handling of ZIP archives; the download for the KMZ part bundles the core library, the KMZ library and the DotNetZip (reduced) binaries.

