using MusicManagementCore.Constant;
using MusicManagementCore.Converter;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V3;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace UpdateTags;

/// <summary>
/// Read the input path for table of contents files and update the files at the selected 
/// converter's output path where the meta information hash has changed compared to what is stored 
/// in the table of contents file.
/// </summary>
class UpdateTags
{
    private readonly MusicManagementConfig _config;
    private readonly MetaDataConverter _metaConverter;
    private readonly Converter _converter;
    private readonly string _uncompressedDir;

    public UpdateTags(Options options)
    {
        _config = new MusicManagementConfig(options.Config);
        _metaConverter = new MetaDataConverter(_config);

        var converter = _config.OutputConfig.Converters.Find(
            conv => string.Equals(conv.Type, options.Format,
                StringComparison.CurrentCultureIgnoreCase));

        _converter = converter ??
                        throw new ArgumentException(
                            $"No converter format found for '{options.Format}'.");
        _uncompressedDir = options.Uncompressed;
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
        var version = TableOfContentsUtil.ReadVersion(e.FileName);
        if (ToCVersion.V3 != version) {
            throw new InvalidDataException(
                $"File '{e.FileName}' is not a supported v3 formatted file. "
                + "Use CreateToc.exe to upgrade.");
        }

        var toc = TableOfContentsUtil.ReadFromFile<TableOfContentsV3>(e.FileName);
        var tocDir = Path.GetDirectoryName(e.FileName);

        var coverHash = ComputeCoverHash(toc, tocDir!);
        var trackList = toc.TrackList
            .Select(track => HandleTrack(tocDir!, track))
            .ToList();

        var newToc = new TableOfContentsV3 {
            Version = ToCVersion.V3,
            CoverHash = coverHash,
            TrackList = trackList
        };

        // Compare new and old toc (custom Equals methods?).

        Console.WriteLine(
            $"Writing updated hashes to table of contents '{e.FileName}'.");
        JsonWriter.WriteToDirectory(tocDir, newToc);
    }

    private TrackV3 HandleTrack(string tocDir, TrackV3 track)
    {
        var updatedMetaData = _metaConverter.ToMetaData(track.MetaData);
        if (!HasAnyMetaTagChanged(track.MetaData, updatedMetaData)) {
            return track;
        }

        var currentCompressedFileName = CreateAbsoluteFileName(track.Files.Compressed);
        if (!File.Exists(currentCompressedFileName)) {
            throw new FileNotFoundException(
                $"{_converter.Type} audio file '{currentCompressedFileName}' does not exist.");
        }

        Console.WriteLine(
            $"Changes detected in file's '{currentCompressedFileName}' meta tags or cover art.");

        AudioTagWriter.WriteTags(tocDir, currentCompressedFileName, track);
        var updatedTrack = CreateUpdatedTrack(updatedMetaData, track.IsCompilation);

        MoveFileToNewDirectory(currentCompressedFileName, updatedTrack);

        return updatedTrack;
    }

    private void MoveFileToNewDirectory(string sourceFile, TrackV3 updatedTrack)
    {
        var newCompressedFileName = CreateAbsoluteFileName(updatedTrack.Files.Compressed);
        var newOutputPath = Path.GetDirectoryName(newCompressedFileName);
        var oldOutputPath = Path.GetDirectoryName(sourceFile);

        if (newOutputPath == oldOutputPath) {
            return;
        }

        if (null == newOutputPath) {
            Console.WriteLine($"Cannot create new output directory as there is none in new file name "
                + "'{newCompressedFileName}'");
            return;
        }

        _ = Directory.CreateDirectory(newOutputPath);
        File.Move(sourceFile, newCompressedFileName);

        DeleteRecursiveIfEmpty(oldOutputPath!);
    }

    private static void DeleteRecursiveIfEmpty(string path)
    {
        var files = Directory.EnumerateFileSystemEntries(path);
        if (!files.Any()) {
            Directory.Delete(path, false);

            var parent = Directory.GetParent(path);
            if (null != parent) {
                DeleteRecursiveIfEmpty(parent.FullName);
            }
        } else {
            return;
        }
    }

    private static string ComputeCoverHash(TableOfContentsV3 toc, string directory)
    {
        var coverArt = CoverArt.OfDirectory(directory);
        return DataHasher.ComputeOfFile(coverArt.Path);
    }

    private static bool HasAnyMetaTagChanged(MetaDataV3 current, MetaDataV3 updated)
    {
        return current.Hash != updated.Hash;
    }

    private TrackV3 CreateUpdatedTrack(MetaDataV3 metaData, bool isCompilation)
    {
        return new TrackV3 {
            IsCompilation = isCompilation,
            MetaData = metaData,
            Files = new FilesV3 {
                Original = _metaConverter.ToOriginalFileName(metaData),
                Uncompressed = _metaConverter.ToUncompressedFileName(metaData),
                Compressed = _metaConverter.ToCompressedFileName(metaData, isCompilation)
            }
        };
    }

    private string CreateAbsoluteFileName(string fileName)
    {
        return Path.Combine(_converter.Output.Path, fileName + "." + _converter.Type.ToLower());
    }
}