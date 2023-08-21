using MusicManagementCore.Config;
using MusicManagementCore.Model;
using MusicManagementCore.Util;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MusicManagementCore.Event;

namespace MusicManagementCore
{
    public class AudioFileFinder
    {
        public delegate void EnterDirectoryHandler(object sender, DirectoryEvent e);
        public delegate void LeaveDirectoryHandler(object sender, DirectoryEvent e);
        public delegate void FoundAudioFileHandler(object sender, AudioFileEvent e);

        /// <summary>
        /// Emitted when a new directory is scanned. Applies to recursive and 
        /// non-recursive operations.
        /// </summary>
        public event EnterDirectoryHandler EnterDirectory;

        /// <summary>
        /// Emitted when scanning a directory completed. Applies to recursive and 
        /// non-recursive operations.
        /// </summary>
        public event LeaveDirectoryHandler LeaveDirectory;

        /// <summary>
        /// Emitted when a file matching the configured extension 
        /// <cref>InputConfig.Extension</cref> is found.
        /// </summary>
        public event FoundAudioFileHandler FoundAudioFile;

        private readonly InputConfig _inputConfig;
        private readonly FilenameParser _parser;

        public AudioFileFinder(InputConfig config, FilenameEncodingConfig encodingConfig)
        {
            _inputConfig = config;
            _parser = new FilenameParser(encodingConfig);
        }

        /// <summary>
        /// Scans the <cref>InputConfig.Path</cref> for files that match 
        /// <cref>InputConfig.Extension</cref>.
        /// </summary>
        /// <remarks>
        /// For every file found, a <cref>FoundAudioFile</cref> event is emitted. 
        /// <cref>EnterDirectory</cref> and <cref>LeaveDirectory</cref> are emitted for
        /// every directory that is scenned, even when recursive scanning is not enabled.
        /// In that case, those events can be ignored.
        /// </remarks>
        public void Walk()
        {
            if (!Directory.Exists(_inputConfig.Path))
            {
                throw new DirectoryNotFoundException($"Source directory {_inputConfig.Path} not found.");
            }

            var dirInfo = new DirectoryInfo(_inputConfig.Path);
            WalkRecursive(dirInfo).Wait();
        }

        private async Task WalkRecursive(DirectoryInfo root)
        {
            EnterDirectory?.Invoke(this, new DirectoryEvent(root.FullName));

            foreach (var fileInfo in root.GetFiles("*.*")) {
                if (fileInfo.Extension == "." + _inputConfig.Extension) {
                    var metaData = _parser.ParseMetaData(fileInfo.FullName);
                    var audioFile = new AudioFile(fileInfo.FullName, metaData);

                    FoundAudioFile?.Invoke(this, new AudioFileEvent(audioFile));
                }
            }

            LeaveDirectory?.Invoke(this, new DirectoryEvent(root.FullName));

            if (_inputConfig.Recurse) {
                var tasks = root.GetDirectories().Select(WalkRecursive);
                await Task.WhenAll(tasks);
            }
        }
    }
}
