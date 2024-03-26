namespace MusicManagementCore.Event;

/// <summary>
/// Emitted by <cref>MusicMgmtFileFinder</cref> when entering and leaving a directory 
/// during a scan. Contains the absolute name of the directory.
/// </summary>
public class DirectoryEvent
{
    public string Path { get; init; }
}
