using System;
using System.IO;
using System.Net;

namespace SharpKml.Engine
{
    /// <summary>
    /// Handles downloading of files from a URI, such as local files or network files.
    /// </summary>
    internal static class FileHandler
    {
        /// <summary>
        /// Reads a file from either http, ftp or local and returns a stream to
        /// its contents.
        /// </summary>
        /// <param name="uri">The uri to obtain the data from.</param>
        /// <returns>The file contents as a read-only stream.</returns>
        /// <exception cref="IOException">
        /// An error occurred reading the file. See the
        /// <see cref="Exception.InnerException"/> for more details.
        /// </exception>
        public static Stream OpenRead(Uri uri)
        {
            return new MemoryStream(ReadBytes(uri), false);
        }

        /// <summary>
        /// Reads a file from either http, ftp or local and returns its entire contents.
        /// </summary>
        /// <param name="uri">The uri to obtain the data from.</param>
        /// <returns>The file contents.</returns>
        /// <exception cref="IOException">
        /// An error occurred reading the file. See the
        /// <see cref="Exception.InnerException"/> for more details.
        /// </exception>
        public static byte[] ReadBytes(Uri uri)
        {
            // Try to convert from relative to absolute
            if (!uri.IsAbsoluteUri)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), uri.OriginalString);
                if (!Uri.TryCreate(path, UriKind.Absolute, out uri))
                {
                    throw new IOException("Path is invalid.");
                }
            }

            using (var client = new WebClient())
            {
                try
                {
                    // The docs says it can't return null, but the code scanner says it could!?
                    return client.DownloadData(uri) ?? new byte[0];
                }
                catch (WebException ex)
                {
                    throw new IOException("Unable to load file.", ex);
                }
            }
        }
    }
}
