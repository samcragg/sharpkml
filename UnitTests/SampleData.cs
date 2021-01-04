using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace UnitTests
{
    internal class SampleData : IDisposable
    {
        private string filename;

        private SampleData()
        {
        }

        public static void CompareElements(Element expected, Element actual)
        {
            static string NormalizeNumbers(string value)
            {
                // When we serialize the number we use extra precision, however,
                // that causes the strings to not be equal. Therefore, replace
                // the decimal numbers with a single digit
                return Regex.Replace(value, @"(?<=\d)+\.\d+", ".1");
            }

            // To compare the Elements we're going to serialize them both
            // and compare the generated Xml.
            var serializer = new Serializer();
            serializer.Serialize(expected);
            string expectedXml = serializer.Xml;

            serializer.Serialize(actual);
            string actualXml = serializer.Xml;
            Assert.That(NormalizeNumbers(actualXml), Is.EqualTo(NormalizeNumbers(expectedXml)));
        }

        public static SampleData CreateFile(string resource)
        {
            var instance = new SampleData();
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("UnitTests." + resource))
            {
                instance.filename = Path.GetTempFileName();
                using FileStream file = File.Create(instance.filename);
                stream.CopyTo(file);
            }
            return instance;
        }

        public static Stream CreateStream(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream("UnitTests." + resource);
            // Don't give them the real stream in case they try to change it!
            var output = new MemoryStream();
            stream.CopyTo(output);
            output.Position = 0; // Reset to the beginning
            return output;
        }

        public Stream OpenRead()
        {
            return File.OpenRead(this.filename);
        }

        public void Dispose()
        {
            if (this.filename != null)
            {
                File.Delete(this.filename);
                this.filename = null;
            }
        }
    }
}
