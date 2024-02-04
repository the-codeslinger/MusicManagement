using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using MusicManagementCore.Constant;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace MusicManagementCore.Domain.ToC
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
        /// Defines whether the track is part of a compilation ("true") or an artist's album
        /// ("false).
        /// </summary>
        [JsonPropertyName(JsonPropertyName.Compilation)]
        public bool IsCompilation { get; set; }

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
        public string TrackTitle { get; set; }

        /// <summary>
        /// The filenames the audio file is known as.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.Filename)]
        public AudioFilename Filename { get; set; }

        /// <summary>
        /// A hash of the audio file's meta information. Used for determining changes to 
        /// support selective update of a single file's tags.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.Hash)]
        public string MetaHash { get; set; }

        /// <summary>
        /// Compute the hash based on the current values of <cref>Artist</cref>, <cref>Album</cref>,
        /// <cref>Genre</cref>, <cref>Year</cref>, <cref>TrackNumber</cref>, 
        /// and <cref>TrackTitle</cref> and set it as the current value.
        /// </summary>
        public void UpdateHash()
        {
            MetaHash = ComputeHash();
        }

        /// <summary>
        /// Compute the hash based on the current values of <cref>Artist</cref>, <cref>Album</cref>,
        /// <cref>Genre</cref>, <cref>Year</cref>, <cref>TrackNumber</cref>, 
        /// and <cref>TrackTitle</cref>.
        /// </summary>
        /// <returns>A SHA256 hash hex string of the track's current meta information.</returns>
        public string ComputeHash()
        {
            var hashSource = Artist + Album + Genre + Year + TrackNumber + TrackTitle;
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(hashSource));
            return Convert.ToHexString(bytes);
        }

        public override string ToString()
        {
            return $"{Artist} - {Album} ({Year}) {Genre} - {TrackNumber} {TrackTitle}";
        }
    }
}
