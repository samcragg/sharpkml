# Using with Unity

When building a project with Unity, you'll need to add the following to the
`link.xml` file in order to allow the runtime reflection used by the library to
work. This is because Unity employs
[code stripping](https://docs.unity3d.com/Manual/ManagedCodeStripping.html) to
reduce the generated image, however, this means that it is unfortunately unable
to detect that some of the methods in the library are only accessed via
reflection. Therefore to use SharpKml with a Unity project, you will need to add
a `link.xml` file and place it into the Project Assets folder (or any
subdirectory of Assets), ensuring it has the following content:

```xml
<linker>
  <assembly fullname="SharpKml.Core" preserve="all"/>
  <assembly fullname="System.Core">
    <type fullname="System.Linq.Expressions.Interpreter.LightLambda" preserve="all" />
  </assembly>
</linker>
```
