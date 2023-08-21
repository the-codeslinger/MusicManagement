using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MusicManagementCore.Json
{
    /// <summary>
    /// Provides convenient accessors for an artist's album table of contents JSON file.
    /// 
    /// The <c>ToC.json</c> file has the following format. This format was used in the Python 
    /// scripts and is retained for backward compatibility with existing archives.
    /// New ToC files are written as <cref>TableOfContentsV2</cref>.
    /// 
    /// <code>
    /// {
    ///   "artist": "Insomnium",
    ///   "album": "Winter's Gate",
    ///   "genre": "Melodic Death Metal",
    ///   "year": "2016",
    ///   "tracks": {
    ///     "track": "01",
    ///     "title": "Winter's Gate",
    ///     "filename": {
    ///       "long": "Insomnium#Winter's Gate#Melodic Death Metal#2016#01#Winter's Gate.wav",
    ///       "short": "01 - Winter's Gate.wav"
    ///      }
    ///    }
    ///  }
    /// </code>
    /// </summary>
    public class TableOfContentsV1
    {
        /// <summary>
        /// The name of the "album"s artist. For example, this can be a single person 
        /// or a band.
        /// </summary>
        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        /// <summary>
        /// The name of the audio disc, commonly referred to as an album title.
        /// </summary>
        [JsonPropertyName("album")]
        public string Album { get; set; }

        /// <summary>
        /// The genre of the "album".
        /// </summary>
        [JsonPropertyName("genre")]
        public string Genre { get; set; }

        /// <summary>
        /// The year the "album" was released.
        /// </summary>
        [JsonPropertyName("year")]
        public string Year { get; set; }

        /// <summary>
        /// The list of audio files that have been ripped for the "album".
        /// </summary>
        [JsonPropertyName("tracks")]
        public IList<TrackV1> TrackList { get; set; } = new List<TrackV1>();
    }
}
