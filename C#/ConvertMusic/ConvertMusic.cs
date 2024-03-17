using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
using MusicManagementCore.Domain.ToC.V2;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace ConvertMusic
{
    internal class ConvertMusic
    {
        private readonly FileCompressor _compressor;
        private readonly MusicMgmtFileFinder _fileFinder;
        private readonly MusicManagementConfig _config;

        public ConvertMusic(Options options)
        {
            _config = new MusicManagementConfig(options.Config);

            var converter = _config.OutputConfig.Converters.Find(
                conv => string.Equals(conv.Type, options.Format,
                    StringComparison.CurrentCultureIgnoreCase));
            if (null == converter)
            {
                throw new ArgumentException(
                    $"No converter format found for '{options.Format}'.");
            }

            _compressor = new FileCompressor(converter);
            _fileFinder = new MusicMgmtFileFinder(_config.InputConfig);
        }

        public int Run()
        {
            _fileFinder.FoundTableOfContentsFile += FoundTableOfContentsFile;
            _fileFinder.Scan();
            return 0;
        }

        private void FoundTableOfContentsFile(object _, TableOfContentsFileEvent e)
        {
            var version = TableOfContentsUtil.ReadVersion(e.Filename);
            var toc = ToCVersion.V1 == version
                ? TableOfContentsUtil.MigrateV1ToV2File(e.Filename, _config)
                : TableOfContentsUtil.ReadFromFile<TableOfContents>(e.Filename);

            var sourceDir = Path.GetDirectoryName(e.Filename);
            toc.TrackList.ForEach(track => HandleTrack(sourceDir!, track));
        }

        private void HandleTrack(string sourceDirectory, Track track)
        {
            var source = Path.Combine(sourceDirectory, track.Files.Uncompressed);
            if (!File.Exists(source))
            {
                throw new FileNotFoundException($"Audio file '{source}' does not exist.");
            }

            _compressor.Compress(sourceDirectory, source, track);
        }
    }
}