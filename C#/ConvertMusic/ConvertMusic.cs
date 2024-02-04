using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace ConvertMusic
{
    internal class ConvertMusic
    {
        private readonly FileCompressor _compressor;
        private readonly MusicMgmtFileFinder _fileFinder;

        public ConvertMusic(Options options)
        {
            var config = new MusicManagementConfig(options.Config);

            var converter = config.OutputConfig.Converters.Find(
                conv => string.Equals(conv.Type, options.Format,
                    StringComparison.CurrentCultureIgnoreCase));
            if (null == converter)
            {
                throw new ArgumentException(
                    $"No converter format found for '{options.Format}'.");
            }

            _compressor = new FileCompressor(config, converter);
            _fileFinder =
                new MusicMgmtFileFinder(config.InputConfig, config.FilenameEncodingConfig);
        }

        public int Run()
        {
            _fileFinder.FoundTableOfContentsFile += FoundTableOfContentsFile;
            _fileFinder.Scan();
            return 0;
        }

        private void FoundTableOfContentsFile(object _, TableOfContentsFileEvent e)
        {
            var version = TableOfContentsUtil.ReadVersion(e.TableOfContentsFile);
            var toc = ToCVersion.V1 == version
                ? TableOfContentsUtil.MigrateV1ToV2File(e.TableOfContentsFile)
                : TableOfContentsUtil.ReadFromFile<TableOfContentsV2>(e.TableOfContentsFile);

            var sourceDir = Path.GetDirectoryName(e.TableOfContentsFile);
            toc.TrackList.ForEach(track => HandleTrack(sourceDir!, toc.RelativeOutDir, track));
        }

        private void HandleTrack(string sourceDirectory, string relativeOutDir,
            TrackV2 track)
        {
            var source = Path.Combine(sourceDirectory, track.Filename.ShortName);
            if (!File.Exists(source))
            {
                throw new FileNotFoundException($"Audio file '{source}' does not exist.");
            }

            _compressor.Compress(sourceDirectory, relativeOutDir, source, track);
        }
    }
}