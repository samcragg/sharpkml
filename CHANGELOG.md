# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.0.3] - 2018-02-27
### Changed
- Fix for https://github.com/samcragg/sharpkml/issues/5
  - SimpleArrayData was trying to return the inner text of the created elements,
    however, it hadn't copied it accross from the UnknownElement. This has been
    fixed so now the nested values can be retrieved.
  - SimpleArrayData didn't have any values if namespaces were being ignored
    during parsing - this has now been fixed.

## [3.0.2] - 2018-02-04
### Changed
- Fix for https://github.com/samcragg/sharpkml/issues/4
  - For improved compatability with Google Earth, `innerBoundaryIs` with
    multiple `LinearyRing` elements (which is invalid) is now handled without
    loss of information.
  - Made `Element.AddChild` virtual
  - Modified `InnerBoundary` to check if it already has a `LinearRing` set - if
    it does then it will try to add a new `InnerBoundary` to its parent


## [3.0.1] - 2017-11-04
### Changed
- Updated package to target .NET Standard 1.2
- Changed license from MS-PL to MIT
