using System.Text.Json.Serialization;

using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.ToC.V3;

/// <summary>
/// Contains all of an audio file's file names as expected by the Music Management tools.
/// </summary>
public class FilesV3
{
    /// <summary>
    /// The uncompressed file's file name including all the meta data separated by a separator.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.FileOriginal)]
    public string Original { get; set; }

    /// <summary>
    /// The uncompressed file's shortened file name after creating a table of contents file.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.FileUncompressed)]
    public string Uncompressed { get; set; }

    /// <summary>
    /// The compressed file's output file name without extension according to the configured 
    /// format. The location is relative to the output directory set in the configuration file.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.FileCompressed)]
    public string Compressed { get; set; }
}
