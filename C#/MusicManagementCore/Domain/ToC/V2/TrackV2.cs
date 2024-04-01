using System.Text.Json.Serialization;

using MusicManagementCore.Constant;
using MusicManagementCore.Domain.ToC.V1;

namespace MusicManagementCore.Domain.ToC.V2;

/// <summary>
/// Version 2 was an intermediate format during the development of the C# version of the Music 
/// Management tools.
/// </summary>
public class TrackV2
{
    /// <summary>
    /// Defines whether the track is part of a compilation ("true") or an artist's album ("false).
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Compilation)]
    public bool IsCompilation { get; set; }

    /// <summary>
    /// The name of the track's artist. For example, this can be a single person or a band.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Artist)]
    public string Artist { get; set; }

    /// <summary>
    /// The name of the record.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Album)]
    public string Album { get; set; }

    /// <summary>
    /// The genre of the track.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Genre)]
    public string Genre { get; set; }

    /// <summary>
    /// The year the "album" was released.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Year)]
    public string Year { get; set; }

    /// <summary>
    /// The track's number, usually with a leading 0.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Number)]
    public string TrackNumber { get; set; }

    /// <summary>
    /// The track's title.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Track)]
    public string Title { get; set; }

    /// <summary>
    /// The file names the audio file is known as.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.FileName)]
    public AudioFileNameV1 FileNameV1 { get; set; }
}
