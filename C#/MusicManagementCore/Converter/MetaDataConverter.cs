using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V3;
using MusicManagementCore.Util;
using System;
using System.IO;
using System.Linq;
using TagLib.Matroska;

namespace MusicManagementCore.Converter;

/// <summary>
/// Extract the <cref>MetaData</cref> of an uncompressed, unprocessed audio file's
/// filename based on a <cref>FilenameEncodingConfig</cref>. This configuration
/// determines the tags to read and the order they appear in the filename.
/// </summary>
public class MetaDataConverter(MusicManagementConfig config)
{

    /// <summary>
    /// Parse an audio file's filename and extract the meta data according to
    /// the encoding configuration.
    /// </summary>
    /// <param name="filename">The audio file's filename, relative or absolute.</param>
    /// <returns>The meta data that was encoded in the filename. Data that was
    /// not listed in the input configuration will be treated as an empty string.</returns>
    public MetaDataV3 ToMetaData(string filename)
    {
        var encodingConfig = config.FilenameEncodingConfig;

        var name = Path.GetFileNameWithoutExtension(filename);
        var exploded = name.Split(encodingConfig.Delimiter);

        var metaData = new MetaDataV3();
        foreach (var zipped in exploded.Zip(encodingConfig.TagFormat, Tuple.Create))
        {
            InsertData(zipped.Item1, zipped.Item2, metaData);
        }
        metaData.Hash = GenerateHash(metaData);

        return metaData;
    }

    /// <summary>
    /// Create a new <cref>MetaData</cref> object from the existing one with an updated hash.
    /// This method can be used when meta data in a ToC file was updated and the hash must be computed again.
    /// </summary>
    /// <param name="metaData">Meta data with an outdated hash.</param>
    /// <returns>A copy of the input parameter but with an updated hash.</returns>
    public MetaDataV3 ToMetaData(MetaDataV3 metaData)
    {
        return MetaDataV3.Of(metaData, GenerateHash(metaData));
    }

    /// <summary>
    /// Create the original filename from the given <cref>MetaData</cref> object. The format is based 
    /// on <cref>FilenameEncoding.TagFormat</cref> and <cref>FilenameEncoding.Delimiter</cref>.
    /// </summary>
    /// <param name="metaData">The meta data of the audio file.</param>
    /// <returns>The filename of the original uncompressed filename with all meta data embedded and
    /// the configured input extension.</returns>
    public string ToOriginalFilename(MetaDataV3 metaData)
    {
        var encodingConfig = config.FilenameEncodingConfig;

        var filename = "";
        for (var c = 0; c < encodingConfig.TagFormat.Count; c++)
        {
            filename += ExtractData(encodingConfig.TagFormat[c], metaData);
            if (c < encodingConfig.TagFormat.Count - 1)
            {
                filename += encodingConfig.Delimiter;
            }
        }

        return filename + "." + config.InputConfig.Extension;
    }

    /// <summary>
    /// Create the short filename of the uncompressed audio file as used after creating the ToC.
    /// </summary>
    /// <param name="metaData">The meta data of the audio file.</param>
    /// <returns>The predefined filename in the format "track# - track title" with the configured 
    /// extension.</returns>
    public string ToUncompressedFilename(MetaDataV3 metaData)
    {
        return $"{metaData.TrackNumber} - {metaData.Title}.{config.InputConfig.Extension}";
    }

    /// <summary>
    /// Create the relative filename of the compressed audio file.
    /// </summary>
    /// <param name="metaData">The meta data of the audio file.</param>
    /// <param name="isCompilation">Whether the meta data is part of a compilation.</param>
    /// <returns>The relative filename of the compressed file based on the configured format and 
    /// without extension.</returns>
    public string ToCompressedFilename(MetaDataV3 metaData, bool isCompilation)
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
        var encodingConfig = config.FilenameEncodingConfig;
        switch (tag)
        {
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
        var encodingConfig = config.FilenameEncodingConfig;
        return tag switch
        {
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
