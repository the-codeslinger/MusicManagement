using System;
using System.IO;
using System.Linq;

using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V3;
using MusicManagementCore.Util;

namespace MusicManagementCore.Converter;

/// <summary>
/// Convert an audio file's meta data between a <cref>MetaData</cref> object and an uncompressed 
/// file's formatted name.
/// </summary>
public class MetaDataConverter(MusicManagementConfig config)
{

    /// <summary>
    /// Parse an audio file's file name and extract the meta data according to the encoding 
    /// configuration.
    /// </summary>
    /// <param name="fileName">The audio file's file name, relative or absolute.</param>
    /// <returns>The meta data that was encoded in the file name. Data that was not listed in the 
    /// input configuration will be treated as an empty string.</returns>
    public MetaDataV3 ToMetaData(string fileName)
    {
        var encodingConfig = config.FileNameEncodingConfig;

        var name = Path.GetFileNameWithoutExtension(fileName);
        var exploded = name.Split(encodingConfig.Delimiter);

        var metaData = new MetaDataV3();
        foreach (var zipped in exploded.Zip(encodingConfig.TagFormat, Tuple.Create)) {
            InsertData(zipped.Item1, zipped.Item2, metaData);
        }
        metaData.Hash = GenerateHash(metaData);

        return metaData;
    }

    /// <summary>
    /// Create a new <cref>MetaData</cref> object from the existing one with an updated hash.
    /// This method can be used when meta data in a ToC file was updated and a new hash must be 
    /// computed.
    /// </summary>
    /// <param name="metaData">Meta data with an outdated hash.</param>
    /// <returns>A copy of the input parameter but with an updated hash.</returns>
    public MetaDataV3 ToMetaData(MetaDataV3 metaData)
    {
        return MetaDataV3.Of(metaData, GenerateHash(metaData));
    }

    /// <summary>
    /// Create the original file name from the given <cref>MetaData</cref> object. The format is 
    /// based on <cref>FileNameEncodingConfig.TagFormat</cref> and 
    /// <cref>FileNameEncodingConfig.Delimiter</cref>.
    /// </summary>
    /// <param name="metaData">The meta data of the audio file.</param>
    /// <returns>The file name of the original uncompressed file name with all meta data embedded and
    /// the configured input extension.</returns>
    public string ToOriginalFileName(MetaDataV3 metaData)
    {
        var encodingConfig = config.FileNameEncodingConfig;

        var fileName = "";
        for (var c = 0; c < encodingConfig.TagFormat.Count; c++) {
            fileName += ExtractData(encodingConfig.TagFormat[c], metaData);
            if (c < encodingConfig.TagFormat.Count - 1) {
                fileName += encodingConfig.Delimiter;
            }
        }

        return fileName + "." + config.InputConfig.Extension;
    }

    /// <summary>
    /// Create the short file name of the uncompressed audio file as used after creating the ToC.
    /// </summary>
    /// <param name="metaData">The meta data of the audio file.</param>
    /// <returns>The predefined file name in the format "track# - track title" with the configured 
    /// extension.</returns>
    public string ToUncompressedFileName(MetaDataV3 metaData)
    {
        return $"{metaData.TrackNumber} - {FileSystemUtil.RemoveInvalidFileNameChars(metaData.Title)}.{config.InputConfig.Extension}";
    }

    /// <summary>
    /// Create the relative file name of the compressed audio file based on the configured output
    /// format.
    /// </summary>
    /// <param name="metaData">The meta data of the audio file.</param>
    /// <param name="isCompilation">Whether the meta data is part of a compilation.</param>
    /// <returns>The relative file name of the compressed file based on the configured format and 
    /// without extension.</returns>
    public string ToCompressedFileName(MetaDataV3 metaData, bool isCompilation)
    {
        var format = config.OutputConfig.Format;
        return format
            .Replace(MetaTagName.Artist, FileSystemUtil.RemoveInvalidFileNameChars(isCompilation
                    ? CompilationArtist.Name
                    : metaData.Artist))
            .Replace(MetaTagName.Album, FileSystemUtil.RemoveInvalidFileNameChars(metaData.Album))
            .Replace(MetaTagName.Genre, FileSystemUtil.RemoveInvalidFileNameChars(metaData.Genre))
            .Replace(MetaTagName.Year, FileSystemUtil.RemoveInvalidFileNameChars(metaData.Year))
            .Replace(MetaTagName.TrackNumber, FileSystemUtil.RemoveInvalidFileNameChars(metaData.TrackNumber))
            .Replace(MetaTagName.Title, FileSystemUtil.RemoveInvalidFileNameChars(metaData.Title));
    }

    private string GenerateHash(MetaDataV3 metaData)
    {
        return GenerateHash(metaData.Artist, metaData.Album, metaData.Genre, metaData.Year,
            metaData.TrackNumber, metaData.Title);
    }

    private string GenerateHash(string artist, string album, string genre, string year,
                                string track, string title)
    {
        var hashSource = artist + album + genre + year + track + title;
        return DataHasher.ComputeOfString(hashSource);
    }

    private void InsertData(string value, string tag, MetaDataV3 metaData)
    {
        var encodingConfig = config.FileNameEncodingConfig;
        switch (tag) {
            case MetaTagName.Artist:
                metaData.Artist = encodingConfig.ReplaceCodeStrings(value);
                break;

            case MetaTagName.Album:
                metaData.Album = encodingConfig.ReplaceCodeStrings(value);
                break;

            case MetaTagName.Genre:
                metaData.Genre = encodingConfig.ReplaceCodeStrings(value);
                break;

            case MetaTagName.Year:
                metaData.Year = value;
                break;

            case MetaTagName.TrackNumber:
                metaData.TrackNumber = value;
                break;

            case MetaTagName.Title:
                metaData.Title = encodingConfig.ReplaceCodeStrings(value);
                break;

            default:
                // TODO Define better exception.
                throw new Exception("Unrecognized tag " + tag);
        }
    }

    private string ExtractData(string tag, MetaDataV3 metaData)
    {
        var encodingConfig = config.FileNameEncodingConfig;
        return tag switch {
            MetaTagName.Artist => encodingConfig.InsertCodeStrings(metaData.Artist),
            MetaTagName.Album => encodingConfig.InsertCodeStrings(metaData.Album),
            MetaTagName.Genre => encodingConfig.InsertCodeStrings(metaData.Genre),
            MetaTagName.Year => encodingConfig.InsertCodeStrings(metaData.Year),
            MetaTagName.TrackNumber => encodingConfig.InsertCodeStrings(metaData.TrackNumber),
            MetaTagName.Title => encodingConfig.InsertCodeStrings(metaData.Title),
            _ => throw new Exception("Unrecognized tag " + tag)
        };
    }
}
