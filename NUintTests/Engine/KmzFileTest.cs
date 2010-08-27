using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;

namespace NUintTests.Engine
{
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
                Assert.IsNotNull(file);
                var kml = file.ReadKml();
                Assert.IsNotNullOrEmpty(kml);
            }

            // Now try a valid archive with no Kml information
            using (var stream = SampleData.CreateStream("Engine.Data.NoKml.kmz"))
            using (var file = KmzFile.Open(stream))
            {
                var kml = file.ReadKml();
                Assert.IsNull(kml);
            }

            // Now try an invalid archive (i.e. somehting that isn't a zip)
            using (var stream = SampleData.CreateStream("Engine.Data.Bounds.kml"))
            {
                Assert.Throws<InvalidDataException>(() => KmzFile.Open(stream));
            }
        }

        [Test]
        public void TestDispose()
        {
            // Make sure we can't call any methods after the object has been disposed
            var file = KmzFile.Create(KmlFile.Create(new Kml(), false));
            file.Dispose();

            Assert.Throws<ObjectDisposedException>(() => file.AddFile(null, null));
            Assert.Throws<ObjectDisposedException>(() => { var temp = file.Files; });
            Assert.Throws<ObjectDisposedException>(() => file.ReadFile(null));
            Assert.Throws<ObjectDisposedException>(() => file.ReadKml());
            Assert.Throws<ObjectDisposedException>(() => file.RemoveFile(null));
            Assert.Throws<ObjectDisposedException>(() => file.Save((Stream)null));
            Assert.Throws<ObjectDisposedException>(() => file.Save(string.Empty));
            Assert.Throws<ObjectDisposedException>(() => file.UpdateFile(null, null));

            // Though we can call Dispose more than once
            Assert.DoesNotThrow(() => file.Dispose());
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
                Assert.AreEqual(expected.Length, file.Files.Count());

                int index = 0;
                foreach (var name in file.Files)
                {
                    Assert.AreEqual(expected[index++], name);
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
                Assert.IsNotNull(data);

                // The archive does not have a file called bar.txt in that folder
                byte[] invalid = file.ReadFile("foo/bar.txt");
                Assert.IsNull(invalid);
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
                Assert.IsTrue(kml.IndexOf("a.kml") != -1); // Make sure the right file was found
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
                Assert.IsTrue(kml.IndexOf("c.kml") != -1); // Make sure z/c.kml was read
            }
        }

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

        [Test]
        public void TestAddFile()
        {
            using (var kmz = CreateArchive())
            {
                // Check AddFile handles invalid names correctly
                byte[] empty = new byte[] { };
                Assert.Throws<ArgumentNullException>(() => kmz.AddFile(null, null));
                Assert.Throws<ArgumentNullException>(() => kmz.AddFile(null, empty));
                Assert.Throws<ArgumentNullException>(() => kmz.AddFile(string.Empty, null));

                Assert.Throws<ArgumentException>(() => kmz.AddFile(string.Empty, empty)); // Empty string
                Assert.Throws<ArgumentException>(() => kmz.AddFile("  ", empty)); // Whitespace
                Assert.Throws<ArgumentException>(() => kmz.AddFile("../invalid.kml", empty)); // Path points above the archive
                Assert.Throws<ArgumentException>(() => kmz.AddFile("/also/invalid.kml", empty)); // Path is absolute

                // Verify the archive contains the files in the correct order
                string[] expected =
                {
                    "doc.kml",
                    "files/new.kml",
                    "other/blah.kml"
                };

                Assert.AreEqual(expected.Length, kmz.Files.Count());
                int index = 0;
                foreach (var name in kmz.Files)
                {
                    Assert.AreEqual(expected[index++], name);
                }
            }
        }

        [Test]
        public void TestRemoveFile()
        {
            using (var kmz = CreateArchive())
            {
                Assert.AreEqual(3, kmz.Files.Count());
                Assert.IsFalse(kmz.RemoveFile(null));
                Assert.IsFalse(kmz.RemoveFile("RandomFile"));
                Assert.IsTrue(kmz.RemoveFile("files/new.kml"));
                Assert.IsTrue(kmz.RemoveFile("doc.kml"));

                Assert.AreEqual(1, kmz.Files.Count());
                Assert.AreEqual("other/blah.kml", kmz.Files.ElementAt(0));
            }
        }

        [Test]
        public void TestUpdateFile()
        {
            using (var kmz = CreateArchive())
            {
                Assert.AreEqual("doc.kml", kmz.Files.ElementAt(0));

                const string Data = "Updated value";
                kmz.UpdateFile("doc.kml", ASCIIEncoding.Default.GetBytes(Data));

                // Make sure the value has changed but the order hasn't
                string data = ASCIIEncoding.Default.GetString(kmz.ReadFile("doc.kml"));
                Assert.AreEqual(Data, data);
                Assert.AreEqual("doc.kml", kmz.Files.ElementAt(0));

                // Make sure that an invalid entry isn't added to the archive
                byte[] empty = new byte[] { };
                kmz.UpdateFile("unknown.txt", empty);
                Assert.IsFalse(kmz.Files.Contains("unknown.txt"));

                // Try some invalid values
                Assert.Throws<ArgumentNullException>(() => kmz.UpdateFile(null, null));
                Assert.Throws<ArgumentNullException>(() => kmz.UpdateFile(string.Empty, null));
                Assert.Throws<ArgumentNullException>(() => kmz.UpdateFile(null, empty));
            }
        }

        private static KmzFile CreateArchive()
        {
            // Create an empty KmlFile
            var kml = KmlFile.Create(new Kml(), false);

            // This adds a "doc.kml" for us
            var file = KmzFile.Create(kml);

            // Add a couple more files and return
            file.AddFile("files/new.kml", ASCIIEncoding.Default.GetBytes("new.kml"));
            file.AddFile("other/blah.kml", ASCIIEncoding.Default.GetBytes("blah.kml"));
            return file;
        }
    }
}
