# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [5.1.0] - 2019-10-31
### Changes
- Additional argument validation
- `Parser.Parse` can now be made to parse legacy KML files

## [5.0.1] - 2019-04-29
### Fixed
- Serialization of decimal values is now consistent across cultures

## [5.0.0] - 2019-03-01
### Fixed
- Serialization of elements should match the order in the specification
### Changes
- Collection classes are now sealed
- Ignore DTD processing when reading the XML

## [4.1.0] - 2019-02-11
### Added
- New constructor to `Serializer` that can be used to specify options for when
  floating numbers are serialized

## [4.0.1] - 2018-11-13
### Fixed
- Parsing of decimal degrees with large numbers of digits

## [4.0.0] - 2018-08-19
### Changed
- Parsing is now more performant (up to 50% in certain scenarios)
- Parsing/serialization now only supports properties on `Element`s with both
  getters and setters
- Parsing of coordinate collections now stops further processing if the correct
  tuple separator (whitespace) is not present
### Fixed
- Extension elements can now be used in derived classes

## [3.4.0] - 2018-08-09
### Added
- `KmlFactory` can now register additional valid child types
### Changed
- Element children are stored in serialization order (slightly improves
  parsing/serialization performance)
- Element children are now exposed publicly

## [3.3.0] - 2018-07-29
### Added
- Registered types can now be replaced (via `KmlFactory.Replace`)
### Changed
- All classes representing KML elements are no longer marked as `sealed`

## [3.2.0] - 2018-07-14
### Changed
- Reduced memory usage for large files with lots of elements (thanks @sylvaneau)

## [3.1.0] - 2018-04-29
### Added
- Arithmetic operators for the Vector class (thanks @MikDal002)

## [3.0.5] - 2018-04-18
### Fixed
- Fix for https://github.com/samcragg/sharpkml/issues/8
  - When serializing, prefixes are not used if the namespace of the element
    matches the default namespace in scope.

## [3.0.4] - 2018-03-08
### Added
- Support for .NET 4.5

## [3.0.3] - 2018-02-27
### Fixed
- Fix for https://github.com/samcragg/sharpkml/issues/5
  - SimpleArrayData was trying to return the inner text of the created elements,
    however, it hadn't copied it across from the UnknownElement. This has been
    fixed so now the nested values can be retrieved.
  - SimpleArrayData didn't have any values if namespaces were being ignored
    during parsing - this has now been fixed.

## [3.0.2] - 2018-02-04
### Fixed
- Fix for https://github.com/samcragg/sharpkml/issues/4
  - For improved compatibility with Google Earth, `innerBoundaryIs` with
    multiple `LinearRing` elements (which is invalid) is now handled without
    loss of information.
  - Made `Element.AddChild` virtual
  - Modified `InnerBoundary` to check if it already has a `LinearRing` set - if
    it does then it will try to add a new `InnerBoundary` to its parent

## [3.0.1] - 2017-11-04
### Changed
- Updated package to target .NET Standard 1.2
- Changed license from MS-PL to MIT
