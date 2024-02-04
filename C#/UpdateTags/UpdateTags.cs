using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
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
            var fileFinder =
                new MusicMgmtFileFinder(_config.InputConfig, _config.FilenameEncodingConfig);
            fileFinder.FoundTableOfContentsFile += FoundTableOfContentsFile;

            fileFinder.Scan(_uncompressedDir);
            return 0;
        }

        private void FoundTableOfContentsFile(object _, TableOfContentsFileEvent e)
        {
            var version = TableOfContentsUtil.ReadVersion(e.TableOfContentsFile);
            if (ToCVersion.V1 == version)
            {
                throw new InvalidDataException(
                    $"File '{e.TableOfContentsFile}' is a v1 file that is not supported. "
                    + "Use CreateToc.exe to upgrade.");
            }

            var toc = TableOfContentsUtil.ReadFromFile<TableOfContentsV2>(e.TableOfContentsFile);
            var tocDir = Path.GetDirectoryName(e.TableOfContentsFile);

            var coverChanged = HasCoverChanged(toc, tocDir!);
            var anyFileChanged = false;
            toc.TrackList.ForEach(track =>
                anyFileChanged |= HandleTrack(tocDir!, track, coverChanged));

            if (!anyFileChanged && !coverChanged) return;

            Console.WriteLine(
                $"Writing updated hashes to table of contents '{e.TableOfContentsFile}'.");
            JsonWriter.WriteToDirectory(tocDir, toc);
        }

        private bool HandleTrack(string relativeOutDir, TrackV2 track, bool coverChanged)
        {
            var compressedFileName = Path.Combine(_converter.Output.Path, relativeOutDir,
                track.Filename.OutName + "." + _converter.Type.ToLower());
            if (!File.Exists(compressedFileName))
            {
                throw new FileNotFoundException(
                    $"{_converter.Type} audio file '{compressedFileName}' does not exist.");
            }

            if (!HasAnyMetaTagChanged(track) && !coverChanged) return false;

            Console.WriteLine(
                $"Changes detected in file's '{compressedFileName}' meta tags or cover art.");

            // Write updated tags.
            // Move to new relative dir.
            // Remove old relative dir(s) if empty.
            
            // Update toc with cover hash.
            // Update toc with meta hash.
            // Update toc with relative dir.
            // Update toc with out file name.
            
            track.UpdateHash();
            
            return true;
        }

        private static bool HasCoverChanged(TableOfContentsV2 toc, string directory)
        {
            var currentHash = TableOfContentsV2.ComputeCoverArtHash(directory);
            return currentHash != toc.CoverHash;
        }

        private static bool HasAnyMetaTagChanged(TrackV2 track)
        {
            var currentHash = track.ComputeHash();
            return currentHash != track.MetaHash;
        }
    }
}