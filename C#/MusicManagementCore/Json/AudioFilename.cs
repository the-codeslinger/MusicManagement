using System.Text.Json.Serialization;
using MusicManagementCore.Constant;

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace MusicManagementCore.Json
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
    public class AudioFilename
    {
        /// <summary>
        /// The initial long filename containing all metadata.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.FilenameLong)]
        public string LongName { get; set; }

        /// <summary>
        /// The final short filename after creating the \c ToC.json file.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.FilenameShort)]
        public string ShortName { get; set; }
    }
}
