using System;
using SharpKml.Base;
using SharpKml.Dom;

namespace Examples
{
    [KmlElement(nameof(FeatureExtension), FeatureExtension.XmlNamespace, 18)]
    public class FeatureExtension : Element
    {
        public const string XmlPrefix = "tst";

        public const string XmlNamespace = "http://www.example.com/test";

        [KmlAttribute("attribute")]
        public string Attribute { get; set; }

        [KmlElement("ftSimpleTypeElement", 1)]
        public string SimpleTypeElement { get; set; }

        [KmlElement(null, 2)]
        public ObjectElement ObjectElement
        {
            get => this.objectElement;
            set => this.UpdatePropertyChild(value, ref this.objectElement);
        }

        private ObjectElement objectElement;
    }

    [KmlElement(nameof(PlacemarkExtension), FeatureExtension.XmlNamespace, 2)]
    public class PlacemarkExtension : Element
    {
        [KmlAttribute("attribute")]
        public string Attribute { get; set; }

        [KmlElement("pcSimpleTypeElement", 1)]
        public string SimpleTypeElement { get; set; }
    }

    [KmlElement("ObjectElement", FeatureExtension.XmlNamespace)]
    public class ObjectElement : Element
    {
        [KmlElement("subSimpleTypeElement", 1)]
        public string SubSimpleTypeElement { get; set; }
    }

    public static class ExtendKml
    {
        public static void Run()
        {
            KmlFactory.RegisterExtension<Feature, FeatureExtension>();
            KmlFactory.RegisterExtension<Placemark, PlacemarkExtension>();

            Placemark placemark = new Placemark
            {
                Name = "Placemark"
            };

            placemark.AddChild(new FeatureExtension()
            {
                Attribute = "attributeValue",
                SimpleTypeElement = "simpleTypeValue",
                ObjectElement = new ObjectElement()
                {
                    SubSimpleTypeElement = "subTypeValue"
                }
            });

            placemark.AddChild(new Style()
            {
                Id = "test"
            });

            placemark.AddChild(new PlacemarkExtension()
            {
                Attribute = "attributeValue",
                SimpleTypeElement = "simpleTypeValue"
            });

            // This is the root element of the file
            Kml kml = new Kml
            {
                Feature = placemark
            };

            Serializer serializer = new Serializer();
            serializer.Serialize(kml);
            Console.WriteLine(serializer.Xml);
        }
    }
}
