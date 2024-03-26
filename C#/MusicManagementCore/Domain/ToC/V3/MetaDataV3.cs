using System.Text.Json.Serialization;

using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.ToC.V3;

/// <summary>
/// All meta data of an audio track. This data contains all umlauts or other special characters
/// to represents the unmodified track data.
/// </summary>
public class MetaDataV3
{
    /// <summary>
    /// The name of the track's artist. For example, this can be a single person 
    /// or a band.
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
    /// A hash of the audio file's meta information. Used for determining changes to 
    /// support selective update of a single file's tags.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.MetaDataHash)]
    public string Hash { get; set; }

    public static MetaDataV3 Of(MetaDataV3 metaData, string hash)
    {
        return new MetaDataV3 {
            Artist = metaData.Artist,
            Album = metaData.Album,
            Genre = metaData.Genre,
            Year = metaData.Year,
            TrackNumber = metaData.TrackNumber,
            Title = metaData.Title,
            Hash = hash
        };
    }

    public override string ToString()
    {
        return $"{Artist} - {Album} ({Year}) {Genre} - {TrackNumber} {Title}";
    }
}