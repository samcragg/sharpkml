using System;
using System.IO;
using System.Reflection;
using SharpKml.Engine;

namespace Examples
{
    class Program
    {
        private static readonly Tuple<string, Type>[] Samples =
        {
            Tuple.Create("BalloonFeatures.cs", typeof(BalloonFeatures)),
            Tuple.Create("Change.cs", typeof(Change)),
            Tuple.Create("Clone.cs", typeof(Clone)),
            Tuple.Create("CreateIconStyle.cs", typeof(CreateIconStyle)),
            Tuple.Create("CreateKml.cs", typeof(CreateKml)),
            Tuple.Create("CreateKmz.cs", typeof(CreateKmz)),
            Tuple.Create("InlineStyles.cs", typeof(InlineStyles)),
            Tuple.Create("ParseKml.cs", typeof(ParseKml)),
            Tuple.Create("ShowStyles.cs", typeof(ShowStyles)),
            Tuple.Create("SortPlacemarks.cs", typeof(SortPlacemarks)),
            Tuple.Create("SplitStyles.cs", typeof(SplitStyles))
        };

        public static void Main(string[] args)
        {
            CreateSampleData();

            while (true)
            {
                Console.Title = string.Empty;
                Console.ResetColor();
                Console.Clear();
                int choice = GetChoice();
                if (choice == 0) // Exit
                {
                    break;
                }

                // Run the test
                Console.Clear();
                Console.Title = Samples[choice - 1].Item1;

                Type type = Samples[choice - 1].Item2;
                type.GetMethod("Run").Invoke(null, null); // This is a static method

                // Display the end message
                Console.ResetColor();
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(); // Add a bit of a space between the end of the test and our message

                int bottom = Math.Max(Console.CursorTop, Console.WindowHeight - 1);
                Console.SetCursorPosition(0, bottom);
                Console.Write("Sample completed. Press any key to continue...".PadRight(Console.WindowWidth));
                Console.SetCursorPosition(46, bottom);
                Console.WindowTop -= 1; // Filling the whole line causes the console to scroll down for us... scroll back up.
                Console.ReadKey();
            }
        }

        // Helper method to ask for a filename and try and open it.
        // Not used by this class but put here for so all samples can use it.
        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public static string GetInputFile(string prompt, string defaultFile)
        {
            Console.WriteLine(prompt);
            Console.Write("(Leave blank to use " + defaultFile + ")");

            Console.SetCursorPosition(prompt.Length + 1, 0);
            string filename = Console.ReadLine();
            Console.Clear();

            if (string.IsNullOrEmpty(filename))
            {
                return defaultFile;
            }
            return filename;
        }

        // Helper method to ask for a filename and try and open it.
        // Not used by this class but put here for so all samples can use it.
        public static KmlFile OpenFile(string prompt)
        {
            string filename = GetInputFile(prompt, "Data/Sample.kml");

            KmlFile file;
            try
            {
                file = KmlFile.Load(filename);
            }
            catch (Exception ex)
            {
                DisplayError(ex.GetType() + "\n" + ex.Message);
                return null;
            }

            if (file.Root == null)
            {
                DisplayError("Unable to find any recognized Kml in the specified file.");
                return null;
            }
            return file;
        }

        private static void CopyResource(string name, string destination)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                using (var file = File.OpenWrite(destination))
                {
                    stream.CopyTo(file);
                }
            }
        }

        private static void CreateSampleData()
        {
            Directory.CreateDirectory("Data/files"); // Need the sub-directory for the Kmz test
            CopyResource("Examples.Data.doc.kml", "Data/doc.kml");
            CopyResource("Examples.Data.Sample.kml", "Data/Sample.kml");
            CopyResource("Examples.Data.camera_mode.png", "Data/files/camera_mode.png");
            CopyResource("Examples.Data.zermatt.jpg", "Data/files/zermatt.jpg");
        }

        private static int GetChoice()
        {
            Console.WriteLine(" 0 - Exit the program.\n");
            int index = 1;
            foreach (var sample in Samples)
            {
                Console.WriteLine("{0,2} - {1}", index++, sample.Item1);
            }
            Console.WriteLine();

            // Keep looping until we get a sensible value
            int choice = 0;
            while (true)
            {
                Console.Write("Please enter a number: ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out choice))
                {
                    if ((choice >= 0) && (choice <= Samples.Length))
                    {
                        return choice;
                    }
                }
            }
        }
    }
}
