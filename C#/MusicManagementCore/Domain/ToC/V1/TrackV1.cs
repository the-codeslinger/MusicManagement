using System.Text.Json.Serialization;
using MusicManagementCore.Constant;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MusicManagementCore.Domain.ToC.V1
{
    /// <summary>
    /// An <cref>AlbumTrack</cref> object that represents a single uncompressed audio 
    /// file. 
    /// 
    /// This format was used in the Python scripts and is retained for backward 
    /// compatibility with existing archives.
    /// 
    /// An <cref>AlbumTrack</cref> contains an audio file's title and number metadata and 
    /// its long and short filenames.
    /// </summary>
    public class TrackV1
    {
        /// <summary>
        /// The track's number, usually with a leading 0.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.Track)]
        public string Number { get; set; }

        /// <summary>
        /// The track's title.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.Title)]
        public string Title { get; set; }

        /// <summary>
        /// The filenames the audio file is known as.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.Filename)]
        public AudioFilenameV1 FilenameV1 { get; set; }
    }
}
