namespace MusicManagementCore.Config
{
    /// <summary>
    /// Contains information which path to scan, if a recursive scan should be perfomed,
    /// and which file extension to look for.
    /// 
    /// <code>
    /// {
    ///     "Input": {
    ///         "Path": "E:\\Music\\Purchased\\Disc\\Album",
    ///         "Recurse": true,
    ///         "Extension": "wav"
    ///     }
    /// }
    /// </code>
    /// </summary>
    public class InputConfig
    {
        /// <summary>
        /// Source directory to scan.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// <c>true</c> to perform a recursive scan or <c>false</c> to only scan
        /// the files located in <cref>Path</cref>.
        /// </summary>
        public bool Recurse { get; set; }

        /// <summary>
        /// Defines the file extension of the files that shall be scanned. The value
        /// only contains the extension without the ".".
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Determines if the source is an album or a compilation. 
        /// See <cref>ToCType</cref> for available options.
        /// </summary>
        public string Type { get; set; }
    }
}
