# Extending KML

KML elements provide the ability to add custom elements to the ones defined in
the specification. Whilst these won't have a meaning in other applications, the
specification mandates they will be preserved.

To do this, you can inherit from one of the existing elements and then replace
the registration in your program start-up code so that the library will use your
version of the class when it parses the KML element:

```csharp
public class MyPlacemark : Placemark
{
    // Extra elements here
}
```

```csharp
public static void Main(string[] args)
{
    KmlFactory.Replace<Placemark, MyPlacemark>();
}
```