using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using MusicManagementCore.Constant;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace MusicManagementCore.Domain.ToC
{
    /// <summary>
    /// Provides convenient accessors for a compilation table of contents JSON file.
    /// 
    /// The <c>ToC.json</c> file has the following format. Lowercase naming is used for
    /// consistency with existing album ToC JSON files.
    /// 
    /// <code>
    /// {
    ///   "version": "2",
    ///   "CoverHash": "5455DA9A92AEA56AE5F65C9EF587412C356A18A25A9C1A95ED58A091BBBF9513",
    ///   "tracks": {
    ///     "IsCompilation": false,
    ///     "artist": "Insomnium",
    ///     "album": "Winter's Gate",
    ///     "genre": "Melodic Death Metal",
    ///     "year": "2016",
    ///     "track": "01",
    ///     "title": "Winter's Gate",
    ///     "MetaHash": "5455DA9A92AEA56AE5F65C9EF587412C356A18A25A9C1A95ED58A091BBBF9513",
    ///     "filename": {
    ///       "long": "Insomnium#Winter's Gate#Melodic Death Metal#2016#01#Winter's Gate.wav",
    ///       "short": "01 - Winter's Gate.wav"
    ///      }
    ///    }
    ///  }
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
        /// The relative output directory based on the output format string.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.RelativeOutDir)]
        public string RelativeOutDir { get; set; }

        /// <summary>
        /// A hash of the record's cover art file. Used for determining changes to 
        /// support updating all files listed in <cref>TrackList</cref> if the art has
        /// changed.
        /// </summary>
        [JsonPropertyName(JsonPropertyName.CoverHash)]
        public string CoverHash { get; set; }

        /// <summary>
        /// The list of audio files that have been ripped for the "album".
        /// </summary>
        [JsonPropertyName(JsonPropertyName.Tracks)]
        public List<TrackV2> TrackList { get; set; } = [];

        /// <summary>
        /// Compute the hash of the cover art file located in the given directory
        /// and set it as the current value.
        /// </summary>
        /// <param name="directory">The directory where the cover art is located. This is 
        /// usually the same folder where the table of contents will be stored.</param>
        /// <see cref="StandardFilename.CoverArt"/>
        public void UpdateHash(string directory)
        {
            CoverHash = ComputeCoverArtHash(directory);
        }

        /// <summary>
        /// Compute the hash of the cover art file located in the given directory.
        /// </summary>
        /// <param name="directory">The directory where the cover art is located. This is 
        /// usually the same folder where the table of contents will be stored.</param>
        /// <returns>A SHA256 hash hex string of the cover art file.</returns>
        public static string ComputeCoverArtHash(string directory)
        {
            var coverFile = new FileInfo(Path.Combine(directory, StandardFilename.CoverArt));
            using var fileStream = coverFile.OpenRead();
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(fileStream);

            return Convert.ToHexString(bytes);
        }
    }
}
