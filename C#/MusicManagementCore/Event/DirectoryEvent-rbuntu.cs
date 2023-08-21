namespace MusicManagementCore.Event
{
    /// <summary>
    /// Emitted by <cref>AudioFileFinder</cref> when entering and leaving a directory 
    /// during a scan. Contains the absolute name of the directory.
    /// </summary>
    public class DirectoryEvent
    {
        public DirectoryEvent(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public string DirectoryPath { get; }
    }
}
