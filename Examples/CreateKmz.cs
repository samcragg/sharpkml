using System;
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
                using (KmzFile kmz = KmzFile.Create(input))
                {
                    kmz.Save(OutputPath);
                    Console.WriteLine("Saved to '{0}'.", OutputPath);
                }

                // Now open the file we saved and list the contents
                using (KmzFile kmz = KmzFile.Open(OutputPath))
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
    }
}
