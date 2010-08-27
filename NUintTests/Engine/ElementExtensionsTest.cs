using System;
using System.Linq;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace NUintTests.Engine
{
    [TestFixture]
    public class ElementExtensionsTest
    {
        [Test]
        public void TestClone()
        {
            Placemark placemark = null;
            Assert.Throws<ArgumentNullException>(() => placemark.Clone());

            placemark = new Placemark();
            placemark.Id = "ID";
            placemark.Geometry = new Point();
            var clone = placemark.Clone();

            Assert.AreNotSame(placemark, clone);
            Assert.AreEqual(placemark.Id, clone.Id);  // Test attribute
            Assert.IsNotNull(clone.Geometry); // Test element

            // Make sure as we change one the other doesn't
            ((Point)clone.Geometry).Extrude = true;
            Assert.AreNotEqual(((Point)placemark.Geometry).Extrude, ((Point)clone.Geometry).Extrude);

            Folder folder = new Folder();
            folder.AddFeature(placemark);
            folder.AddFeature(new Document());

            var copy = folder.Clone();
            Assert.AreEqual(folder.Features.Count(), copy.Features.Count());
            placemark.Id = "changed";
            Assert.IsNotNull(folder.FindFeature(placemark.Id));
            Assert.IsNull(copy.FindFeature(placemark.Id));
        }

        [Test]
        public void TestCloneIcon()
        {
            // This is an oddball case because there are two Kml <icon>'s
            var link = new IconStyle.IconLink(new Uri("link", UriKind.Relative));
            var clone = link.Clone();
            Assert.AreEqual(link.Href, clone.Href);
            Assert.IsNull(link.Parent);
            Assert.IsNull(clone.Parent);

            IconStyle iconStyle = new IconStyle();
            iconStyle.Icon = link;
            clone = iconStyle.Icon.Clone();
            Assert.AreEqual(iconStyle.Icon.Href, clone.Href);

            Icon icon = new Icon();
            icon.Id = "icon";
            var iconClone = icon.Clone();
            Assert.AreEqual(icon.Id, iconClone.Id);
        }

        [Test]
        public void TestGetParent()
        {
            Placemark placemark = null;
            Assert.Throws<ArgumentNullException>(() => placemark.GetParent<Folder>());

            placemark = new Placemark();
            Assert.IsNull(placemark.GetParent<Folder>());

            var folder = new Folder();
            folder.AddFeature(placemark);
            Assert.AreSame(folder, placemark.GetParent<Folder>());
            Assert.IsNull(placemark.GetParent<Kml>());

            var kml = new Kml();
            kml.Feature = folder;
            Assert.AreSame(kml, placemark.GetParent<Kml>());
        }

        [Test]
        public void TestIsChildOf()
        {
            Placemark placemark = null;
            Assert.Throws<ArgumentNullException>(() => placemark.IsChildOf<Folder>());

            placemark = new Placemark();
            Assert.IsFalse(placemark.IsChildOf<Folder>());

            var folder = new Folder();
            folder.AddFeature(placemark);
            Assert.IsTrue(placemark.IsChildOf<Folder>());
            Assert.IsFalse(placemark.IsChildOf<Kml>());

            var kml = new Kml();
            kml.Feature = folder;
            Assert.IsTrue(placemark.IsChildOf<Kml>());
        }

        [Test]
        public void TestMerge()
        {
            Placemark placemark = null;
            Placemark target = null;
            Assert.Throws<ArgumentNullException>(() => target.Merge(placemark));

            placemark = new Placemark();
            Assert.Throws<ArgumentNullException>(() => target.Merge(placemark));

            // Test basic merge on empty target
            placemark.Id = "source";
            Point point = new Point();
            point.Coordinate = new Vector(1.1, -1.1);
            placemark.Geometry = point;

            target = new Placemark();
            target.Merge(placemark);
            Assert.AreEqual(placemark.Id, target.Id);
            Assert.AreEqual(((Point)placemark.Geometry).Coordinate, ((Point)target.Geometry).Coordinate);
            Assert.AreNotSame(placemark.Geometry, target.Geometry); // Make sure it's not the same instance

            // Test overwrite
            target = new Placemark();
            target.Id = "target";
            target.Name = "target";
            target.Geometry = new Point();

            target.Merge(placemark);
            Assert.AreEqual(placemark.Id, target.Id);
            Assert.AreEqual("target", target.Name);
            Assert.AreEqual(((Point)placemark.Geometry).Coordinate, ((Point)target.Geometry).Coordinate);
            Assert.AreNotSame(placemark.Geometry, target.Geometry);
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
            Assert.AreEqual(4, target.Features.Count());
            Assert.AreEqual("SubFolder2", target.Features.ElementAt(3).Id);
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
            Assert.AreEqual(1.3, source.Icon.Scale);
            Assert.AreEqual(1.3, target.Icon.Scale);

            Assert.IsNull(source.Icon.Heading);
            Assert.AreEqual(123, target.Icon.Heading);

            Assert.AreEqual(new Uri("cool.jpeg", UriKind.Relative), source.Icon.Icon.Href);
            Assert.AreEqual(new Uri("cool.jpeg", UriKind.Relative), target.Icon.Icon.Href);
        }
    }
}
