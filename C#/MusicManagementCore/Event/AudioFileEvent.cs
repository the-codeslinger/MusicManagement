using MusicManagementCore.Domain.Audio;

namespace MusicManagementCore.Event
{
    /// <summary>
    /// Emitted by <cref>MusicMgmtFileFinder</cref> when scanning a directory for audio 
    /// files. Contains the <cref>AudioFile</cref>.
    /// </summary>
    public class AudioFileEvent
    {
        public AudioFileEvent(UncompressedFile uncompressedFile)
        {
            UncompressedFile = uncompressedFile;
        }

        public UncompressedFile UncompressedFile { get; }
    }
}
