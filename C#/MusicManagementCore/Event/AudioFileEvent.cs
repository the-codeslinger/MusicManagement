using MusicManagementCore.Domain.Audio;

namespace MusicManagementCore.Event;

/// <summary>
/// Emitted by <cref>MusicMgmtFileFinder</cref> when scanning a directory for audio 
/// files. Contains the <cref>AudioFile</cref>.
/// </summary>
public class AudioFileEvent
{
    public UncompressedFile UncompressedFile { get; init; }
}
