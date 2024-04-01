using System.Text.Json.Serialization;

using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.ToC.V1;

/// <summary>
/// Contains the long and the short name of an audio track.
/// 
/// This format was used in the Python scripts and is retained for backward compatibility with 
/// existing archives.
/// 
/// An audio file goes through some stages during the conversion process.
/// 
/// <list type="bullet">
/// <item>
///     <description>
///     Ripped from audio disc to WAV file with a long file name.
///     </description>
/// </item>
/// <item>
///     <description>
///     Create ToC.json from the long file name and rename it to a short file name.
///     </description>
/// </item>
/// <item>
///     <description>
///     Read ToC.json for compressing to MP3 or another format using the short file name.
///     </description>
/// </item>
/// </list>
/// 
/// This class contains the initial long file name after ripping to a WAV file and the short file 
/// name used for archiving and converting to a compressed format.
/// </summary>
public class AudioFileNameV1
{
    /// <summary>
    /// The initial long file name containing all metadata.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.FileNameLong)]
    public string LongName { get; set; }

    /// <summary>
    /// The final short file name after creating the \c ToC.json file.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.FileNameShort)]
    public string ShortName { get; set; }
}
