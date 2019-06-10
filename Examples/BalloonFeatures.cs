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
        public static void Run()
        {
            KmlFile file = Program.OpenFile("Enter a file to show the features of:");

            if ((file != null) && (file.Root != null))
            {
                var mapper = new EntityMapper(file);
                foreach (Element element in file.Root.Flatten())
                {
                    if (element is Feature feature)
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
