using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace ConvertMusic
{
    internal class ConvertMusic
    {
        private readonly Options _options;
        private readonly MusicManagementConfig _config;
        private readonly Converter _converter;

        public ConvertMusic(Options options)
        {
            _options = options;
            _config = new MusicManagementConfig(_options.Config);

            var converter = _config.OutputConfig.Converters.Find(
                conv => string.Equals(conv.Type, _options.Format, StringComparison.CurrentCultureIgnoreCase));
            _converter = converter ??
                         throw new ArgumentException($"No converter format found for '{_options.Format}'.");
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
            var toc = ToCVersion.V1 == version
                ? TableOfContentsUtil.MigrateV1ToV2File(e.TableOfContentsFile)
                : TableOfContentsUtil.ReadFromFile<TableOfContentsV2>(e.TableOfContentsFile);

            var directory = Path.GetDirectoryName(e.TableOfContentsFile);
            toc.TrackList.ForEach(track => HandleTrack(directory!, track));
        }

        private void HandleTrack(string directory, TrackV2 track)
        {
            var source = Path.Combine(directory, track.Filename.ShortName);
            if (!File.Exists(source))
            {
                throw new FileNotFoundException($"Audio file '{source}' does not exist.");
            }


            var compressedFile = new CompressedFile(_config, _converter, track);
            if (compressedFile.Exists) return;
            compressedFile.MakeDestinationFolder();
            compressedFile.Compress(source);
            compressedFile.WriteAudioTags(directory);
        }
    }
}