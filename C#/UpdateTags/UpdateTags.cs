using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
using MusicManagementCore.Domain.ToC.V2;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace UpdateTags
{
    /// <summary>
    /// Read the input path for table of contents files and update the files at the selected
    /// converter's output path where the meta information hash has changed to what is stored
    /// in the table of contents file.
    /// </summary>
    class UpdateTags
    {
        private readonly MusicManagementConfig _config;
        private readonly Converter _converter;
        private readonly string _uncompressedDir;
        private readonly string _compressedDir;

        public UpdateTags(Options options)
        {
            _config = new MusicManagementConfig(options.Config);

            var converter = _config.OutputConfig.Converters.Find(
                conv => string.Equals(conv.Type, options.Format,
                    StringComparison.CurrentCultureIgnoreCase));

            _converter = converter ??
                         throw new ArgumentException(
                             $"No converter format found for '{options.Format}'.");
            _uncompressedDir = options.Uncompressed;
            _compressedDir = options.Compressed;
        }

        public int Run()
        {
            var fileFinder = new MusicMgmtFileFinder(_config.InputConfig);
            fileFinder.FoundTableOfContentsFile += FoundTableOfContentsFile;

            fileFinder.Scan(_uncompressedDir);
            return 0;
        }

        private void FoundTableOfContentsFile(object _, TableOfContentsFileEvent e)
        {
            var version = TableOfContentsUtil.ReadVersion(e.Filename);
            if (ToCVersion.V1 == version)
            {
                throw new InvalidDataException(
                    $"File '{e.Filename}' is a v1 file that is not supported. "
                    + "Use CreateToc.exe to upgrade.");
            }

            var toc = TableOfContentsUtil.ReadFromFile<TableOfContents>(e.Filename);
            var tocDir = Path.GetDirectoryName(e.Filename);

            var coverChanged = HasCoverChanged(toc, tocDir!);
            var anyFileChanged = false;
            toc.TrackList.ForEach(track =>
                anyFileChanged |= HandleTrack(tocDir!, toc.RelativeOutDir, track, coverChanged));

            if (!anyFileChanged && !coverChanged) return;

            Console.WriteLine(
                $"Writing updated hashes to table of contents '{e.Filename}'.");
            JsonWriter.WriteToDirectory(tocDir, toc);

            // Remove old relative dir(s) if empty.
            // Update toc with cover hash.
            // Update toc with relative dir.
        }

        private bool HandleTrack(string tocDir, string relativeOutDir, Track track,
            bool coverChanged)
        {
            if (!HasAnyMetaTagChanged(track) && !coverChanged) return false;

            var currentCompressedFileName = Path.Combine(_converter.Output.Path, relativeOutDir,
                track.Filename.OutName + "." + _converter.Type.ToLower());
            var pathBuilder = new TrackFilePathBuilder(_config.OutputConfig.Format);

            Console.WriteLine(
                $"Changes detected in file's '{currentCompressedFileName}' meta tags or cover art.");

            if (!File.Exists(currentCompressedFileName))
            {
                throw new FileNotFoundException(
                    $"{_converter.Type} audio file '{currentCompressedFileName}' does not exist.");
            }

            AudioTagWriter.WriteTags(tocDir, currentCompressedFileName, track);
            track.UpdateMetaHash();
            track.Filename.OutName = pathBuilder.BuildFile(track);
            
            // Move to new relative dir.
            var newRelativeOutDir = pathBuilder.BuildPath(track);
            if (relativeOutDir != newRelativeOutDir)
            {
                Console.WriteLine($"Move track to new folder {newRelativeOutDir}");
            }

            return true;
        }

        private static bool HasCoverChanged(TableOfContents toc, string directory)
        {
            var currentHash = TableOfContents.ComputeCoverArtHash(directory);
            return currentHash != toc.CoverHash;
        }

        private static bool HasAnyMetaTagChanged(Track track)
        {
            var currentHash = track.ComputeMetaHash();
            return currentHash != track.MetaHash;
        }

        private string BuildDestinationFileName(Track track)
        {
            var fileName = new TrackFilePathBuilder(_config.OutputConfig.Format).Build(track);
            return Path.Combine(_converter.Output.Path, fileName + "." + _converter.Type.ToLower());
        }
    }
}