namespace MusicManagementCore.Event
{
    /// <summary>
    /// Emitted by <cref>MusicMgmtFileFinder</cref> when scanning a directory for files.
    /// Contains the absolute path of the ToC.json file.
    /// </summary>
    public class TableOfContentsFileEvent
    {
        public string Filename { get; init; }
    }
}
