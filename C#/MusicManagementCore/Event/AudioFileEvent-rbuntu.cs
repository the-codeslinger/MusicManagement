using MusicManagementCore.Model;

namespace MusicManagementCore.Event
{
    /// <summary>
    /// Emitted by <cref>AudioFileFinder</cref> when scanning a directory for audio 
    /// files. Contains the <cref>AudioFile</cref>.
    /// </summary>
    public class AudioFileEvent
    {
        public AudioFileEvent(AudioFile audioFile)
        {
            AudioFile = audioFile;
        }

        public AudioFile AudioFile { get; }
    }
}
