using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MusicManagementCore.Constant;
using MusicManagementCore.Converter;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V1;
using MusicManagementCore.Domain.ToC.V3;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace CreateToc
{
    internal class CreateToc
    {
        private readonly Dictionary<string, List<MetaDataV3>> _records = [];
        private readonly MusicManagementConfig _config;
        private readonly MetaDataConverter _metaDataConverter;

        public CreateToc(Options options)
        {
            _config = new MusicManagementConfig(options.Config);
            _metaDataConverter = new MetaDataConverter(_config);
        }

        public int Run()
        {
            var fileFinder = new MusicMgmtFileFinder(_config.InputConfig);
            fileFinder.EnterDirectory += EnterDirectory;
            fileFinder.LeaveDirectory += LeaveDirectory;
            fileFinder.FoundAudioFile += FoundAudioFile;

            fileFinder.Scan();
            return 0;
        }

        private void EnterDirectory(object _, DirectoryEvent e)
        {
            var tocFilename = Path.Combine(e.Path, StandardFilename.TableOfContents);
            if (!File.Exists(tocFilename)) {
                _records.Add(e.Path, []);
            } else {
                var version = TableOfContentsUtil.ReadVersion(tocFilename);
                if (ToCVersion.V1 == version) {
                    TableOfContentsUtil.MigrateV1ToV3File(tocFilename, _config);
                    Console.WriteLine(
                        $"'{e.Path}' V1 table of contents file migrated to V3.");
                } else if (ToCVersion.V2 == version) {
                    TableOfContentsUtil.MigrateV2ToV3File(tocFilename, _config);
                    Console.WriteLine(
                        $"'{e.Path}' V2 table of contents file migrated to V3.");
                } else {
                    Console.WriteLine(
                            $"'{e.Path}' already contains a table of contents file.");
                }
            }
        }

        private void LeaveDirectory(object _, DirectoryEvent e)
        {
            if (!_records.ContainsKey(e.Path)) {
                return;
            }

            var metas = _records.First(item => item.Key == e.Path);
            FinalizeRecord(e.Path, metas.Value);
            _records.Remove(e.Path);
        }

        private void FoundAudioFile(object _, AudioFileEvent e)
        {
            if (!_records.ContainsKey(e.UncompressedFile.Directory)) {
                return;
            }

            var metas = _records.First(item => item.Key == e.UncompressedFile.Directory);
            metas.Value.Add(ParseMetaData(e.UncompressedFile));
        }

        private MetaDataV3 ParseMetaData(UncompressedFile uncompressedFile)
        {
            return _metaDataConverter.ToMetaData(uncompressedFile.Filename);
        }

        private void FinalizeRecord(string directory, List<MetaDataV3> metas)
        {
            if (metas.Count == 0) {
                return;
            }

            var distinctArtists = metas.DistinctBy(track => track.Artist);
            var isCompilation = distinctArtists.Count() > 1;

            var tracks = metas.ConvertAll(meta => TrackFromMetaData(meta, isCompilation));
            tracks.Sort((t1, t2) =>
                string.Compare(t1.MetaData.TrackNumber, t2.MetaData.TrackNumber, StringComparison.Ordinal));

            var coverArt = CoverArt.OfDirectory(directory);
            var toc = new TableOfContentsV3 {
                Version = ToCVersion.V3,
                CoverHash = DataHasher.ComputeOfFile(coverArt.Path),
                TrackList = tracks
            };

            JsonWriter.WriteToDirectory(directory, toc);
            toc.TrackList.ForEach(track => RenameFile(directory, track));
            Console.WriteLine($"'{directory}' table of contents created");
        }

        private TrackV3 TrackFromMetaData(MetaDataV3 metaData, bool isCompilation)
        {
            var files = new FilesV3 {
                Original = _metaDataConverter.ToOriginalFilename(metaData),
                Uncompressed = _metaDataConverter.ToUncompressedFilename(metaData),
                Compressed = _metaDataConverter.ToCompressedFilename(metaData, isCompilation)
            };

            return new TrackV3 {
                IsCompilation = isCompilation,
                MetaData = metaData,
                Files = files
            };
        }

        private static void RenameFile(string directory, TrackV3 track)
        {
            File.Move(
                Path.Combine(directory, track.Files.Original),
                Path.Combine(directory, track.Files.Uncompressed));
        }
    }
}