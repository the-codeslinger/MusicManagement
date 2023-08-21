using System;
using System.Collections.Generic;
using System.IO;

namespace MusicManagementCore.Model
{
    /// <summary>
    /// Represents a single audio file as found on the filesystem.
    /// </summary>
    public class AudioFile
    {
        /// <summary>
        /// The name of the audio file including the extension, excluding the directory
        /// (for example "01 - Winter's Gate.wav").
        /// </summary>
        public string Filename { get; init; }

        /// <summary>
        /// The name of the audio file including the extension and the absolute
        /// directory (for example "D:\Music\Insomnium\01 - Winter's Gate.wav").
        /// </summary>
        public string AbsolutePath { get; init; }

        /// <summary>
        /// The absolute name of the directory where the audio file was found (for
        /// example "D:\Music\Insomnium").
        /// </summary>
        public string Directory { get; init; }

        /// <summary>
        /// The audio file meta data as inferred from the filename.
        /// </summary>
        public MetaData MetaData { get; init; }

        /// <summary>
        /// Create a new audio file from a given filename and the associated 
        /// <cref>MetaData</cref>.
        /// </summary>
        /// <param name="filename">Absolute or relative filename of the audio file.</param>
        /// <param name="metaData">The parsed meta data.</param>
        public AudioFile(string filename, MetaData metaData)
        {
            Filename = Path.GetFileName(filename);
            AbsolutePath = Path.GetFullPath(filename);
            Directory = Path.GetDirectoryName(filename);
            MetaData = metaData;
        }

        public override bool Equals(object obj)
        {
            return obj is AudioFile file &&
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
