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

If the class you are extending is a base class for others, then the above
technique won't work. In this scenario you can register an extension as a valid
child type for the class and use the `AddChild`/`RemoveChild` methods and
`Children` property to manually add/remove/find the element (this can be done
from extension methods to simplify calling code):

```csharp
public static void Main(string[] args)
{
    KmlFactory.RegisterExtension<Feature, MyElement>();
}
```

```csharp
// Convenience methods
public static class MyElementExtensions
{
    public static MyElement GetMyElement(this Feature feature)
    {
        return feature.Children.OfType<MyElement>().FirstOrDefault();
    }

    public static void SetMyElement(this Feature feature, MyElement value)
    {
        // Remove any existing element
        MyElement existing = GetMyElement(feature);
        if (existing != null)
        {
            feature.RemoveChild(existing);
        }

        // Setting the element to null is the same as removing it, so only add
        // non-null values
        if (value != null)
        {
            feature.AddChild(value);
        }
    }
}
```
