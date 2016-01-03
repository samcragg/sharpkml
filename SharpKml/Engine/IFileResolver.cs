namespace SharpKml.Engine
{
    using System.IO;

    /// <summary>
    /// Allows the loading of external files when resolving styles.
    /// </summary>
    public interface IFileResolver
    {
        /// <summary>
        /// Gets a value indicating whether Kmz files are supported.
        /// </summary>
        bool SupportsKmz { get; }

        /// <summary>
        /// Reads the contents of the specified path.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <returns>
        /// The entire file contents.
        /// </returns>
        byte[] ReadFile(string path);

        /// <summary>
        /// Loads the defaul Kmz file from a Kmz archive.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the Kmz archive contents.
        /// </param>
        /// <returns>
        /// The default<see cref="KmlFile"/> inside the specified archive.
        /// </returns>
        /// <remarks>
        /// This method is only called if <see cref="SupportsKmz"/> returns
        /// <c>true</c> - see the example for a reference implementation.
        /// </remarks>
        /// <example>
        /// <code>
        /// public KmlFile ExtractDefaultKmlFileFromKmzArchive(Stream stream)
        /// {
        ///     using (KmzFile kmz = KmzFile Open(Stream stream))
        ///     {
        ///         return kmz.GetDefaultKmlFile();
        ///     }
        /// }
        /// </code>
        /// </example>
        KmlFile ExtractDefaultKmlFileFromKmzArchive(Stream stream);
    }
}
