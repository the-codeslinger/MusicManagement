﻿namespace MusicManagementCore.Constant;

/// <summary>
/// Defines the names of the supported audio file meta tags.
///
/// The constants are used as configuration parameters, property names in JSON
/// objects, and the tags in an audio file, e.g., as an ID3 tag.
/// </summary>
public static class MetaTagName
{
    public const string Artist = "Artist";
    public const string Album = "Album";
    public const string Genre = "Genre";
    public const string Year = "Year";
    public const string TrackNumber = "TrackNumber";
    public const string Title = "Title";
}

/// <summary>
/// Defines the names of properties that are used in a table of contents JSON file.
/// </summary>
public static class JsonPropertyName
{
    public const string Version = "version";
    public const string CoverHash = "cover-hash";
    public const string Tracks = "tracks";

    public const string Compilation = "compilation";

    public const string MetaData = "meta";
    public const string Artist = "artist";
    public const string Album = "album";
    public const string Genre = "genre";
    public const string Year = "year";
    public const string Title = "title";
    public const string Number = "number";
    public const string Track = "track";
    public const string MetaDataHash = "hash";

    public const string Files = "files";
    public const string FileOriginal = "original";
    public const string FileUncompressed = "uncompressed";
    public const string FileCompressed = "compressed";

    public const string FileName = "filename";
    public const string FileNameLong = "long";
    public const string FileNameShort = "short";
}

/// <summary>
/// Standard file names used by the music management tools.
/// </summary>
public static class StandardFileName
{
    public const string TableOfContents = "ToC.json";
    public const string CoverArt = "Cover.jpg";
}

/// <summary>
/// Standard values to identify the type of a table of contents JSON file.
/// </summary>
public static class ToCVersion
{
    public const string V1 = "1";
    public const string V2 = "2";
    public const string V3 = "3";
}

/// <summary>
/// Defines the input and output variables used for the converter command line.
/// </summary>
public static class ConverterArgs
{
    public const string Input = "%input%";
    public const string Output = "%output%";
}

/// <summary>
/// Defines the name of "album artist" to use for sorting compilations.
/// </summary>
public static class CompilationArtist
{
    public const string Name = "Compilation";
}