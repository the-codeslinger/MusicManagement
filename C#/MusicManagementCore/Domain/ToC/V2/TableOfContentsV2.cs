using MusicManagementCore.Constant;
using MusicManagementCore.Domain.ToC.V1;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MusicManagementCore.Domain.ToC.V2;

/// <summary>
/// Provides convenient accessors for a compilation table of contents JSON file.
/// 
/// The <c>ToC.json</c> file has the following format. Lowercase naming is used for
/// consistency with existing album ToC JSON files.
/// 
/// <code>
/// {
///   "version": "2",
///   "tracks": {
///     "compilation": false,
///     "artist": "Insomnium",
///     "album": "Winter's Gate",
///     "genre": "Melodic Death Metal",
///     "year": "2016",
///     "track": "01",
///     "title": "Winter's Gate",
///     "filename": {
///       "long": "Insomnium#Winter's Gate#Melodic Death Metal#2016#01#Winter's Gate.wav",
///       "short": "01 - Winter's Gate.wav"
///     }
///   }
/// }
/// </code>
/// </summary>
public class TableOfContentsV2
{
    /// <summary>
    /// The version of the ToC file format.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Version)]
    public string Version { get; set; } = ToCVersion.V2;

    /// <summary>
    /// The list of audio files that have been ripped for the "album".
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Tracks)]
    public List<TrackV2> TrackList { get; set; } = new();
}
