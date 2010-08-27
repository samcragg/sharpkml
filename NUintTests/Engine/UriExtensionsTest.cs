using System;
using NUnit.Framework;
using SharpKml.Engine;

namespace NUintTests.Engine
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
                Assert.AreEqual(test.Resolved, uri);

                if (test.KmzBase != null)
                {
                    Uri kmz = uri.KmzUrl();
                    Assert.AreEqual(test.KmzBase, kmz);
                    Assert.AreNotSame(uri, kmz);

                    uri = kmz.Relative(test.Target);
                    Assert.AreEqual(test.KmzRelative, uri);
                }
            }
        }

        [Test]
        public void TestFragment()
        {
            Uri uri = null;
            Assert.Throws<ArgumentNullException>(() => uri.GetFragment()); // uri hasn't been created yet

            uri = CreateUri("http://example.com/path?query=here#fragment-text");
            Assert.AreEqual("fragment-text", uri.GetFragment());

            uri = CreateUri("/realative#fragment-text");
            Assert.AreEqual("fragment-text", uri.GetFragment());

            uri = CreateUri("/no-fragment-text");
            Assert.IsNullOrEmpty(uri.GetFragment());
        }

        [Test]
        public void TestKmzSplit()
        {
            Uri uri = null;
            Assert.Throws<ArgumentNullException>(() => uri.SplitKmz()); // uri hasn't been created yet

            uri = CreateUri("http://host.com/bldgs/model-macky.kmz/photos/bh-flowers.jpg");
            var kmz = uri.SplitKmz();

            Assert.AreEqual(CreateUri("http://host.com/bldgs/model-macky.kmz"), kmz.Item1);
            Assert.AreEqual(CreateUri("photos/bh-flowers.jpg"), kmz.Item2);

            uri = CreateUri("http://example.com/path/archive.KMZ");
            kmz = uri.SplitKmz();
            Assert.AreEqual(CreateUri("http://example.com/path/archive.KMZ"), kmz.Item1);
            Assert.AreNotSame(uri, kmz.Item1);
            Assert.IsNull(kmz.Item2);

            uri = CreateUri("http://example.com");
            Assert.IsNull(uri.SplitKmz());
        }

        [Test]
        public void TestNormalize()
        {
            Uri uri = null;
            Assert.Throws<ArgumentNullException>(() => uri.Normalize()); // uri hasn't been created yet

            uri = CreateUri("this/../is/a/relative/../uri.x");
            Uri normalized = uri.Normalize();

            Assert.AreEqual(CreateUri("is/a/uri.x"), normalized);
            Assert.AreNotSame(normalized, normalized.Normalize());

            uri = CreateUri(@"this\..\a\relative\url.kmz\..\file.kml#id");
            Assert.AreEqual(CreateUri("a/relative/file.kml#id"), uri.Normalize());

            uri = CreateUri("this/../a/relative/url.kmz/../file.kml#id");
            Assert.AreEqual(CreateUri("a/relative/file.kml#id"), uri.Normalize());
        }

        [Test]
        public void TestPath()
        {
            Uri uri = null;
            Assert.Throws<ArgumentNullException>(() => uri.GetPath()); // uri hasn't been created yet

            uri = CreateUri("http://example.com/path/x/y?query=here#fragment-text");
            Assert.AreEqual("path/x/y", uri.GetPath());

            uri = CreateUri("/relative/x/y#fragment-text");
            Assert.AreEqual("relative/x/y", uri.GetPath());

            uri = CreateUri("#no-path");
            Assert.IsNullOrEmpty(uri.GetPath());

            uri = CreateUri("../relative.kml");
            Assert.AreEqual("../relative.kml", uri.GetPath());
        }

        [Test]
        public void TestRelative()
        {
            Uri uri = null;
            Assert.Throws<ArgumentNullException>(() => uri.Relative(null)); // uri hasn't been created yet

            uri = CreateUri("http://host.com/path/file.kml");

            // These should be the same (but not the same instance)
            Assert.AreEqual(uri, uri.Relative(null));
            Assert.AreNotSame(uri, uri.Resolve(null, null));
            Assert.AreEqual(uri, uri.Relative(CreateUri(string.Empty)));

            // Basic combine
            Uri target = CreateUri("image.jpg");
            Assert.AreEqual(CreateUri("http://host.com/path/image.jpg"), uri.Relative(target));

            uri = CreateUri("http://foo.com");
            target = CreateUri("file.kml");
            Assert.AreEqual(CreateUri("http://foo.com/file.kml"), uri.Relative(target));
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
            Assert.Throws<ArgumentNullException>(() => uri.Resolve(null, null)); // uri hasn't been created yet

            uri = CreateUri(Base);

            Assert.AreEqual(uri, uri.Resolve(null, null));
            Assert.AreNotSame(uri, uri.Resolve(null, null));
            Assert.AreEqual(uri, uri.Resolve(CreateUri(string.Empty), CreateUri(string.Empty)));

            Uri output = uri.Resolve(CreateUri(ModelHref), CreateUri(TargetHref));
            Assert.AreEqual(CreateUri(Result), output);
        }

        // these are more a test that Uri performs the same as the C++ code expects
        /*public void TestSplitUri()
        {
            // Verify behavior of a URI with all desired components.
            const string Scheme = "http";
            const string Host = "example.com";
            const string Port = "8081";
            const string Path = "x/y/z";
            const string Query = "name=val";
            const string Fragment = "some-fragment";
            const string Url = Scheme + "://" + Host + ":" + Port + "/" + Path + "?" + Query + "#" + Fragment;

            Uri uri = CreateUri(Url);
            Assert(uri.Fragment.Substring(1) == Fragment);
            Assert(uri.Host == Host);
            Assert(uri.LocalPath.Substring(1) == Path);
            Assert(uri.Port.ToString() == Port);
            Assert(uri.Query.Substring(1) == Query);
            Assert(uri.Scheme == Scheme);
        }

        public void TestFilename()
        {
            const string File = @"C:\home\libkml\foo.bar";
            const string Expected = @"file:///C:/home/libkml/foo.bar";

            Uri uri = CreateUri(File);
            Assert(uri != null && uri.ToString() == Expected);

            uri = CreateUri(Expected);
            Assert(uri != null && uri.LocalPath == File);
        }*/

        private static Uri CreateUri(string uri)
        {
            Uri output;
            Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out output);
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
