using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MusicManagementCore.Constant;
using MusicManagementCore.Converter;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V1;
using MusicManagementCore.Domain.ToC.V2;
using MusicManagementCore.Domain.ToC.V3;

namespace MusicManagementCore.Util;

/// <summary>
/// A helper class to group a few useful methods in one place that are required in several
/// locations.
/// </summary>
public static class TableOfContentsUtil
{
    /// <summary>
    /// Read the 'version' property of the given table of contents file.
    /// </summary>
    /// <param name="filename">The filename of the table of contents file. The file will
    /// not be parsed into any of the classes located in the <cref>MusicManagementCore.Json</cref>
    /// namespace.</param>
    /// <returns>The version string if found or <cref>ToCVersion.V1</cref> otherwise.</returns>
    public static string ReadVersion(string filename)
    {
        using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        using var json = JsonDocument.Parse(stream);

        try
        {
            var versionElement = json.RootElement.GetProperty(JsonPropertyName.Version);
            return versionElement.GetString() ?? ToCVersion.V1;
        }
        catch (KeyNotFoundException)
        {
            // If "version" does not exist, we have a V1 at our hands.
            return ToCVersion.V1;
        }
    }

    /// <summary>
    /// Parse the given table of contents file into <cref>TableOfContentsV1</cref> or
    /// <cref>TableOfContentsV2</cref>.
    /// </summary>
    /// <typeparam name="T">One of <cref>TableOfContentsV1</cref> or
    /// <cref>TableOfContentsV2</cref>.</typeparam>
    /// <param name="filename">The filename of the table of contents file.</param>
    /// <returns>The table of contents file parsed into the desired C# class instance.</returns>
    /// <exception cref="InvalidDataException">The file does not contain a valid table of 
    /// contents file.</exception>
    public static T ReadFromFile<T>(string filename)
    {
        using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
        using var json = JsonDocument.Parse(stream);

        var toc = json.RootElement.Deserialize<T>();
        if (null == toc)
        {
            throw new InvalidDataException(
                $"File '{filename}' is not a valid table of contents.");
        }

        return toc;
    }

    /// <summary>
    /// Migrate the given V1 table of contents file to V3 and create a backup of the V1 file.
    /// </summary>
    /// <param name="filename">The V1 table of contents file to migrate.</param>
    /// <param name="format">The format configuration to build the compressed path and file
    /// name.</param>
    /// <returns>The result of the migration.</returns>
    public static TableOfContentsV3 MigrateV1ToV3File(string filename, MusicManagementConfig config)
    {
        var tocV1 = ReadFromFile<TableOfContentsV1>(filename);
        var tocV3 = new TableOfContentsConverter(config).Convert(tocV1, Path.GetDirectoryName(filename));
        return WriteToC(tocV3, filename);
    }

    /// <summary>
    /// Migrate the given V2 table of contents file to V3 and create a backup of the V2 file.
    /// </summary>
    /// <param name="filename">The V2 table of contents file to migrate.</param>
    /// <param name="format">The format configuration to build the compressed path and file
    /// name.</param>
    /// <returns>The result of the migration.</returns>
    public static TableOfContentsV3 MigrateV2ToV3File(string filename, MusicManagementConfig config)
    {
        var tocV2 = ReadFromFile<TableOfContentsV2>(filename);
        var tocV3 = new TableOfContentsConverter(config).Convert(tocV2, Path.GetDirectoryName(filename));
        return WriteToC(tocV3, filename);
    }

    private static TableOfContentsV3 WriteToC(TableOfContentsV3 tocV3, string filename)
    {
        File.Copy(filename, filename + "_bak");
        JsonWriter.WriteToFilename(filename, tocV3);
        return tocV3;
    }
}