using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V3;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace ConvertMusic;

/// <summary>
/// Convert all source audio files to the format specified on the command line.
/// </summary>
internal class ConvertMusic
{
    private readonly FileCompressor _compressor;
    private readonly MusicManagementConfig _config;

    public ConvertMusic(Options options)
    {
        _config = new MusicManagementConfig(options.Config);

        var converter = _config.OutputConfig.Converters.Find(
            conv => string.Equals(conv.Type, options.Format,
                StringComparison.CurrentCultureIgnoreCase));
        if (null == converter) {
            throw new ArgumentException(
                $"No converter format found for '{options.Format}'.");
        }

        _compressor = new FileCompressor(converter);
    }

    public int Run()
    {
        var fileFinder = new MusicMgmtFileFinder(_config.InputConfig);
        fileFinder.FoundTableOfContentsFile += FoundTableOfContentsFile;
        fileFinder.Scan();
        return 0;
    }

    private void FoundTableOfContentsFile(object _, TableOfContentsFileEvent e)
    {
        var version = TableOfContentsUtil.ReadVersion(e.FileName);
        var toc = ToCVersion.V1 == version
            ? TableOfContentsUtil.MigrateV1ToV3File(e.FileName, _config)
            : ToCVersion.V2 == version
                ? TableOfContentsUtil.MigrateV2ToV3File(e.FileName, _config)
                : TableOfContentsUtil.ReadFromFile<TableOfContentsV3>(e.FileName);

        var sourceDir = Path.GetDirectoryName(e.FileName);
        toc.TrackList.ForEach(track => HandleTrack(sourceDir!, track));
    }

    private void HandleTrack(string sourceDirectory, TrackV3 track)
    {
        var source = Path.Combine(sourceDirectory, track.Files.Uncompressed);
        if (!File.Exists(source)) {
            throw new FileNotFoundException($"Audio file '{source}' does not exist.");
        }

        _compressor.Compress(sourceDirectory, source, track);
    }
}
