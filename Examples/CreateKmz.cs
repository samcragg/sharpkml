using System;
using System.IO;
using SharpKml.Engine;

namespace Examples
{
    /// <summary>
    /// Creates a Kmz archive from the specified Kml file.
    /// </summary>
    public static class CreateKmz
    {
        public static void Run()
        {
            const string OutputPath = "output.kmz";
            string input = Program.GetInputFile("Enter a file to convert to Kmz:", "Data/doc.kml");

            try
            {
                KmlFile kml = LoadKml(input);

                using (KmzFile kmz = SaveKmlAndLinkedContentIntoAKmzArchive(kml, input))
                using (Stream output = File.Create(OutputPath))
                {
                    kmz.Save(output);
                    Console.WriteLine("Saved to '{0}'.", OutputPath);
                }

                // Now open the file we saved and list the contents
                using (Stream file = File.OpenRead(OutputPath))
                using (KmzFile kmz = KmzFile.Open(file))
                {
                    Console.WriteLine("Contents:");
                    foreach (string name in kmz.Files)
                    {
                        Console.WriteLine(name);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.DisplayError(ex.GetType() + "\n" + ex.Message);
            }
        }

        private static KmlFile LoadKml(string path)
        {
            using (Stream file = File.OpenRead(path))
            {
                return KmlFile.Load(file);
            }
        }

        private static KmzFile SaveKmlAndLinkedContentIntoAKmzArchive(KmlFile kml, string path)
        {
            // All the links in the KML will be relative to the KML file, so
            // find it's directory so we can add them later
            string basePath = Path.GetDirectoryName(path);

            // Create the archive with the KML data
            KmzFile kmz = KmzFile.Create(kml);

            // Now find all the linked content in the KML so we can add the
            // files to the KMZ archive
            var links = new LinkResolver(kml);

            // Next gather the local references and add them.
            foreach (string relativePath in links.GetRelativePaths())
            {
                // Make sure it doesn't point to a directory below the base path
                if (relativePath.StartsWith("..", StringComparison.Ordinal))
                {
                    continue;
                }

                // Add it to the archive
                string fullPath = Path.Combine(basePath, relativePath);
                using (var file = File.OpenRead(fullPath))
                {
                    kmz.AddFile(relativePath, file);
                }
            }

            return kmz;
        }
    }
}
