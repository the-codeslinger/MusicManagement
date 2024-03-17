using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V1;
using MusicManagementCore.Domain.ToC.V2;
using MusicManagementCore.Domain.ToC.V3;
using MusicManagementCore.Util;
using System;
using System.Collections.Generic;

namespace MusicManagementCore.Converter;

/// <summary>
/// Converter to turn version 1 of the table of contents file format to version 2.
/// </summary>
/// <param name="config">The converter configuration.</param>
public class TableOfContentsConverter(MusicManagementConfig config)
{
    /// <summary>
    /// Converts version 1 of the table of contents file format to version 2.
    /// </summary>
    /// <param name="tocV1">The table of contents version 1 to convert.</param>
    /// <param name="directory">The source directory of the V1 table of contents file.</param>
    /// <returns>The same table of contents converted to version 2.</returns>
    public TableOfContentsV3 Convert(TableOfContentsV1 tocV1, string directory)
    {
        var converter = new MetaDataConverter(config);

        var tracks = tocV1.TrackList.ConvertAll(trackV1 =>
        {
            var metaData = new MetaDataV3
            {
                Artist = tocV1.Artist,
                Album = tocV1.Album,
                Genre = tocV1.Genre,
                Year = tocV1.Year,
                TrackNumber = trackV1.Number,
                Title = trackV1.Title,
                Hash = ""
            };

            return CreateTrack(metaData);
        });

        return CreateToC(tracks, directory);
    }

    public TableOfContentsV3 Convert(TableOfContentsV2 tocV2, string directory)
    {

        var tracks = tocV2.TrackList.ConvertAll(trackV2 =>
        {
            var metaData = new MetaDataV3
            {
                Artist = trackV2.Artist,
                Album = trackV2.Album,
                Genre = trackV2.Genre,
                Year = trackV2.Year,
                TrackNumber = trackV2.TrackNumber,
                Title = trackV2.Title,
                Hash = ""
            };

            return CreateTrack(metaData);
        });

        return CreateToC(tracks, directory);
    }

    private TrackV3 CreateTrack(MetaDataV3 metaData)
    {
        var converter = new MetaDataConverter(config);
        return new TrackV3
        {
            // V1 does not support compilations.
            IsCompilation = false,
            MetaData = converter.ToMetaData(metaData),

            Files = new FilesV3
            {
                Original = converter.ToOriginalFilename(metaData),
                Uncompressed = converter.ToUncompressedFilename(metaData),
                Compressed = converter.ToCompressedFilename(metaData, false)
            }
        };
    }

    private TableOfContentsV3 CreateToC(List<TrackV3> tracks, string directory)
    {
        tracks.Sort((t1, t2) =>
            string.Compare(t1.MetaData.TrackNumber, t2.MetaData.TrackNumber, StringComparison.Ordinal));

        var coverArt = CoverArt.OfDirectory(directory);
        var tocV3 = new TableOfContentsV3
        {
            Version = ToCVersion.V3,
            CoverHash = DataHasher.ComputeOfFile(coverArt.Path),
            TrackList = tracks
        };

        return tocV3;
    }
}