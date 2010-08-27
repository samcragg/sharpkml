using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using SharpKml.Base;
using SharpKml.Dom;

namespace NUintTests
{
    internal class SampleData : IDisposable
    {
        private string _filename;

        private SampleData()
        {
        }

        public static void CompareElements(Element expected, Element actual)
        {
            // To compare the Elements we're going to serialize them both
            // and compare the generated Xml.
            Serializer serializer = new Serializer();
            serializer.Serialize(expected);
            string expectedXml = serializer.Xml;

            serializer.Serialize(actual);
            Assert.AreEqual(expectedXml, serializer.Xml);
        }

        public static SampleData CreateFile(string resource)
        {
            SampleData instance = new SampleData();
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("NUintTests." + resource))
            {
                instance._filename = Path.GetTempFileName();
                using (var file = File.OpenWrite(instance._filename))
                {
                    stream.CopyTo(file);
                }
            }
            return instance;
        }

        public static Stream CreateStream(string resource)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("NUintTests." + resource))
            {
                // Don't give them the real stream in case they try to change it!
                MemoryStream output = new MemoryStream();
                stream.CopyTo(output);
                output.Position = 0; // Reset to the begining
                return output;
            }
        }

        public Stream OpenRead()
        {
            return File.OpenRead(_filename);
        }

        public void Dispose()
        {
            if (_filename != null)
            {
                File.Delete(_filename);
                _filename = null;
            }
        }
    }
}
