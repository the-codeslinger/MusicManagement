using System.Text.Json.Serialization;
using MusicManagementCore.Constant;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MusicManagementCore.Domain.ToC
{
    /// <summary>
    /// Contains the long and the short version of an audio track.
    /// 
    /// This format was used in the Python scripts and is retained for backward 
    /// compatibility with existing archives.
    /// 
    /// An audio file goes through some stages during the conversion process.
    /// 
    /// <list type="bullet">
    /// <item>
    ///     <description>
    ///     Ripped from audio disc to WAV file with long filename.
    ///     </description>
    /// </item>
    /// <item>
    ///     <description>
    ///     Create ToC.json from long filename and rename to short filename.
    ///     </description>
    /// </item>
    /// <item>
    ///     <description>
    ///     Read ToC.json for compressing to MP3 or other format using short filename.
    ///     </description>
    /// </item>
    /// </list>
    /// 
    /// This class contains the initial long filename after ripping to a WAV file and the
    /// short filename used for archiving and converting to a compressed format.
    /// </summary>
    public class AudioFilenameV2
    {
        /// <summary>
        /// The initial long file name containing all metadata.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.FilenameOriginal)]
        public string OriginalName { get; set; }

        /// <summary>
        /// The short uncompressed source file name after creating the \c ToC.json file.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.FilenameIn)]
        public string InName { get; set; }

        /// <summary>
        /// The final output file name of the compressed file.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.FilenameOut)]
        public string OutName { get; set; }
    }
}
