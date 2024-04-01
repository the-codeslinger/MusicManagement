using System.Text.Json.Serialization;

using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.ToC.V3;

/// <summary>
/// Version 3 contains the meta data, whether it is part of a compilation, and all the source and
/// output file name data required to convert the audio to another format and update the meta data
/// in the compressed files.
/// </summary>
public class TrackV3
{
    /// <summary>
    /// Defines whether the track is part of a compilation ("true") or an artist's album ("false).
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Compilation)]
    public bool IsCompilation { get; set; }

    /// <summary>
    /// The audio file's meta information.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.MetaData)]
    public MetaDataV3 MetaData { get; set; }

    /// <summary>
    /// The "files" object containing the original (incl. encoded meta data) and shortened file 
    /// name of the uncompressed audio track.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Files)]
    public FilesV3 Files { get; set; }
}