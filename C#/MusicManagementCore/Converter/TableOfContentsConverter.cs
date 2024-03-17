using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V1;
using MusicManagementCore.Domain.ToC.V2;
using MusicManagementCore.Util;
using System;

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
    public TableOfContents Convert(TableOfContentsV1 tocV1, string directory)
    {
        var converter = new MetaDataConverter(config);

        var tracks = tocV1.TrackList.ConvertAll(trackV1 =>
        {
            var metaData = new MetaData
            {
                Artist = tocV1.Artist,
                Album = tocV1.Album,
                Genre = tocV1.Genre,
                Year = tocV1.Year,
                TrackNumber = trackV1.Number,
                Title = trackV1.Title,
                Hash = ""
            };

            var trackV2 = new Track
            {
                // V1 does not support compilations.
                IsCompilation = false,
                MetaData = converter.ToMetaData(metaData),

                Files = new Files
                {
                    Original = converter.ToOriginalFilename(metaData),
                    Uncompressed = converter.ToUncompressedFilename(metaData),
                    Compressed = converter.ToCompressedFilename(metaData, false)
                }
            };
            return trackV2;
        });

        tracks.Sort((t1, t2) =>
            string.Compare(t1.MetaData.TrackNumber, t2.MetaData.TrackNumber, StringComparison.Ordinal));

        var coverArt = CoverArt.OfDirectory(directory);
        var tocV2 = new TableOfContents
        {
            Version = ToCVersion.V2,
            CoverHash = DataHasher.ComputeOfFile(coverArt.Path),
            TrackList = tracks
        };

        return tocV2;
    }
}