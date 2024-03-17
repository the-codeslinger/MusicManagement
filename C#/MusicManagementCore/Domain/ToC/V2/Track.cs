using System.Text.Json.Serialization;
using MusicManagementCore.Constant;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MusicManagementCore.Domain.ToC.V2;

/// <summary>
/// An <cref>CompilationTrack</cref> object that represents a single uncompressed audio 
/// file.
/// 
/// This format was used in the Python scripts and is retained for backward 
/// compatibility with existing archives.
/// 
/// An <cref>CompilationTrack</cref> contains an audio file's title and number metadata and 
/// its long and short filenames.
/// </summary>
public class Track
{
    /// <summary>
    /// Defines whether the track is part of a compilation ("true") or an artist's album
    /// ("false).
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Compilation)]
    public bool IsCompilation { get; set; }

    /// <summary>
    /// The audio file's meta information.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.MetaData)]
    public MetaData MetaData { get; set; }

    /// <summary>
    /// The "files" object containing the original (incl. encoded meta data) and shortened file name of the 
    /// uncompressed audio track.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Files)]
    public Files Files { get; set; }
}