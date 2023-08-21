namespace MusicManagementCore.Constant
{
    /// <summary>
    /// Defines the names of the supported audio file meta tags.
    ///
    /// The constants are used as configuration parameters, property names in JSON
    /// objects, and the tags in an audio file, e.g., as an ID3 tag.
    /// </summary>
    public sealed class MetaTagName
    {
        public const string Artist = "Artist";
        public const string Album = "Album";
        public const string Genre = "Genre";
        public const string Year = "Year";
        public const string TrackNumber = "TrackNumber";
        public const string Title = "Title";
    }

    /// <summary
    /// Defines the names of properties that are used in JSON objects.
    /// </summary>
    public sealed class JsonPropertyName
    {
        public const string Tracks = "tracks";
        public const string Filename = "filename";
        public const string FilenameLong = "long";
        public const string FilenameShort = "short";
    }

    /// <summary>
    /// Standard filenames used by the music management tools.
    /// </summary>
    public sealed class StandardFilename
    {
        public const string TableOfContents = "ToC.json";
        public const string AlbumCover = "Cover.jpg";
    }

    /// <summary>
    /// Standard values to identify the type of a table of contents JSON file.
    /// </summary>
    public sealed class ToCVersion
    {
        public const string V1 = "1";
        public const string V2 = "2";
    }

    /// <summary>
    /// Defines the input and output variables used for the converter command line.
    /// </summary>
    public sealed class ConverterArgs
    {
        public const string Input = "%input%";
        public const string Output = "%output%";
    }
}
