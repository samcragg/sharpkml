# Project Description

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/v/SharpKml.Core.svg)](https://www.nuget.org/packages/SharpKml.Core/) [![Build status](https://ci.appveyor.com/api/projects/status/13t6474tofy4gjb0?svg=true)](https://ci.appveyor.com/project/samcragg/sharpkml)

SharpKML is an implementation of the
[Open Geospatial Consortium (OGC) KML 2.2 standard](http://www.opengeospatial.org/standards/kml/)
 developed in C#, able to read/write both KML files and KMZ files.

The library has been based on Google's C++ implementation of the standard
([libkml](http://code.google.com/p/libkml/)), however, instead of using the SWIG
bindings the code has been written from scratch to give a more C# feel, by using
properties, extension methods and the built in XML handling of the .NET
framework.

## Getting Started

The easiest way to use the library is to install the
[SharpKml.Core NuGet package](https://www.nuget.org/packages/SharpKml.Core/),
however, you can also download the binaries/source code from this project page.

Check out the [Getting Started](docs/GettingStarted.md) page for some useful
documentation.
