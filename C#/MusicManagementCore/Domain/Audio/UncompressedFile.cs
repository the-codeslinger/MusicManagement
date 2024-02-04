using System;
using System.Collections.Generic;
using System.IO;

namespace MusicManagementCore.Domain.Audio
{
    /// <summary>
    /// Represents a single audio file as found on the filesystem.
    /// </summary>
    public class UncompressedFile
    {
        /// <summary>
        /// The name of the audio file including the extension, excluding the directory
        /// (for example "01 - Winter's Gate.wav").
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// The name of the audio file including the extension and the absolute
        /// directory (for example "D:\Music\Insomnium\01 - Winter's Gate.wav").
        /// </summary>
        public string AbsolutePath { get; }

        /// <summary>
        /// The absolute name of the directory where the audio file was found (for
        /// example "D:\Music\Insomnium").
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// The audio file meta data as inferred from the filename.
        /// </summary>
        public MetaData MetaData { get; }

        /// <summary>
        /// Create a new audio file from a given filename and the associated 
        /// <cref>MetaData</cref>.
        /// </summary>
        /// <param name="filename">Absolute or relative filename of the audio file.</param>
        /// <param name="metaData">The parsed meta data.</param>
        public UncompressedFile(string filename, MetaData metaData)
        {
            Filename = Path.GetFileName(filename);
            AbsolutePath = Path.GetFullPath(filename);
            Directory = Path.GetDirectoryName(filename);
            MetaData = metaData;
        }

        public override bool Equals(object obj)
        {
            return obj is UncompressedFile file &&
                   Filename == file.Filename &&
                   AbsolutePath == file.AbsolutePath &&
                   Directory == file.Directory &&
                   EqualityComparer<MetaData>.Default.Equals(MetaData, file.MetaData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Filename, AbsolutePath, Directory, MetaData);
        }
    }
}
