using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace UnitTests.Engine
{
    // Test currently disabled for silverlight as there is no Kmz support available
    // due to a bug in DotNetZip under silverlight.
#if !SILVERLIGHT
    [TestFixture]
    public class KmzFileTest
    {
        [Test]
        public void TestBasicOpen()
        {
            // Try a valid archive
            using (var stream = SampleData.CreateStream("Engine.Data.Doc.kmz"))
            using (var file = KmzFile.Open(stream))
            {
                Assert.That(file, Is.Not.Null);
                var kml = file.ReadKml();
                Assert.That(kml, Is.Not.Null.Or.Empty);
            }

            // Now try a valid archive with no Kml information
            using (var stream = SampleData.CreateStream("Engine.Data.NoKml.kmz"))
            using (var file = KmzFile.Open(stream))
            {
                var kml = file.ReadKml();
                Assert.That(kml, Is.Null);
            }

            // Now try an invalid archive (i.e. something that isn't a zip)
            using (var stream = SampleData.CreateStream("Engine.Data.Bounds.kml"))
            {
#if SILVERLIGHT
                var exception = typeof(IOException);
#else
                var exception = typeof(InvalidDataException);
#endif
                Assert.That((TestDelegate)(() => KmzFile.Open(stream)),
                            Throws.TypeOf(exception));
            }
        }

        [Test]
        public void TestDispose()
        {
            // Make sure we can't call any methods after the object has been disposed
            var file = KmzFile.Create(KmlFile.Create(new Kml(), false));
            file.Dispose();

            // Make sure we can call Dispose more than once
            Assert.That(() => file.Dispose(), Throws.Nothing);

            // Check all methods and properties throw.
            var flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
            foreach (var method in typeof(KmzFile).GetMethods(flags))
            {
                // Dispose should be able to be called mutliple times without
                // an exception being thrown.
                if (method.Name != "Dispose")
                {
                    // Actual exception thrown will be TargetInvocationException
                    // so check the InnerException.
                    object[] arguments = new object[method.GetParameters().Length];
                    Assert.That(() => method.Invoke(file, arguments),
                                Throws.InnerException.TypeOf<ObjectDisposedException>());
                }
            }
            foreach (var property in typeof(KmzFile).GetProperties(flags))
            {
                // Mono doesn't throw a TargetInvocationException so just check that the property
                // throws, as it doesn't throw any other type of exception.
                Assert.That(() => property.GetValue(file, null),
                            Throws.Exception);
            }
        }

        [Test]
        public void TestList()
        {
            // Make sure Files returns the entries in the correct order
            string[] expected =
            {
                "z/c.kml",
                "b.kml",
                "a/a.kml"
            };

            using (var stream = SampleData.CreateStream("Engine.Data.MultiKml.kmz"))
            using (var file = KmzFile.Open(stream))
            {
                Assert.That(file.Files.Count(), Is.EqualTo(expected.Length));

                int index = 0;
                foreach (var name in file.Files)
                {
                    Assert.That(name, Is.EqualTo(expected[index++]));
                }
            }
        }

        [Test]
        public void TestReadFile()
        {
            // NoKml.kmz has a file called foo.txt in a folder called foo.
            using (var stream = SampleData.CreateStream("Engine.Data.NoKml.kmz"))
            using (var file = KmzFile.Open(stream))
            {
                byte[] data = file.ReadFile("foo/foo.txt");
                Assert.That(data, Is.Not.Null);

                // The archive does not have a file called bar.txt in that folder
                byte[] invalid = file.ReadFile("foo/bar.txt");
                Assert.That(invalid,Is.Null);
            }
        }

        [Test]
        public void TestReadKml()
        {
            // Doc.kmz has two Kml files at the root level, a.kml and doc.kml,
            // which were added to the archive in that order. Assert that a.kml
            // is read instead of doc.kml.
            using (var stream = SampleData.CreateStream("Engine.Data.Doc.kmz"))
            using (var file = KmzFile.Open(stream))
            {
                var kml = file.ReadKml();
                Assert.That(kml.IndexOf("a.kml"), Is.Not.EqualTo(-1)); // Make sure the right file was found
            }

            // MultiKml.kmz has three Kml files added in the following order:
            // - z/c.kml
            // - b.kml
            // - a/a.kml
            // Each file has a placemark whose <name> is the archived filename.
            using (var stream = SampleData.CreateStream("Engine.Data.MultiKml.kmz"))
            using (var file = KmzFile.Open(stream))
            {
                var kml = file.ReadKml();
                Assert.That(kml.IndexOf("c.kml"), Is.Not.EqualTo(-1)); // Make sure z/c.kml was read
            }
        }

#if SILVERLIGHT
        [Test]
        public void TestSave()
        {
            // Create the Kml data
            const string Xml = "<Placemark xmlns='http://www.opengis.net/kml/2.2'><name>tmp kml</name></Placemark>";
            var parser = new Parser();
            parser.ParseString(Xml, true);
            var kml = KmlFile.Create(parser.Root, false);

            using (var stream = new MemoryStream())
            {
                // Create and save the archive
                using (var file = KmzFile.Create(kml))
                {
                    file.Save(stream);
                }

                // Try to open the saved archive, rewinding the stream
                stream.Position = 0;
                using (var file = KmzFile.Open(stream))
                {
                    // Make sure it's the same as what we saved
                    parser.ParseString(file.ReadKml(), true);
                    SampleData.CompareElements(kml.Root, parser.Root);
                }
            }
        }
#else
        [Test]
        public void TestSave()
        {
            // Create the Kml data
            const string Xml = "<Placemark xmlns='http://www.opengis.net/kml/2.2'><name>tmp kml</name></Placemark>";
            var parser = new Parser();
            parser.ParseString(Xml, true);
            var kml = KmlFile.Create(parser.Root, false);

            // This will be where we temporary save the archive to
            string tempFile = Path.GetTempFileName();
            try
            {
                // Create and save the archive
                using (var file = KmzFile.Create(kml))
                {
                    file.Save(tempFile);
                }

                // Try to open the saved archive
                using (var file = KmzFile.Open(tempFile))
                {
                    // Make sure it's the same as what we saved
                    parser.ParseString(file.ReadKml(), true);
                    SampleData.CompareElements(kml.Root, parser.Root);
                }
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
#endif

        [Test]
        public void TestAddFile()
        {
            using (var kmz = CreateArchive())
            {
                // Check AddFile handles invalid names correctly
                byte[] empty = new byte[] { };
                Assert.That(() => kmz.AddFile(null, empty),
                            Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => kmz.AddFile(string.Empty, null),
                            Throws.TypeOf<ArgumentNullException>());

                Assert.That(() => kmz.AddFile(string.Empty, empty), // Empty string
                            Throws.TypeOf<ArgumentException>());
                Assert.That(() => kmz.AddFile("  ", empty), // Whitespace
                            Throws.TypeOf<ArgumentException>());
                Assert.That(() => kmz.AddFile("../invalid.kml", empty), // Path points above the archive
                            Throws.TypeOf<ArgumentException>());
                Assert.That(() => kmz.AddFile("/also/invalid.kml", empty), // Path is absolute
                            Throws.TypeOf<ArgumentException>());

                // Verify the archive contains the files in the correct order
                string[] expected =
                {
                    "doc.kml",
                    "files/new.kml",
                    "other/blah.kml"
                };

                Assert.That(kmz.Files.Count(), Is.EqualTo(expected.Length));
                int index = 0;
                foreach (var name in kmz.Files)
                {
                    Assert.That(name, Is.EqualTo(expected[index++]));
                }
            }
        }

        [Test]
        public void TestRemoveFile()
        {
            using (var kmz = CreateArchive())
            {
                Assert.That( kmz.Files.Count(), Is.EqualTo(3));
                Assert.False(kmz.RemoveFile(null));
                Assert.False(kmz.RemoveFile("RandomFile"));
                Assert.True(kmz.RemoveFile("files/new.kml"));
                Assert.True(kmz.RemoveFile("doc.kml"));

                Assert.That(kmz.Files.Count(), Is.EqualTo(1));
                Assert.That(kmz.Files.ElementAt(0), Is.EqualTo("other/blah.kml"));
            }
        }

        [Test]
        public void TestUpdateFile()
        {
            using (var kmz = CreateArchive())
            {
                Assert.That(kmz.Files.ElementAt(0), Is.EqualTo("doc.kml"));

                const string Data = "Updated value";
                kmz.UpdateFile("doc.kml", Encoding.Unicode.GetBytes(Data));

                // Make sure the value has changed but the order hasn't
                byte[] bytes = kmz.ReadFile("doc.kml");
                string data = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                Assert.That(data, Is.EqualTo(Data));
                Assert.That( kmz.Files.ElementAt(0), Is.EqualTo("doc.kml"));

                // Make sure that an invalid entry isn't added to the archive
                byte[] empty = new byte[] { };
                kmz.UpdateFile("unknown.txt", empty);
                Assert.False(kmz.Files.Contains("unknown.txt"));

                // Try some invalid values
                Assert.That(() => kmz.UpdateFile(string.Empty, null),
                            Throws.TypeOf<ArgumentNullException>());
                Assert.That(() => kmz.UpdateFile(null, empty),
                            Throws.TypeOf<ArgumentNullException>());
            }
        }

        private static KmzFile CreateArchive()
        {
            // Create an empty KmlFile
            var kml = KmlFile.Create(new Kml(), false);

            // This adds a "doc.kml" for us
            var file = KmzFile.Create(kml);

            // Add a couple more files and return
            file.AddFile("files/new.kml", Encoding.Unicode.GetBytes("new.kml"));
            file.AddFile("other/blah.kml", Encoding.Unicode.GetBytes("blah.kml"));
            return file;
        }
    }
#endif
}
