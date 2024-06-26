﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.ToC.V3;

/// <summary>
/// Version 3 of the table of contents file.
/// 
/// The <c>ToC.json</c> file has the following format.
/// 
/// <code>
/// {
///   "version": "3",
///   "cover-hash": "5455DA9A92AEA56AE5F65C9EF587412C356A18A25A9C1A95ED58A091BBBF9513",
///   "tracks": {
///     "compilation": false,
///     "meta": {
///       "artist": "Insomnium",
///       "album": "Winter's Gate",
///       "genre": "Melodic Death Metal",
///       "year": "2016",
///       "track": "01",
///       "title": "Winter's Gate",
///       "hash": "5455DA9A92AEA56AE5F65C9EF587412C356A18A25A9C1A95ED58A091BBBF9513"
///     },
///     "files": {
///       "original": "Insomnium#Winter's Gate#Melodic Death Metal#2016#01#Winter's Gate.wav",
///       "uncompressed": "01 - Winter's Gate"
///       "compressed": "Melodic Death Metal\\Insomnium\\2016 - Winter's gate"
///     }
///   }
/// }
/// </code>
/// </summary>
public class TableOfContentsV3
{
    /// <summary>
    /// The version of the ToC file format.
    /// </summary>
    [JsonPropertyName(JsonPropertyName.Version)]
    public string Version { get; set; } = ToCVersion.V3;

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
    public List<TrackV3> TrackList { get; set; } = [];
}