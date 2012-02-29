using System;
using NUnit.Framework;
using SharpKml.Engine;

namespace UnitTests.Engine
{
    [TestFixture]
    public class UriExtensionsTest
    {
        private static readonly TestCase[] TestCases = new TestCase[]
        {
            new TestCase("base/must/have/scheme/to/be/valid",
                         "image.jpg",
                         null),

            new TestCase("http://a.com/x",
                         "y",
                         "http://a.com/y"),

            new TestCase("http://host.com/path/file.kml",
                         "image.jpg",
                         "http://host.com/path/image.jpg"),

            new TestCase("http://host.com/path/file.kml",
                         "http://otherhost.com/dir/image.jpg",
                         "http://otherhost.com/dir/image.jpg"),

            new TestCase("http://host.com/kmz/screenoverlay-continents.kmz/doc.kml",
                         "pngs/africa.png",
                         "http://host.com/kmz/screenoverlay-continents.kmz/pngs/africa.png",
                         "http://host.com/kmz/screenoverlay-continents.kmz",
                         "http://host.com/kmz/pngs/africa.png"),

            new TestCase("http://host.com/kmz/rumsey/kml/lc01.kmz/L_and_C/kml/01.kml",
                         "../imagery/01_4.png",
                         "http://host.com/kmz/rumsey/kml/lc01.kmz/L_and_C/imagery/01_4.png",
                         "http://host.com/kmz/rumsey/kml/lc01.kmz",
                         "http://host.com/kmz/rumsey/imagery/01_4.png"),

            new TestCase("http://host.com/path/file.kmz/doc.kml",
                         "image.jpg",
                         "http://host.com/path/file.kmz/image.jpg",
                         "http://host.com/path/file.kmz",
                         "http://host.com/path/image.jpg")
        };

        [Test]
        public void TestGeneralCases()
        {
            foreach (var test in TestCases)
            {
                Uri uri = test.Base.Relative(test.Target);
                Assert.That(uri, Is.EqualTo(test.Resolved));

                if (test.KmzBase != null)
                {
                    Uri kmz = uri.KmzUrl();
                    Assert.That(kmz, Is.EqualTo(test.KmzBase));
                    Assert.That(kmz, Is.Not.SameAs(uri));

                    uri = kmz.Relative(test.Target);
                    Assert.That(uri, Is.EqualTo(test.KmzRelative));
                }
            }
        }

        [Test]
        public void TestFragment()
        {
            Uri uri = null;
            Assert.That(() => uri.GetFragment(),
                        Throws.TypeOf<ArgumentNullException>()); // uri hasn't been created yet

            uri = CreateUri("http://example.com/path?query=here#fragment-text");
            Assert.That(uri.GetFragment(), Is.EqualTo("fragment-text"));

            uri = CreateUri("/realative#fragment-text");
            Assert.That(uri.GetFragment(), Is.EqualTo("fragment-text"));

            uri = CreateUri("/no-fragment-text");
            Assert.That(uri.GetFragment(), Is.Null.Or.Empty);
        }

        [Test]
        public void TestKmzSplit()
        {
            Uri uri = null;
            Assert.That(() => uri.SplitKmz(),
                        Throws.TypeOf<ArgumentNullException>()); // uri hasn't been created yet

            uri = CreateUri("http://host.com/bldgs/model-macky.kmz/photos/bh-flowers.jpg");
            var kmz = uri.SplitKmz();

            Assert.That(kmz.Item1, Is.EqualTo(CreateUri("http://host.com/bldgs/model-macky.kmz")));
            Assert.That(kmz.Item2, Is.EqualTo(CreateUri("photos/bh-flowers.jpg")));

            uri = CreateUri("http://example.com/path/archive.KMZ");
            kmz = uri.SplitKmz();
            Assert.That(kmz.Item1, Is.EqualTo(CreateUri("http://example.com/path/archive.KMZ")));
            Assert.That(kmz.Item1, Is.Not.SameAs(uri));
            Assert.That(kmz.Item2, Is.Null);

            uri = CreateUri("http://example.com");
            Assert.That(uri.SplitKmz(), Is.Null);
        }

        [Test]
        public void TestNormalize()
        {
            Uri uri = null;
            Assert.That(() => uri.Normalize(),
                        Throws.TypeOf<ArgumentNullException>()); // uri hasn't been created yet

            uri = CreateUri("this/../is/a/relative/../uri.x");
            Uri normalized = uri.Normalize();

            Assert.That(normalized, Is.EqualTo(CreateUri("is/a/uri.x")));
            Assert.That(normalized.Normalize(), Is.Not.SameAs(normalized));

            uri = CreateUri(@"this\..\a\relative\url.kmz\..\file.kml#id");
            Assert.That(uri.Normalize(), Is.EqualTo(CreateUri("a/relative/file.kml#id")));

            uri = CreateUri("this/../a/relative/url.kmz/../file.kml#id");
            Assert.That(uri.Normalize(), Is.EqualTo(CreateUri("a/relative/file.kml#id")));
        }

        [Test]
        public void TestPath()
        {
            Uri uri = null;
            Assert.That(() => uri.GetPath(),
                        Throws.TypeOf<ArgumentNullException>()); // uri hasn't been created yet

            uri = CreateUri("http://example.com/path/x/y?query=here#fragment-text");
            Assert.That(uri.GetPath(), Is.EqualTo("path/x/y"));

            uri = CreateUri("/relative/x/y#fragment-text");
            Assert.That(uri.GetPath(), Is.EqualTo("relative/x/y"));

            uri = CreateUri("#no-path");
            Assert.That(uri.GetPath(), Is.Null.Or.Empty);

            uri = CreateUri("../relative.kml");
            Assert.That(uri.GetPath(), Is.EqualTo("../relative.kml"));
        }

        [Test]
        public void TestRelative()
        {
            Uri uri = null;
            Assert.That(() => uri.Relative(null),
                        Throws.TypeOf<ArgumentNullException>()); // uri hasn't been created yet

            uri = CreateUri("http://host.com/path/file.kml");

            // These should be the same (but not the same instance)
            Assert.That(uri.Relative(null), Is.EqualTo(uri));
            Assert.That(uri.Resolve(null, null), Is.Not.SameAs(uri));
            Assert.That(uri.Relative(CreateUri(string.Empty)), Is.EqualTo(uri));

            // Basic combine
            Uri target = CreateUri("image.jpg");
            Assert.That(uri.Relative(target), Is.EqualTo(CreateUri("http://host.com/path/image.jpg")));

            uri = CreateUri("http://foo.com");
            target = CreateUri("file.kml");
            Assert.That(uri.Relative(target), Is.EqualTo(CreateUri("http://foo.com/file.kml")));
        }

        [Test]
        public void TestResolve()
        {
            // Verify behavior for a common case.
            const string Base = "http://host.com/dir/foo.kmz/doc.kml";
            const string ModelHref = "dir/bldg.dae";
            const string TargetHref = "../textures/brick.jpg";
            const string Result = "http://host.com/dir/foo.kmz/textures/brick.jpg";

            Uri uri = null;
            Assert.That(() => uri.Resolve(null, null),
                        Throws.TypeOf<ArgumentNullException>()); // uri hasn't been created yet

            uri = CreateUri(Base);

            Assert.That(uri.Resolve(null, null), Is.EqualTo(uri));
            Assert.That(uri.Resolve(null, null), Is.Not.SameAs(uri));
            Assert.That(uri.Resolve(CreateUri(string.Empty), CreateUri(string.Empty)), Is.EqualTo(uri));

            Uri output = uri.Resolve(CreateUri(ModelHref), CreateUri(TargetHref));
            Assert.That(output, Is.EqualTo(CreateUri(Result)));
        }

        private static Uri CreateUri(string uri)
        {
            // UriKind.RelativeOrAbsolute doesn't work too well under OS X (and possible Linux) so
            // firt try a relative then an absolute uri
            Uri output;
            if (!Uri.TryCreate(uri, UriKind.Relative, out output))
            {
                Uri.TryCreate(uri, UriKind.Absolute, out output);
            }
            return output;
        }

        private class TestCase
        {
            public TestCase(string _base, string _target, string _resolved, string _kBase, string _kRel)
                : this(_base, _target, _resolved)
            {
                KmzBase = CreateUri(_kBase);
                KmzRelative = CreateUri(_kRel);
            }

            public TestCase(string _base, string _target, string _resolved)
            {
                Base = CreateUri(_base);
                Target = CreateUri(_target);
                if (_resolved != null)
                {
                    Resolved = CreateUri(_resolved);
                }
            }

            public Uri Base { get; private set; }
            public Uri Target { get; private set; }
            public Uri Resolved { get; private set; }
            public Uri KmzBase { get; private set; }
            public Uri KmzRelative { get; private set; }
        }
    }
}
