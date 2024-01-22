using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MusicManagementCore.Config;
using MusicManagementCore.Constant;
using MusicManagementCore.Event;
using MusicManagementCore.Model;
using MusicManagementCore.Util;

namespace MusicManagementCore.Service
{
    /// <summary>
    /// Helper class to (recursively) iterate the contents of a directory to find files
    /// relevant to music management. This includes audio files to convert as well as
    /// table of contents JSON files.
    /// </summary>
    public class MusicMgmtFileFinder
    {
        public delegate void EnterDirectoryHandler(object sender, DirectoryEvent e);
        public delegate void LeaveDirectoryHandler(object sender, DirectoryEvent e);
        public delegate void FoundAudioFileHandler(object sender, AudioFileEvent e);
        public delegate void FoundTableOfContentsFileHandler(object sender, TableOfContentsFileEvent e);

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

        /// <summary>
        /// Emitted when a file <cref>StandardFilename.TableOfContents</cref> is found.
        /// </summary>
        public event FoundTableOfContentsFileHandler FoundTableOfContentsFile;

        private readonly InputConfig _inputConfig;
        private readonly FilenameParser _parser;

        public MusicMgmtFileFinder(InputConfig config, FilenameEncodingConfig encodingConfig)
        {
            _inputConfig = config;
            _parser = new FilenameParser(encodingConfig);
        }

        /// <summary>
        /// Scans the <cref>InputConfig.Path</cref> for files that match 
        /// <cref>InputConfig.Extension</cref> or <cref>StandardFilename.TableOfContents</cref>.
        /// </summary>
        /// <remarks>
        /// For every audio file found, a <cref>FoundAudioFile</cref> event is emitted. 
        /// For every table of contents file found, a <cref>FoundTableOfContentsFile</cref> 
        /// event is emitted. 
        /// <cref>EnterDirectory</cref> and <cref>LeaveDirectory</cref> are emitted for
        /// every directory that is scanned, even when recursive scanning is not enabled.
        /// In that case, those events can be ignored.
        /// </remarks>
        public void Scan()
        {
            if (!Directory.Exists(_inputConfig.Path)) {
                throw new DirectoryNotFoundException($"Source directory {_inputConfig.Path} not found.");
            }

            var dirInfo = new DirectoryInfo(_inputConfig.Path);
            WalkRecursive(dirInfo).Wait();
        }

        private async Task WalkRecursive(DirectoryInfo root)
        {
            EnterDirectory?.Invoke(this, new DirectoryEvent(root.FullName));

            foreach (var fileInfo in root.GetFiles("*.*")) {
                if (MatchesAudioFile(fileInfo)) {
                    var metaData = _parser.ParseMetaData(fileInfo.FullName);
                    var audioFile = new AudioFile(fileInfo.FullName, metaData);

                    FoundAudioFile?.Invoke(this, new AudioFileEvent(audioFile));
                }
                else if (MatchesTableOfContentsFile(fileInfo)) {
                    FoundTableOfContentsFile?.Invoke(this, new TableOfContentsFileEvent(fileInfo.FullName));
                }
            }

            LeaveDirectory?.Invoke(this, new DirectoryEvent(root.FullName));

            if (_inputConfig.Recurse) {
                var tasks = root.GetDirectories().Select(WalkRecursive);
                await Task.WhenAll(tasks);
            }
        }

        private bool MatchesAudioFile(FileSystemInfo fileInfo)
        {
            return fileInfo.Extension == "." + _inputConfig.Extension;
        }

        private static bool MatchesTableOfContentsFile(FileSystemInfo fileInfo)
        {
            return fileInfo.Name == StandardFilename.TableOfContents;
        }
    }
}
