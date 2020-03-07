// Copyright (c) Samuel Cragg.
//
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.

namespace SharpKml.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a Kmz archive, containing Kml data and associated files.
    /// </summary>
    /// <remarks>
    /// The entire Kmz archive (in its compressed state) will be held in memory
    /// until a call to <see cref="KmzFile.Dispose"/> is made.
    /// </remarks>
    public sealed class KmzFile : IDisposable
    {
        // This is the default name for writing a KML file to a new archive,
        // however, the default file for reading from an archive is the first
        // file in the table of contents that ends with ".kml".
        private const string DefaultKmlFilename = "doc.kml";

        // The ZipArchive makes changes to the stream when we dispose it but
        // we need access to the stream to copy it to another stream etc.
        private readonly MemoryStream zipStream;
        private ZipArchive zip;

        private KmzFile(Stream stream = null)
        {
            this.zipStream = new MemoryStream();
            if (stream != null)
            {
                stream.CopyTo(this.zipStream);
            }

            this.CreateZipArchive();
        }

        /// <summary>
        /// Gets or sets the default string encoding to use when extracting
        /// the Kml from a Kmz archive. Defaults to UTF8.
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets the filenames for the entries contained in the archive.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance or the
        /// stream was closed.
        /// </exception>
        public IEnumerable<string> Files
        {
            get
            {
                this.ThrowIfDisposed();
                return this.zip.Entries.Select(e => e.FullName);
            }
        }

        /// <summary>
        /// Creates a new KmzFile using the data specified in the KmlFile.
        /// </summary>
        /// <param name="kml">The Kml data to add to the archive.</param>
        /// <returns>
        /// A new KmzFile populated with the data specified in the KmlFile.
        /// </returns>
        /// <remarks>
        /// This overloaded version does not resolve any links in the Kml data
        /// and, therefore, will not add any local references to the archive.
        /// </remarks>
        /// <exception cref="ArgumentNullException">kml is null.</exception>
        public static KmzFile Create(KmlFile kml)
        {
            Check.IsNotNull(kml, nameof(kml));

            var instance = new KmzFile();
            ZipArchiveEntry entry = instance.zip.CreateEntry(DefaultKmlFilename);
            using (Stream stream = entry.Open())
            {
                kml.Save(stream);
            }

            return instance;
        }

        /// <summary>Opens a KmzFile from the specified stream.</summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <returns>A KmzFile representing the specified stream.</returns>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="IOException">
        /// The Kmz archive is not in the expected format.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support reading.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The stream was closed.
        /// </exception>
        public static KmzFile Open(Stream stream)
        {
            Check.IsNotNull(stream, nameof(stream));

            return new KmzFile(stream);
        }

        /// <summary>
        /// Adds the specified data to the Kmz archive, using the specified
        /// filename and directory path within the archive.
        /// </summary>
        /// <param name="path">
        /// The name, including any path, to use within the archive.
        /// </param>
        /// <param name="data">The data to add to the archive.</param>
        /// <exception cref="ArgumentException">
        /// path is a zero-length string, contains only white space, or contains
        /// one or more invalid characters as defined by
        /// <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">path/data is null.</exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public void AddFile(string path, byte[] data)
        {
            this.ThrowIfDisposed();
            Check.IsNotNull(data, nameof(data));

            using (var stream = new MemoryStream(data, writable: false))
            {
                this.AddFile(path, stream);
            }
        }

        /// <summary>
        /// Adds the specified data to the Kmz archive, using the specified
        /// filename and directory path within the archive.
        /// </summary>
        /// <param name="path">
        /// The name, including any path, to use within the archive.
        /// </param>
        /// <param name="stream">The data to add to the archive.</param>
        /// <exception cref="ArgumentException">
        /// path is a zero-length string, contains only white space, or contains
        /// one or more invalid characters as defined by
        /// <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">path/data is null.</exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public void AddFile(string path, Stream stream)
        {
            this.ThrowIfDisposed();
            Check.IsNotNullOrWhitespace(path, nameof(path));
            Check.IsNotNull(stream, nameof(stream));

            // GetPathRoot will validate the path for us. If an absolute path
            // is passed in then GetPathRoot will return the root directory
            // (i.e. not return an empty string). However, a relative path of
            // "../directory/file.name" will pass the test, so we manually check
            // for that case, as AddEntry is quite lax compared to the C++ version.
            if (!string.IsNullOrEmpty(Path.GetPathRoot(path)) ||
                path.StartsWith(".", StringComparison.Ordinal))
            {
                throw new ArgumentException("path is invalid.", nameof(path));
            }

            ZipArchiveEntry entry = this.zip.CreateEntry(path);
            using (Stream entryStream = entry.Open())
            {
                stream.CopyTo(entryStream);
            }
        }

        /// <summary>
        /// Releases all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            if (this.zip != null)
            {
                this.zip.Dispose();
                this.zip = null;
            }

            this.zipStream.Dispose();
        }

        /// <summary>
        /// Loads a default <see cref="KmlFile"/> inside this archive.
        /// </summary>
        /// <returns>
        /// A KmlFile representing the default KML file in the specified KMZ archive
        /// or null if no KML data was found.
        /// </returns>
        /// <remarks>
        /// This method checks for duplicate Id's in the file and throws an
        /// exception if duplicate Id's are found. To enable duplicate Id's
        /// use the <see cref="SharpKml.Base.Parser"/> class and pass the root
        /// element to <see cref="KmlFile.Create"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        /// <exception cref="System.Xml.XmlException">
        /// An error occurred while parsing the KML.
        /// </exception>
        public KmlFile GetDefaultKmlFile()
        {
            string kml = this.ReadKml();
            if (kml != null)
            {
                using (var reader = new StringReader(kml))
                {
                    return KmlFile.Load(reader);
                }
            }

            return null;
        }

        /// <summary>Extracts the specified file from the Kmz archive.</summary>
        /// <param name="path">
        /// The file, including directory information, to locate in the archive.
        /// </param>
        /// <returns>
        /// A byte array if the specified value parameter was found in the
        /// archive; otherwise, null.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public byte[] ReadFile(string path)
        {
            this.ThrowIfDisposed();

            if (!string.IsNullOrEmpty(path))
            {
                ZipArchiveEntry file = this.zip.GetEntry(path);
                if (file != null)
                {
                    return ExtractResource(file);
                }
            }

            return null;
        }

        /// <summary>Extracts the default Kml file from the archive.</summary>
        /// <returns>
        /// A string containing the Kml content if a suitable file was found in
        /// the Kmz archive; otherwise, null.
        /// </returns>
        /// <remarks>
        /// This returns the first file in the Kmz archive table of contents
        /// which has a ".kml" extension. Note that the file found may not
        /// necessarily be in the root directory.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public string ReadKml()
        {
            this.ThrowIfDisposed();

            ZipArchiveEntry kml = this.zip.Entries.FirstOrDefault(
                e => string.Equals(".kml", Path.GetExtension(e.FullName), StringComparison.OrdinalIgnoreCase));

            if (kml != null)
            {
                using (Stream stream = kml.Open())
                using (var reader = new StreamReader(stream, DefaultEncoding))
                {
                    return reader.ReadToEnd();
                }
            }

            return null;
        }

        /// <summary>Removes the specified file from the Kmz archive.</summary>
        /// <param name="path">
        /// The file, including directory information, to locate in the archive.
        /// </param>
        /// <returns>
        /// true if the specified file was found in the archive and successfully
        /// removed; otherwise, false.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public bool RemoveFile(string path)
        {
            this.ThrowIfDisposed();

            if (!string.IsNullOrEmpty(path))
            {
                ZipArchiveEntry entry = this.zip.GetEntry(path);
                if (entry != null)
                {
                    entry.Delete();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves this instance to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="ArgumentException">stream is not writable.</exception>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support writing.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance or the
        /// stream was closed.
        /// </exception>
        public void Save(Stream stream)
        {
            this.ThrowIfDisposed();
            Check.IsNotNull(stream, nameof(stream));

            // ZipFile doesn't commit the changes until it's disposed
            this.zip.Dispose();

            try
            {
                this.zipStream.Position = 0;
                this.zipStream.CopyTo(stream);
            }
            finally
            {
                // Try to set things back to how they were
                this.CreateZipArchive();
            }
        }

        /// <summary>
        /// Replaces the specified file's contents in the Kmz archive with the
        /// specified data.
        /// </summary>
        /// <param name="path">
        /// The name, including any path, of the file within the archive.
        /// </param>
        /// <param name="data">The data to add to the archive.</param>
        /// <exception cref="ArgumentNullException">path/data is null.</exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="Dispose"/> has been called on this instance.
        /// </exception>
        public void UpdateFile(string path, byte[] data)
        {
            this.ThrowIfDisposed();
            Check.IsNotNull(path, nameof(path));
            Check.IsNotNull(data, nameof(data));

            ZipArchiveEntry entry = this.zip.GetEntry(path);
            if (entry != null)
            {
                using (Stream stream = entry.Open())
                {
                    stream.SetLength(0);
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        private static byte[] ExtractResource(ZipArchiveEntry entry)
        {
            using (var ms = new MemoryStream())
            using (Stream stream = entry.Open())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private void CreateZipArchive()
        {
            this.zipStream.Position = 0;
            this.zip = new ZipArchive(this.zipStream, ZipArchiveMode.Update, leaveOpen: true);
        }

        private void ThrowIfDisposed()
        {
            // We set zip to null once it's been disposed
            if (this.zip == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
