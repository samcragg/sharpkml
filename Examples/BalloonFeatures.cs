using System;
using SharpKml.Dom;
using SharpKml.Engine;

namespace Examples
{
    /// <summary>
    /// Iterates over all the Features in a Kml file and displays an information balloon
    /// </summary>
    public static class BalloonFeatures
    {
        private const string InputFile = "Sample.kml";

        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to show the features of:");

            if ((file != null) && (file.Root != null))
            {
                EntityMapper mapper = new EntityMapper(file);
                foreach (var element in file.Root.Flatten())
                {
                    Feature feature = element as Feature;
                    if (feature != null)
                    {
                        string name = feature.Name ?? "Unnamed feature";
                        string balloon = mapper.CreateBalloonText(feature);

                        Console.WriteLine("Feature balloon text for '{0}'\n{1}\n", name, balloon);
                    }
                }
            }
        }
    }
}
