using System.Text.Json.Serialization;

namespace MusicManagementCore.Json
{
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
    public class TrackV2
    {
        /// <summary>
        /// The name of the track's artist. For example, this can be a single person 
        /// or a band.
        /// </summary>
        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        /// <summary>
        /// The name of the audio disc.
        /// </summary>
        [JsonPropertyName("album")]
        public string Album { get; set; }

        /// <summary>
        /// The genre of the track.
        /// </summary>
        [JsonPropertyName("genre")]
        public string Genre { get; set; }

        /// <summary>
        /// The year the "album" was released.
        /// </summary>
        [JsonPropertyName("year")]
        public string Year { get; set; }

        /// <summary>
        /// The track's number, usually with a leading 0.
        /// </summary>
        [JsonPropertyName("number")]
        public string TrackNumber { get; set; }

        /// <summary>
        /// The track's title.
        /// </summary>
        [JsonPropertyName("track")]
        public string TrackTitle { get; set; }

        /// <summary>
        /// The filenames the audio file is known as.
        /// </summary>
        [JsonPropertyName("filename")]
        public AudioFilename Filename { get; set; }

        public string ToString()
        {
            return $"{Artist} - {Album} ({Year}) {Genre} - {TrackNumber} {TrackTitle}";
        }
    }
}
