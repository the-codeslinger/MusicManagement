

using MusicManagementCore;
using MusicManagementCore.Config;
using MusicManagementCore.Constant;
using MusicManagementCore.Event;
using MusicManagementCore.Json;
using MusicManagementCore.Model;
using MusicManagementCore.Util;
using System;

namespace UpdateTags
{
    /// <summary>
    /// Read the input path for table of contents files and update the files at the selected
    /// converter's output path where the meta information hash has changed to what is stored
    /// in the table of contents file.
    /// </summary>
    class UpdateTags
    {

        private readonly Options _options;
        private readonly Configuration _config;
        private readonly Converter _converter;

        public UpdateTags(Options options)
        {
            _options = options;
            _config = new Configuration(_options.Config);

            var converter = _config.OutputConfig.Converters.Find(
                conv => conv.Type.ToLower() == _options.Format.ToLower());
            if (null == converter)
            {
                throw new ArgumentException($"No converter format found for '{_options.Format}'.");
            }
            _converter = converter;
        }

        public int Run()
        {
            var fileFinder = new MusicMgmtFileFinder(_config.InputConfig, _config.FilenameEncodingConfig);
            fileFinder.FoundTableOfContentsFile += FoundTableOfContentsFile;

            fileFinder.Scan();
            return 0;
        }

        private void FoundTableOfContentsFile(object _, TableOfContentsFileEvent e)
        {
            var version = TableOfContentsUtil.ReadVersion(e.TableOfContentsFile);
            if (ToCVersion.V1 == version)
            {
                throw new InvalidDataException($"File '{e.TableOfContentsFile}' is a v1 file that is not supported. "
                    + "Use CreateToc.exe to upgrade.");
            }

            var toc = TableOfContentsUtil.ReadFromFile<TableOfContentsV2>(e.TableOfContentsFile);
            var directory = Path.GetDirectoryName(e.TableOfContentsFile);
            
            var coverChanged = HasCoverChanged(toc, directory!);
            var anyFileChanged = false;
            toc.TrackList.ForEach(track => anyFileChanged |= HandleTrack(directory!, track, coverChanged));

            if (anyFileChanged || coverChanged)
            {
                Console.WriteLine($"Writing updated hashes to table of contents '{e.TableOfContentsFile}'.");
                JsonWriter.WriteToDirectory(directory, toc);
            }
        }

        private bool HandleTrack(string directory, TrackV2 track, bool coverChanged)
        {
            var compressedFile = new CompressedFile(_config, _converter, track);
            if (!compressedFile.Exists)
            {
                throw new FileNotFoundException($"{_converter.Type} audio file '{compressedFile.DestinationFilename}' does not exist.");
            }

            var currentHash = track.ComputeHash();
            if (currentHash != track.MetaHash || coverChanged)
            {
                Console.WriteLine($"Changes detected in file's '{compressedFile.DestinationFilename}' meta tags or cover art.");
                compressedFile.WriteAudioTags(directory);
                track.MetaHash = currentHash;
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool HasCoverChanged(TableOfContentsV2 toc, string directory)
        {
            var currentHash = toc.ComputeHash(directory);
            return currentHash != toc.CoverHash;
        }
    }
}
