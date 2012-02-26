using System;
using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace UnitTests.Engine
{
    [TestFixture]
    public class ElementExtensionsTest
    {
        [Test]
        public void TestClone()
        {
            Placemark placemark = null;
            Assert.That(() => placemark.Clone(),
                        Throws.TypeOf<ArgumentNullException>());

            placemark = new Placemark();
            placemark.Id = "ID";
            placemark.Geometry = new Point();
            var clone = placemark.Clone();

            Assert.That(clone, Is.Not.SameAs(placemark));
            Assert.That(clone.Id, Is.EqualTo(placemark.Id));  // Test attribute
            Assert.That(clone.Geometry, Is.Not.Null); // Test element

            // Make sure as we change one the other doesn't
            ((Point)clone.Geometry).Extrude = true;
            Assert.That(((Point)clone.Geometry).Extrude,
                       Is.Not.EqualTo(((Point)placemark.Geometry).Extrude));

            Folder folder = new Folder();
            folder.AddFeature(placemark);
            folder.AddFeature(new Document());

            var copy = folder.Clone();
            Assert.That(copy.Features.Count(), Is.EqualTo(folder.Features.Count()));
            placemark.Id = "changed";
            Assert.That(folder.FindFeature(placemark.Id), Is.Not.Null);
            Assert.That(copy.FindFeature(placemark.Id), Is.Null);
        }

        [Test]
        public void TestCloneIcon()
        {
            // This is an oddball case because there are two Kml <icon>'s
            var link = new IconStyle.IconLink(new Uri("link", UriKind.Relative));
            var clone = link.Clone();
            Assert.That(clone.Href, Is.EqualTo(link.Href));
            Assert.That(link.Parent, Is.Null);
            Assert.That(clone.Parent, Is.Null);

            IconStyle iconStyle = new IconStyle();
            iconStyle.Icon = link;
            clone = iconStyle.Icon.Clone();
            Assert.That(clone.Href, Is.EqualTo(iconStyle.Icon.Href));

            Icon icon = new Icon();
            icon.Id = "icon";
            var iconClone = icon.Clone();
            Assert.That(iconClone.Id, Is.EqualTo(icon.Id));
        }

        [Test]
        public void TestGetParent()
        {
            Placemark placemark = null;
            Assert.That(() => placemark.GetParent<Folder>(),
                        Throws.TypeOf<ArgumentNullException>());

            placemark = new Placemark();
            Assert.That(placemark.GetParent<Folder>(), Is.Null);

            var folder = new Folder();
            folder.AddFeature(placemark);
            Assert.That(placemark.GetParent<Folder>(), Is.SameAs(folder));
            Assert.That(placemark.GetParent<Kml>(), Is.Null);

            var kml = new Kml();
            kml.Feature = folder;
            Assert.That(placemark.GetParent<Kml>(), Is.SameAs(kml));
        }

        [Test]
        public void TestIsChildOf()
        {
            Placemark placemark = null;
            Assert.That(() => placemark.IsChildOf<Folder>(),
                        Throws.TypeOf<ArgumentNullException>());

            placemark = new Placemark();
            Assert.False(placemark.IsChildOf<Folder>());

            var folder = new Folder();
            folder.AddFeature(placemark);
            Assert.True(placemark.IsChildOf<Folder>());
            Assert.False(placemark.IsChildOf<Kml>());

            var kml = new Kml();
            kml.Feature = folder;
            Assert.True(placemark.IsChildOf<Kml>());
        }

        [Test]
        public void TestMerge()
        {
            Placemark placemark = null;
            Placemark target = null;
            Assert.That(() => target.Merge(placemark),
                        Throws.TypeOf<ArgumentNullException>());

            placemark = new Placemark();
            Assert.That(() => target.Merge(placemark),
                        Throws.TypeOf<ArgumentNullException>());

            // Test basic merge on empty target
            placemark.Id = "source";
            Point point = new Point();
            point.Coordinate = new Vector(1.1, -1.1);
            placemark.Geometry = point;

            target = new Placemark();
            target.Merge(placemark);
            Assert.That(target.Id, Is.EqualTo(placemark.Id));
            Assert.That(((Point)target.Geometry).Coordinate, Is.EqualTo(((Point)placemark.Geometry).Coordinate));
            Assert.That(target.Geometry, Is.Not.SameAs(placemark.Geometry)); // Make sure it's not the same instance

            // Test overwrite
            target = new Placemark();
            target.Id = "target";
            target.Name = "target";
            target.Geometry = new Point();

            target.Merge(placemark);
            Assert.That(target.Id, Is.EqualTo(placemark.Id));
            Assert.That(target.Name, Is.EqualTo("target"));
            Assert.That(((Point)target.Geometry).Coordinate, Is.EqualTo(((Point)placemark.Geometry).Coordinate));
            Assert.That(target.Geometry, Is.Not.SameAs(placemark.Geometry));
        }

        [Test]
        public void TestMergeChildren()
        {
            var source = new Folder();
            source.AddFeature(new Folder { Id = "SubFolder1" });
            source.AddFeature(new Folder { Id = "SubFolder2" });

            var target = new Folder();
            target.AddFeature(new Placemark { Id = "Placemark1" });
            target.AddFeature(new Placemark { Id = "Placemark2" });

            target.Merge(source);
            Assert.That(target.Features.Count(), Is.EqualTo(4));
            Assert.That(target.Features.ElementAt(3).Id, Is.EqualTo("SubFolder2"));
        }

        [Test]
        public void TestMergeStyle()
        {
            const string SourceStyle =
                "<Style>" +
                "  <IconStyle>" +
                "     <scale>1.3</scale>" +
                "     <Icon>" +
                "       <href>cool.jpeg</href>" +
                "     </Icon>" +
                "  </IconStyle>" +
                "</Style>";

            const string TargetStyle =
                "<Style>" +
                "  <IconStyle>" +
                "     <scale>1.5</scale>" +
                "     <heading>123</heading>" +
                "  </IconStyle>" +
                "</Style>";

            Parser parser = new Parser();
            parser.ParseString(SourceStyle, false);
            Style source = parser.Root as Style;

            parser.ParseString(TargetStyle, false);
            Style target = parser.Root as Style;

            target.Merge(source);

            // Make sure merge worked correctly and only affected the target
            Assert.That(source.Icon.Scale, Is.EqualTo(1.3));
            Assert.That(target.Icon.Scale, Is.EqualTo(1.3));

            Assert.That(source.Icon.Heading, Is.Null);
            Assert.That(target.Icon.Heading, Is.EqualTo(123));

            Assert.That(source.Icon.Icon.Href, Is.EqualTo(new Uri("cool.jpeg", UriKind.Relative)));
            Assert.That(target.Icon.Icon.Href, Is.EqualTo(new Uri("cool.jpeg", UriKind.Relative)));
        }
    }
}
